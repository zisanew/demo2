namespace HJ.EngineeringCost.Web.Dtos;

public class GetPageProjectInput : BasePageInput
{
    /// <summary>
    /// 项目名称
    /// </summary>
    public string? ProjectName { get; set; }

    /// <summary>
    /// 厂区
    /// </summary>
    public string? Factory { get; set; }

    /// <summary>
    /// 开始时间
    /// </summary>
    public DateTime? StartDate { get; set; }

    /// <summary>
    /// 结束时间
    /// </summary>
    public DateTime? EndDate { get; set; }
}

public class ProjectInput : Project
{
}
