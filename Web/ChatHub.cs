using Microsoft.AspNetCore.SignalR;

namespace Web
{
	public class ChatHub : Hub
	{
		public override async Task OnConnectedAsync()
		{
			var user = Context.User;
			if (user.IsInRole("Admin"))
			{
				await Groups.AddToGroupAsync(Context.ConnectionId, "Admins");
			}
			await base.OnConnectedAsync();
		}

		public async Task SendMessageToGroup(string groupName, string message)
		{
			await Clients.Group(groupName).SendAsync("ReceiveMessage", message);
		}

		public async Task JoinGroup(string groupName)
		{
			await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
		}

		public async Task LeaveGroup(string groupName)
		{
			await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
		}

		public async Task NotifyMessageRead(string groupName, int messageId)
		{
			await Clients.Group(groupName).SendAsync("MessageRead", messageId);
		}

		public async Task NotifyTyping(string groupName)
		{
			await Clients.OthersInGroup(groupName).SendAsync("UserTyping");
		}

		public async Task NotifyCancelTyping(string groupName)
		{
			await Clients.OthersInGroup(groupName).SendAsync("UserCancelTyping");
		}

	}
}
