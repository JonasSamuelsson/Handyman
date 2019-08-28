using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Handyman.Azure.Functions
{
    public class FunctionResult : ActionResult
    {
        public FunctionResult()
        {
        }

        public FunctionResult(ProblemDetails problemDetails)
        {
            Result = problemDetails;
        }

        public FunctionResult(ActionResult result)
        {
            Result = result;
        }

        public ActionResult Result { get; }

        public static implicit operator FunctionResult(ProblemDetails problemDetails)
        {
            return new FunctionResult(problemDetails);
        }

        public override Task ExecuteResultAsync(ActionContext context)
        {
            return Result?.ExecuteResultAsync(context) ?? new OkResult().ExecuteResultAsync(context);
        }
    }

    public class FunctionResult<T> : ActionResult
    {
        public FunctionResult(T value)
        {
            Value = value;
        }

        public FunctionResult(ProblemDetails problemDetails)
        {
            Result = problemDetails;
        }

        public FunctionResult(ActionResult result)
        {
            Result = result;
        }

        public ActionResult Result { get; }
        public T Value { get; }

        public static implicit operator FunctionResult<T>(T value)
        {
            return new FunctionResult<T>(value);
        }

        public static implicit operator FunctionResult<T>(ProblemDetails problemDetails)
        {
            return new FunctionResult<T>(problemDetails);
        }

        public override Task ExecuteResultAsync(ActionContext context)
        {
            return Result?.ExecuteResultAsync(context) ?? new OkObjectResult(Value).ExecuteResultAsync(context);
        }
    }
}