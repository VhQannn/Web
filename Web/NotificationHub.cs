using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Web
{
	public class NotificationHub : Hub
	{
        public override Task OnConnectedAsync()
        {
            var userId = Context.User.Identity.Name;
            Groups.AddToGroupAsync(Context.ConnectionId, userId);

            return base.OnConnectedAsync();
        }

        public async Task ReceivedWebHook()
		{
			await Clients.Others.SendAsync("ProcessPayment");
		}

		public async Task NewWithdrawalRequest()
		{
			await Clients.Others.SendAsync("NewWithdrawalRequest");
		}


	}
}
