// HETIC - Main view model for Hetic-Stream
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using HeticStream.Core.Config;
using HeticStream.Core.Models;
using HeticStream.Core.Services;
using ReactiveUI;

namespace HeticStream.UI.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly IAuthService _authService;
        private readonly IChannelService _channelService;
        private readonly IUserService _userService;
        private readonly INavigationService _navigationService;
        private readonly AppSettings _settings;
        
        private User? _currentUser;
        private Channel? _selectedChannel;
        private string _newMessage = string.Empty;
        private bool _isLoading;
        private bool _isDarkTheme;
        
        public User? CurrentUser
        {
            get => _currentUser;
            private set => SetProperty(ref _currentUser, value);
        }
        
        public Channel? SelectedChannel
        {
            get => _selectedChannel;
            set
            {
                if (SetProperty(ref _selectedChannel, value) && value != null)
                {
                    LoadMessagesAsync(value.Id);
                    LoadOnlineUsersAsync(value.Id);
                }
            }
        }
        
        public string NewMessage
        {
            get => _newMessage;
            set => SetProperty(ref _newMessage, value);
        }
        
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }
        
        public bool IsDarkTheme
        {
            get => _isDarkTheme;
            set
            {
                if (SetProperty(ref _isDarkTheme, value))
                {
                    // TODO: Implement theme switching logic
                }
            }
        }
        
        public ObservableCollection<Channel> Channels { get; } = new ObservableCollection<Channel>();
        public ObservableCollection<Message> Messages { get; } = new ObservableCollection<Message>();
        public ObservableCollection<User> OnlineUsers { get; } = new ObservableCollection<User>();
        
        public ICommand SendMessageCommand { get; }
        public ICommand LogoutCommand { get; }
        public ICommand ToggleThemeCommand { get; }
        
        public MainViewModel(
            IAuthService authService,
            IChannelService channelService,
            IUserService userService,
            INavigationService navigationService)
        {
            _authService = authService;
            _channelService = channelService;
            _userService = userService;
            _navigationService = navigationService;
            _settings = AppSettings.Instance;
            
            _isDarkTheme = _settings.DarkThemeEnabled;
            
            SendMessageCommand = ReactiveCommand.CreateFromTask(SendMessageAsync);
            LogoutCommand = ReactiveCommand.Create(Logout);
            ToggleThemeCommand = ReactiveCommand.Create(ToggleTheme);
            
            // Load data when view model is created
            LoadInitialDataAsync();
        }
        
        private async void LoadInitialDataAsync()
        {
            try
            {
                IsLoading = true;
                
                // Load current user
                CurrentUser = await _userService.GetCurrentUserAsync();
                
                // Load channels
                var channels = await _channelService.GetChannelsAsync();
                
                Channels.Clear();
                foreach (var channel in channels.OrderByDescending(c => c.LastActivity))
                {
                    Channels.Add(channel);
                }
                
                // Select the first channel if available
                if (Channels.Count > 0 && SelectedChannel == null)
                {
                    SelectedChannel = Channels.First();
                }
            }
            catch (Exception ex)
            {
                _navigationService.ShowNotification("Error", $"Failed to load data: {ex.Message}", NotificationType.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }
        
        private async void LoadMessagesAsync(string channelId)
        {
            try
            {
                IsLoading = true;
                
                var messages = await _channelService.GetMessagesAsync(channelId);
                
                Messages.Clear();
                foreach (var message in messages.OrderBy(m => m.Timestamp))
                {
                    Messages.Add(message);
                }
            }
            catch (Exception ex)
            {
                _navigationService.ShowNotification("Error", $"Failed to load messages: {ex.Message}", NotificationType.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }
        
        private async void LoadOnlineUsersAsync(string channelId)
        {
            try
            {
                var users = await _userService.GetOnlineUsersInChannelAsync(channelId);
                
                OnlineUsers.Clear();
                foreach (var user in users)
                {
                    OnlineUsers.Add(user);
                }
            }
            catch (Exception ex)
            {
                _navigationService.ShowNotification("Error", $"Failed to load online users: {ex.Message}", NotificationType.Error);
            }
        }
        
        private async Task SendMessageAsync()
        {
            if (SelectedChannel == null || string.IsNullOrWhiteSpace(NewMessage))
            {
                return;
            }
            
            try
            {
                var message = await _channelService.SendMessageAsync(SelectedChannel.Id, NewMessage);
                
                if (message != null)
                {
                    Messages.Add(message);
                    NewMessage = string.Empty;
                }
            }
            catch (Exception ex)
            {
                _navigationService.ShowNotification("Error", $"Failed to send message: {ex.Message}", NotificationType.Error);
            }
        }
        
        private void Logout()
        {
            _authService.Logout();
            _navigationService.NavigateTo<LoginViewModel>();
        }
        
        private void ToggleTheme()
        {
            IsDarkTheme = !IsDarkTheme;
            
            // TODO: Implement actual theme switching in the application
            _navigationService.ShowNotification("Theme Changed", 
                IsDarkTheme ? "Dark theme enabled" : "Light theme enabled", 
                NotificationType.Info);
        }
    }
}