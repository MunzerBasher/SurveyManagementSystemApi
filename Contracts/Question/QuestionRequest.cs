namespace SurveyManagementSystemApi.Contracts.Question
{
    public record QuestionRequest
    {
        public string Contant { get; set; } = string.Empty;
        public int pollId { get; set; }
        public List<string> Answers { get; set; } = [];

 
    }


}