using FreeSql.DataAnnotations;

namespace HJ.EngineeringCost.Web.Models;

/// <summary>
/// 结构类型表
/// </summary>
[Table(Name = "structure_type")]
public class StructureType : EntityBase
{
    /// <summary>
    /// 类型名称
    /// </summary>
    [Column(StringLength = 50)]
    public string TypeName { get; set; }
}