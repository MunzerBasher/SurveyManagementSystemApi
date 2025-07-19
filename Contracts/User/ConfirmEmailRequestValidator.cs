namespace SurveyManagementSystemApi.Contracts.User
{
    public class ConfirmEmailRequestValidator :AbstractValidator<ConfirmEmailRequest>
    {


        public ConfirmEmailRequestValidator() 
        {
            RuleFor(x =>x.Code).NotEmpty();
            RuleFor(x => x.UserId).NotEmpty().GreaterThanOrEqualTo(1);  
        }
    }
}
