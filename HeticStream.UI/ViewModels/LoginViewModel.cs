// HETIC - Login view model for Hetic-Stream
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using HeticStream.Core.Services;
using ReactiveUI;

namespace HeticStream.UI.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        private readonly IAuthService _authService;
        private readonly INavigationService _navigationService;
        
        private string _email = string.Empty;
        private string _password = string.Empty;
        private bool _isLoading;
        
        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }
        
        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }
        
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }
        
        public ICommand LoginCommand { get; }
        public ICommand GoToRegisterCommand { get; }
        
        public LoginViewModel(IAuthService authService, INavigationService navigationService)
        {
            _authService = authService;
            _navigationService = navigationService;
            
            LoginCommand = ReactiveCommand.CreateFromTask(LoginAsync);
            GoToRegisterCommand = ReactiveCommand.Create(GoToRegister);
        }
        
        private async Task LoginAsync()
        {
            // Validate inputs
            if (string.IsNullOrWhiteSpace(Email))
            {
                _navigationService.ShowNotification("Validation Error", "Email is required", NotificationType.Error);
                return;
            }
            
            if (string.IsNullOrWhiteSpace(Password))
            {
                _navigationService.ShowNotification("Validation Error", "Password is required", NotificationType.Error);
                return;
            }
            
            try
            {
                IsLoading = true;
                
                // Attempt to login
                var result = await _authService.LoginAsync(Email, Password);
                
                if (result.Success)
                {
                    // Show success notification
                    _navigationService.ShowNotification("Success", "Login successful", NotificationType.Success);
                    
                    // Wait a moment before navigating to the main view
                    await Task.Delay(2000);
                    
                    // Navigate to the main view
                    _navigationService.NavigateTo<MainViewModel>();
                }
                else
                {
                    // Show error notification
                    _navigationService.ShowNotification("Login Failed", result.Message, NotificationType.Error);
                }
            }
            catch (Exception ex)
            {
                // Show exception notification
                _navigationService.ShowNotification("Error", $"An error occurred: {ex.Message}", NotificationType.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }
        
        private void GoToRegister()
        {
            _navigationService.NavigateTo<RegisterViewModel>();
        }
    }
}