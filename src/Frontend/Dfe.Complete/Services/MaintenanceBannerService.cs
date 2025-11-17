using Dfe.Complete.Configuration;
using Dfe.Complete.Extensions;
using Microsoft.Extensions.Options;

namespace Dfe.Complete.Services
{
    public interface IMaintenanceBannerService
    {
        bool ShouldShowBanner();
        string GetBannerMessage();
        MaintenanceBannerOptions GetConfiguration();
    }

    public class MaintenanceBannerService(IOptions<MaintenanceBannerOptions> options, ILogger<MaintenanceBannerService> logger, IWebHostEnvironment environment) : IMaintenanceBannerService
    {
        private readonly MaintenanceBannerOptions _options = options.Value;
        private readonly ILogger<MaintenanceBannerService> _logger = logger;
        private readonly IWebHostEnvironment _environment = environment;

        public bool ShouldShowBanner()
        {
            try
            {
                // Only show in production environment
                if (!_environment.IsProduction())
                    return false;

                // If not enabled, don't show
                if (!_options.Enabled)
                    return false;

                // Validate required dates are set
                if (!_options.NotifyFrom.HasValue)
                {
                    _logger.LogError("Maintenance banner configuration is incomplete. NotifyFrom must be set.");
                    return false;
                }

                var now = DateTime.UtcNow;
                var notifyFrom = _options.NotifyFrom.Value;
                
                // Validate maintenance end is not in the past (if provided)
                if (_options.MaintenanceEnd.HasValue && _options.MaintenanceEnd.Value < now)
                {
                    _logger.LogError("Maintenance banner configuration error: MaintenanceEnd ({MaintenanceEnd}) is in the past", _options.MaintenanceEnd.Value);
                    return false;
                }

                // Validate maintenance end is not before maintenance start (if both provided)
                if (_options.MaintenanceStart.HasValue && _options.MaintenanceEnd.HasValue && _options.MaintenanceEnd.Value < _options.MaintenanceStart.Value)
                {
                    _logger.LogError("Maintenance banner configuration error: MaintenanceEnd ({MaintenanceEnd}) is before MaintenanceStart ({MaintenanceStart})",
                        _options.MaintenanceEnd.Value, _options.MaintenanceStart.Value);
                    return false;
                }
                
                // Set NotifyTo to MaintenanceEnd if not specified, or to far future if no MaintenanceEnd
                var notifyTo = _options.NotifyTo ?? _options.MaintenanceEnd ?? DateTime.MaxValue;

                // Validate NotifyTo is after NotifyFrom
                if (notifyTo <= notifyFrom)
                {
                    _logger.LogError("Maintenance banner configuration error: NotifyTo ({NotifyTo}) must be after NotifyFrom ({NotifyFrom})",
                        notifyTo, notifyFrom);
                    return false;
                }

                // Show banner if notify from is in past and notify to is in future
                var shouldShow = notifyFrom <= now && notifyTo > now;

                _logger.LogInformation("Maintenance banner check: NotifyFrom={NotifyFrom}, NotifyTo={NotifyTo}, MaintenanceStart={MaintenanceStart}, MaintenanceEnd={MaintenanceEnd}, Now={Now}, ShouldShow={ShouldShow}",
                    notifyFrom, notifyTo, _options.MaintenanceStart, _options.MaintenanceEnd, now, shouldShow);

                return shouldShow;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if maintenance banner should be shown");
                return false;
            }
        }

        public string GetBannerMessage()
        {
            // If a custom message is provided, use it
            if (!string.IsNullOrWhiteSpace(_options.Message))
                return _options.Message;

            // Generate default message based on available dates
            var hasStart = _options.MaintenanceStart.HasValue;
            var hasEnd = _options.MaintenanceEnd.HasValue;

            if (!hasStart && !hasEnd)
                return "The Complete conversions, transfers and changes service will be temporarily unavailable due to scheduled maintenance work.";

            if (hasStart && !hasEnd)
                return $"The Complete conversions, transfers and changes service will be unavailable from {_options.MaintenanceStart!.Value.ToUkDateTimeWithAmPmString()} due to scheduled maintenance work.";

            if (!hasStart)
                return $"The Complete conversions, transfers and changes service will be unavailable until {_options.MaintenanceEnd!.Value.ToUkDateTimeWithAmPmString()} due to scheduled maintenance work.";

            // Both dates provided (hasStart && hasEnd)
            var startDateTime = _options.MaintenanceStart!.Value;
            var endDateTime = _options.MaintenanceEnd!.Value;

            // Check if same date: "from {start time} to {end time} on {date}"
            if (startDateTime.Date == endDateTime.Date)
            {
                var date = startDateTime.ToDateString(); // Just the date part
                var startTime = startDateTime.ToString("h:mm tt").ToLower(); // Time part in AM/PM format
                var endTime = endDateTime.ToString("h:mm tt").ToLower();
                
                return $"The Complete conversions, transfers and changes service will be unavailable from {startTime} to {endTime} on {date} due to scheduled maintenance work.";
            }

            // Different dates: "from {full start date/time} until {full end date/time}"
            return $"The Complete conversions, transfers and changes service will be unavailable from {startDateTime.ToUkDateTimeWithAmPmString()} until {endDateTime.ToUkDateTimeWithAmPmString()} due to scheduled maintenance work.";
        }

        public MaintenanceBannerOptions GetConfiguration() => _options;
    }
}