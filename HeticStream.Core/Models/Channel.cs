// HETIC - Channel model for Hetic-Stream application
using System.Collections.Generic;

namespace HeticStream.Core.Models
{
    public enum ChannelType
    {
        Direct,
        Group
    }

    public class Channel
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public ChannelType Type { get; set; }
        public List<User> Members { get; set; } = new List<User>();
        public DateTime LastActivity { get; set; }
        public string LastMessagePreview { get; set; } = string.Empty;
    }
}