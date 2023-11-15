namespace Web.Models
{
    public class QuestionTemplatesDetailDto
    {
        public int QId { get; set; }
        public int QAid { get; set; }
        public string QText { get; set; }

        public IFormFile ImageFile { get; set; }
    }
}
