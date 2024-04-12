using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Stellamod.Tiles.Ishtar
{
    public class IshtarFoliage : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileCut[Type] = true;
            Main.tileSolid[Type] = false;
            Main.tileMergeDirt[Type] = true;
            //Main.tileBlockLight[Type] = true;
            Main.tileLighted[Type] = false;

            DustType = DustID.WhiteTorch;
            HitSound = SoundID.Grass;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
            TileObjectData.newTile.LavaDeath = true;
            TileObjectData.newTile.WaterDeath = false;
            Main.tileLighted[Type] = true;
            Main.tileBlockLight[Type] = true;
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinateHeights = new int[]
            {
                20
            };
            TileObjectData.newTile.DrawYOffset = -2;
            TileObjectData.newTile.Style = 0;
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.UsesCustomCanPlace = true;

            for (int i = 0; i < 8; i++)
            {
                TileObjectData.newSubTile.CopyFrom(TileObjectData.newTile);
                TileObjectData.addSubTile(TileObjectData.newSubTile.Style);
            }

            TileObjectData.addTile(Type);
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = 2;
        }


        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {

            Tile tile = Main.tile[i, j];
            var zero = new Vector2(Main.offScreenRange, Main.offScreenRange);
            if (Main.drawToScreen)
            {
                zero = Vector2.Zero;
            }
            int height = tile.TileFrameY == 36 ? 18 : 16;
           
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.1f;
            g = 0.1f;
            b = 0.1f;
        }
    }
}