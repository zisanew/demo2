namespace HJ.EngineeringCost.Web.Dtos;

public class GetPageMaterialsInput : BasePageInput
{
    /// <summary>
    /// 材料编码
    /// </summary>
    public string? MaterialCode { get; set; }

    /// <summary>
    /// 材料名称
    /// </summary>
    public string? MaterialName { get; set; }
}

public class MaterialsInput : Materials
{
}
