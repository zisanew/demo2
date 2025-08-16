namespace HJ.EngineeringCost.Web.Dtos;

public class SysRoleMenuDto
{
    /// <summary>
    /// 角色ID
    /// </summary>
    public long RoleId { get; set; }

    /// <summary>
    /// 菜单ID列表
    /// </summary>
    public List<long> MenuIdList { get; set; }
}
