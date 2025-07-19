namespace SurveyBasket.Errors;

public record RoleErrors
{
    public static readonly Error RoleNotFound =
        new("Role is not found", StatusCodes.Status404NotFound);

    public static readonly Error InvalidPermissions =
        new("Invalid permissions", StatusCodes.Status400BadRequest);

    public static readonly Error DuplicatedRole =
        new( "Another role with the same name is already exists", StatusCodes.Status409Conflict);
}