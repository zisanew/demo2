namespace HJ.EngineeringCost.Web.Dtos;

public class BasePageInput
{
    /// <summary>
    /// 页码
    /// </summary>
    public int PageIndex { get; set; } = 1;

    /// <summary>
    /// 每页记录数
    /// </summary>
    public int PageSize { get; set; } = 20;

    /// <summary>
    /// 排序
    /// </summary>
    public string? Sort { get; set; }
}
