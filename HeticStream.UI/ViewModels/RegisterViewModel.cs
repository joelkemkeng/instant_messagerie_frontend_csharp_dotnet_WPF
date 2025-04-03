// HETIC - Register view model for Hetic-Stream
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using HeticStream.Core.Services;
using ReactiveUI;

namespace HeticStream.UI.ViewModels
{
    public class RegisterViewModel : ViewModelBase
    {
        private readonly IAuthService _authService;
        private readonly INavigationService _navigationService;
        
        private string _email = string.Empty;
        private string _username = string.Empty;
        private string _password = string.Empty;
        private string _confirmPassword = string.Empty;
        private bool _isLoading;
        
        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }
        
        public string Username
        {
            get => _username;
            set => SetProperty(ref _username, value);
        }
        
        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }
        
        public string ConfirmPassword
        {
            get => _confirmPassword;
            set => SetProperty(ref _confirmPassword, value);
        }
        
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }
        
        public ICommand RegisterCommand { get; }
        public ICommand GoToLoginCommand { get; }
        
        public RegisterViewModel(IAuthService authService, INavigationService navigationService)
        {
            _authService = authService;
            _navigationService = navigationService;
            
            RegisterCommand = ReactiveCommand.CreateFromTask(RegisterAsync);
            GoToLoginCommand = ReactiveCommand.Create(GoToLogin);
        }
        
        private async Task RegisterAsync()
        {
            // Validate inputs
            if (string.IsNullOrWhiteSpace(Email))
            {
                _navigationService.ShowNotification("Validation Error", "Email is required", NotificationType.Error);
                return;
            }
            
            if (string.IsNullOrWhiteSpace(Username))
            {
                _navigationService.ShowNotification("Validation Error", "Username is required", NotificationType.Error);
                return;
            }
            
            if (string.IsNullOrWhiteSpace(Password))
            {
                _navigationService.ShowNotification("Validation Error", "Password is required", NotificationType.Error);
                return;
            }
            
            if (Password != ConfirmPassword)
            {
                _navigationService.ShowNotification("Validation Error", "Passwords do not match", NotificationType.Error);
                return;
            }
            
            try
            {
                IsLoading = true;
                
                // Attempt to register
                var result = await _authService.RegisterAsync(Email, Username, Password);
                
                if (result.Success)
                {
                    // Show success notification
                    _navigationService.ShowNotification("Success", "Registration successful", NotificationType.Success);
                    
                    // Wait a moment before navigating to login
                    await Task.Delay(2000);
                    
                    // Navigate to the login view
                    _navigationService.NavigateTo<LoginViewModel>();
                }
                else
                {
                    // Show error notification
                    _navigationService.ShowNotification("Registration Failed", result.Message, NotificationType.Error);
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
        
        private void GoToLogin()
        {
            _navigationService.NavigateTo<LoginViewModel>();
        }
    }
}