namespace Web.Models
{
	public class WithdrawalRequestDTO
	{
		public decimal Amount { get; set; } // Số tiền muốn rút
		public string Comments { get; set; } // Bình luận hoặc ghi chú cho yêu cầu
	}
}
