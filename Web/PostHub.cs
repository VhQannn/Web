using Microsoft.AspNetCore.SignalR;

namespace Web
{
    public class PostHub : Hub
    {
        public async Task NewPostAdded()
        {
            await Clients.Others.SendAsync("UpdatePosts");
        }
    }
}
