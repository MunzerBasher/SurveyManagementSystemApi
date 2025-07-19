namespace SurveyManagementSystemApi.Contracts.Roles
{
    public class RoleResponse
    {

        public string? Id { get; set; }  
        public string Name { get; set; } = string.Empty;
        public bool IsDeleted { get; set; } 
    }
}
