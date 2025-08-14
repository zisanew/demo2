using FreeSql.DataAnnotations;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace HJ.EngineeringCost.Web.Models;

/// <summary>
/// 实体基类Id
/// </summary>
public class EntityBaseId
{
    /// <summary>
    /// 主键Id
    /// </summary>
    [Description("主键Id")]
    [Column(Position = 1, IsIdentity = true, IsPrimary = true)]
    [JsonPropertyOrder(-30)]
    public virtual long Id { get; set; }
}
