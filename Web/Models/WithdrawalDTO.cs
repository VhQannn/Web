using Web.DbConnection;

namespace Web.Models
{
	public class WithdrawalDTO
	{
		public int WithdrawalRequestId { get; set; }

		public int? PaymentId { get; set; }

		public DateTime? RequestDate { get; set; }

		public string? Status { get; set; }

		public string? Comments { get; set; }
	}
}
