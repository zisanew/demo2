using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HJ.EngineeringCost.Web.Controllers;

[Authorize]
[Route("/api/[controller]/[action]")]
public class SysRoleMenuController : BaseController
{
    public SysRoleMenuController(IServiceScopeFactory serviceScopeFactory)
        : base(serviceScopeFactory)
    {
    }

    [HttpGet]
    public async Task<IActionResult> GetRoleMenus(long roleId)
    {
        var menuIds = await _fsql.Select<SysRoleMenu>()
            .Where(x => x.RoleId == roleId)
            .ToListAsync(x => x.MenuId);

        return Ok(new { Code = 0, Data = menuIds });
    }

    [HttpPost]
    public async Task<BaseResult> AssignMenus([FromBody] RoleMenuInput input)
    {
        var result = new BaseResult();
        try
        {
            using var uow = _fsql.CreateUnitOfWork();
            // 先删除原有关联
            await uow.Orm.Delete<SysRoleMenu>()
                .Where(x => x.RoleId == input.RoleId)
                .ExecuteAffrowsAsync();

            // 新增关联
            if (input.MenuIds != null && input.MenuIds.Any())
            {
                var roleMenus = input.MenuIds.Select(menuId => new SysRoleMenu
                {
                    RoleId = input.RoleId,
                    MenuId = menuId
                }).ToList();

                await uow.Orm.Insert(roleMenus).ExecuteAffrowsAsync();
            }

            uow.Commit();
        }
        catch (Exception ex)
        {
            result.Set(1001, ex.Message);
            _logger.Error(ex, $"角色分配菜单异常，角色ID：{input.RoleId}");
        }
        return result;
    }
}

public class RoleMenuInput
{
    public long RoleId { get; set; }
    public List<long> MenuIds { get; set; }
}