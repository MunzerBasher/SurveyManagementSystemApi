namespace SurveyManagementSystemApi.Securty.Filters
{
    public class HasPermissionAttribute(string permission) : AuthorizeAttribute(permission)
    {

    }

}
