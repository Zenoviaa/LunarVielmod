
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Dusts;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Stellamod.Tiles.Structures.Cathedral
{
    public class Orb1 : ModTile
    {
        public override void SetStaticDefaults()
        {
          
            DustType = ModContent.DustType<Orb1Glow>();
            HitSound = SoundID.DD2_WitherBeastCrystalImpact;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);

            TileObjectData.addTile(Type);
            DustType = ModContent.DustType<Dusts.SalfaceDust>();
            AdjTiles = new int[] { TileID.Bookcases };
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLavaDeath[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2xX);
            TileObjectData.newTile.Height = 10;
            TileObjectData.newTile.Width = 4;

            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16, 16, 16, 16, 16, 16, 16, 16};
            TileObjectData.newTile.StyleWrapLimit = 2; //not really necessary but allows me to add more subtypes of chairs below the example chair texture
            TileObjectData.newTile.StyleMultiplier = 2; //same as above
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.addTile(Type);
            LocalizedText name = CreateMapEntryName();

            // name.SetDefault("Crystal Orb");
       

            AddMapEntry(new Color(109, 225, 90));
        }
        public override bool KillSound(int i, int j, bool fail)
        {
            if (!fail)
            {
                SoundEngine.PlaySound(SoundID.DD2_WitherBeastHurt, new Vector2(i, j).ToWorldCoordinates());
                return false;
            }

            return base.KillSound(i, j, fail);
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.125f;
            g = 0.165f;
            b = 0.2f;
        }

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Point p = new Point(i, j);
            Tile tile = Main.tile[p.X, p.Y];

            if (tile == null || !tile.HasTile) { return false; }

            Texture2D texture = ModContent.Request<Texture2D>("Stellamod/Particles/GradientPillar").Value;

            Vector2 offScreen = new Vector2(Main.offScreenRange);
            Vector2 globalPosition = p.ToWorldCoordinates(0f, 0f);
            Vector2 position = globalPosition + offScreen - Main.screenPosition + new Vector2(0f, -100f + 16f);
            Color color = new Color(0.05f, 0.08f, 0.2f, 0f) * (2 * (((float)Math.Sin(Main.GameUpdateCount * 0.02f) + 4) / 4));

            Main.EntitySpriteDraw(texture, position, null, color, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);

            return true;
        }

        public override void NearbyEffects(int i, int j, bool closer)
        {
            Vector2 pos = new Vector2(i, j) * 16;
            Lighting.AddLight(pos, new Vector3(0.1f, 0.32f, 0.5f) * 0.35f);

            if (Main.rand.NextBool(50))
            {
                if (!Main.tile[i, j - 1].HasTile)
                {
                    Dust.NewDustPerfect(pos + new Vector2(Main.rand.NextFloat(0, 16), Main.rand.NextFloat(-32, -16)),
                        ModContent.DustType<Orb1Glow>(), new Vector2(Main.rand.NextFloat(-0.02f, 0.02f), -Main.rand.NextFloat(0.1f, 0.36f)), 0, new Color(0.05f, 0.08f, 0.2f, 0f), Main.rand.NextFloat(0.25f, 0.5f));
                }
            }
        }

        /* public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
         {
             Point p = new Point(i, j);
             Tile tile = Main.tile[p.X, p.Y];

             if (tile == null || !tile.HasTile) { return; }

             Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;

             Vector2 offScreen = new Vector2(Main.offScreenRange);
             Vector2 globalPosition = p.ToWorldCoordinates(0f, 0f);
             Vector2 position = globalPosition + offScreen - Main.screenPosition;
             Color color = Color.White;

             Main.EntitySpriteDraw(texture, position, null, color, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
         }*/
    }









}