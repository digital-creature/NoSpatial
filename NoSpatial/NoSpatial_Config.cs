using ResoniteModLoader;

namespace NoSpatial;

public partial class NoSpatial// Config
{
    [AutoRegisterConfigKey]
    public static readonly ModConfigurationKey<bool> Enabled = new("Enabled", "Disable the spatial effect on audio", () => true);
    
}
