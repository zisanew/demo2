using HJ.EngineeringCost.Web.Enums;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HJ.EngineeringCost.Web.Controllers;

[AllowAnonymous]
public class AccountController : Controller
{
    private readonly IFreeSql _fsql;
    private readonly ILogger _logger;

    public AccountController(IFreeSql fsql, ILogger logger)
    {
        _fsql = fsql;
        _logger = logger;
    }

    public IActionResult Login(string returnUrl = "/")
    {
        if (User.Identity!.IsAuthenticated)
        {
            return Redirect(returnUrl);
        }

        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    /// <summary>
    /// 处理登录请求
    /// </summary>
    [HttpPost]
    public async Task<BaseResult> LoginOn([FromBody] LoginDto input)
    {
        var result = new BaseResult();

        try
        {
            if (string.IsNullOrEmpty(input.UserName) || string.IsNullOrEmpty(input.Password))
            {
                return result.Set(1001, "用户名和密码不能为空");
            }

            var user = await _fsql.Select<SysUser>()
                .Where(u => u.UserName == input.UserName && u.Password == input.Password)
                .FirstAsync();

            if (user == null)
            {
                _logger.Debug($"用户 {input.UserName} 登录失败：用户名或密码错误");
                return result.Set(1001, $"用户 {input.UserName} 登录失败：用户名或密码错误");
            }

            if (user.UserStatus == UserStatus.Disable)
            {
                _logger.Debug($"用户 {input.UserName} 登录失败：账户已禁用");
                return result.Set(1001, $"账户已禁用，请联系管理员");
            }

            // 获取用户角色信息
            var role = await _fsql.Select<SysRole>()
                .Where(r => r.Id == user.RoleId)
                .FirstAsync();

            var menuIdList = await _fsql.Select<SysMenu, SysRoleMenu>()
                .InnerJoin<SysRoleMenu>((m, rm) => m.Id == rm.MenuId)
                .Where((m, rm) => rm.RoleId == user.RoleId)
                .ToListAsync((m, rm) => m.Id);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim("RealName", user.RealName),
                new Claim(ClaimTypes.Role, role?.RoleName ?? ""),
                new Claim("MenuIdList", string.Join(",", menuIdList))
            };

            var claimsIdentity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme,
                ClaimTypes.Name,
                ClaimTypes.Role
            );

            var authProperties = new AuthenticationProperties
            {
                // 是否持久化Cookie（记住我功能）
                IsPersistent = true,
                // Cookie过期时间（7天）
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7),
                // 允许刷新认证
                AllowRefresh = true
            };

            // 登录用户，创建Cookie
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties
            );

            _logger.Debug($"用户 {input.UserName} 登录成功");
            return result;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "登录过程发生错误");
            return result.Set(500, $"登录失败：{ex.Message}");
        }
    }

    /// <summary>
    /// 退出登录
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> LoginOut()
    {
        if (User.Identity!.IsAuthenticated)
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }

        return RedirectToAction("Login", "Account");
    }

    /// <summary>
    /// 获取当前登录用户信息
    /// </summary>
    [HttpGet]
    //[LoginAuthorize]
    public IActionResult GetCurrentUser()
    {
        if (!User.Identity!.IsAuthenticated)
        {
            return Json(new { Code = 401, Message = "未登录" });
        }

        // 从Claims中获取用户基本信息
        var userInfo = new
        {
            id = User.FindFirstValue(ClaimTypes.NameIdentifier),
            userName = User.FindFirstValue(ClaimTypes.Name),
            realName = User.FindFirstValue("RealName"),
            roleName = User.FindFirstValue(ClaimTypes.Role)
        };

        return Json(new { Code = 200, Data = userInfo });
    }

    //[HttpGet]
    //public async Task<IActionResult> GetUserMenus()
    //{
    //    if (!User.Identity.IsAuthenticated)
    //    {
    //        return Json(new { Code = 401, Message = "未登录" });
    //    }

    //    // 获取当前用户角色ID
    //    var userId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
    //    var user = await _fsql.Select<SysUser>()
    //        .Where(x => x.Id == userId)
    //        .FirstAsync();

    //    if (user == null)
    //    {
    //        return Json(new { Code = 404, Message = "用户不存在" });
    //    }

    //    // 获取角色关联的菜单
    //    var menuIds = await _fsql.Select<SysRoleMenu>()
    //        .Where(x => x.RoleId == user.RoleId)
    //        .ToListAsync(x => x.MenuId);

    //    if (!menuIds.Any())
    //    {
    //        return Json(new { Code = 0, Data = new List<SysMenuDto>() });
    //    }

    //    // 获取菜单列表并构建树形结构
    //    var menus = await _fsql.Select<SysMenu>()
    //        .Where(x => menuIds.Contains(x.Id) && x.IsShow)
    //        .OrderBy(x => x.ParentId)
    //        .OrderBy(x => x.Sort)
    //        .ToListAsync();

    //    var menuDtos = menus.Adapt<List<SysMenuDto>>();
    //    //var treeMenus = BuildMenuTree(menuDtos, 0);

    //    return Json(new { Code = 0, Data = treeMenus });
    //}
}
