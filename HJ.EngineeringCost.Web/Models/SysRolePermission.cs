using FreeSql.DataAnnotations;

namespace HJ.EngineeringCost.Web.Models;

/// <summary>
/// 角色权限关联表
/// </summary>
[Table(Name = "sys_role_permission")]
public class SysRolePermission : EntityBase
{
    /// <summary>
    /// 角色ID
    /// </summary>
    [Column(IsPrimary = true)]
    public long RoleId { get; set; }

    /// <summary>
    /// 权限ID
    /// </summary>
    [Column(IsPrimary = true)]
    public long PermissionId { get; set; }
}
