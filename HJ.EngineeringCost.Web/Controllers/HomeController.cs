﻿using Microsoft.AspNetCore.Mvc;

namespace HJ.EngineeringCost.Web.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }
}
