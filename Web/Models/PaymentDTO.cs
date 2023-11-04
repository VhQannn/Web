namespace Web.Models
{
	public class PaymentDTO
	{
		public decimal Amount { get; set; }
		public int? RelatedId { get; set; }
		public string ServiceType { get; set; }
		public string Status { get; set; }
	}
}
