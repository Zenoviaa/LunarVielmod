using Terraria.ModLoader;

namespace Stellamod.Core.Foreground;

public abstract class ForegroundGore : ModTexturedType
{
    public int type;
    public int frameCount;
    protected override void Register()
    {
        ModTypeLookup<ForegroundGore>.Register(this);
    }
    public sealed override void SetupContent()
    {
        base.SetupContent();
        SetStaticDefaults();
    }
}
