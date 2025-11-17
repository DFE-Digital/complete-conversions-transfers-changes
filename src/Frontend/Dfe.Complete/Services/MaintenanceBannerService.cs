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
                if (!_options.MaintenanceStart.HasValue || !_options.MaintenanceEnd.HasValue || !_options.NotifyFrom.HasValue)
                {
                    _logger.LogError("Maintenance banner configuration is incomplete. MaintenanceStart, MaintenanceEnd, and NotifyFrom must all be set.");
                    return false;
                }

                var now = DateTime.UtcNow;
                var maintenanceStart = _options.MaintenanceStart.Value;
                var maintenanceEnd = _options.MaintenanceEnd.Value;
                var notifyFrom = _options.NotifyFrom.Value;
                
                // Set NotifyTo to MaintenanceEnd if not specified
                var notifyTo = _options.NotifyTo ?? maintenanceEnd;

                // Validate maintenance end is not in the past
                if (maintenanceEnd < now)
                {
                    _logger.LogError("Maintenance banner configuration error: MaintenanceEnd ({MaintenanceEnd}) is in the past", maintenanceEnd);
                    return false;
                }

                // Validate maintenance end is not before maintenance start
                if (maintenanceEnd < maintenanceStart)
                {
                    _logger.LogError("Maintenance banner configuration error: MaintenanceEnd ({MaintenanceEnd}) is before MaintenanceStart ({MaintenanceStart})",
                        maintenanceEnd, maintenanceStart);
                    return false;
                }

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
                    notifyFrom, notifyTo, maintenanceStart, maintenanceEnd, now, shouldShow);

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

            // Generate default message based on dates
            if (!_options.MaintenanceStart.HasValue || !_options.MaintenanceEnd.HasValue)
                return "The Complete conversions, transfers and changes service will be temporarily unavailable due to scheduled maintenance work.";

            var startDateTime = _options.MaintenanceStart.Value;
            var endDateTime = _options.MaintenanceEnd.Value;

            return $"The Complete conversions, transfers and changes service will be unavailable from {startDateTime.ToUkDateTimeWithAmPmString()} until {endDateTime.ToUkDateTimeWithAmPmString()} due to scheduled maintenance work.";
        }

        public MaintenanceBannerOptions GetConfiguration() => _options;
    }
}