// HETIC - User service interface for Hetic-Stream
using System.Collections.Generic;
using System.Threading.Tasks;
using HeticStream.Core.Models;

namespace HeticStream.Core.Services
{
    public interface IUserService
    {
        /// <summary>
        /// Get the current user profile
        /// </summary>
        /// <returns>Current user</returns>
        Task<User?> GetCurrentUserAsync();
        
        /// <summary>
        /// Get a user by ID
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>User if found</returns>
        Task<User?> GetUserByIdAsync(string userId);
        
        /// <summary>
        /// Get online users in a channel
        /// </summary>
        /// <param name="channelId">Channel ID</param>
        /// <returns>List of online users</returns>
        Task<IEnumerable<User>> GetOnlineUsersInChannelAsync(string channelId);
    }
}