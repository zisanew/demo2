using FreeSql.DataAnnotations;
using HJ.EngineeringCost.Web.Attributes;
using System.ComponentModel.DataAnnotations;

namespace HJ.EngineeringCost.Web.Models;

/// <summary>
/// 项目类型表
/// </summary>
[Table(Name = "project_type")]
public class ProjectType : EntityBase
{
    /// <summary>
    /// 项目名称
    /// </summary>
    [Unique]
    [Display(Name = "项目名称")]
    [Column(StringLength = 50)]
    public string TypeName { get; set; }
}