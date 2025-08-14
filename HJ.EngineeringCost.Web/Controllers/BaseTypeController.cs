using HJ.EngineeringCost.Web.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace HJ.EngineeringCost.Web.Controllers;

[Route("/api/[controller]/[action]")]
public class BaseTypeController : BaseController<BaseType, BaseTypeInput>
{
    public BaseTypeController(IServiceScopeFactory serviceScopeFactory)
        : base(serviceScopeFactory)
    {
    }

    /// <summary>
    /// 首页
    /// </summary>
    /// <returns></returns>
    public IActionResult Index()
    {
        ViewBag.UserId = CurrentUserId;
        return View();
    }
}
