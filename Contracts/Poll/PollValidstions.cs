namespace SurveyManagementSystemApi.Contracts.Poll
{
    public class PollValidstions : AbstractValidator<PollRequest>
    {
        public PollValidstions()
        {
            RuleFor(x => x.Title).NotEmpty().Length(10, 30);
            RuleFor(c => c).Must(IsTrue)
                .WithName(nameof(PollRequest.EndAt))
                .WithMessage("Start Data is greater than End Data ");
            RuleFor(c => c.StartAt).GreaterThanOrEqualTo(DateOnly.FromDateTime(DateTime.Today));
        }


        private bool IsTrue(PollRequest value)
        {
            if (value.EndAt >= value.StartAt) return true;
            return false;
        }


    }
}
