namespace SurveyManagementSystemApi.Contracts.Roles
{
    public class RoleRequest
    {
        public string Name { get; set; } = string.Empty;                            
        public IList<string> Permissions {  get; set; } = new List<string>();
    }
}
