namespace SurveyManagementSystemApi.Contracts.Vote
{
    public class VoteAnswersValid : AbstractValidator<VoteAnswers>
    {
        public VoteAnswersValid()
        {
            RuleFor(x => x.AnswerId > 0).NotNull();
            RuleFor(x => x.QuestionId > 0).NotNull();
        }
    }
}
