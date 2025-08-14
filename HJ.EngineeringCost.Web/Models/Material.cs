using FreeSql.DataAnnotations;

namespace HJ.EngineeringCost.Web.Models;

[Table(Name = "material")]
public class Material : EntityBase
{
    /// <summary>
    /// 材料编号
    /// </summary>
    [Column(StringLength = 255)]
    public string MaterialCode { get; set; }

    /// <summary>
    /// 材料名称
    /// </summary>
    [Column(StringLength = 255, IsNullable = false)]
    public string MaterialName { get; set; }

    /// <summary>
    /// 材料属性
    /// </summary>
    [Column(StringLength = 255)]
    public string MaterialType { get; set; }

    /// <summary>
    /// 材料规格
    /// </summary>
    [Column(StringLength = 1000)]
    public string Spec { get; set; }

    /// <summary>
    /// 价格
    /// </summary>
    [Column(Precision = 10, Scale = 2)]
    public decimal Price { get; set; }

    /// <summary>
    /// 数量
    /// </summary>
    [Column(Precision = 10, Scale = 2)]
    public decimal Qty { get; set; }

    /// <summary>
    /// 单位
    /// </summary>
    [Column(StringLength = 20)]
    public string Unit { get; set; }

    /// <summary>
    /// 供应商
    /// </summary>
    [Column(StringLength = 255)]
    public string Supplier { get; set; }

    /// <summary>
    /// 联系电话
    /// </summary>
    [Column(StringLength = 30)]
    public string ContactPhone { get; set; }

    /// <summary>
    /// 材质
    /// </summary>
    [Column(StringLength = 100)]
    public string Quality { get; set; }

    /// <summary>
    /// 品牌
    /// </summary>
    [Column(StringLength = 100)]
    public string Brand { get; set; }

    /// <summary>
    /// 工资单价
    /// </summary>
    [Column(Precision = 10, Scale = 2)]
    public decimal WagesPrice { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    [Column(StringLength = 255)]
    public string Remark { get; set; }
}