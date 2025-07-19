namespace SurveyBasket.Errors;

public record VoteErrors
{
    public static readonly Error InvalidQuestions =
        new( "Invalid questions", StatusCodes.Status400BadRequest);

    public static readonly Error InvalidAnswerForQuestions =
        new("Invalid Answer For questions", StatusCodes.Status400BadRequest);


    public static readonly Error DuplicatedVote =
        new( "This user already voted before for this poll", StatusCodes.Status409Conflict);
}