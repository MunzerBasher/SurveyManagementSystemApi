namespace SurveyManagementSystemApi.Services
{
    public class Error
    {
        public Error (string message, int? statusCode = null)
        {
            this.Message = message;
            this.statusCode = statusCode;
        }
        public string Message { get; }
        public int ? statusCode { get; }


    }


}