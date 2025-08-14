using Microsoft.AspNetCore.Mvc;

namespace HJ.EngineeringCost.Web.Controllers;

public class HomeController : Controller
{
    /// <summary>
    /// 首页
    /// </summary>
    /// <returns></returns>
    public IActionResult Index()
    {
        return View();
    }

    /// <summary>
    /// 主页面
    /// </summary>
    /// <returns></returns>
    public IActionResult Main()
    {
        return View();
    }
}
