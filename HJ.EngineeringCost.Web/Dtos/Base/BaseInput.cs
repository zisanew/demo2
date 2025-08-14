using System.ComponentModel;
using System.Diagnostics;

namespace HJ.EngineeringCost.Web.Dtos;

/// <summary>
/// 传入参数基类
/// </summary>
[DebuggerStepThrough]
public class BaseInput
{
    public void Init(BaseInput input)
    {
        ClientId = input.ClientId;
        SessionId = input.SessionId;
        Timestamp = input.Timestamp;
    }

    /// <summary>
    /// 操作终端Id
    /// </summary>
    [DefaultValue("TEST")]
    public string ClientId { get; set; }

    /// <summary>
    /// 会话Id
    /// </summary>
    [DefaultValue("00000000-0000-0000-0000-000000000000")]
    public string SessionId { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// 时间戳
    /// </summary>
    public DateTime? Timestamp { get; set; } = DateTime.Now;
}
