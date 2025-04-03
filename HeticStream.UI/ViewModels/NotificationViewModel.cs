// HETIC - Notification view model for Hetic-Stream
using System.Windows.Input;
using ReactiveUI;

namespace HeticStream.UI.ViewModels
{
    public class NotificationViewModel : ViewModelBase
    {
        private string _title = string.Empty;
        private string _message = string.Empty;
        private NotificationType _type = NotificationType.Info;
        
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }
        
        public string Message
        {
            get => _message;
            set => SetProperty(ref _message, value);
        }
        
        public NotificationType Type
        {
            get => _type;
            set => SetProperty(ref _type, value);
        }
        
        public ICommand CloseCommand { get; }
        
        // Background brush based on notification type
        public string BackgroundBrush => Type switch
        {
            NotificationType.Success => "{DynamicResource SuccessBrush}",
            NotificationType.Error => "{DynamicResource ErrorBrush}",
            NotificationType.Warning => "{DynamicResource WarningBrush}",
            _ => "{DynamicResource AccentBrush}"
        };
        
        // Icon based on notification type
        public string Icon => Type switch
        {
            NotificationType.Success => "✓",
            NotificationType.Error => "✗",
            NotificationType.Warning => "⚠",
            _ => "ℹ"
        };
        
        public NotificationViewModel()
        {
            CloseCommand = ReactiveCommand.Create(Close);
        }
        
        public void Close()
        {
            // This will be handled by the view
        }
    }
}