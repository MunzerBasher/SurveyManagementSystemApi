namespace SurveyManagementSystemApi.Contracts.User
{
    public class UserResponse
    {
         public string Id { get; set; }
         public string FirstName { get; set; } = string.Empty;
         public string LastName {  get; set; } = string.Empty;
         public string Email {  get; set; } = string.Empty;
         public bool IsDisabled { get; set; }
         public IEnumerable<string> Roles { get; set; } = [];
    }

}
