using System.Collections.Concurrent;
using System.Security.Claims;
using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Project.AppContext;
using Project.DTO;
using Project.Interfaces;
using Project.Models;
using Project.Utils;

namespace Project.Hubs
{
    [Authorize]
    public class ChatHub(
        IMapper mapper,
        IConversationService conversationService,
        IMessageService messageService,
        DataContext context
    ) : Hub
    {
        private static readonly ConcurrentDictionary<string, string> ConnectedUsers = new();

        private readonly IMapper _mapper = mapper;
        private readonly DataContext _context = context;
        private readonly IConversationService _conversationService = conversationService;
        private readonly IMessageService _messageService = messageService;

        public override async Task OnConnectedAsync()
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(userId))
            {
                ConnectedUsers[userId] = Context.ConnectionId;
                Console.WriteLine($"User {userId} connected. ConnectionId: {Context.ConnectionId}");
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(userId))
            {
                ConnectedUsers.TryRemove(userId, out _);
                Console.WriteLine($"User {userId} disconnected.");
            }

            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendPrivateMessage(MessageRequest request)
        {
            var senderId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(senderId))
                throw new HubException("Unauthorized");

            var message = _mapper.Map<Message>(request);
            message.SenderId = Guid.Parse(senderId);

            if (request.RecipientId == null)
            {
                var managerIds = _context
                    .Users.Where(u => u.Role.Equals(UserType.Manager))
                    .Select(u => u.Id.ToString())
                    .ToList();

                foreach (var managerId in managerIds)
                {
                    var conversation = await _conversationService.FindOrCreateConversation(
                        Guid.Parse(senderId),
                        Guid.Parse(managerId)
                    );

                    var msg = new Message
                    {
                        Id = Guid.NewGuid(),
                        Content = message.Content,
                        SenderId = message.SenderId,
                        ConversationId = conversation.Id,
                        Timestamp = message.Timestamp,
                    };

                    message.Id = Guid.NewGuid();
                    var success = await _messageService.CreateMessage(msg);
                    if (success)
                    {
                        await Clients
                            .User(managerId)
                            .SendAsync("ReceivePrivateMessage", senderId, request.Content);
                    }
                }

                // Gửi lại cho chính người gửi để hiển thị
                await Clients
                    .User(senderId)
                    .SendAsync("ReceivePrivateMessage", senderId, request.Content);
            }
            else
            {
                // Gửi tin nhắn tới một người cụ thể (RecipientId)
                var recipientId = request.RecipientId.ToString();

                var conversation = await _conversationService.FindOrCreateConversation(
                    Guid.Parse(senderId),
                    Guid.Parse(recipientId)
                );

                message.ConversationId = conversation.Id;
                message.Timestamp = DateTime.UtcNow;

                message.Id = Guid.NewGuid();
                var success = await _messageService.CreateMessage(message);
                if (!success)
                    throw new HubException("Failed to save message");

                await Clients
                    .User(recipientId)
                    .SendAsync("ReceivePrivateMessage", senderId, request.Content);

                await Clients
                    .User(senderId)
                    .SendAsync("ReceivePrivateMessage", senderId, request.Content);
            }
        }

        public async Task GetChatHistory(string withUserId)
        {
            var currentUserId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(currentUserId) || string.IsNullOrEmpty(withUserId))
                throw new HubException("Invalid users");

            List<Message> history = await _conversationService.GetMessagesByUserId(
                Guid.Parse(currentUserId),
                Guid.Parse(withUserId)
            );
            Console.WriteLine($"History: {history.Count}");

            await Clients.Caller.SendAsync("LoadChatHistory", history);
        }
    }
}
