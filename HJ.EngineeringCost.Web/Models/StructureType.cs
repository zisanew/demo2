using FreeSql.DataAnnotations;
using HJ.EngineeringCost.Web.Attributes;
using System.ComponentModel.DataAnnotations;

namespace HJ.EngineeringCost.Web.Models;

/// <summary>
/// 结构类型表
/// </summary>
[Table(Name = "structure_type")]
public class StructureType : EntityBase
{
    /// <summary>
    /// 结构名称
    /// </summary>
    [Unique]
    [Display(Name = "结构名称")]
    [Column(StringLength = 50)]
    public string TypeName { get; set; }
}