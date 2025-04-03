// HETIC - Application settings configuration for Hetic-Stream
using System;

namespace HeticStream.Core.Config
{
    public class AppSettings
    {
        // API Configuration
        public string ApiBaseUrl { get; set; } = string.Empty;
        public bool ApiEnabled { get; set; } = false;
        public string EndpointLogin { get; set; } = string.Empty;
        public string EndpointRegister { get; set; } = string.Empty;
        public string EndpointChannels { get; set; } = string.Empty;
        public string EndpointMessages { get; set; } = string.Empty;
        
        // Assets Configuration
        public string ImageAssetsPath { get; set; } = string.Empty;
        
        // Theme Configuration
        public bool DarkThemeEnabled { get; set; } = true;

        // Singleton instance
        private static AppSettings? _instance;
        
        // Lock object for thread safety
        private static readonly object _lock = new object();
        
        private AppSettings()
        {
            LoadFromEnvironment();
        }
        
        public static AppSettings Instance
        {
            get
            {
                lock (_lock)
                {
                    _instance ??= new AppSettings();
                    return _instance;
                }
            }
        }
        
        private void LoadFromEnvironment()
        {
            // Load API configuration
            ApiBaseUrl = Environment.GetEnvironmentVariable("ApiBaseUrl") ?? "https://api.heticstream.com";
            ApiEnabled = bool.TryParse(Environment.GetEnvironmentVariable("ApiEnabled"), out bool apiEnabled) && apiEnabled;
            
            // Load endpoints
            EndpointLogin = Environment.GetEnvironmentVariable("EndpointLogin") ?? "/auth/login";
            EndpointRegister = Environment.GetEnvironmentVariable("EndpointRegister") ?? "/auth/register";
            EndpointChannels = Environment.GetEnvironmentVariable("EndpointChannels") ?? "/channels";
            EndpointMessages = Environment.GetEnvironmentVariable("EndpointMessages") ?? "/messages";
            
            // Load assets configuration
            ImageAssetsPath = Environment.GetEnvironmentVariable("ImageAssetsPath") ?? "Assets/Images";
            
            // Load theme configuration
            DarkThemeEnabled = !bool.TryParse(Environment.GetEnvironmentVariable("LightThemeEnabled"), out bool lightThemeEnabled) || !lightThemeEnabled;
        }
    }
}