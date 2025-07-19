namespace SurveyManagementSystemApi.Contracts.User
{
    public class ConfirmEmailRequest
    {
        public int UserId { get; set; }
        public string Code { get; set; }
    }
}
