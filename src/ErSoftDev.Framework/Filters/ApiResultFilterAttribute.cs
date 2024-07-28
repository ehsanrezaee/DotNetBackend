using ErSoftDev.DomainSeedWork;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;


namespace ErSoftDev.Framework.Filters
{
    public class ApiResultFilterAttribute : ActionFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext context)
        {
            var stringLocalizer = context.HttpContext.RequestServices.GetRequiredService<IStringLocalizer<SharedTranslate>>();

            if (context.Result is OkObjectResult okObjectResult)
            {
                context.Result = new JsonResult(new ApiResult<object>(stringLocalizer, ApiResultStatusCode.Success, okObjectResult.Value));
            }
            else if (context.Result is OkResult)
            {
                context.Result = new JsonResult(new ApiResult(stringLocalizer, ApiResultStatusCode.Success));
            }
            else if (context.Result is BadRequestResult)
            {
                context.Result = new JsonResult(new ApiResult(stringLocalizer, ApiResultStatusCode.BadRequest));
            }
            else if (context.Result is BadRequestObjectResult badRequestObjectResult)
            {
                var message = string.Empty;
                if (badRequestObjectResult.Value is ValidationProblemDetails errors)
                    message = errors.Errors.Aggregate(message, (current, item) => current + (" " + item.Key));

                context.Result = new JsonResult(new ApiResult(stringLocalizer, ApiResultStatusCode.BadRequest));
            }
            else if (context.Result is ContentResult)
            {
                context.Result = new JsonResult(new ApiResult(stringLocalizer, ApiResultStatusCode.Success));
            }
            else if (context.Result is NotFoundResult)
            {
                context.Result = new JsonResult(new ApiResult(stringLocalizer, ApiResultStatusCode.NotFound));
            }
            else if (context.Result is NotFoundObjectResult notFoundObjectResult)
            {
                context.Result = new JsonResult(new ApiResult<object>(stringLocalizer, ApiResultStatusCode.NotFound));
            }
            else if (context.Result is ObjectResult objectResult && !(objectResult.Value is ApiResult))
            {
                context.Result = new JsonResult(new ApiResult<object>(stringLocalizer, ApiResultStatusCode.Success, objectResult.Value));
            }

            base.OnResultExecuting(context);
        }

        public override Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            return base.OnActionExecutionAsync(context, next);
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            base.OnActionExecuted(context);
        }
    }
}
