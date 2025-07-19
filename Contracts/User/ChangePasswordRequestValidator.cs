namespace SurveyManagementSystemApi.Contracts.User
{
    public class ChangePasswordRequestValidator : AbstractValidator<ChangePasswordRequest>
    {
        public ChangePasswordRequestValidator()
        {
            RuleFor(x => x.OldPasswor)
                .NotEmpty();

            RuleFor(x => x.NewPasswor)
                .NotEmpty()
                .Matches(RegexPatterns.Password)
                .WithMessage("Password should be at least 8 digits and should contains Lowercase, NonAlphanumeric and Uppercase")
                .NotEqual(x => x.OldPasswor)
                .WithMessage("New password cannot be same as the current password");
        }
    }
}
