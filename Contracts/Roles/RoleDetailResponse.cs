namespace SurveyManagementSystemApi.Contracts.Roles
{
    public class RoleDetailResponse
    {
        public string? Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsDeleted { get; set; }
        public IEnumerable<string> Permissions { get; set; } = Enumerable.Empty<string>();
    
    }
}
