using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Core.Utilities;

public class DebugWand : ModItem
{
    public override string Texture => AssetReferences.Assets.Textures.Item.DebugWand.KEY;
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.DefaultToStaff(ModContent.ProjectileType<StormingTornado>(), 1, 30, 1);
    }

}
