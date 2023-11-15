namespace Web.DTOs
{
    public class MarkReportDTO
    {
        public int MarkReportId { get; set; }

        public int UserId { get; set; }

        public double? MarkScore { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime UpdatedDate { get; set; }

        public string? CreatedBy { get; set; }

        public string? UpdatedBy { get; set; }
    }
}
