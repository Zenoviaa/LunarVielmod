using ReLogic.Content;
using Stellamod.Content.Areas.SpringHills.TilesSH;
using Stellamod.Core.Utilities;
using Stellamod.Core.ZTileSystem;
using Stellamod.Helpers;
using Stellamod.Tiles.Abyss;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace Stellamod.Content.Areas.WorldsEnd.TilesWE
{
    public class BigWhiteFlower : ZTile
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            frameCount = 1;
            drawOrigin = TileDrawOrigin.Center;

            //idk
            windSwayOffset = 0f;

            //The max it can sway
            windSwayMagnitude = 0.2f;

            //How fast it sways
            windSwaySpeed = 0.02f;
        }
    }

    public class WhiteFlower : ZTile
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            frameCount = 4;
            drawOrigin = TileDrawOrigin.Center;

            //idk
            windSwayOffset = 0f;

            //The max it can sway
            windSwayMagnitude = 0.2f;

            //How fast it sways
            windSwaySpeed = 0.02f;
        }
    }

    public class WhiteGrassBlock : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 100;

        }

        public override void SetDefaults()
        {
            Item.width = 12;
            Item.height = 12;
            Item.maxStack = Item.CommonMaxStack;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 10;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.createTile = ModContent.TileType<WhiteGrass>();
        }
    }

    public class WhiteGrass : ModTile
    {
        private UnifiedRandom _flowerRandom;
        private Asset<Texture2D> _flowerTextureAsset;
        public override void Unload()
        {
            base.Unload();
            _flowerTextureAsset = null;
        }
        public override void SetStaticDefaults()
        {
            _flowerTextureAsset = ModContent.Request<Texture2D>(Texture + "_Flowers");
            Main.tileSolid[Type] = true;
            Main.tileMerge[Type][Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileMerge[TileID.Dirt][Type] = true;
            Main.tileMerge[TileID.Grass][Type] = true;
            Main.tileBlendAll[Type] = true;
            Main.tileLighted[Type] = true;
            Main.tileBlockLight[Type] = true;
            AddMapEntry(Color.LightGray);
        }

        public override void RandomUpdate(int i, int j)
        {

        }

        private Rectangle GetFrame(int index)
        {
            int frameCount = 4;
            int frameWidth = _flowerTextureAsset.Width();
            int frameHeight = _flowerTextureAsset.Height() / frameCount;
            Rectangle sourceRect = new Rectangle(0, frameHeight * index, frameWidth, frameHeight);
            return sourceRect;
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            base.PostDraw(i, j, spriteBatch);
            Tile tileAbove = Framing.GetTileSafely(i, j - 1);
            if (tileAbove.HasTile)
                return;
            _flowerRandom ??= new UnifiedRandom();
            _flowerRandom.SetSeed(i + j);
            float numFlowers = _flowerRandom.Next(1, 3);
            VelocityMap velocityMap = ModContent.GetInstance<VelocityMap>();
            Vector2 worldPos = new Point(i, j).ToWorldCoordinates();
            for (int k = 0; k < numFlowers; k++)
            {
        
                Vector2 flowerDrawPos = worldPos - Main.screenPosition + new Vector2(Main.offScreenRange);
                flowerDrawPos.X += _flowerRandom.NextFloat(-4f, 4f);
                flowerDrawPos.Y += _flowerRandom.NextFloat(-4f, 4f);
                flowerDrawPos.Y -= 12;
                int index = _flowerRandom.Next(0, 4);
                Rectangle frame = GetFrame(index);

                Color lightColor = Lighting.GetColor(i, j);
                float range = MathHelper.ToRadians(15);
                float rotation = ExtraMath.Osc(-range, range, speed: 2, i + k);
        
                Vector2 origin = frame.Size() * 0.5f;
                float scale = _flowerRandom.NextFloat(0.5f, 1f);
                spriteBatch.Draw(_flowerTextureAsset.Value, flowerDrawPos, frame, lightColor, rotation, origin, scale, SpriteEffects.None, 0);
            }
        
        }
    }
}
