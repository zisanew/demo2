using FreeSql.DataAnnotations;

namespace HJ.EngineeringCost.Web.Models;

/// <summary>
/// 项目类型表
/// </summary>
[Table(Name = "project_type")]
public class ProjectType : EntityBase
{
    /// <summary>
    /// 类型名称
    /// </summary>
    [Column(StringLength = 50)]
    public string TypeName { get; set; }
}