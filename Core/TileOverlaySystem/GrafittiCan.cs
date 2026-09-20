using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Core.TileOverlaySystem;

public class GrafittiCan : ModItem
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

    public override bool AltFunctionUse(Player player)
    {
        return true;
    }
    public override bool? UseItem(Player player)
    {
        if (Main.myPlayer == player.whoAmI)
        {
            if (player.altFunctionUse == 2)
            {
                TileOverlaySelector.OverlayCenter = Main.MouseScreen;
                TileOverlaySelector selector = ModContent.GetInstance<TileOverlaySelector>();
                selector.ToggleUI();
            }

            Point tilePoint = Main.MouseWorld.ToTileCoordinates();
            byte type = TileOverlayUtility.TileOverlayType<GoldenLeafTileOverlayData>();
            TileOverlayUtility.PlaceTileOverlay(tilePoint.X, tilePoint.Y, type, new TileOverlayPlacer { frame = (byte)Main.rand.Next(20) });
            if (Main.netMode != NetmodeID.SinglePlayer)
                TileOverlayUtility.SendTileOverlayData(-1, -1, tilePoint.X, tilePoint.Y, 1, 1);
        }

        return true;
    }
}
