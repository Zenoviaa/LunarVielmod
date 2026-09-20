using Stellamod.Core;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Common.AutoHammerSystem;

public class AutoHammer : ModItem
{
    public int index;
    public static SlopeType slope;
    public static bool halfBlock;
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.rare = ItemRarityID.White;
        Item.useTime = 4;
        Item.useAnimation = 4;
        Item.useStyle = ItemUseStyleID.Swing;
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
                index++;
    
                halfBlock = index == 5;
                if (index >= 6)
                {
                    index = 0;
                }
                if (index < 5)
                {
                    slope = (SlopeType)index;
                }
                var sound = SoundID.Tink;
                SoundEngine.PlaySound(sound);
            }
            else
            {
                Point tilePoint = Main.MouseWorld.ToTileCoordinates();
                if (halfBlock)
                {
                    WorldGen.PoundTile(tilePoint.X, tilePoint.Y);
                }
                else
                {
                    WorldGen.SlopeTile(tilePoint.X, tilePoint.Y, (int)slope);
                }
                NetMessage.SendTileSquare(-1, tilePoint.X, tilePoint.Y);
            }
        }

        return true;
    }
    public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
    {
        base.PostDrawInInventory(spriteBatch, position, frame, drawColor, itemColor, origin, scale);
        var textureAsset = AssetReferences.Common.AutoHammerSystem.AutoHammer_SlopeTypes.Asset;
        SpritebatchDrawer drawer;
        if (!halfBlock)
        {
            drawer = SpritebatchDrawer.FromTextureAsset(textureAsset, Main.screenPosition + position);
            drawer.VerticalFrame((int)slope, 6);
       
        }
        else
        {
            drawer = SpritebatchDrawer.FromTextureAsset(textureAsset, Main.screenPosition + position);
            drawer.VerticalFrame(5, 6);

        }
        drawer.color = Color.White;
        drawer.CenterOrigin();
        drawer.worldPosition += new Vector2(8);
        spriteBatch.Draw(drawer);
    }
}
