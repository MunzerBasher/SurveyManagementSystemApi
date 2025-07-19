namespace SurveyManagementSystemApi.Contracts.Answer
{
    public class AnswerResponse
    {
        public int Id { get; set; }
        public string Contant { get; set; } = string.Empty;
        public bool IsActive { get; set; }  
    }
}
