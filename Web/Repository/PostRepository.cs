using Web.DbConnection;
using Web.IRepository;

namespace Web.Repository
{
    public class PostRepository : IPostRepository
    {
        private readonly WebContext _context;

        public PostRepository(WebContext context)
        {
            _context = context;
        }

        public int? GetPostOwnerId(int postId)
        {
            return _context.Posts
                           .Where(p => p.PostId == postId)
                           .Select(p => p.UserId)
                           .FirstOrDefault();
        }
    }
}
