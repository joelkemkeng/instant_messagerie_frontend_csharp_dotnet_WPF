# Hetic-Stream API Documentation

This document provides detailed information about the API endpoints required by the Hetic-Stream application.

## API Configuration

The API configuration is controlled by the following environment variables:

- `ApiBaseUrl`: Base URL for the API (default: https://api.heticstream.com)
- `ApiEnabled`: Boolean flag to enable or disable the real API (default: false)
- `EndpointLogin`: Endpoint path for login (default: /auth/login)
- `EndpointRegister`: Endpoint path for registration (default: /auth/register)
- `EndpointChannels`: Endpoint path for channels (default: /channels)
- `EndpointMessages`: Endpoint path for messages (default: /messages)

## Authentication Endpoints

### Login

**Endpoint**: POST `{ApiBaseUrl}{EndpointLogin}`

**Request**:
```json
{
  "email": "string",
  "password": "string"
}
```

**Response**:
```json
{
  "success": boolean,
  "token": "string",
  "userId": "string",
  "message": "string"
}
```

**Example Request**:
```json
{
  "email": "user@example.com",
  "password": "password123"
}
```

**Example Success Response**:
```json
{
  "success": true,
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "userId": "user_12345",
  "message": "Login successful"
}
```

**Example Error Response**:
```json
{
  "success": false,
  "token": "",
  "userId": "",
  "message": "Invalid email or password"
}
```

### Register

**Endpoint**: POST `{ApiBaseUrl}{EndpointRegister}`

**Request**:
```json
{
  "email": "string",
  "username": "string",
  "password": "string"
}
```

**Response**:
```json
{
  "success": boolean,
  "message": "string"
}
```

**Example Request**:
```json
{
  "email": "newuser@example.com",
  "username": "newuser",
  "password": "password123"
}
```

**Example Success Response**:
```json
{
  "success": true,
  "message": "Registration successful"
}
```

**Example Error Response**:
```json
{
  "success": false,
  "message": "Email already in use"
}
```

## Channel Endpoints

### Get Channels

**Endpoint**: GET `{ApiBaseUrl}{EndpointChannels}`

**Headers**:
- Authorization: Bearer {token}

**Response**:
```json
{
  "channels": [
    {
      "id": "string",
      "name": "string",
      "type": "direct|group",
      "members": [
        {
          "id": "string",
          "username": "string",
          "avatarUrl": "string"
        }
      ],
      "lastActivity": "date",
      "lastMessagePreview": "string"
    }
  ]
}
```

**Example Response**:
```json
{
  "channels": [
    {
      "id": "channel_1",
      "name": "General",
      "type": "group",
      "members": [
        {
          "id": "user_1",
          "username": "User1",
          "avatarUrl": "avatar_1.png"
        },
        {
          "id": "user_2",
          "username": "User2",
          "avatarUrl": "avatar_2.png"
        }
      ],
      "lastActivity": "2025-04-03T14:30:00Z",
      "lastMessagePreview": "Hello everyone!"
    },
    {
      "id": "channel_2",
      "name": "John Doe",
      "type": "direct",
      "members": [
        {
          "id": "user_1",
          "username": "User1",
          "avatarUrl": "avatar_1.png"
        },
        {
          "id": "user_3",
          "username": "John",
          "avatarUrl": "avatar_3.png"
        }
      ],
      "lastActivity": "2025-04-03T15:20:00Z",
      "lastMessagePreview": "Let me know when you're available"
    }
  ]
}
```

## Message Endpoints

### Get Messages

**Endpoint**: GET `{ApiBaseUrl}{EndpointMessages}?channelId={channelId}`

**Headers**:
- Authorization: Bearer {token}

**Response**:
```json
{
  "messages": [
    {
      "id": "string",
      "content": "string",
      "authorId": "string",
      "authorName": "string",
      "authorAvatarUrl": "string",
      "timestamp": "date",
      "isRead": boolean
    }
  ]
}
```

**Example Response**:
```json
{
  "messages": [
    {
      "id": "msg_1",
      "content": "Hello everyone!",
      "authorId": "user_1",
      "authorName": "User1",
      "authorAvatarUrl": "avatar_1.png",
      "timestamp": "2025-04-03T14:30:00Z",
      "isRead": true
    },
    {
      "id": "msg_2",
      "content": "Welcome to the channel!",
      "authorId": "user_2",
      "authorName": "User2",
      "authorAvatarUrl": "avatar_2.png",
      "timestamp": "2025-04-03T14:35:00Z",
      "isRead": true
    }
  ]
}
```

### Send Message

**Endpoint**: POST `{ApiBaseUrl}{EndpointMessages}`

**Headers**:
- Authorization: Bearer {token}

**Request**:
```json
{
  "channelId": "string",
  "content": "string"
}
```

**Response**: Returns the created message
```json
{
  "id": "string",
  "content": "string",
  "authorId": "string",
  "authorName": "string",
  "authorAvatarUrl": "string",
  "timestamp": "date",
  "isRead": boolean
}
```

**Example Request**:
```json
{
  "channelId": "channel_1",
  "content": "Hello everyone!"
}
```

**Example Response**:
```json
{
  "id": "msg_3",
  "content": "Hello everyone!",
  "authorId": "user_1",
  "authorName": "User1",
  "authorAvatarUrl": "avatar_1.png",
  "timestamp": "2025-04-03T15:40:00Z",
  "isRead": true
}
```

## User Endpoints

### Get Current User

**Endpoint**: GET `{ApiBaseUrl}/users/me`

**Headers**:
- Authorization: Bearer {token}

**Response**:
```json
{
  "id": "string",
  "email": "string",
  "username": "string",
  "avatarUrl": "string",
  "isOnline": boolean,
  "lastSeen": "date"
}
```

**Example Response**:
```json
{
  "id": "user_1",
  "email": "user@example.com",
  "username": "User1",
  "avatarUrl": "avatar_1.png",
  "isOnline": true,
  "lastSeen": "2025-04-03T15:40:00Z"
}
```

### Get User by ID

**Endpoint**: GET `{ApiBaseUrl}/users/{userId}`

**Headers**:
- Authorization: Bearer {token}

**Response**:
```json
{
  "id": "string",
  "email": "string",
  "username": "string",
  "avatarUrl": "string",
  "isOnline": boolean,
  "lastSeen": "date"
}
```

**Example Response**:
```json
{
  "id": "user_2",
  "email": "user2@example.com",
  "username": "User2",
  "avatarUrl": "avatar_2.png",
  "isOnline": false,
  "lastSeen": "2025-04-03T14:30:00Z"
}
```

### Get Online Users in Channel

**Endpoint**: GET `{ApiBaseUrl}/channels/{channelId}/users/online`

**Headers**:
- Authorization: Bearer {token}

**Response**:
```json
{
  "users": [
    {
      "id": "string",
      "username": "string",
      "avatarUrl": "string",
      "isOnline": boolean,
      "lastSeen": "date"
    }
  ]
}
```

**Example Response**:
```json
{
  "users": [
    {
      "id": "user_1",
      "username": "User1",
      "avatarUrl": "avatar_1.png",
      "isOnline": true,
      "lastSeen": "2025-04-03T15:40:00Z"
    },
    {
      "id": "user_3",
      "username": "User3",
      "avatarUrl": "avatar_3.png",
      "isOnline": true,
      "lastSeen": "2025-04-03T15:35:00Z"
    }
  ]
}
```

## Simulation Mode

When `ApiEnabled` is set to `false`, the application will simulate API calls and log requests and responses to the console. This is useful for development and testing without a real backend.