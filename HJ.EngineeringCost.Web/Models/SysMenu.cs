using FreeSql.DataAnnotations;

namespace HJ.EngineeringCost.Web.Models;

/// <summary>
/// 菜单表
/// </summary>
[Table(Name = "sys_menu")]
public class SysMenu : EntityBase
{
    /// <summary>
    /// 菜单名称
    /// </summary>
    [Column(StringLength = 50, IsNullable = false)]
    public string MenuName { get; set; }

    /// <summary>
    /// 菜单地址
    /// </summary>
    [Column(StringLength = 200)]
    public string Path { get; set; }

    /// <summary>
    /// 菜单图标
    /// </summary>
    [Column(StringLength = 50)]
    public string Icon { get; set; }

    /// <summary>
    /// 父级菜单ID
    /// </summary>
    public long ParentId { get; set; } = 0;

    /// <summary>
    /// 排序号
    /// </summary>
    public int Sort { get; set; } = 0;

    /// <summary>
    /// 是否显示
    /// </summary>
    public bool IsShow { get; set; } = true;
}
