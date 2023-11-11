namespace Web.Models
{
	public class RatingDTO
	{
		public int? SupporterId { get; set; }
		public int? RelatedId { get; set; }

		public string? ServiceType { get; set; }

		public int? RatingValue { get; set; }

		public string? Comments { get; set; }

		public DateTime? RatingDate { get; set; }
	}
}
