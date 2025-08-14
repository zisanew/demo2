namespace HJ.EngineeringCost.Web.Models;

using FreeSql.DataAnnotations;
using HJ.EngineeringCost.Web.Attributes;
using System.ComponentModel.DataAnnotations;

/// <summary>
/// 基础类型表
/// </summary>
[Table(Name = "base_type")]
public class BaseType : EntityBase
{
    /// <summary>
    /// 类型名称
    /// </summary>
    [Unique]
    [Display(Name = "类型名称")]
    [Column(StringLength = 50)]
    public string TypeName { get; set; }
}