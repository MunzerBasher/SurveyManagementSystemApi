using FluentValidation;

namespace SurveyManagementSystemApi.Contracts.User
{
    public class RegisterValidator : AbstractValidator<RegisterRequest>
    {

        public RegisterValidator() 
        {

            RuleFor(x => x.Email).NotEmpty();
            RuleFor(x => x.Password).NotEmpty();
            RuleFor(x => x.FirstName).Length(3, 100);
            RuleFor(x => x.LastName).Length(3, 100);

        }

    }
}
