// HETIC - Message view model for Hetic-Stream
using System;
using HeticStream.Core.Models;

namespace HeticStream.UI.ViewModels
{
    public class MessageViewModel : ViewModelBase
    {
        private Message _message;
        
        public MessageViewModel(Message message)
        {
            _message = message;
        }
        
        public string Id => _message.Id;
        
        public string Content => _message.Content;
        
        public string AuthorId => _message.AuthorId;
        
        public string AuthorName => _message.AuthorName;
        
        public string AuthorAvatarUrl => _message.AuthorAvatarUrl;
        
        public DateTime Timestamp => _message.Timestamp;
        
        public string FormattedTime => _message.Timestamp.ToString("HH:mm");
        
        public string FormattedDate => _message.Timestamp.ToString("MMM dd");
        
        public bool IsRead => _message.IsRead;
        
        public bool IsFromCurrentUser(string currentUserId)
        {
            return AuthorId == currentUserId;
        }
    }
}