using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace MyHealthcareApp.Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(int chatId, string sender, string content)
        {
            // Log the received message to the terminal
            Console.WriteLine($"Received message - ChatId: {chatId}, Sender: {sender}, Content: {content}");

            // Broadcast the message to all clients in the chat group
            await Clients.Group(chatId.ToString()).SendAsync("ReceiveMessage", chatId, sender, content);
        }

        public async Task JoinRoom(string roomId)
        {
            Console.WriteLine($"Client {Context.ConnectionId} joining room {roomId}");
            await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
            await Clients.Group(roomId).SendAsync("UserJoined", Context.ConnectionId);
        }

        public async Task JoinChat(string chatId)
        {
            await JoinRoom(chatId);
        }

        public async Task LeaveRoom(string roomId)
        {
            Console.WriteLine($"Client {Context.ConnectionId} leaving room {roomId}");
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId);
            await Clients.Group(roomId).SendAsync("UserLeft", Context.ConnectionId);
        }

        public override async Task OnConnectedAsync()
        {
            Console.WriteLine($"Client connected - ConnectionId: {Context.ConnectionId}");
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            Console.WriteLine($"Client {Context.ConnectionId} disconnected");
            await base.OnDisconnectedAsync(exception);
        }
    }
}
