namespace Web.Models
{
    public class RatingCommentDTO
    {
        public string Comment { get; set; }
        public string RaterName { get; set; }
        public double RatingValue { get; set; }
        public DateTime? RatingDate { get; set; }
    }
}
