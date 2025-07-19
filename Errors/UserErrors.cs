namespace SurveyBasket.Errors;

public record UserErrors
{
    public static readonly Error InvalidCredentials =
        new("Invalid email/password", StatusCodes.Status401Unauthorized);

    public static readonly Error DisabledUser =
        new( "Disabled user, please contact your administrator", StatusCodes.Status401Unauthorized);

    public static readonly Error LockedUser =
        new("Locked user, please contact your administrator", StatusCodes.Status401Unauthorized);

    public static readonly Error InvalidJwtToken =
        new( "Invalid Jwt token", StatusCodes.Status401Unauthorized);

    public static readonly Error InvalidRefreshToken =
        new("Invalid refresh token", StatusCodes.Status401Unauthorized);

    public static readonly Error DuplicatedEmail =
        new("Another user with the same email is already exists", StatusCodes.Status409Conflict);

    public static readonly Error EmailNotConfirmed =
        new("Email is not confirmed", StatusCodes.Status401Unauthorized);

    public static readonly Error InvalidCode =
        new("Invalid code", StatusCodes.Status401Unauthorized);

    public static readonly Error DuplicatedConfirmation =
        new("Email already confirmed", StatusCodes.Status400BadRequest);

    public static readonly Error UserNotFound =
    new("User is not found", StatusCodes.Status404NotFound);

    public static readonly Error InvalidRoles =
        new( "Invalid roles", StatusCodes.Status400BadRequest);
}