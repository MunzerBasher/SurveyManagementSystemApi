using Mapster;

namespace SurveyManagementSystemApi.MapsterConfigurations
{
    public class QuestionRequestMapping : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<QuestionRequest, Question>().Map(dest => dest.Content, sour => sour.Contant);
            config.NewConfig<QuestionRequest, Question>().Map(dest => dest.PollId, sour => sour.pollId);
            
        }
    }
}
