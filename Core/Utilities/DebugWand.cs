using Stellamod.Common.TornadoRendering;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Stellamod.Core.Utilities;

public class DebugWand : ModItem
{
    public override string Texture => AssetReferences.Assets.Textures.Item.DebugWand.KEY;
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.DefaultToStaff(ModContent.ProjectileType<StormingTornadoV2>(), 1, 30, 1);
        Item.channel = true;
        Item.autoReuse = false;
    }

    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        base.ModifyShootStats(player, ref position, ref velocity, ref type, ref damage, ref knockback);
        type = ModContent.ProjectileType<StormingTornadoV2>();
        position = Main.MouseWorld;
    }
    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {

        return base.Shoot(player, source, position, velocity, type, damage, knockback);
    }
}
