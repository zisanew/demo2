using FreeSql.DataAnnotations;

namespace HJ.EngineeringCost.Web.Models;

/// <summary>
/// 角色表
/// </summary>
[Table(Name = "sys_role")]
public class SysRole : EntityBase
{
    /// <summary>
    /// 角色名称
    /// </summary>
    [Column(StringLength = 50, IsNullable = false)]
    public string RoleName { get; set; }

    /// <summary>
    /// 角色描述
    /// </summary>
    [Column(StringLength = 200)]
    public string Description { get; set; }
}
