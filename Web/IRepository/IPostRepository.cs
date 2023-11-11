namespace Web.IRepository
{
    public interface IPostRepository
    {
        int? GetPostOwnerId(int postId);
    }
}
