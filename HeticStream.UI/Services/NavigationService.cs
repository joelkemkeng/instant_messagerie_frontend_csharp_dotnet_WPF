// HETIC - Navigation service implementation for Hetic-Stream
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Threading;
using HeticStream.UI.ViewModels;
using HeticStream.UI.Views;
using Microsoft.Extensions.DependencyInjection;

namespace HeticStream.UI
{
    public class NavigationService : INavigationService
    {
        private readonly Stack<Window> _navigationStack = new();
        private NotificationView? _currentNotification;
        
        public void NavigateTo<TViewModel>(object? parameter = null) where TViewModel : notnull
        {
            // Get service provider
            var serviceProvider = App.GetServiceProvider();
            
            // Get the view model instance
            var viewModel = serviceProvider.GetRequiredService<TViewModel>();
            
            // Create the appropriate view based on the view model type
            Window? view = null;
            
            if (viewModel is LoginViewModel)
            {
                view = new LoginView { DataContext = viewModel };
            }
            else if (viewModel is RegisterViewModel)
            {
                view = new RegisterView { DataContext = viewModel };
            }
            else if (viewModel is MainViewModel)
            {
                view = new MainView { DataContext = viewModel };
            }
            
            if (view == null)
            {
                throw new InvalidOperationException($"No view registered for view model type {typeof(TViewModel).Name}");
            }
            
            // Set the parameter if the view model supports it
            if (viewModel is ViewModelBase baseViewModel && parameter != null)
            {
                baseViewModel.NavigationParameter = parameter;
            }
            
            // Get the current application lifetime
            if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                // Get the current window if any
                var currentWindow = desktop.MainWindow;
                
                // Push the current window to the navigation stack if it exists
                if (currentWindow != null)
                {
                    _navigationStack.Push(currentWindow);
                    currentWindow.Hide();
                }
                
                // Set the new window as the main window
                desktop.MainWindow = view;
                view.Show();
            }
            else if (Application.Current?.ApplicationLifetime is ISingleViewApplicationLifetime singleView)
            {
                // Single view platforms like web
                singleView.MainView = view.Content as Control;
            }
        }
        
        public void NavigateBack()
        {
            if (_navigationStack.Count > 0)
            {
                // Get the current application lifetime
                if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                {
                    // Get the previous window
                    var previousWindow = _navigationStack.Pop();
                    
                    // Hide the current window
                    desktop.MainWindow?.Hide();
                    
                    // Show the previous window
                    desktop.MainWindow = previousWindow;
                    previousWindow.Show();
                }
            }
        }
        
        public void ShowNotification(string title, string message, NotificationType type)
        {
            // Close any existing notification
            _currentNotification?.Close();
            
            // Create a new notification view model
            var viewModel = new NotificationViewModel
            {
                Title = title,
                Message = message,
                Type = type
            };
            
            // Create the notification view
            _currentNotification = new NotificationView
            {
                DataContext = viewModel
            };
            
            // Show the notification
            _currentNotification.Show();
            
            // Auto-close after 5 seconds for success messages
            if (type == NotificationType.Success)
            {
                // Use Dispatcher to ensure we're on the UI thread
                Dispatcher.UIThread.InvokeAsync(async () =>
                {
                    await Task.Delay(5000);
                    _currentNotification?.Close();
                    _currentNotification = null;
                });
            }
        }
    }
}