namespace Web.Models
{
	public class MyPaymentSupporterDTO
	{
		public int PaymentId { get; set; }
		public AccountDTO User { get; set; }
		public decimal Amount { get; set; }
		public DateTime? PaymentDate { get; set; }
		public string? Status { get; set; }
		public int? RelatedId { get; set; }
		public string ServiceType { get; set; }
		public AccountDTO Receiver { get; set; }

		public WithdrawalDTO WithdrawalRequest { get; set; }
	}
}
