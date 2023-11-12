
namespace Web.Models
{
	public class MyAssignment
	{
		public AccountDTO Poster { get; set; }

		public int PostId { get; set; }

		public DateTime DateSlot { get; set; }

		public string? TimeSlot { get; set; }

		public string? Status { get; set; }

		public RatingDTO Rating { get; set; }
	}
}
