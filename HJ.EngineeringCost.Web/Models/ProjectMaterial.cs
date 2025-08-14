using FreeSql.DataAnnotations;

namespace HJ.EngineeringCost.Web.Models;

[Table(Name = "project_material")]
public class ProjectMaterial : EntityBase
{
    /// <summary>
    /// 项目Id
    /// </summary>
    public long ProjectId { get; set; }

    /// <summary>
    /// 材料Id
    /// </summary>
    public long MaterialId { get; set; }

    /// <summary>
    /// 材料数量
    /// </summary>
    [Column(Precision = 10, Scale = 2)]
    public decimal Qty { get; set; }

    /// <summary>
    /// 材料价格
    /// </summary>
    [Column(Precision = 10, Scale = 2)]
    public decimal Price { get; set; }

    /// <summary>
    /// 工资单价
    /// </summary>
    [Column(Precision = 10, Scale = 2)]
    public decimal WagesPrice { get; set; }
}