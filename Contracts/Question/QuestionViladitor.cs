namespace SurveyManagementSystemApi.Contracts.Question
{
    public class QuestionViladitor : AbstractValidator<QuestionRequest>   
    {
        public QuestionViladitor() {

            RuleFor(x => x.Contant).NotEmpty().Length(3, 1000);
            RuleFor(x => x.Answers).Must(x => x.Count > 1).WithMessage("Question Should Has at least 2 answer");
            RuleFor(x => x.Answers).Must(x => x.Count == x.Distinct().Count()).WithMessage("You Can't add Duoblicated answers fro same Question");
        }
    }
}
