// HETIC - User service implementation for Hetic-Stream
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using HeticStream.Core.Config;
using HeticStream.Core.Models;
using HeticStream.Core.Services;

namespace HeticStream.Services
{
    public class UserService : IUserService
    {
        private readonly HttpClient _httpClient;
        private readonly IAuthService _authService;
        private readonly AppSettings _settings;
        
        // In-memory cache for simulated data
        private static readonly Dictionary<string, User> _simulatedUsers = new();
        
        public UserService(HttpClient httpClient, IAuthService authService)
        {
            _httpClient = httpClient;
            _authService = authService;
            _settings = AppSettings.Instance;
            
            InitializeSimulatedData();
        }
        
        private void InitializeSimulatedData()
        {
            // Only initialize if we're in simulation mode and data is not already initialized
            if (!_settings.ApiEnabled && !_simulatedUsers.Any())
            {
                // Create some simulated users
                var adminUser = new User
                {
                    Id = "user_admin",
                    Email = "admin@heticstream.com",
                    Username = "Admin",
                    AvatarUrl = "avatar_admin.png",
                    IsOnline = true,
                    LastSeen = DateTime.Now
                };
                
                var janeUser = new User
                {
                    Id = "user_jane",
                    Email = "jane@heticstream.com",
                    Username = "Jane",
                    AvatarUrl = "avatar_jane.png",
                    IsOnline = true,
                    LastSeen = DateTime.Now
                };
                
                var johnUser = new User
                {
                    Id = "user_john",
                    Email = "john@heticstream.com",
                    Username = "John",
                    AvatarUrl = "avatar_john.png",
                    IsOnline = false,
                    LastSeen = DateTime.Now.AddHours(-1)
                };
                
                // Add simulated current user
                var currentUser = new User
                {
                    Id = _authService.CurrentUserId ?? "user_current",
                    Email = "current@heticstream.com",
                    Username = "Current User",
                    AvatarUrl = "avatar_current.png",
                    IsOnline = true,
                    LastSeen = DateTime.Now
                };
                
                _simulatedUsers.Add(adminUser.Id, adminUser);
                _simulatedUsers.Add(janeUser.Id, janeUser);
                _simulatedUsers.Add(johnUser.Id, johnUser);
                _simulatedUsers.Add(currentUser.Id, currentUser);
            }
        }
        
        public async Task<User?> GetCurrentUserAsync()
        {
            try
            {
                // If not authenticated, return null
                if (!_authService.IsAuthenticated)
                {
                    return null;
                }
                
                // If API is disabled, return simulated data
                if (!_settings.ApiEnabled)
                {
                    Console.WriteLine("[SIMULATED] GetCurrentUser request");
                    
                    if (_simulatedUsers.TryGetValue(_authService.CurrentUserId ?? "user_current", out var user))
                    {
                        Console.WriteLine($"[SIMULATED] GetCurrentUser response: User {user.Username}");
                        return user;
                    }
                    
                    // If current user doesn't exist in simulated data, create a new one
                    var newCurrentUser = new User
                    {
                        Id = _authService.CurrentUserId ?? "user_current",
                        Email = "current@heticstream.com",
                        Username = "Current User",
                        AvatarUrl = "avatar_current.png",
                        IsOnline = true,
                        LastSeen = DateTime.Now
                    };
                    
                    _simulatedUsers.Add(newCurrentUser.Id, newCurrentUser);
                    return newCurrentUser;
                }
                
                // Set the authentication token in the request header
                _httpClient.DefaultRequestHeaders.Authorization = 
                    new AuthenticationHeaderValue("Bearer", _authService.CurrentToken);
                
                // Make the API request
                var response = await _httpClient.GetAsync($"{_settings.ApiBaseUrl}/users/me");
                
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<User>();
                }
                
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting current user: {ex.Message}");
                return null;
            }
        }
        
        public async Task<User?> GetUserByIdAsync(string userId)
        {
            try
            {
                // If API is disabled, return simulated data
                if (!_settings.ApiEnabled)
                {
                    Console.WriteLine($"[SIMULATED] GetUserById request for user: {userId}");
                    
                    if (_simulatedUsers.TryGetValue(userId, out var user))
                    {
                        Console.WriteLine($"[SIMULATED] GetUserById response: User {user.Username}");
                        return user;
                    }
                    
                    // If user doesn't exist in simulated data, return null
                    return null;
                }
                
                // Set the authentication token in the request header
                if (_authService.IsAuthenticated)
                {
                    _httpClient.DefaultRequestHeaders.Authorization = 
                        new AuthenticationHeaderValue("Bearer", _authService.CurrentToken);
                }
                
                // Make the API request
                var response = await _httpClient.GetAsync($"{_settings.ApiBaseUrl}/users/{userId}");
                
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<User>();
                }
                
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting user: {ex.Message}");
                return null;
            }
        }
        
        public async Task<IEnumerable<User>> GetOnlineUsersInChannelAsync(string channelId)
        {
            try
            {
                // If API is disabled, return simulated data
                if (!_settings.ApiEnabled)
                {
                    Console.WriteLine($"[SIMULATED] GetOnlineUsersInChannel request for channel: {channelId}");
                    
                    // Return all online users from simulated data
                    var onlineUsers = _simulatedUsers.Values.Where(u => u.IsOnline).ToList();
                    Console.WriteLine($"[SIMULATED] GetOnlineUsersInChannel response: {onlineUsers.Count} online users");
                    
                    return onlineUsers;
                }
                
                // Set the authentication token in the request header
                if (_authService.IsAuthenticated)
                {
                    _httpClient.DefaultRequestHeaders.Authorization = 
                        new AuthenticationHeaderValue("Bearer", _authService.CurrentToken);
                }
                
                // Make the API request
                var response = await _httpClient.GetAsync($"{_settings.ApiBaseUrl}/channels/{channelId}/users/online");
                
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<OnlineUsersResponse>();
                    return result?.Users ?? Enumerable.Empty<User>();
                }
                
                return Enumerable.Empty<User>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting online users: {ex.Message}");
                return Enumerable.Empty<User>();
            }
        }
        
        // Response model for API deserialization
        private class OnlineUsersResponse
        {
            public List<User> Users { get; set; } = new List<User>();
        }
    }
}