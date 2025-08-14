using FreeSql.DataAnnotations;

namespace HJ.EngineeringCost.Web.Models;

[Table(Name = "indices_detail")]
public class IndicesDetail : EntityBase
{
    /// <summary>
    /// 材料名称
    /// </summary>
    [Column(StringLength = 100)]
    public string MaterialName { get; set; }

    /// <summary>
    /// 材质
    /// </summary>
    [Column(StringLength = 50)]
    public string Quality { get; set; }

    /// <summary>
    /// 材料规格
    /// </summary>
    [Column(StringLength = 1000)]
    public string Spec { get; set; }

    /// <summary>
    /// 厂牌
    /// </summary>
    [Column(StringLength = 255)]
    public string Brand { get; set; }

    /// <summary>
    /// 单位
    /// </summary>
    [Column(StringLength = 30)]
    public string Unit { get; set; }

    /// <summary>
    /// 数量
    /// </summary>
    [Column(Precision = 10, Scale = 2)]
    public decimal Qty { get; set; }

    /// <summary>
    /// 材料单价
    /// </summary>
    [Column(Precision = 10, Scale = 2)]
    public decimal Price { get; set; }

    /// <summary>
    /// 旧材料单价
    /// </summary>
    [Column(Precision = 10, Scale = 2)]
    public decimal OldPrice { get; set; }

    /// <summary>
    /// 人工单价
    /// </summary>
    [Column(Precision = 10, Scale = 2)]
    public decimal WagesPrice { get; set; }

    /// <summary>
    /// 旧人工单价
    /// </summary>
    [Column(Precision = 10, Scale = 2)]
    public decimal OldWagesPrice { get; set; }

    /// <summary>
    /// 项目Id
    /// </summary>
    public long IndicesId { get; set; }

    /// <summary>
    /// 比较项目Id
    /// </summary>
    public long CompareId { get; set; }
}