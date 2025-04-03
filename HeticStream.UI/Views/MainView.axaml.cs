// HETIC - Main view for Hetic-Stream
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using HeticStream.Core.Models;
using HeticStream.UI.ViewModels;

namespace HeticStream.UI.Views
{
    public partial class MainView : Window
    {
        private MainViewModel? _viewModel;
        
        public MainView()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            DataContextChanged += MainView_DataContextChanged;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
        
        private void MainView_DataContextChanged(object? sender, System.EventArgs e)
        {
            if (DataContext is MainViewModel viewModel)
            {
                _viewModel = viewModel;
            }
        }
        
        protected void Channel_Tapped(object sender, TappedEventArgs e)
        {
            // Get the clicked channel
            if (sender is Grid grid && grid.DataContext is Channel channel && _viewModel != null)
            {
                // Set it as selected in the view model
                _viewModel.SelectedChannel = channel;
            }
        }
    }
}