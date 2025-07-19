namespace SurveyManagementSystemApi.Securty.Filters
{

    public class PermissionRequirement(string permission) : IAuthorizationRequirement
    {
        public string Permission { get; } = permission;
    }

}
