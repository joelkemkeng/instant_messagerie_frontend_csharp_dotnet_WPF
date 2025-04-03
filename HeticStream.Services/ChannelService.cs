// HETIC - Channel service implementation for Hetic-Stream
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using HeticStream.Core.Config;
using HeticStream.Core.Models;
using HeticStream.Core.Services;

namespace HeticStream.Services
{
    public class ChannelService : IChannelService
    {
        private readonly HttpClient _httpClient;
        private readonly IAuthService _authService;
        private readonly AppSettings _settings;
        
        // In-memory cache for simulated data
        private static readonly List<Channel> _simulatedChannels = new();
        private static readonly Dictionary<string, List<Message>> _simulatedMessages = new();
        
        public ChannelService(HttpClient httpClient, IAuthService authService)
        {
            _httpClient = httpClient;
            _authService = authService;
            _settings = AppSettings.Instance;
            
            InitializeSimulatedData();
        }
        
        private void InitializeSimulatedData()
        {
            // Only initialize if we're in simulation mode and data is not already initialized
            if (!_settings.ApiEnabled && !_simulatedChannels.Any())
            {
                // Create some simulated channels
                var generalChannel = new Channel
                {
                    Id = "channel_1",
                    Name = "General",
                    Type = ChannelType.Group,
                    LastActivity = DateTime.Now,
                    LastMessagePreview = "Welcome to Hetic-Stream!"
                };
                
                var randomChannel = new Channel
                {
                    Id = "channel_2",
                    Name = "Random",
                    Type = ChannelType.Group,
                    LastActivity = DateTime.Now.AddMinutes(-30),
                    LastMessagePreview = "What's everyone up to?"
                };
                
                var directChannel = new Channel
                {
                    Id = "channel_3",
                    Name = "Jane Doe",
                    Type = ChannelType.Direct,
                    LastActivity = DateTime.Now.AddHours(-2),
                    LastMessagePreview = "Let me know when you're available"
                };
                
                _simulatedChannels.Add(generalChannel);
                _simulatedChannels.Add(randomChannel);
                _simulatedChannels.Add(directChannel);
                
                // Create some simulated messages
                var generalMessages = new List<Message>
                {
                    new Message
                    {
                        Id = "msg_1",
                        Content = "Welcome to Hetic-Stream!",
                        AuthorId = "user_admin",
                        AuthorName = "Admin",
                        Timestamp = DateTime.Now.AddDays(-1),
                        IsRead = true
                    },
                    new Message
                    {
                        Id = "msg_2",
                        Content = "Thanks for joining our platform.",
                        AuthorId = "user_admin",
                        AuthorName = "Admin",
                        Timestamp = DateTime.Now.AddDays(-1).AddMinutes(5),
                        IsRead = true
                    },
                    new Message
                    {
                        Id = "msg_3",
                        Content = "Feel free to explore the channels and features!",
                        AuthorId = "user_admin",
                        AuthorName = "Admin",
                        Timestamp = DateTime.Now.AddHours(-3),
                        IsRead = true
                    }
                };
                
                var randomMessages = new List<Message>
                {
                    new Message
                    {
                        Id = "msg_4",
                        Content = "Hey everyone!",
                        AuthorId = "user_jane",
                        AuthorName = "Jane",
                        Timestamp = DateTime.Now.AddHours(-5),
                        IsRead = true
                    },
                    new Message
                    {
                        Id = "msg_5",
                        Content = "What's everyone up to?",
                        AuthorId = "user_jane",
                        AuthorName = "Jane",
                        Timestamp = DateTime.Now.AddMinutes(-30),
                        IsRead = true
                    }
                };
                
                var directMessages = new List<Message>
                {
                    new Message
                    {
                        Id = "msg_6",
                        Content = "Hi there!",
                        AuthorId = "user_jane",
                        AuthorName = "Jane",
                        Timestamp = DateTime.Now.AddHours(-3),
                        IsRead = true
                    },
                    new Message
                    {
                        Id = "msg_7",
                        Content = "I wanted to discuss the project",
                        AuthorId = "user_jane",
                        AuthorName = "Jane",
                        Timestamp = DateTime.Now.AddHours(-3).AddMinutes(1),
                        IsRead = true
                    },
                    new Message
                    {
                        Id = "msg_8",
                        Content = "Let me know when you're available",
                        AuthorId = "user_jane",
                        AuthorName = "Jane",
                        Timestamp = DateTime.Now.AddHours(-2),
                        IsRead = false
                    }
                };
                
                _simulatedMessages.Add(generalChannel.Id, generalMessages);
                _simulatedMessages.Add(randomChannel.Id, randomMessages);
                _simulatedMessages.Add(directChannel.Id, directMessages);
            }
        }
        
