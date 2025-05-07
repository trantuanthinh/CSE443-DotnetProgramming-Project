using System.Collections.Concurrent;
using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Project.DTO;
using Project.Interfaces;
using Project.Models;

namespace Project.Hubs
{
    [Authorize]
    public class ChatHub(
        IMapper mapper,
        IConversationService conversationService,
        IMessageService messageService
    ) : Hub
    {
        // ConcurrentDictionary for thread-safe operations
        private static readonly ConcurrentDictionary<string, string> ConnectedUsers =
            new ConcurrentDictionary<string, string>();

        private readonly IMapper _mapper = mapper;
        private readonly IConversationService _conversationService = conversationService;
        private readonly IMessageService _messageService = messageService;

        public override async Task OnConnectedAsync()
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!string.IsNullOrEmpty(userId))
            {
                ConnectedUsers[userId] = Context.ConnectionId; // Map userId to connectionId
                Console.WriteLine($"User {userId} connected. ConnectionId: {Context.ConnectionId}");
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!string.IsNullOrEmpty(userId))
            {
                ConnectedUsers.TryRemove(userId, out _); // Remove user from ConnectedUsers
                Console.WriteLine($"User {userId} disconnected.");
            }

            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendPrivateMessage(string recipientId, MessageRequest item)
        {
            var senderId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(senderId))
            {
                throw new HubException("Unauthorized user.");
            }

            // Find or create the conversation
            Conversation conversation = await _conversationService.FindOrCreateConversation(
                Guid.Parse(senderId),
                Guid.Parse(recipientId)
            );
            Message _item = _mapper.Map<Message>(item);
            _item.ConversationId = conversation.Id;

            // Check if the recipient is connected
            if (ConnectedUsers.TryGetValue(recipientId, out var recipientConnectionId))
            {
                // Send the message to the recipient
                if (_item.Content != null)
                {
                    await SendTextMessageToRecipient(
                        recipientConnectionId,
                        senderId,
                        _item.Content
                    );
                }

                async Task SendTextMessageToRecipient(
                    string recipientConnectionId,
                    string senderId,
                    string content
                )
                {
                    await Clients
                        .Client(recipientConnectionId)
                        .SendAsync("ReceivePrivateMessage", senderId, content);
                }

                // Notify the sender of successful delivery
                await Clients
                    .Client(Context.ConnectionId)
                    .SendAsync("SendPrivateMessageStatus", "Message sent.");

                // Save the message to the conversation
                await _messageService.CreateMessage(_item);
            }
            else
            {
                // Notify the sender if the recipient is not available
                await Clients
                    .Client(Context.ConnectionId)
                    .SendAsync("SendPrivateMessageStatus", "User not available.");

                // Save the message to the conversation for later delivery
                await _messageService.CreateMessage(_item);
            }
        }

        public async Task SendMessage(string user, string message)
        {
            Console.WriteLine($"Received message from {user}: {message}");
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        public async Task GetChatHistory(string senderId, string recipientId)
        {
            if (string.IsNullOrEmpty(senderId) || string.IsNullOrEmpty(recipientId))
            {
                throw new HubException("Invalid sender or recipient.");
            }

            try
            {
                List<MessageResponse> messages = await _conversationService.GetMessagesByUserId(
                    Guid.Parse(senderId),
                    Guid.Parse(recipientId)
                );
                await Clients.Caller.SendAsync("LoadChatHistory", messages);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching chat history: {ex.Message}");
                await Clients.Caller.SendAsync("LoadChatHistory", Array.Empty<string>());
            }
        }
    }
}
