using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HJ.EngineeringCost.Web.Controllers;

//[Authorize]
[AllowAnonymous]
[Route("/api/[controller]/[action]")]
public class SysRoleMenuController : BaseController
{
    public SysRoleMenuController(IServiceScopeFactory serviceScopeFactory)
        : base(serviceScopeFactory)
    {
    }

    public IActionResult Permission(long roleId)
    {
        ViewBag.RoleId = roleId;
        return View();
    }

    /// <summary>
    /// 获取角色已分配的菜单ID列表
    /// </summary>
    /// <param name="roleId">角色ID</param>
    /// <returns>菜单ID列表</returns>
    [HttpGet]
    public async Task<IActionResult> GetRoleMenuIdsAsync(long roleId)
    {
        var menuIds = await _fsql.Select<SysRoleMenu>()
            .Where(x => x.RoleId == roleId)
            .ToListAsync()
            .ContinueWith(t => t.Result.Select(x => x.MenuId).ToList());

        return Ok(new { Code = 0, Data = menuIds });
    }

    [HttpPost]
    public async Task<BaseResult> SaveRoleMenusAsync([FromBody] SysRoleMenuDto input)
    {
        var result = new BaseResult();
        try
        {
            if (input.RoleId <= 0)
            {
                return result.Set(1001, "角色ID不能为空");
            }

            // 1. 删除该角色原有的所有权限关联
            await _fsql.Delete<SysRoleMenu>()
                .Where(x => x.RoleId == input.RoleId)
                .ExecuteAffrowsAsync();

            // 2. 批量插入新的权限关联
            if (input.MenuIdList != null && input.MenuIdList.Any())
            {
                var roleMenus = input.MenuIdList.Select(menuId => new SysRoleMenu
                {
                    RoleId = input.RoleId,
                    MenuId = menuId
                }).ToList();

                await _fsql.Insert(roleMenus).ExecuteAffrowsAsync();
            }

            result.Message = "权限分配成功";
        }
        catch (Exception ex)
        {
            result.Set(1001, $"权限分配失败：{ex.Message}");
            _logger.Error(ex, $"角色权限分配异常，RoleId：{input.RoleId}");
        }

        return result;
    }
}