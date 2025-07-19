namespace SurveyBasket.Errors;

public record PollErrors
{
    public static readonly Error PollNotFound =
        new( "No poll was found with the given ID", StatusCodes.Status404NotFound);


    public static readonly Error PollBadRequest =
        new("Invalid Value ", StatusCodes.Status400BadRequest);


    public static readonly Error DuplicatedPollTitle =
        new("Another poll with the same title is already exists", StatusCodes.Status409Conflict);

    public static readonly Error InternalServerError =
        new("Internal Server Error", StatusCodes.Status409Conflict);

}