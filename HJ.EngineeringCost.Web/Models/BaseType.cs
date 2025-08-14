namespace HJ.EngineeringCost.Web.Models;

using FreeSql.DataAnnotations;

/// <summary>
/// 基础类型表
/// </summary>
[Table(Name = "base_type")]
public class BaseType : EntityBase
{
    /// <summary>
    /// 类型名称
    /// </summary>
    [Column(StringLength = 50)]
    public string TypeName { get; set; }
}