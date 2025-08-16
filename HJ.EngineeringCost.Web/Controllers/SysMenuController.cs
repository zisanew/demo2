using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HJ.EngineeringCost.Web.Controllers;

//[Authorize]
[AllowAnonymous]
[Route("/api/[controller]/[action]")]
public class SysMenuController : BaseController<SysMenu, SysMenuDto>
{
    public SysMenuController(IServiceScopeFactory serviceScopeFactory)
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
    /// 分页查询菜单列表
    /// </summary>
    /// <param name="input">查询参数</param>
    /// <returns></returns>
    [HttpGet]
    [Route("/api/[controller]")]
    public async Task<IActionResult> GetListAsync([FromQuery] GetPageSysMenuInput input)
    {
        var query = _fsql.Select<SysMenu>()
            .WhereIf(!string.IsNullOrEmpty(input.MenuName), x => x.MenuName.Contains(input.MenuName))
            .WhereIf(input.ParentId.HasValue, x => x.ParentId == input.ParentId)
            .WhereIf(input.IsShow.HasValue, x => x.IsShow == input.IsShow);

        var total = await query.CountAsync();
        var list = await query
            .OrderBy(x => x.ParentId)
            .OrderBy(x => x.Sort)
            .Page(input.PageIndex, input.PageSize)
            .ToListAsync();

        return Ok(new { Code = 0, Total = total, Data = list });
    }

    public async Task<List<MenuTree>> GetMenuTreeByRoleIdAsync(long roleId)
    {
        // 1. 获取角色拥有的所有菜单ID
        var menuIds = await _fsql.Select<SysRoleMenu>()
            .Where(rm => rm.RoleId == roleId)
            .ToListAsync()
            .ContinueWith(t => t.Result.Select(rm => rm.MenuId).ToList());

        if (!menuIds.Any())
        {
            return new List<MenuTree>();
        }

        // 2. 获取这些菜单ID对应的所有菜单信息
        var menus = await _fsql.Select<SysMenu>()
            .Where(m => menuIds.Contains(m.Id) && m.IsShow)
            .OrderBy(m => m.ParentId)
            .OrderBy(m => m.Sort)
            .ToListAsync();

        // 3. 构建树形结构
        return BuildMenuTree2(menus, 0);
    }

    /// <summary>
    /// 递归构建菜单树
    /// </summary>
    /// <param name="allMenus">所有菜单</param>
    /// <param name="parentId">父级ID</param>
    /// <returns>菜单树列表</returns>
    private List<MenuTree> BuildMenuTree2(List<SysMenu> allMenus, long parentId)
    {
        // 获取当前父节点的所有子节点
        var childMenus = allMenus.Where(m => m.ParentId == parentId).ToList();

        if (!childMenus.Any())
        {
            return new List<MenuTree>();
        }

        // 转换为MenuTree并递归处理子节点
        return childMenus.Select(menu => new MenuTree
        {
            id = (int)menu.Id,
            title = menu.MenuName,
            href = menu.Url,
            icon = menu.Icon,
            fontFamily = "ok-icon",
            children = BuildMenuTree2(allMenus, menu.Id)
        }).ToList();
    }

    /// <summary>
    /// 获取树形结构菜单（用于前端菜单展示）
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> GetTreeListAsync()
    {
        // 查询所有启用的菜单
        var allMenus = await _fsql.Select<SysMenu>()
            .Where(x => x.IsShow)
            .OrderBy(x => x.Sort)
            .ToListAsync();

        // 构建树形结构（递归处理父子关系）
        var treeMenus = BuildMenuTree(allMenus, 0);
        return Ok(new { Code = 0, Data = treeMenus });
    }

    /// <summary>
    /// 构建菜单树形结构
    /// </summary>
    /// <param name="allMenus">所有菜单列表</param>
    /// <param name="parentId">父级ID（顶级菜单为0）</param>
    /// <returns>树形菜单列表</returns>
    private List<SysMenu> BuildMenuTree(List<SysMenu> allMenus, long parentId)
    {
        var children = allMenus.Where(x => x.ParentId == parentId).ToList();
        foreach (var child in children)
        {
            child.Children = BuildMenuTree(allMenus, child.Id);
        }
        return children;
    }

    /// <summary>
    /// 重写唯一性校验（菜单名称在同一父级下唯一）
    /// </summary>
    protected override async Task<BaseResult> CheckUniqueAsync(SysMenuDto dto)
    {
        var result = new BaseResult();
        // 检查同一父级下是否存在相同菜单名称
        var exists = await _fsql.Select<SysMenu>()
            .Where(x => x.MenuName == dto.MenuName
                        && x.ParentId == dto.ParentId
                        && x.Id != dto.Id) // 排除自身（编辑场景）
            .AnyAsync();

        if (exists)
        {
            return result.Set(1001, $"父级菜单下已存在名称为【{dto.MenuName}】的菜单");
        }
        return result;
    }

    public class MenuTree
    {
        /// <summary>
        /// 菜单Id
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// 菜单标题
        /// </summary>
        public string title { get; set; }

        /// <summary>
        /// 菜单路径
        /// </summary>
        public string href { get; set; }

        /// <summary>
        /// 字体样式
        /// </summary>
        public string fontFamily { get; set; } = "ok-icon";

        /// <summary>
        /// 菜单图标
        /// </summary>
        public string icon { get; set; }

        /// <summary>
        /// 子节点
        /// </summary>
        public IEnumerable<MenuTree> children { get; set; }
    }
}
