using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HJ.EngineeringCost.Web.Controllers;

[AllowAnonymous]
[Route("/api/[controller]/[action]")]
public class IndicesController : BaseController<Indices, IndicesDto>
{
    public IndicesController(IServiceScopeFactory serviceScopeFactory)
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
    public async Task<IActionResult> GetListAsync([FromQuery] GetPageIndicesInput input)
    {
        var list = await _fsql.Select<Indices>()
            .WhereIf(input.ProjectName != null, x => x.ProjectName.Contains(input.ProjectName))
            .WhereIf(input.StartDate != null, x => x.CreateTime <= input.StartDate)
            .WhereIf(input.EndDate != null, x => x.CreateTime >= input.EndDate)
            .OrderBy(!string.IsNullOrEmpty(input.Sort), input.Sort)
            .Count(out var total)
            .Page(input.PageIndex, input.PageSize)
            .ToListAsync();

        var result = new { Code = 0, total, Data = list };
        return Ok(result);
    }
}
