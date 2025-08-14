using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace HJ.EngineeringCost.Web.Filter;

/// <summary>
/// 登录过滤器
/// </summary>
public class LoginFilter : IAsyncAuthorizationFilter
{
    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        // 检查是否有 [AllowAnonymous] 标记
        var endpoint = context.HttpContext.GetEndpoint();
        if (endpoint?.Metadata?.GetMetadata<IAllowAnonymous>() != null)
        {
            await Task.CompletedTask;
            return;
        }

        // 检查当前用户是否已登录
        if (context.HttpContext.User == null
            || context.HttpContext.User.Identity == null
            || !context.HttpContext.User.Identity.IsAuthenticated)
        {
            // 用户未登录，处理未授权请求
            await HandleUnauthorizedRequest(context);
            return;
        }

        await Task.CompletedTask;
    }

    private async Task HandleUnauthorizedRequest(AuthorizationFilterContext context)
    {
        // 判断是否为 AJAX 请求
        if (IsAjaxRequest(context.HttpContext.Request))
        {
            // 处理 AJAX 请求
            context.Result = new JsonResult(new { Code = 401, Message = "登录信息已失效，请登录后操作！", });
        }
        else
        {
            context.Result = new ContentResult { Content = "<script>top.location.href = '/Account/Login';</script>", ContentType = "text/html" };
            //context.Result = new RedirectResult($"/Account/Login?returnUrl={context.HttpContext.Request.Path}");
        }

        await Task.CompletedTask;
    }

    /// <summary>
    /// 判断是否为 AJAX 请求
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    private static bool IsAjaxRequest(HttpRequest request)
    {
        if (request.Headers.ContainsKey("X-Requested-With") &&
            request.Headers["X-Requested-With"] == "XMLHttpRequest")
        {
            return true;
        }

        if (request.Headers.ContainsKey("Accept") &&
            request.Headers["Accept"].ToString().Contains("application/json"))
        {
            return true;
        }

        if (request.Headers.ContainsKey("Content-Type") &&
            request.Headers["Content-Type"].ToString().Contains("application/json"))
        {
            return true;
        }

        return false;
    }
}
