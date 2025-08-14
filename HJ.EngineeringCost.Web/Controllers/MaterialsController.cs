using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HJ.EngineeringCost.Web.Controllers;

/// <summary>
/// 物料信息
/// </summary>
[AllowAnonymous]
[Route("/api/[controller]/[action]")]
public class MaterialsController : BaseController<Materials, MaterialsInput>
{
    public MaterialsController(IServiceScopeFactory serviceScopeFactory)
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

    /// <summary>
    /// 修改
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    public override async Task<BaseResult<IEnumerable<MaterialsInput>>> UpdateAsync([FromBody] BaseCUDInput<MaterialsInput> input)
    {
        var result = new BaseResult<IEnumerable<MaterialsInput>>();
        try
        {
            if (input.Data == null || input.Data.Count() == 0)
            {
                return result.Set(1001, "请求参数Data为空");
            }

            var list = new List<MaterialsInput>();

            using var uow = _fsql.CreateUnitOfWork();

            foreach (var item in input.Data)
            {
                var checkResult = await CheckUniqueAsync(item);
                if (!checkResult.Success)
                {
                    return result.Set(checkResult.Code, checkResult.Message);
                }

                var model = await uow.Orm.Select<Materials>()
                    .Where(m => m.Id == item.Id)
                    .FirstAsync();

                if (model == null)
                {
                    uow.Rollback();
                    return result.Set(1001, $"$物料ID {item.Id} 不存在");
                }

                item.UpdateTime = DateTime.Now;
                item.UpdateUserName = CurrentUserId;
                var entity = item.Adapt<Materials>();

                if (entity != null)
                {
                    var count = await uow.Orm.Update<Materials>().SetSource(entity).ExecuteAffrowsAsync();
                    if (count > 0)
                    {
                        if (model.Price != entity.Price)
                        {
                            await uow.Orm.Insert(new HistoryPrice
                            {
                                Price = entity.Price,
                                MaterialId = entity.Id,
                                CreateTime = DateTime.Now,
                                CreateUserName = CurrentUserId
                            }).ExecuteAffrowsAsync();
                        }

                        if (model.WagesPrice != entity.WagesPrice)
                        {
                            await uow.Orm.Insert(new HistoryWagesPrice
                            {
                                WagesPrice = entity.WagesPrice,
                                MaterialId = entity.Id,
                                CreateTime = DateTime.Now,
                                CreateUserName = CurrentUserId
                            }).ExecuteAffrowsAsync();
                        }

                        list.Add(item);
                    }
                }
            }

            uow.Commit();

            result.Data = list;
        }
        catch (Exception ex)
        {
            result.Set(1001, ex.Message);
            _logger.Error(ex, $"物料信息|Update 异常，请求参数：{input}");
        }

        return result;
    }
}
