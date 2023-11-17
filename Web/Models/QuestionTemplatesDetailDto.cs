namespace Web.Models
{
    public class QuestionTemplatesDetailDto
    {
        public int QID { get; set; }
        public int[] QAIDs { get; set; }
        public string ImageURL { get; set; }
        public string Qtext { get; set; }
    }
}
