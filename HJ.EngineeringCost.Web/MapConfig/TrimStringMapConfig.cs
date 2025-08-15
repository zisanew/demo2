using Mapster;

namespace HJ.EngineeringCost.Web.MapConfig;

public class TrimStringMapConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.ForType<string, string>()
            .MapWith(src => src != null ? src.Trim() : string.Empty);
    }
}