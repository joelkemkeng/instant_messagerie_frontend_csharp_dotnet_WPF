// HETIC - Main application class for Hetic-Stream
using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using HeticStream.Core.Config;
using HeticStream.Core.Services;
using HeticStream.Services;
using HeticStream.UI.ViewModels;
using HeticStream.UI.Views;
using Microsoft.Extensions.DependencyInjection;

namespace HeticStream.UI
{
    public partial class App : Application
    {
        private IServiceProvider? _serviceProvider;
        
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            // Configure services
            ConfigureServices();
            
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var loginViewModel = _serviceProvider!.GetRequiredService<LoginViewModel>();
                desktop.MainWindow = new LoginView
                {
                    DataContext = loginViewModel
                };
            }
            else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
            {
                var loginViewModel = _serviceProvider!.GetRequiredService<LoginViewModel>();
                singleViewPlatform.MainView = new LoginView
                {
                    DataContext = loginViewModel
                };
            }

            base.OnFrameworkInitializationCompleted();
        }
        
        private void ConfigureServices()
        {
            var services = new ServiceCollection();
            
            // Register configuration
            var appSettings = AppSettings.Instance;
            
            // Register HttpClient
            services.AddSingleton<HttpClient>();
            
            // Register services
            services.AddSingleton<IAuthService, AuthService>();
            services.AddSingleton<IChannelService, ChannelService>();
            services.AddSingleton<IUserService, UserService>();
            
            // Register view models
            services.AddTransient<LoginViewModel>();
            services.AddTransient<RegisterViewModel>();
            services.AddTransient<MainViewModel>();
            services.AddTransient<ChannelViewModel>();
            services.AddTransient<MessageViewModel>();
            services.AddTransient<UserListViewModel>();
            
            // Register navigation service
            services.AddSingleton<INavigationService, NavigationService>();
            
            // Build the service provider
            _serviceProvider = services.BuildServiceProvider();
        }
        
        public static IServiceProvider GetServiceProvider()
        {
            var app = Current as App;
            return app?._serviceProvider ?? throw new InvalidOperationException("Service provider is not initialized");
        }
    }
}