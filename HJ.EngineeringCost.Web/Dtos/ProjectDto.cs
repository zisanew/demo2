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

public class GetProjectMaterialInput
{
    public long ProjectId { get; set; }
}

public class ProjectDto : Project
{
    public List<ProjectMaterial> ProjectMaterialList { get; set; }
}

public class ProjectResult
{
    /// <summary>
    /// 基础类型
    /// </summary>
    public List<string> BaseTypeList { get; set; }

    /// <summary>
    /// 项目类型
    /// </summary>
    public List<string> ProjectTypeList { get; set; }

    /// <summary>
    /// 结构类型
    /// </summary>
    public List<string> StructureTypeList { get; set; }

    /// <summary>
    /// 厂牌
    /// </summary>
    public List<string> MaterialBrandList { get; set; }

    /// <summary>
    /// 材料名称
    /// </summary>
    public List<string> MaterialNameList { get; set; }
}