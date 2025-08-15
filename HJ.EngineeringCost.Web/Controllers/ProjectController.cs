using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HJ.EngineeringCost.Web.Controllers;

[AllowAnonymous]
[Route("/api/[controller]/[action]")]
public class ProjectController : BaseController<Project, ProjectDto>
{
    public ProjectController(IServiceScopeFactory serviceScopeFactory)
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
    public async Task<IActionResult> GetListAsync([FromQuery] GetPageProjectInput input)
    {
        var list = await _fsql.Select<Project>()
            .WhereIf(input.ProjectName != null, x => x.ProjectName.Contains(input.ProjectName))
            .WhereIf(input.Factory != null, x => x.Factory.Contains(input.Factory))
            .WhereIf(input.StartDate != null, x => x.CreateTime <= input.StartDate)
            .WhereIf(input.EndDate != null, x => x.CreateTime >= input.EndDate)
            .OrderBy(!string.IsNullOrEmpty(input.Sort), input.Sort)
            .Count(out var total)
            .Page(input.PageIndex, input.PageSize)
            .ToListAsync();

        var result = new { Code = 0, total, Data = list };
        return Ok(result);
    }

    /// <summary>
    /// 查询明细
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> GetDetailListAsync([FromQuery] GetProjectMaterialInput input)
    {
        var list = await _fsql.Select<ProjectMaterial>()
            .Where(x => x.ProjectId == input.ProjectId)
            .ToListAsync();

        var result = new { Code = 0, total = list.Count, Data = list };
        return Ok(result);
    }

    /// <summary>
    /// 获取基础数据
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<BaseResult<ProjectResult>> GetBasicData()
    {
        var baseTypeList = await _fsql.Select<BaseType>().ToListAsync(x => x.TypeName);
        var projectTypeList = await _fsql.Select<ProjectType>().ToListAsync(x => x.TypeName);
        var structureTypeList = await _fsql.Select<StructureType>().ToListAsync(x => x.TypeName);
        var materialList = await _fsql.Select<Materials>().ToListAsync();

        var nameList = materialList.Select(x => x.MaterialName).Distinct().ToList();
        var brandList = materialList.Select(x => x.Brand).Distinct().ToList();

        var result = new BaseResult<ProjectResult>()
        {
            Data = new ProjectResult
            {
                BaseTypeList = baseTypeList,
                ProjectTypeList = projectTypeList,
                StructureTypeList = structureTypeList,
                MaterialNameList = nameList,
                MaterialBrandList = brandList
            }
        };

        return result;
    }

    /// <summary>
    /// 获取材料列表
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<BaseResult<List<MaterialsDto>>> GetMaterialListAsync([FromQuery] GetMaterialsInput input)
    {
        var result = new BaseResult<List<MaterialsDto>>();

        var list = await _fsql.Select<Materials>()
            .WhereIf(input.MaterialName != null, x => x.MaterialName.Contains(input.MaterialName))
            .WhereIf(input.Brand != null, x => x.Brand.Contains(input.Brand))
            .ToListAsync();

        result.Data = list.Adapt<List<MaterialsDto>>();

        return result;
    }

    /// <summary>
    /// 新增
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    public override async Task<BaseResult<IEnumerable<ProjectDto>>> CreateAsync([FromBody] BaseCUDInput<ProjectDto> input)
    {
        var result = new BaseResult<IEnumerable<ProjectDto>>();
        try
        {
            if (input.Data == null || input.Data.Count() == 0)
            {
                return result.Set(1001, "请求参数Data为空");
            }

            var list = new List<ProjectDto>();

            using var uow = _fsql.CreateUnitOfWork();

            foreach (var item in input.Data)
            {
                item.CreateTime = DateTime.Now;
                item.CreateUserName = CurrentUserId;
                var entity = item.Adapt<Project>();

                if (entity != null)
                {
                    await uow.Orm.Insert(entity).ExecuteAffrowsAsync();
                    item.Id = entity.Id;

                    item.ProjectMaterialList.ForEach(x => x.ProjectId = entity.Id);
                    var projectMaterialEntityList = item.ProjectMaterialList.Adapt<List<ProjectMaterial>>();
                    await uow.Orm.Insert(projectMaterialEntityList).ExecuteAffrowsAsync();

                    list.Add(item);
                }
            }

            uow.Commit();

            result.Data = list;
        }
        catch (Exception ex)
        {
            result.Set(1001, ex.Message);
            _logger.Error(ex, $"项目管理|Create 异常，请求参数：{input}");
        }

        return result;
    }

    /// <summary>
    /// 修改
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    public override async Task<BaseResult<IEnumerable<ProjectDto>>> UpdateAsync([FromBody] BaseCUDInput<ProjectDto> input)
    {
        var result = new BaseResult<IEnumerable<ProjectDto>>();
        try
        {
            if (input.Data == null || input.Data.Count() == 0)
            {
                return result.Set(1001, "请求参数Data为空");
            }

            var list = new List<ProjectDto>();

            using var uow = _fsql.CreateUnitOfWork();

            foreach (var item in input.Data)
            {
                var model = await uow.Orm.Select<Project>()
                    .Where(m => m.Id == item.Id)
                    .FirstAsync();

                if (model == null)
                {
                    uow.Rollback();
                    return result.Set(1001, $"$项目ID {item.Id} 不存在");
                }

                item.UpdateTime = DateTime.Now;
                item.UpdateUserName = CurrentUserId;
                var entity = item.Adapt<Project>();

                if (entity != null)
                {
                    var count = await uow.Orm.Update<Project>().SetSource(entity).ExecuteAffrowsAsync();
                    if (count > 0)
                    {
                        item.ProjectMaterialList.ForEach(x => x.ProjectId = entity.Id);

                        // 删除旧的材料数据
                        var ss = await uow.Orm.Delete<ProjectMaterial>()
                            .Where(x => x.ProjectId == entity.Id)
                            .ExecuteAffrowsAsync();

                        var projectMaterialEntityList = item.ProjectMaterialList.Adapt<List<ProjectMaterial>>();
                        await uow.Orm.Insert(projectMaterialEntityList).ExecuteAffrowsAsync();

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
            _logger.Error(ex, $"项目管理|Update 异常，请求参数：{input}");
        }

        return result;
    }
}