using Microsoft.AspNetCore.SignalR;

namespace Web
{
	public class ChatHub : Hub
	{
		public async Task SendMessage(string receiverId)
		{
			// Gửi tin nhắn đến người nhận cụ thể
			await Clients.User(receiverId).SendAsync("ReceiveMessage");
		}
	}
}
