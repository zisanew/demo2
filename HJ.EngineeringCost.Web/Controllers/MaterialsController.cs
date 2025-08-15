using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HJ.EngineeringCost.Web.Controllers;

[AllowAnonymous]
[Route("/api/[controller]/[action]")]
public class MaterialsController : BaseController<Materials, MaterialsDto>
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
    public override async Task<BaseResult<IEnumerable<MaterialsDto>>> UpdateAsync([FromBody] BaseCUDInput<MaterialsDto> input)
    {
        var result = new BaseResult<IEnumerable<MaterialsDto>>();
        try
        {
            if (input.Data == null || input.Data.Count() == 0)
            {
                return result.Set(1001, "请求参数Data为空");
            }

            var list = new List<MaterialsDto>();

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

    /// <summary>
    /// 数据清洗
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<BaseResult> DataCleaningAsync()
    {
        var result = new BaseResult();
        var materialList = await _fsql.Select<Materials>().ToListAsync();

        var duplicateCount = 0;
        var delIdList = new List<long>();
        var existsCombinationList = new HashSet<string>();

        foreach (var material in materialList)
        {
            var combination = $"{material.MaterialName}_{material.Brand}_{material.Spec}";
            if (existsCombinationList.Contains(combination))
            {
                delIdList.Add(material.Id);
                duplicateCount++;
            }
            else
            {
                existsCombinationList.Add(combination);
            }
        }

        if (delIdList.Any())
        {
            await _fsql.Delete<Materials>().Where(m => delIdList.Contains(m.Id)).ExecuteAffrowsAsync();
        }

        result.Message = $"已排查 {materialList.Count} 条数据，其中重复记录：{duplicateCount} 条";
        return result;
    }
}
