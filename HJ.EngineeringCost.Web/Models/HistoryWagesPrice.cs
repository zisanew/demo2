using FreeSql.DataAnnotations;

namespace HJ.EngineeringCost.Web.Models;

/// <summary>
/// 历史工资价格表
/// </summary>
[Table(Name = "history_wages_price")]
public class HistoryWagesPrice : EntityBase
{
    /// <summary>
    /// 工资单价
    /// </summary>
    [Column(Precision = 10, Scale = 2)]
    public decimal WagesPrice { get; set; }

    /// <summary>
    /// 材料ID
    /// </summary>
    public long MaterialId { get; set; }
}