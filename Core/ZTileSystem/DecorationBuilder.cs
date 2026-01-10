using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Core.ZTileSystem;

public class DecorationBuilder : ModItem
{
    public static int z = 5;
    public static ZRenderLayer renderLayer = ZRenderLayer.InFrontOfWalls;
    public static ZTileInstanceData templateData;
    public static float scale = 1;
    public static Rotation rotation;
    public static ushort frame;
    public static bool flip;
    public static byte value;
    public override void SetDefaults()
    {
        base.SetDefaults();
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
        if (Main.myPlayer == player.whoAmI)
        {
            ZTileMap tileMap = ModContent.GetInstance<ZTileMap>();
            if (player.altFunctionUse == 2)
            {
                tileMap.KillTile(renderLayer, Main.MouseWorld, z);
            }
            else
            {

                ZTileInstanceData instanceData = templateData;
                instanceData.scale = scale;
                instanceData.rotation = rotation;
                instanceData.frameNumber = frame;
                instanceData.flipX = flip;
                instanceData.value = value;
                tileMap.CreateTile(renderLayer, Main.MouseWorld, z, instanceData);
            }
        }
        return base.UseItem(player);
    }
}
