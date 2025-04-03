// HETIC - Register view for Hetic-Stream
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace HeticStream.UI.Views
{
    public partial class RegisterView : Window
    {
        public RegisterView()
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