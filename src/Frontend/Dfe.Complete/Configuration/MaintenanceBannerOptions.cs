namespace Dfe.Complete.Configuration
{
    public class MaintenanceBannerOptions
    {
        public const string Section = "MaintenanceBanner";

        public DateTime? MaintenanceStart { get; set; }
        public DateTime? MaintenanceEnd { get; set; }
        public DateTime? NotifyFrom { get; set; }
        public DateTime? NotifyTo { get; set; }
        public string Message { get; set; } = string.Empty;
        public bool Enabled { get; set; } = false;
    }
}