namespace Dfe.Complete.Security
{
    public static class SecurityHeadersDefinitions
    {
        private static readonly string[] DefaultSrcExclusions = ["wss://localhost:*/Dfe.Complete/"];

        private static readonly string[] ScriptSrcExclusions =
        [
            "https://*.googletagmanager.com", "https://*.google-analytics.com",
            "https://js.monitor.azure.com/scripts/b/ext/ai.clck.2.8.18.min.js",
            "https://js.monitor.azure.com/scripts/b/ai.3.gbl.min.js"
        ];

        private static readonly string[] ConnectSrcExclusions =
        [
            "https://js.monitor.azure.com/scripts/b/ai.config.1.cfg.json",
            "https://*.in.applicationinsights.azure.com/v2/track", "https://*.googletagmanager.com",
            "https://*.google-analytics.com"
        ];

        private static readonly string[] ImageSrcExclusions =
        [
            "https://www.googletagmanager.com", "https://*.google-analytics.com"
        ];

        public static HeaderPolicyCollection GetHeaderPolicyCollection(bool isDev)
        {

            var policy = new HeaderPolicyCollection()
                .AddFrameOptionsDeny()
                .AddXssProtectionBlock()
                .AddContentTypeOptionsNoSniff()
                .AddReferrerPolicyStrictOriginWhenCrossOrigin()
                .RemoveServerHeader()
                .AddCrossOriginOpenerPolicy(builder =>
                {
                    builder.SameOrigin();
                })
                .AddCrossOriginEmbedderPolicy(builder =>
                {
                    builder.RequireCorp();
                })
                .AddCrossOriginResourcePolicy(builder =>
                {
                    builder.SameOrigin();
                })
                .AddContentSecurityPolicy(builder =>
                {
                    builder.AddDefaultSrc().Self().From(DefaultSrcExclusions);
                    builder.AddScriptSrc().Self().WithNonce().From(ScriptSrcExclusions);
                    builder.AddConnectSrc().Self().From(ConnectSrcExclusions);
                    builder.AddImgSrc().Self().From(ImageSrcExclusions);
                    builder.AddObjectSrc().None();
                    builder.AddBlockAllMixedContent();
                    builder.AddFormAction().Self();
                    builder.AddFormAction().OverHttps();
                    builder.AddFontSrc().Self();
                    builder.AddStyleSrc().Self().WithNonce();
                    builder.AddBaseUri().Self();
                    builder.AddFrameAncestors().None();
                })
                .RemoveServerHeader()
                .AddPermissionsPolicy(builder =>
                {
                    builder.AddAccelerometer().None();
                    builder.AddAutoplay().None();
                    builder.AddCamera().None();
                    builder.AddEncryptedMedia().None();
                    builder.AddFullscreen().All();
                    builder.AddGeolocation().None();
                    builder.AddGyroscope().None();
                    builder.AddMagnetometer().None();
                    builder.AddMicrophone().None();
                    builder.AddMidi().None();
                    builder.AddPayment().None();
                    builder.AddPictureInPicture().None();
                    builder.AddSyncXHR().None();
                    builder.AddUsb().None();
                });

            if (!isDev)
            {
                // max age = one year in seconds
                policy.AddStrictTransportSecurityMaxAgeIncludeSubDomains(maxAgeInSeconds: 60 * 60 * 24 * 365);
            }

            return policy;
        }
    }
}

