using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HJ.EngineeringCost.Web.Controllers;

[AllowAnonymous]
[Route("/api/[controller]/[action]")]
public class HistoryPriceController : BaseController
{
    public HistoryPriceController(IServiceScopeFactory serviceScopeFactory)
        : base(serviceScopeFactory)
    {
    }

    public async Task<IActionResult> Index()
    {
        var brandList = await _fsql.Select<Materials>()
            .Distinct()
            .ToListAsync(x => x.Brand);

        ViewBag.BrandList = brandList;

        return View();
    }

    public async Task<IActionResult> Form(long id)
    {
        var model = await _fsql.Select<Materials>(id).FirstAsync();
        return View(model);
    }

    public IActionResult Graph(long id)
    {
        ViewBag.MaterialId = id;
        return View();
    }

    /// <summary>
    /// 查询
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("/api/[controller]")]
    public async Task<IActionResult> GetListAsync([FromQuery] GetPageMaterialsInput input)
    {
        var list = await _fsql.Select<Materials>()
            .WhereIf(input.MaterialCode != null, x => x.MaterialCode.Contains(input.MaterialCode))
            .WhereIf(input.MaterialName != null, x => x.MaterialName.Contains(input.MaterialName))
            .WhereIf(input.Brand != null, x => x.Brand.Contains(input.Brand))
            .OrderBy(!string.IsNullOrEmpty(input.Sort), input.Sort)
            .Count(out var total)
            .Page(input.PageIndex, input.PageSize)
            .ToListAsync();

        var result = new { Code = 0, total, Data = list };
        return Ok(result);
    }

    /// <summary>
    /// 历史价格走势
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task<BaseResult<MaterialsDto>> GetHistoryPriceTrendsAsync([FromQuery] GetMaterialsByIdInput input)
    {
        var result = new BaseResult<MaterialsDto>();

        var material = await _fsql.Select<Materials>()
            .IncludeMany(m => m.HistoryPriceList)
            .IncludeMany(m => m.HistoryWagesPriceList)
            .Where(m => m.Id == input.Id)
            .ToOneAsync();

        result.Data = material.Adapt<MaterialsDto>();

        return result;
    }
}
