using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Core.Godrays;
using Stellamod.WorldG;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace Stellamod.Content.Areas.PunkerTown.TilesPT
{
    public class MangroveTreeTop : ModTile
    {
        private UnifiedRandom _random;
        private Asset<Texture2D> _topsTextureAsset;
        private int _frameCount;
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            _topsTextureAsset = ModContent.Request<Texture2D>(Texture + "_Tops");
            _frameCount = 1;
            _random = new UnifiedRandom(0);
            LocalizedText name = CreateMapEntryName();
            TileID.Sets.IsATreeTrunk[Type] = true;
         
            Main.tileAxe[Type] = true;
            AddMapEntry(new Color(169, 200, 93), name);
            RegisterItemDrop(ItemID.RichMahogany);
        }

        private float GetLeafSway(float offset, float magnitude, float speed)
        {
            return (float)Math.Sin(Main.GameUpdateCount * speed + offset) * magnitude;
        }
        public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
        {
           // Main.instance.TilesRenderer.AddSpecialLegacyPoint(new Point(i, j));
        }

        private Rectangle GetTopFrame(int rand)
        {
            int frameWidth = _topsTextureAsset.Width() / _frameCount;
            int frameHeight = _topsTextureAsset.Height();
            Rectangle frame = new Rectangle(frameWidth * rand, 0, frameWidth, frameHeight);
            return frame;
        }

        public override void SpecialDraw(int i, int j, SpriteBatch spriteBatch)
        {

        }

        public void DrawTreeTops(int i, int j, SpriteBatch spriteBatch)
        {
            _random.SetSeed(i + j);
            Vector2 pos = (new Vector2(i + 1, j)) * 16;

            Color color = Lighting.GetColor(i, j);
            Rectangle frame = GetTopFrame(_random.Next(0, 1));
            Vector2 offset = new Vector2(-13, 32);
            Vector2 topLeftOffset = new Vector2(-128, -32);

            Color backColor = color.MultiplyRGB(Color.Lerp(Color.White, Color.Black, 0.3f));
            spriteBatch.Draw(_topsTextureAsset.Value, pos - Main.screenPosition + offset + topLeftOffset, frame, backColor, GetLeafSway(3, 0.05f, 0.008f),
         new Vector2(frame.Width / 2, frame.Height), 1, 0, 1);
            Vector2 topRightOffset = new Vector2(128, -64);
            spriteBatch.Draw(_topsTextureAsset.Value, pos - Main.screenPosition + offset + topRightOffset, frame, backColor, GetLeafSway(3, 0.05f, 0.008f),
         new Vector2(frame.Width / 2, frame.Height), 1, 0, 1);


            spriteBatch.Draw(_topsTextureAsset.Value, pos - Main.screenPosition + offset, frame, color, GetLeafSway(3, 0.05f, 0.008f),
                new Vector2(frame.Width / 2, frame.Height), 1, 0, 1);

            if (Main.rand.NextBool(250))
            {
                GodrayRenderer godrayRenderer = ModContent.GetInstance<GodrayRenderer>();
                Vector2 centerPos = new Point(i, j).ToWorldCoordinates();
                centerPos.Y += 128;
                godrayRenderer.AddGodrayParticle(centerPos + Main.rand.NextVector2Circular(64, 64));
            }
        }
        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            if (fail || effectOnly)
                return;

            Framing.GetTileSafely(i, j).HasTile = false;
        }

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            short x = 0;
            short y = 0;

            bool down = Framing.GetTileSafely(i, j + 1).TileType == ModContent.TileType<MangroveTree>();

            if (down)
            {
                y = (short)(Main.rand.Next(_frameCount) * 18);
            }

            Tile tile = Framing.GetTileSafely(i, j);
            tile.TileFrameX = x;
            tile.TileFrameY = y;
            return false;
        }
    }

    public class MangroveTree : ModTile
    {
        private Asset<Texture2D> _rootsTextureAsset;
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            _rootsTextureAsset = ModContent.Request<Texture2D>(Texture + "_Roots");
            LocalizedText name = CreateMapEntryName();
            TileID.Sets.IsATreeTrunk[Type] = true;
            Main.tileAxe[Type] = true;
            AddMapEntry(new Color(169, 200, 93), name);
            RegisterItemDrop(ItemID.RichMahogany);
        }

        private float GetLeafSway(float offset, float magnitude, float speed)
        {
            return (float)Math.Sin(Main.GameUpdateCount * speed + offset) * magnitude;
        }

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            //Draw roots
            bool down = Framing.GetTileSafely(i, j + 1).TileType == ModContent.TileType<MangroveTree>();
            bool left = Framing.GetTileSafely(i - 1, j).TileType == ModContent.TileType<MangroveTree>();
            bool right = Framing.GetTileSafely(i + 1, j).TileType == ModContent.TileType<MangroveTree>();
            if(left && right && !down)
            {
                Vector2 pos = (new Vector2(i + 1, j + 2) + VeilGen.TileAdj) * 16;
                Color color = Lighting.GetColor(i, j);
                pos -= new Vector2(0, 64);
                spriteBatch.Draw(_rootsTextureAsset.Value, pos - Main.screenPosition, null, color.MultiplyRGB(Color.Gray),
                    GetLeafSway(0, 0.05f, 0.01f), new Vector2(_rootsTextureAsset.Width() / 2, 0), 1, 0, 1);
            }
            return base.PreDraw(i, j, spriteBatch);
        }

        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            if (fail || effectOnly)
                return;

            Framing.GetTileSafely(i, j).HasTile = false;

            bool up = Framing.GetTileSafely(i, j - 1).TileType == ModContent.TileType<MangroveTree>() || Framing.GetTileSafely(i, j - 1).TileType == ModContent.TileType<MangroveTreeTop>();
            bool down = Framing.GetTileSafely(i, j + 1).TileType == ModContent.TileType<MangroveTree>();
            bool left = Framing.GetTileSafely(i - 1, j).TileType == ModContent.TileType<MangroveTree>();
            bool right = Framing.GetTileSafely(i + 1, j).TileType == ModContent.TileType<MangroveTree>();


            if (left)
                WorldGen.KillTile(i - 1, j);
            if (right)
                WorldGen.KillTile(i + 1, j);
            if (up)
                WorldGen.KillTile(i, j - 1);
        }

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            short x = 0;
            short y = 0;

            bool up = Framing.GetTileSafely(i, j - 1).TileType == ModContent.TileType<MangroveTree>() || Framing.GetTileSafely(i, j - 1).TileType == ModContent.TileType<MangroveTreeTop>();
            bool down = Framing.GetTileSafely(i, j + 1).TileType == ModContent.TileType<MangroveTree>();
            bool left = Framing.GetTileSafely(i - 1, j).TileType == ModContent.TileType<MangroveTree>();
            bool right = Framing.GetTileSafely(i + 1, j).TileType == ModContent.TileType<MangroveTree>();
            if (right && !left)
            {
                x = 0;
            }
            if (left && !right)
            {
                x = 18 * 2;
            }
            if (left && right)
            {
                x = 18;
            }
            if (up || down)
            {
                //just keep looping over these textures
                int index = j % 6;
                y = (short)(index * 18);
            }

            Tile tile = Framing.GetTileSafely(i, j);
            tile.TileFrameX = x;
            tile.TileFrameY = y;
            return false;
        }
    }
}
