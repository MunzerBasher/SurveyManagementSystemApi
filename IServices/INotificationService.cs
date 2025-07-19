namespace SurveyManagementSystemApi.IServices
{
    public interface INotificationService
    {

        public  Task SendNewPollsNotification(int? pollId = null);
    }
}
