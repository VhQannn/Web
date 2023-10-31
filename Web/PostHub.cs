using Microsoft.AspNetCore.SignalR;

namespace Web
{
    public class PostHub : Hub
    {
        public async Task NewPostAdded()
        {
            await Clients.Others.SendAsync("UpdatePosts");
        }

		public async Task NotifyNewComment()
		{
			await Clients.All.SendAsync("NewComment");
		}

		public async Task NotifyNewChildComment()
		{
			await Clients.All.SendAsync("NewChildComment");
		}

	}
}
