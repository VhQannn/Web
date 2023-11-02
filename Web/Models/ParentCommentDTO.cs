namespace Web.Models
{
	public class ParentCommentDTO
	{
		public int PostId { get; set; }
		public string Content { get; set; }
		public decimal Price { get; set; }
		public int UserId { get; set; }
		public DateTime CommentDate { get; set; }
	}


}
