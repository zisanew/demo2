namespace HJ.EngineeringCost.Web.Dtos;

public class GetPageProjectTypeInput : BasePageInput
{
    /// <summary>
    /// 类型名称
    /// </summary>
    public string? TypeName { get; set; }
}

public class ProjectTypeDto : ProjectType
{
}
