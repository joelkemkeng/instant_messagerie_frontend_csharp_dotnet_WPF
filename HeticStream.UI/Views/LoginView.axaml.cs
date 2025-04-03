// HETIC - Login view for Hetic-Stream
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace HeticStream.UI.Views
{
    public partial class LoginView : Window
    {
        public LoginView()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}