        public async Task<IEnumerable<Channel>> GetChannelsAsync()
        {
            try
            {
                // If API is disabled, return simulated data
                if (!_settings.ApiEnabled)
                {
                    Console.WriteLine("[SIMULATED] GetChannels request");
                    Console.WriteLine($"[SIMULATED] GetChannels response: {_simulatedChannels.Count} channels");
                    
                    return _simulatedChannels;
                }
                
                // Set the authentication token in the request header
                if (_authService.IsAuthenticated)
                {
                    _httpClient.DefaultRequestHeaders.Authorization = 
                        new AuthenticationHeaderValue("Bearer", _authService.CurrentToken);
                }
                
                // Make the API request
                var response = await _httpClient.GetAsync($"{_settings.ApiBaseUrl}{_settings.EndpointChannels}");
                
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<ChannelsResponse>();
                    return result?.Channels ?? Enumerable.Empty<Channel>();
                }
                
                return Enumerable.Empty<Channel>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting channels: {ex.Message}");
                return Enumerable.Empty<Channel>();
            }
        }
        
        public async Task<IEnumerable<Message>> GetMessagesAsync(string channelId)
        {
            try
            {
                // If API is disabled, return simulated data
                if (!_settings.ApiEnabled)
                {
                    Console.WriteLine($"[SIMULATED] GetMessages request for channel: {channelId}");
                    
                    if (_simulatedMessages.TryGetValue(channelId, out var messages))
                    {
                        Console.WriteLine($"[SIMULATED] GetMessages response: {messages.Count} messages");
                        return messages;
                    }
                    
                    // If channel doesn't exist in simulated data, return empty list
                    return Enumerable.Empty<Message>();
                }
                
                // Set the authentication token in the request header
                if (_authService.IsAuthenticated)
                {
                    _httpClient.DefaultRequestHeaders.Authorization = 
                        new AuthenticationHeaderValue("Bearer", _authService.CurrentToken);
                }
                
                // Make the API request
                var response = await _httpClient.GetAsync(
                    $"{_settings.ApiBaseUrl}{_settings.EndpointMessages}?channelId={channelId}");
                
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<MessagesResponse>();
                    return result?.Messages ?? Enumerable.Empty<Message>();
                }
                
                return Enumerable.Empty<Message>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting messages: {ex.Message}");
                return Enumerable.Empty<Message>();
            }
        }
        
        public async Task<Message?> SendMessageAsync(string channelId, string content)
        {
            try
            {
                // Create message request model
                var messageModel = new
                {
                    channelId,
                    content
                };
                
                // If API is disabled, simulate sending a message
                if (!_settings.ApiEnabled)
                {
                    Console.WriteLine($"[SIMULATED] SendMessage request: {JsonSerializer.Serialize(messageModel)}");
                    
                    // Create a simulated message
                    var message = new Message
                    {
                        Id = $"msg_{Guid.NewGuid():N}",
                        Content = content,
                        AuthorId = _authService.CurrentUserId ?? "user_simulated",
                        AuthorName = "Current User",
                        Timestamp = DateTime.Now,
                        IsRead = true
                    };
                    
                    // Add the message to the simulated data
                    if (_simulatedMessages.TryGetValue(channelId, out var messages))
                    {
                        messages.Add(message);
                        
                        // Update the last message preview in the channel
                        var channel = _simulatedChannels.FirstOrDefault(c => c.Id == channelId);
                        if (channel != null)
                        {
                            channel.LastActivity = DateTime.Now;
                            channel.LastMessagePreview = content.Length > 30 
                                ? content.Substring(0, 27) + "..." 
                                : content;
                        }
                    }
                    else
                    {
                        _simulatedMessages[channelId] = new List<Message> { message };
                    }
                    
                    Console.WriteLine($"[SIMULATED] SendMessage response: Message sent with ID {message.Id}");
                    
                    return message;
                }
                
                // Set the authentication token in the request header
                if (_authService.IsAuthenticated)
                {
                    _httpClient.DefaultRequestHeaders.Authorization = 
                        new AuthenticationHeaderValue("Bearer", _authService.CurrentToken);
                }
                
                // Make the API request
                var response = await _httpClient.PostAsJsonAsync(
                    $"{_settings.ApiBaseUrl}{_settings.EndpointMessages}", 
                    messageModel);
                
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<Message>();
                }
                
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending message: {ex.Message}");
                return null;
            }
        }
        
        // Response models for API deserialization
        private class ChannelsResponse
        {
            public List<Channel> Channels { get; set; } = new List<Channel>();
        }
        
        private class MessagesResponse
        {
            public List<Message> Messages { get; set; } = new List<Message>();
        }
    }
}