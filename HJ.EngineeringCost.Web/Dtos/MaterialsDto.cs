namespace HJ.EngineeringCost.Web.Dtos;

public class GetMaterialsByIdInput
{
    public long Id { get; set; }
}

public class GetMaterialsInput
{
    public string? MaterialName { get; set; }

    public string? Brand { get; set; }
}

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

    /// <summary>
    /// 厂牌
    /// </summary>
    public string? Brand { get; set; }
}

public class MaterialsDto : Materials
{
}
