using FreeSql.DataAnnotations;

namespace HJ.EngineeringCost.Web.Models;

/// <summary>
/// 权限表
/// </summary>
[Table(Name = "sys_permission")]
public class SysPermission : EntityBase
{
    /// <summary>
    /// 权限编码
    /// </summary>
    [Column(StringLength = 100, IsNullable = false)]
    public string PermissionCode { get; set; }

    /// <summary>
    /// 权限名称
    /// </summary>
    [Column(StringLength = 50, IsNullable = false)]
    public string PermissionName { get; set; }
}
