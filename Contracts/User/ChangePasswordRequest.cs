namespace SurveyManagementSystemApi.Contracts.User
{
    public class ChangePasswordRequest
    {
        public string OldPasswor { get; set; } = string.Empty;
        public string NewPasswor { get; set; } = string.Empty;

    }


}
