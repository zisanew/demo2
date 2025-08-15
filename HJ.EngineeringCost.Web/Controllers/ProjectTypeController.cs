using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HJ.EngineeringCost.Web.Controllers;

[AllowAnonymous]
[Route("/api/[controller]/[action]")]
public class ProjectTypeController : BaseController<ProjectType, ProjectTypeDto>
{
    public ProjectTypeController(IServiceScopeFactory serviceScopeFactory)
        : base(serviceScopeFactory)
    {
    }

    public IActionResult Index()
    {
        return View();
    }

    public async Task<IActionResult> Form(long id)
    {
        var model = await GetByIdAsync(id);
        return View(model);
    }

    /// <summary>
    /// 查询
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("/api/[controller]")]
    public async Task<IActionResult> GetListAsync([FromQuery] GetPageProjectTypeInput input)
    {
        var list = await _fsql.Select<ProjectType>()
            .WhereIf(input.TypeName != null, x => x.TypeName.Contains(input.TypeName))
            .OrderBy(!string.IsNullOrEmpty(input.Sort), input.Sort)
            .Count(out var total)
            .Page(input.PageIndex, input.PageSize)
            .ToListAsync();

        var result = new { Code = 0, total, Data = list };
        return Ok(result);
    }
}
