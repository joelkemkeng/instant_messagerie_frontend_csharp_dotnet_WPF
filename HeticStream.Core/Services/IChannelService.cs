// HETIC - Channel service interface for Hetic-Stream
using System.Collections.Generic;
using System.Threading.Tasks;
using HeticStream.Core.Models;

namespace HeticStream.Core.Services
{
    public interface IChannelService
    {
        /// <summary>
        /// Get all channels for the current user
        /// </summary>
        /// <returns>List of channels</returns>
        Task<IEnumerable<Channel>> GetChannelsAsync();
        
        /// <summary>
        /// Get messages for a specific channel
        /// </summary>
        /// <param name="channelId">Channel ID</param>
        /// <returns>List of messages</returns>
        Task<IEnumerable<Message>> GetMessagesAsync(string channelId);
        
        /// <summary>
        /// Send a message to a channel
        /// </summary>
        /// <param name="channelId">Channel ID</param>
        /// <param name="content">Message content</param>
        /// <returns>The sent message if successful</returns>
        Task<Message?> SendMessageAsync(string channelId, string content);
    }
}