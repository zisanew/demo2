using Microsoft.AspNetCore.Mvc;

namespace HJ.EngineeringCost.Web.Controllers;

public class HistoryPriceController : BaseController
{
    public HistoryPriceController(IServiceScopeFactory serviceScopeFactory)
        : base(serviceScopeFactory)
    {
    }

    public IActionResult Index()
    {
        return View();
    }

    public async Task<IActionResult> Form(long id)
    {
        //var model = await GetByIdAsync(id);
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
            .OrderBy(!string.IsNullOrEmpty(input.Sort), input.Sort)
            .Count(out var total)
            .Page(input.PageIndex, input.PageSize)
            .ToListAsync();

        var result = new { Code = 0, total, Data = list };
        return Ok(result);
    }

    //public async Task<IActionResult> GetHistoryPriceTrendsAsync([FromQuery] GetHistoryPriceInput input)
    //{
    //    // Implement the logic to fetch historical prices based on the input criteria
    //    var historyPrices = await _fsql.Select<HistoryPrice>()
    //        .WhereIf(input.MaterialId > 0, x => x.MaterialId == input.MaterialId)
    //        .WhereIf(input.StartDate != null, x => x.Date >= input.StartDate)
    //        .WhereIf(input.EndDate != null, x => x.Date <= input.EndDate)
    //        .OrderBy(!string.IsNullOrEmpty(input.Sort), input.Sort)
    //        .Count(out var total)
    //        .Page(input.PageIndex, input.PageSize)
    //        .ToListAsync();
    //    var result = new { Code = 0, total, Data = historyPrices };
    //    return Ok(result);
    //}
}
