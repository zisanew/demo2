using FreeSql;
using HJ.EngineeringCost.Web.Attributes;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Reflection;

namespace HJ.EngineeringCost.Web.Controllers;

[Authorize]
public class BaseController : Controller
{
    protected readonly IMapper _mapper;
    protected readonly IFreeSql _fsql;
    protected readonly ILogger _logger;

    public BaseController(IServiceScopeFactory serviceScopeFactory)
    {
        var scope = serviceScopeFactory.CreateScope();
        _mapper = scope.ServiceProvider.GetRequiredService<IMapper>();
        _fsql = scope.ServiceProvider.GetRequiredService<IFreeSql>();
        _logger = scope.ServiceProvider.GetRequiredService<ILogger>();
    }

    /// <summary>
    /// 下载模板文件
    /// </summary>
    /// <param name="templateName"></param>
    /// <returns></returns>
    [HttpGet]
    public IActionResult DownloadTemplateAsync(string templateName)
    {
        var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "doc", templateName);

        if (!System.IO.File.Exists(templatePath))
        {
            return NotFound($"模板文件 {templateName} 不存在");
        }

        var fileBytes = System.IO.File.ReadAllBytes(templatePath);
        return File(fileBytes, "application/octet-stream", templateName);
    }
}

public class BaseController<TEntity, TEntityDto> : BaseController
    where TEntity : EntityBase
    where TEntityDto : EntityBase
{
    public BaseController(IServiceScopeFactory serviceScopeFactory) : base(serviceScopeFactory)
    {
        var scope = serviceScopeFactory.CreateScope();
        BaseEntity.Initialization(_fsql, null);
    }

    /// <summary>
    /// 根据ID获取实体
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet]
    public virtual async Task<TEntity> GetByIdAsync(long id)
    {
        return await _fsql.Select<TEntity>(id).FirstAsync();
    }

    /// <summary>
    /// 新增
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    public virtual async Task<BaseResult<IEnumerable<TEntityDto>>> CreateAsync([FromBody] BaseCUDInput<TEntityDto> input)
    {
        var result = new BaseResult<IEnumerable<TEntityDto>>();
        try
        {
            if (input.Data == null || input.Data.Count() == 0)
            {
                return result.Set(1001, "请求参数Data为空");
            }

            var list = new List<TEntityDto>();

            foreach (var item in input.Data)
            {
                try
                {
                    var checkResult = await CheckUniqueAsync(item);
                    if (!checkResult.Success)
                    {
                        return result.Set(checkResult.Code, checkResult.Message);
                    }

                    item.CreateTime = DateTime.Now;
                    item.CreateUserName = CurrentUserId;
                    var entity = item.Adapt<TEntity>();

                    if (entity != null)
                    {
                        await _fsql.Insert(entity).ExecuteAffrowsAsync();
                        item.Id = entity.Id;
                        list.Add(item);
                    }
                }
                catch (Exception ex)
                {
                    result.Set(1001, ex.Message);
                    _logger.Error(ex, $"新增数据：{typeof(TEntity).Name}失败，参数信息：{item}");
                }
            }

            result.Data = list;
            _logger.Debug($"{typeof(TEntity)}|Create 请求参数：{input}，返回结果：{result}");
        }
        catch (Exception ex)
        {
            result.Set(1001, ex.Message);
            _logger.Error(ex, $"{typeof(TEntity)}|Create 异常，请求参数：{input}");
        }

        return result;
    }

    /// <summary>
    /// 修改
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    public virtual async Task<BaseResult<IEnumerable<TEntityDto>>> UpdateAsync([FromBody] BaseCUDInput<TEntityDto> input)
    {
        var result = new BaseResult<IEnumerable<TEntityDto>>();
        try
        {
            if (input.Data == null || input.Data.Count() == 0)
            {
                return result.Set(1001, "请求参数Data为空");
            }

            var list = new List<TEntityDto>();

            foreach (var item in input.Data)
            {
                try
                {
                    var checkResult = await CheckUniqueAsync(item);
                    if (!checkResult.Success)
                    {
                        return result.Set(checkResult.Code, checkResult.Message);
                    }

                    item.UpdateTime = DateTime.Now;
                    item.UpdateUserName = CurrentUserId;
                    var entity = item.Adapt<TEntity>();

                    if (entity != null)
                    {
                        var count = await _fsql.Update<TEntity>().SetSource(entity).ExecuteAffrowsAsync();
                        if (count > 0)
                        {
                            list.Add(item);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, $"更新数据：{typeof(TEntity).Name}失败，参数信息：{item}");
                }
            }

            result.Data = list;
            _logger.Debug($"{typeof(TEntity)}|Update 请求参数：{input}，返回结果：{result}");
        }
        catch (Exception ex)
        {
            result.Set(1001, ex.Message);
            _logger.Error(ex, $"{typeof(TEntity)}|Update 异常，请求参数：{input}");
        }

        return result;
    }

    /// <summary>
    /// 删除
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    public virtual async Task<BaseResult<IEnumerable<long>>> DeleteAsync([FromBody] BaseCUDInput<long> input)
    {
        var result = new BaseResult<IEnumerable<long>>();
        try
        {
            if (input.Data == null || input.Data.Count() == 0)
            {
                return result.Set(1001, "请求参数Data为空");
            }

            var list = new List<long>();

            foreach (var id in input.Data)
            {
                try
                {
                    var count = await _fsql.Delete<TEntity>(id).ExecuteAffrowsAsync();
                    if (count > 0)
                    {
                        list.Add(id);
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, $"删除数据：{typeof(TEntity).Name}失败，参数信息：{id}");
                }
            }

            result.Data = list;
            _logger.Debug($"{typeof(TEntity)}|Delete 请求参数：{input}，返回结果：{result}");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, $"{typeof(TEntity)}|Delete 异常，请求参数：{input}");
        }

        return result;
    }

    /// <summary>
    /// 检查字段是否唯一
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    protected virtual async Task<BaseResult> CheckUniqueAsync(TEntityDto dto)
    {
        var result = new BaseResult();
        var entityType = typeof(TEntity);
        var properties = entityType.GetProperties()
            .Where(p => p.GetCustomAttribute<UniqueAttribute>() != null)
            .ToList();

        if (!properties.Any())
        {
            return result;
        }

        var parameter = Expression.Parameter(entityType, "x");
        Expression combinedCondition = null;
        var fieldList = new List<string>();

        foreach (var property in properties)
        {
            var value = property.GetValue(dto);
            if (value == null) continue;

            var displayName = property.GetCustomAttribute<DisplayAttribute>()?.Name ?? property.Name;
            fieldList.Add($"{displayName} {value}");

            var propertyExpr = Expression.Property(parameter, property.Name);
            var constantExpr = Expression.Constant(value);
            var equalExpr = Expression.Equal(propertyExpr, constantExpr);

            combinedCondition = combinedCondition == null
                ? equalExpr
                : Expression.AndAlso(combinedCondition, equalExpr);
        }

        if (combinedCondition == null)
        {
            return result;
        }

        var predicate = Expression.Lambda<Func<TEntity, bool>>(combinedCondition, parameter);
        var query = _fsql.Select<TEntity>().Where(predicate);

        if (dto.Id > 0)
        {
            query = query.Where(x => x.Id != dto.Id);
        }

        if (await query.AnyAsync())
        {
            return result.Set(1001, $"{string.Join("，", fieldList)} 已存在");
        }

        return result;
    }

    protected string CurrentUserId => User?.Identity?.Name ?? "unknow";
}