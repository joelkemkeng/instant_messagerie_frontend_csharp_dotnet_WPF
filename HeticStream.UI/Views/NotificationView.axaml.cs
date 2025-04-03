// HETIC - Notification view for Hetic-Stream
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using System.Threading.Tasks;

namespace HeticStream.UI.Views
{
    public partial class NotificationView : Window
    {
        public NotificationView()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            
            // Set the position to the bottom right of the screen
            Opened += NotificationView_Opened;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
        
        private async void NotificationView_Opened(object? sender, System.EventArgs e)
        {
            // Position the notification at the bottom right of the screen
            var screen = Screens.Primary;
            if (screen != null)
            {
                Position = new PixelPoint(
                    (int)(screen.Bounds.Width - Width - 20),
                    (int)(screen.Bounds.Height - Height - 40));
            }
            
            // Start with 0 opacity and animate to full opacity
            Opacity = 0;
            
            await Task.Delay(100);
            
            Dispatcher.UIThread.Post(() => Opacity = 0.9);
        }
        
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}