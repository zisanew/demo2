using FreeSql.DataAnnotations;

namespace HJ.EngineeringCost.Web.Models;

[Table(Name = "project")]
public class Project : EntityBase
{
    /// <summary>
    /// 项目名称
    /// </summary>
    [Column(StringLength = 255, IsNullable = false)]
    public string ProjectName { get; set; }

    /// <summary>
    /// 项目编号
    /// </summary>
    [Column(StringLength = 25)]
    public string ProjectCode { get; set; }

    /// <summary>
    /// 工程类型
    /// </summary>
    [Column(StringLength = 30)]
    public string EngineeringType { get; set; }

    /// <summary>
    /// 工程标准
    /// </summary>
    [Column(StringLength = 30)]
    public string EngineeringStandard { get; set; }

    /// <summary>
    /// 结构类型
    /// </summary>
    [Column(StringLength = 50)]
    public string StructureType { get; set; }

    /// <summary>
    /// 基础类型
    /// </summary>
    [Column(StringLength = 50)]
    public string BaseType { get; set; }

    /// <summary>
    /// 工程价格
    /// </summary>
    [Column(Precision = 10, Scale = 2)]
    public decimal EngineeringPrice { get; set; }

    /// <summary>
    /// 建设单位
    /// </summary>
    [Column(StringLength = 255)]
    public string ConstructionUnit { get; set; }

    /// <summary>
    /// 设计单位
    /// </summary>
    [Column(StringLength = 255)]
    public string DesignUnit { get; set; }

    /// <summary>
    /// 施工单位
    /// </summary>
    [Column(StringLength = 255)]
    public string ConstructionCompany { get; set; }

    /// <summary>
    /// 厂区
    /// </summary>
    [Column(StringLength = 50)]
    public string Factory { get; set; }
}