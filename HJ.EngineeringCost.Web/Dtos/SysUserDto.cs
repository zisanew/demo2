using HJ.EngineeringCost.Web.Enums;

namespace HJ.EngineeringCost.Web.Dtos;

public class GetPageSysUserInput : BasePageInput
{
    public string? UserName { get; set; }

    public string? RealName { get; set; }

    public UserStatus? UserStatus { get; set; }
}

public class SysUserDto : SysUser
{
}
