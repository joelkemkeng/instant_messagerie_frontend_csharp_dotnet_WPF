// HETIC - Base view model class for Hetic-Stream
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ReactiveUI;

namespace HeticStream.UI.ViewModels
{
    public class ViewModelBase : ReactiveObject
    {
        /// <summary>
        /// Optional parameter passed during navigation
        /// </summary>
        public object? NavigationParameter { get; set; }
        
        /// <summary>
        /// Raises the PropertyChanged event
        /// </summary>
        /// <param name="propertyName">Name of the property that changed</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            this.RaisePropertyChanged(propertyName);
        }
        
        /// <summary>
        /// Sets a property value and raises the PropertyChanged event if the value has changed
        /// </summary>
        /// <typeparam name="T">The type of the property</typeparam>
        /// <param name="storage">Reference to the backing field</param>
        /// <param name="value">The new value</param>
        /// <param name="propertyName">Name of the property</param>
        /// <returns>True if the value has changed, false otherwise</returns>
        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string? propertyName = null)
        {
            if (Equals(storage, value))
            {
                return false;
            }
            
            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}