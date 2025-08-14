using FreeSql.DataAnnotations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace HJ.EngineeringCost.Web.Models;

/// <summary>
/// 实体基类
/// </summary>
public class EntityBase : EntityBaseId
{
    /// <summary>
    /// 创建人Id
    /// </summary>
    [Description("创建人Id")]
    [Column(Position = -23, CanUpdate = false)]
    public virtual long? CreateUserId { get; set; }

    /// <summary>
    /// 创建人
    /// </summary>
    [Description("创建人")]
    [Column(Position = -22, CanUpdate = false), MaxLength(60)]
    public virtual string? CreateUserName { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    [Description("创建时间")]
    [Column(Position = -20, CanUpdate = false)]
    public virtual DateTime? CreateTime { get; set; }

    /// <summary>
    /// 更新人Id
    /// </summary>
    [Description("修改者用户Id")]
    [Column(Position = -13)]
    [JsonPropertyOrder(10000)]
    public virtual long? UpdateUserId { get; set; }

    /// <summary>
    /// 更新人
    /// </summary>
    [Description("修改者用户名")]
    [Column(Position = -12), MaxLength(60)]
    [JsonPropertyOrder(10001)]
    public virtual string? UpdateUserName { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    [Description("修改时间")]
    [JsonPropertyOrder(10002)]
    [Column(Position = -10, CanUpdate = false)]
    public virtual DateTime? UpdateTime { get; set; }
}
