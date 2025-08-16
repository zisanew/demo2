namespace HJ.EngineeringCost.Web.Dtos;

public class GetPageSysMenuInput : BasePageInput
{
    public string? MenuName { get; set; }

    public long? ParentId { get; set; }

    public bool? IsShow { get; set; }
}

public class SysMenuDto : SysMenu
{
}
