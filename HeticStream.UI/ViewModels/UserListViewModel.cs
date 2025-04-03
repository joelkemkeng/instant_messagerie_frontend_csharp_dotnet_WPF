// HETIC - User list view model for Hetic-Stream
using System.Collections.ObjectModel;
using HeticStream.Core.Models;

namespace HeticStream.UI.ViewModels
{
    public class UserListViewModel : ViewModelBase
    {
        private string _channelId;
        
        public UserListViewModel(string channelId)
        {
            _channelId = channelId;
        }
        
        public ObservableCollection<User> OnlineUsers { get; } = new ObservableCollection<User>();
        
        public ObservableCollection<User> OfflineUsers { get; } = new ObservableCollection<User>();
        
        public string ChannelId => _channelId;
        
        public void SetUsers(System.Collections.Generic.IEnumerable<User> users)
        {
            OnlineUsers.Clear();
            OfflineUsers.Clear();
            
            foreach (var user in users)
            {
                if (user.IsOnline)
                {
                    OnlineUsers.Add(user);
                }
                else
                {
                    OfflineUsers.Add(user);
                }
            }
        }
    }
}