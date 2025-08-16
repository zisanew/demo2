using System.ComponentModel;

namespace HJ.EngineeringCost.Web.Enums;

/// <summary>
/// 用户状态
/// </summary>
public enum UserStatus
{
    /// <summary>
    /// 启用
    /// </summary>
    [Description("启用")]
    Enable = 1,

    /// <summary>
    /// 禁用
    /// </summary>
    [Description("禁用")]
    Disable = 2
}