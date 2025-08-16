using Microsoft.AspNetCore.Mvc;

namespace HJ.EngineeringCost.Web.Controllers;

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

    ///// <summary>
    ///// 
    ///// </summary>
    ///// <param name="input"></param>
    ///// <returns></returns>
    //[HttpGet]
    //[Route("/api/[controller]")]
    //public async Task<IActionResult> GetListAsync([FromQuery] GetPageSysMenuInput input)
    //{
    //    var query = _fsql.Select<SysMenu>()
    //        .WhereIf(!string.IsNullOrEmpty(input.MenuName), x => x.MenuName.Contains(input.MenuName))
    //        .WhereIf(input.ParentId.HasValue, x => x.ParentId == input.ParentId)
    //        .OrderBy(x => x.ParentId)
    //        .ThenBy(x => x.Sort);

    //    var total = await query.CountAsync();
    //    var list = await query
    //        .Page(input.PageIndex, input.PageSize)
    //        .ToListAsync();

    //    var treeList = BuildTree(list, 0);
    //    return Ok(new { Code = 0, Total = total, Data = treeList });
    //}

    ///// <summary>
    ///// 构建树形结构
    ///// </summary>
    //private List<SysMenuDto> BuildTree(List<SysMenu> list, long parentId)
    //{
    //    var treeList = list
    //        .Where(x => x.ParentId == parentId)
    //        .Adapt<List<SysMenuDto>>();

    //    foreach (var item in treeList)
    //    {
    //        item.Children = BuildTree(list, item.Id);
    //    }
    //    return treeList;
    //}
}
