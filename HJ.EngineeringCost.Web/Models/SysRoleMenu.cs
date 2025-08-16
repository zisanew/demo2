using FreeSql.DataAnnotations;

namespace HJ.EngineeringCost.Web.Models;

/// <summary>
/// 角色权限关联表
/// </summary>
[Table(Name = "sys_role_menu")]
public class SysRoleMenu : EntityBase
{
    /// <summary>
    /// 角色ID
    /// </summary>
    [Column(IsPrimary = true)]
    public long RoleId { get; set; }

    /// <summary>
    /// 菜单ID
    /// </summary>
    [Column(IsPrimary = true)]
    public long MenuId { get; set; }
}
