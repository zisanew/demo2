using FreeSql;
using HJ.EngineeringCost.Web.Dtos;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HJ.EngineeringCost.Web.Controllers;

[Authorize]
public class BaseController<TEntity, TEntityDto> : Controller
    where TEntity : EntityBase
    where TEntityDto : EntityBase
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

        BaseEntity.Initialization(_fsql, null);
    }

    /// <summary>
    /// 新增
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    public virtual async Task<BaseResult<IEnumerable<TEntityDto>>> Create([FromBody] BaseCUDInput<TEntityDto> input)
    {
        var result = new BaseResult<IEnumerable<TEntityDto>>();
        try
        {
            if (input.Data == null || input.Data.Count() == 0)
            {
                return result.Set(100101, "请求参数Data为空");
            }

            var list = new List<TEntityDto>();

            foreach (var item in input.Data)
            {
                try
                {
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
                    result.Set(100500, ex.Message);
                    _logger.Error(ex, $"新增数据：{typeof(TEntity).Name}失败，参数信息：{item}");
                }
            }

            result.Data = list;
            _logger.Debug($"{typeof(TEntity)}|Create 请求参数：{input}，返回结果：{result}");
        }
        catch (Exception ex)
        {
            result.Set(100500, ex.Message);
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
    public virtual async Task<BaseResult<IEnumerable<TEntityDto>>> Update([FromBody] BaseCUDInput<TEntityDto> input)
    {
        var result = new BaseResult<IEnumerable<TEntityDto>>();
        try
        {
            if (input.Data == null || input.Data.Count() == 0)
            {
                return result.Set(100101, "请求参数Data为空");
            }

            var list = new List<TEntityDto>();

            foreach (var item in input.Data)
            {
                try
                {
                    item.UpdateTime = DateTime.Now;
                    item.UpdateUserName = CurrentUserId;
                    var entity = item.Adapt<TEntity>();

                    if (entity != null)
                    {
                        var count = await _fsql.Update<TEntity>(entity).ExecuteAffrowsAsync();
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
            result.Set(100500, ex.Message);
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
                return result.Set(100101, "请求参数Data为空");
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

    protected string CurrentUserId => User?.Identity?.Name ?? "unknow";
}