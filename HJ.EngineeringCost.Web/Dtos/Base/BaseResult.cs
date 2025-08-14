namespace HJ.EngineeringCost.Web.Dtos;

public class BaseResult
{
    /// <summary>
    /// 状态码：0表示成功，其他值表示失败
    /// </summary>
    public int Code { get; set; } = 0;

    /// <summary>
    /// 消息：默认成功消息为"OK"
    /// </summary>
    public string Message { get; set; } = "OK";

    /// <summary>
    /// 是否成功：Code为0时表示成功
    /// </summary>
    public bool Success => Code == 0;

    public BaseResult Set(string message)
    {
        Message = message;
        return this;
    }

    public BaseResult Set(int code, string message)
    {
        Code = code;
        Message = message;
        return this;
    }
}

/// <summary>
/// 带泛型数据的返回结果
/// </summary>
/// <typeparam name="TData"></typeparam>
public class BaseResult<TData> : BaseResult
{
    /// <summary>
    /// 返回的数据
    /// </summary>
    public TData Data { get; set; }

    public new BaseResult<TData> Set(int code, string message)
    {
        base.Set(code, message);
        Data = default;
        return this;
    }

    public BaseResult<TData> Set(int code, string message, TData data)
    {
        base.Set(code, message);
        Data = data;
        return this;
    }
}