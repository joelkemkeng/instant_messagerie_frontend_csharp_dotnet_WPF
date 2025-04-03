// HETIC - Channel view model for Hetic-Stream
using HeticStream.Core.Models;

namespace HeticStream.UI.ViewModels
{
    public class ChannelViewModel : ViewModelBase
    {
        private Channel _channel;
        
        public ChannelViewModel(Channel channel)
        {
            _channel = channel;
        }
        
        public string Id => _channel.Id;
        
        public string Name => _channel.Name;
        
        public ChannelType Type => _channel.Type;
        
        public string LastMessagePreview => _channel.LastMessagePreview;
        
        public string TypeIcon => _channel.Type == ChannelType.Direct ? "🟢" : "#";
        
        public bool IsDirect => _channel.Type == ChannelType.Direct;
        
        public bool IsGroup => _channel.Type == ChannelType.Group;
    }
}