using System.Collections.Generic;
using Terraria.ModLoader;

namespace Stellamod.Core.DungeonFogSystem;

/// <summary>
/// Implement this type to create a rectangle with fog around a certain area
/// </summary>
public abstract class DungeonFogType : ModType
{
    public sealed override void SetupContent()
    {
        base.SetupContent();
 
        SetStaticDefaults();
    }
    protected override void Register()
    {
        ModTypeLookup<DungeonFogType>.Register(this);
    }

    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        DungeonFogSystem.OnPrepareFog += PrepareFogRectangle;
    }

    public override void Unload()
    {
        base.Unload();
        DungeonFogSystem.OnPrepareFog -= PrepareFogRectangle;
    }

    
    protected abstract void PrepareFogRectangle(List<DungeonFog> fogRectangles);
}
