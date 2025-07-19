namespace SurveyManagementSystemApi.Contracts.Question
{
    public class QuestionResponse
    {
        public int Id { get; set; }
        public string Contant { get; set; } = string.Empty;
        public IEnumerable<AnswerResponse> AnswerResponses { get; set; } = [];


    }




}