using Microsoft.AspNetCore.Mvc;
using System.Net;
using WebCompilerServer.Services;

namespace WebCompilerServer.Extensions
{
    public static class ControllerExtensions
    {
        public static ActionResult WrapWithTryCatch(this ControllerBase controller, Func<ActionResult> func)
        {
            try
            {
                return func();
            }
            catch (Exception ex)
            {
                LoggerService.Log(ex.ToString(), LogSeverity.ERROR, "ERR", true);
                return controller.Problem(ex.Message, statusCode: (int)HttpStatusCode.InternalServerError);
            }
        }

        public static async Task<ActionResult> WrapWithTryCatch(this ControllerBase controller, Func<Task<ActionResult>> func)
        {
            try
            {
                return await func();
            }
            catch (Exception ex)
            {
                LoggerService.Log(ex.ToString(), LogSeverity.ERROR, "ERR", true);
                return controller.Problem(ex.Message, statusCode: (int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
