using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Core.TileOverlaySystem;

public class GrafittiSponge : ModItem
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.rare = ItemRarityID.Expert;
        Item.useTime = 2;
        Item.useAnimation = 2;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.autoReuse = false;
    }

    public override bool? UseItem(Player player)
    {
        if (Main.myPlayer == player.whoAmI)
        {
            Point tilePoint = Main.MouseWorld.ToTileCoordinates();
            TileOverlayUtility.KillTileOverlay(tilePoint.X, tilePoint.Y);
            if(Main.netMode != NetmodeID.SinglePlayer)
                TileOverlayUtility.SendTileOverlayData(-1, -1, tilePoint.X, tilePoint.Y, 1, 1);
        }

        return true;
    }
}
