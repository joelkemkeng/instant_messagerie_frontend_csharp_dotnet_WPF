// HETIC - Navigation service interface for Hetic-Stream
using System;
using Avalonia.Controls;

namespace HeticStream.UI
{
    public interface INavigationService
    {
        /// <summary>
        /// Navigate to a view with the specified view model
        /// </summary>
        /// <typeparam name="TViewModel">The type of the view model</typeparam>
        /// <param name="parameter">Optional navigation parameter</param>
        void NavigateTo<TViewModel>(object? parameter = null) where TViewModel : notnull;
        
        /// <summary>
        /// Navigate back to the previous view
        /// </summary>
        void NavigateBack();
        
        /// <summary>
        /// Show a modal dialog
        /// </summary>
        /// <param name="title">The title of the dialog</param>
        /// <param name="message">The message to display</param>
        /// <param name="type">The type of notification</param>
        void ShowNotification(string title, string message, NotificationType type);
    }
    
    public enum NotificationType
    {
        Success,
        Error,
        Warning,
        Info
    }
}