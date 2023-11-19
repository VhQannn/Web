namespace Web.DTOs
{
	public class MessageDto
	{
		public int MessageId { get; set; }

		public string SenderName { get; set; }	
		public string SenderRole { get; set; }	

		public string MessageText { get; set; }	

		public string MessageType { get; set; }

		public DateTime? SentTime { get; set; }
	}
}
