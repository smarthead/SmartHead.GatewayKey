using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace SmartHead.GatewayKey
{
    public static class HttpContextExtensions  
    {
        public static Task CreateResponse<TResponse>(this HttpContext context, TResponse response)
        {
            var result = new ObjectResult(response)
            {
                DeclaredType = typeof(TResponse)
            };

            var executor = context.RequestServices.GetRequiredService<IActionResultExecutor<ObjectResult>>();

            var actionContext = new ActionContext(
                context,
                context.GetRouteData() ?? new RouteData(),
                new ActionDescriptor());

            return executor.ExecuteAsync(actionContext, result);
        }
    }
}