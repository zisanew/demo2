using FreeSql.DataAnnotations;
using HJ.EngineeringCost.Web.Enums;

namespace HJ.EngineeringCost.Web.Models;

/// <summary>
/// 用户表
/// </summary>
[Table(Name = "sys_user")]
public class SysUser : EntityBase
{
    /// <summary>
    /// 用户名
    /// </summary>
    [Column(StringLength = 60)]
    public string UserName { get; set; }

    /// <summary>
    /// 密码
    /// </summary>
    [Column(StringLength = 50)]
    public string Password { get; set; }

    /// <summary>
    /// 昵称
    /// </summary>
    [Column(StringLength = 60)]
    public string NickName { get; set; }

    /// <summary>
    /// 真实姓名
    /// </summary>
    [Column(StringLength = 60)]
    public string RealName { get; set; }

    /// <summary>
    /// 电子邮箱
    /// </summary>
    [Column(StringLength = 50)]
    public string Email { get; set; }

    /// <summary>
    /// 用户状态 (1 启动 - 2 禁用)
    /// </summary>
    [Column(MapType = typeof(int))]
    public UserStatus UserStatus { get; set; } = UserStatus.Enable;

    /// <summary>
    /// 手机号
    /// </summary>
    [Column(StringLength = 20)]
    public string Phone { get; set; }

    /// <summary>
    /// 角色ID
    /// </summary>
    [Column(IsNullable = false)]
    public long RoleId { get; set; }
}