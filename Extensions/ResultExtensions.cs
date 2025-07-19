namespace SurveyManagementSystemApi.Extensions
{
    public static class ResultExtensions
    {

        public static ObjectResult ToProblem(this Result results)
        {
            if (results.IsSuccess)
                throw new InvalidOperationException("Can't Convert success result to a problem");
            var problem = Results.Problem(statusCode : results.Erorr.statusCode);
            var problemDetails = problem.GetType().GetProperty(nameof(ProblemDetails))!.GetValue(problem) as ProblemDetails;
            problemDetails!.Extensions = new Dictionary<string, object?>()
            { 
                {
                   
                    "erorr" , new 
                    {
                        results.Erorr.statusCode,
                        results.Erorr.Message
                    
                    }
                },

            };
             return new ObjectResult(problemDetails);
        }




    }
}
