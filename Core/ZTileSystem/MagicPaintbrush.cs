using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Core.ZTileSystem;

public class MagicPaintbrush : ModItem
{
    public static int z;
    public static ZRenderLayer renderLayer;
    public static ZTileInstanceData templateData;
    public override void SetDefaults()
    {
        base.SetDefaults();
        z = 5;
        renderLayer = ZRenderLayer.InFrontOfWalls;
        Item.width = 24;
        Item.height = 24;
        Item.useAnimation = 2;
        Item.useTime = 2;
        Item.useStyle = ItemUseStyleID.Swing;
        Item.UseSound = SoundID.Bird;
        Item.autoReuse = false;
    }

    public override bool AltFunctionUse(Player player)
    {
        return true;
    }

    public override bool? UseItem(Player player)
    {
        if(Main.myPlayer == player.whoAmI)
        {
            if(player.altFunctionUse == 2)
            {

            } 
            else
            {
                ZTileMap tileMap = ModContent.GetInstance<ZTileMap>();
                tileMap.CreateTile(renderLayer, Main.MouseWorld, z, templateData);
            }    
        }
        return base.UseItem(player);
    }
}
