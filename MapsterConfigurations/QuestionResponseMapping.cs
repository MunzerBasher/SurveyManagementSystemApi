namespace SurveyManagementSystemApi.MapsterConfigurations
{

    public class QuestionResponseMapping : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
           config.NewConfig<Question, QuestionResponse>().Map(
               dest => dest.Contant, sour => sour.Content
               );
            config.NewConfig<Question, QuestionResponse>().Map(
              dest => dest.Id, sour => sour.Id
              );
            config.NewConfig<Question, QuestionResponse>().Map(
              dest => dest.AnswerResponses, sour => sour.Answers
              );
        }

    }
}
