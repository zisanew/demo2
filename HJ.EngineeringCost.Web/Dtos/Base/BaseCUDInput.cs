namespace HJ.EngineeringCost.Web.Dtos;

/// <summary>
/// 增删改入参
/// </summary>
/// <typeparam name="TData"></typeparam>
public class BaseCUDInput<TData> : BaseInput
{
    /// <summary>
    /// 传入数据
    /// </summary>
    public IEnumerable<TData>? Data { get; set; }
}
