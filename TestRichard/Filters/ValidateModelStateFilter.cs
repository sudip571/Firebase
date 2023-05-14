using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;
using System.Web.Http.Controllers;


namespace TestRichard.Filters
{
    public class ValidateModelStateFilter : ActionFilterAttribute
    {

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                //var errorsInModelState = context.ModelState
                //.Where(x => x.Value?.Errors.Count > 0)
                //.ToDictionary(kvp => kvp.Key, kvp => kvp.Value?.Errors.Select(x => x.ErrorMessage)).ToArray();

                var errors = context.ModelState
                .Where(ms => ms.Value.Errors.Any())
                .ToDictionary(ms => ms.Key, ms => ms.Value.Errors.Select(e => e.ErrorMessage).ToArray());

                var responseObj = new
                {
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Message = "One or more validation errors occurred.",
                    Errors = errors
                };

                context.Result = new JsonResult(responseObj)
                {
                    StatusCode = (int)HttpStatusCode.BadRequest
                };
                return;
            }
        }
    }

}
