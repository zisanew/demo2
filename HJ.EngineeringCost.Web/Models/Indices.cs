using FreeSql.DataAnnotations;

namespace HJ.EngineeringCost.Web.Models;

[Table(Name = "indices")]
public class Indices : EntityBase
{
    /// <summary>
    /// 项目名称
    /// </summary>
    [Column(StringLength = 200)]
    public string ProjectName { get; set; }
}
