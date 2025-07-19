namespace SurveyBasket.Errors;

public record QuestionErrors
{
    public static readonly Error QuestionNotFound =
        new( "No question was found with the given ID", StatusCodes.Status404NotFound);

    public static readonly Error DuplicatedQuestionContent =
        new("Another question with the same content is already exists", StatusCodes.Status409Conflict);


    public static readonly Error InternalServerError =
       new(" Internal Server Error ", StatusCodes.Status500InternalServerError);
}