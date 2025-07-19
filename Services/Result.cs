namespace SurveyManagementSystemApi.Services
{
    public class Result 
    {

        public Result(bool isSeccuss, Error? erorr)
        {
            if ((isSeccuss && erorr != null) || (!isSeccuss && erorr == null))
                new Exception();
            Erorr = erorr!;
            IsSuccess = isSeccuss;
            IsFialer = !isSeccuss;
        }

        public Error Erorr { get; }
        public bool IsSuccess { get; }
        public bool IsFialer { get; } 
    

        public static Result Success() => new Result(true, null);
        public static Result Failure(Error Erorr) => new Result(false, Erorr);

    }

    public class Result<T> : Result
    {
       
        public Result(T value,bool isSeccuss, Error? erorr) : base(isSeccuss, erorr)
        {
            Value = value;
        }

        private Result(bool isSeccuss, Error? erorr) : base(isSeccuss, erorr)
        {
        
        }

        public T? Value { get; }
        public static Result<T> Success<T>(T value) => new Result<T>(value, true, null);
        public static Result<T> Failure<T>(Error Erorr) => new Result<T>(false, Erorr);
       

    }


}