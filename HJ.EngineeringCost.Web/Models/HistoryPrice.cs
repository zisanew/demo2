using FreeSql.DataAnnotations;

namespace HJ.EngineeringCost.Web.Models;

/// <summary>
/// 历史价格表
/// </summary>
[Table(Name = "history_price")]
public class HistoryPrice : EntityBase
{
    /// <summary>
    /// 价格
    /// </summary>
    [Column(Precision = 10, Scale = 2)]
    public decimal Price { get; set; }

    /// <summary>
    /// 材料ID
    /// </summary>
    public long MaterialId { get; set; }
}