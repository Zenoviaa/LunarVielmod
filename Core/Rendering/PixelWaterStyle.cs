using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Core.Rendering;

public abstract class PixelWaterStyle : ModType
{
    /// <summary>
    /// If multiple waters are active, this determines which one gets used
    /// </summary>
    public int priority;
    protected sealed override void Register()
    {
        ModTypeLookup<PixelWaterStyle>.Register(this);
    }
    public sealed override void SetupContent()
    {
        base.SetupContent();
        SetStaticDefaults();
    }

    public virtual bool IsActive(Player player)
    {
        return true;
    }
    public virtual void ModifyPixelWater(ref PixelWater pixelWater)
    {

    }
}
