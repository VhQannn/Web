namespace Web.Models
{
    public class QuestionTemplateDto
    {
        public string QuestionTemplateCode { get; set; }
        public string CreatedBy { get; set; }
        public List<QuestionTemplatesDetailDto> QuestionTemplatesDetails { get; set; }
    }
}
