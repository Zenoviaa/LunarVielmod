using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.WorldG;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.ObjectData;
using Terraria.Utilities;

namespace Stellamod.TilesNew.RainforestTiles
{
    internal class RainforestTreeSapling : ModTile
    {

        public override void SetStaticDefaults()
        {
            TileObjectData.newTile.RandomStyleRange = 3;
            TileObjectData.newTile.StyleHorizontal = true;

            var bottomAnchor = new AnchorData(Terraria.Enums.AnchorType.SolidTile, 1, 0);

            Main.tileTable[Type] = true;
            Main.tileSolidTop[Type] = false;
            Main.tileNoAttach[Type] = true;
            Main.tileLavaDeath[Type] = true;
            Main.tileFrameImportant[Type] = true;
            TileID.Sets.DisableSmartCursor[Type] = true;
            TileID.Sets.IgnoredByNpcStepUp[Type] = true; // This line makes NPCs not try to step up this tile during their movement. Only use this for furniture with solid tops.

            AdjTiles = new int[] { TileID.Bookcases };
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLavaDeath[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2xX);
            TileObjectData.newTile.Height = 2;
            TileObjectData.newTile.Width = 1;

            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16 };
            TileObjectData.newTile.StyleWrapLimit = 2; //not really necessary but allows me to add more subtypes of chairs below the example chair texture
            TileObjectData.newTile.StyleMultiplier = 2; //same as above
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.addTile(Type);
            LocalizedText name = CreateMapEntryName();

            // name.SetDefault("Alcaology Station");
            AddMapEntry(new Color(126, 200, 59), name);
        }

        public override void RandomUpdate(int i, int j)
        {
            Tile tile = Main.tile[i, j];

            if (!(tile.TileFrameX % 36 == 0 && tile.TileFrameY == 0))
                return;

            // vanilla tree seems to be about >5%?
            // at 3% after over an hour and a half 6/10 grew
            if (WorldGen.genRand.NextBool(10))
            { // still needs testing but should be fast enough
                return;
            }
            if (!VeilGen.IsRainforestTreeGround(i - 1, j + 3, 4))
                return;

            int height = Main.rand.Next(10, 75);
            const int AbsMinHeight = 9; // lower than the min height normally

            for (int h = 0; h < height; h++) // finds clear area
            {
                if (!IsAir(i - 1, j + 2 - h, 4))
                {
                    if (h > AbsMinHeight) // if above min height, just use current height
                    {
                        height = h - 4; // needs to account for the height of the base
                        break;
                    }
                    else // if below min height, cancel
                    {
                        return;
                    }
                }
            }

            // removes sapling
            for (int g = 0; g < 2; g++)
            {
                for (int h = 0; h < 3; h++)
                    Main.tile[i + g, j + h].ClearTile();
            }

            VeilGen.PlaceRaintrees(i, j + 3, height);
        }

        private bool IsAir(int x, int y, int w) // method from worldgen, but needs to skip sapling and platform
        {
            for (int k = 0; k < w; k++)
            {
                Tile tile = Framing.GetTileSafely(x + k, y);
                if (tile.HasTile && tile.TileType != Type && Main.tileSolid[tile.TileType] && !Main.tileSolidTop[tile.TileType]) // this version allows the tree to break stuff the player can stand on but pass though (platforms, tool stations)
                    return false;
            }

            return true;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = 1;
        }
    }

    internal class RainForestTreeTopSystem : ModSystem
    {
        private List<Point> _treeTopsToNotDraw;
        public override void ClearWorld()
        {
            base.ClearWorld();
            _treeTopsToNotDraw = new();
        }

        public bool ShouldDrawTreeTop(int i, int j)
        {
            Point point = new Point(i, j);
            return !_treeTopsToNotDraw.Contains(point);
        }

        public void SetTreeTopData(int i, int j, bool shouldDrawTreeTop)
        {
            Point point = new Point(i, j);
            if (shouldDrawTreeTop)
            {
                if (_treeTopsToNotDraw.Contains(point))
                    _treeTopsToNotDraw.Remove(point);
            }
            else
            {
                if (!_treeTopsToNotDraw.Contains(point))
                    _treeTopsToNotDraw.Add(point);
            };
        }

        public override void SaveWorldData(TagCompound tag)
        {
            base.SaveWorldData(tag);
            tag["treeTops"] = _treeTopsToNotDraw;
        }

        public override void LoadWorldData(TagCompound tag)
        {
            base.LoadWorldData(tag);
            if (tag.ContainsKey("treeTops"))
            {
                _treeTopsToNotDraw = tag.Get<List<Point>>("treeTops");
            }
        }
    }

    internal class RainforestTree : ModTile
    {

        private UnifiedRandom _random;
        public override void SetStaticDefaults()
        {
            _random = new UnifiedRandom(0);
            LocalizedText name = CreateMapEntryName();
            TileID.Sets.IsATreeTrunk[Type] = true;
            Main.tileAxe[Type] = true;
            AddMapEntry(new Color(169, 200, 93), name);
            RegisterItemDrop(ItemID.Wood);
        }

        private float GetLeafSway(float offset, float magnitude, float speed)
        {
            return (float)Math.Sin(Main.GameUpdateCount * speed + offset) * magnitude;
        }

        public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
        {
            bool right = Framing.GetTileSafely(i + 1, j).TileType == ModContent.TileType<RainforestTree>();
            bool up = Framing.GetTileSafely(i, j - 1).TileType == ModContent.TileType<RainforestTree>();
            bool down = Framing.GetTileSafely(i, j + 1).TileType == ModContent.TileType<RainforestTree>();

            if (right && !up && down || !up && !down)
                Main.instance.TilesRenderer.AddSpecialLegacyPoint(new Point(i, j));
        }


        private void DrawBranches(int i, int j, SpriteBatch spriteBatch)
        {
            Vector2 pos2 = (new Vector2(i + 1, j) + VeilGen.TileAdj) * 16;
            Color color2 = Lighting.GetColor(i, j);
            _random.SetSeed(i + j);
            SpriteEffects Flipper = 0;
            if (_random.NextBool(2))
            {
                Flipper = SpriteEffects.FlipHorizontally;
            }

            bool Drawbranch = _random.NextBool(6);
            int Treebranch = _random.Next(4) + 1;


            Vector2 BranchOffset = new Vector2(50, 40);
            if (Drawbranch)
            {
                if (Flipper == 0)
                {
                    Texture2D tex2 = ModContent.Request<Texture2D>(Texture + "Branches" + Treebranch).Value;
                    spriteBatch.Draw(tex2, pos2 + BranchOffset - Main.screenPosition, null, color2.MultiplyRGB(Color.White), GetLeafSway(0, 0.05f, 0.01f), new Vector2(tex2.Width / 2, tex2.Height), 1, 0, 0);

                }

                if (Flipper == SpriteEffects.FlipHorizontally)
                {
                    Texture2D tex3 = ModContent.Request<Texture2D>(Texture + "Branches" + Treebranch).Value;
                    spriteBatch.Draw(tex3, pos2 + BranchOffset - new Vector2(116, 0) - Main.screenPosition, null, color2.MultiplyRGB(Color.White), GetLeafSway(0, 0.05f, 0.01f), new Vector2(tex3.Width / 2, tex3.Height), 1, SpriteEffects.FlipHorizontally, 0);

                }
            }
        }

        public override void SpecialDraw(int i, int j, SpriteBatch spriteBatch)
        {
            bool left = Framing.GetTileSafely(i - 1, j).TileType == ModContent.TileType<RainforestTree>();
            bool right = Framing.GetTileSafely(i + 1, j).TileType == ModContent.TileType<RainforestTree>();
            bool up = Framing.GetTileSafely(i, j - 1).TileType == ModContent.TileType<RainforestTree>();
            bool down = Framing.GetTileSafely(i, j + 1).TileType == ModContent.TileType<RainforestTree>();

            RainForestTreeTopSystem rainForestTreeTopSystem = ModContent.GetInstance<RainForestTreeTopSystem>();
            bool shouldDraw = rainForestTreeTopSystem.ShouldDrawTreeTop(i, j);

            //Draw Tree Tops
            if (shouldDraw && right && !up && down)
            {
                Texture2D tex = ModContent.Request<Texture2D>(Texture + "Top").Value;
                Vector2 pos = (new Vector2(i + 1, j) + VeilGen.TileAdj) * 16;

                Color color = Lighting.GetColor(i, j);
                spriteBatch.Draw(tex, pos - Main.screenPosition, null, color, GetLeafSway(3, 0.05f, 0.008f), new Vector2(tex.Width / 2, tex.Height), 1, 0, 1);
            }
        }

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            bool left = Framing.GetTileSafely(i - 1, j).TileType == ModContent.TileType<RainforestTree>();
            bool right = Framing.GetTileSafely(i + 1, j).TileType == ModContent.TileType<RainforestTree>();
            bool up = Framing.GetTileSafely(i, j - 1).TileType == ModContent.TileType<RainforestTree>();
            bool down = Framing.GetTileSafely(i, j + 1).TileType == ModContent.TileType<RainforestTree>();


            RainForestTreeTopSystem rainForestTreeTopSystem = ModContent.GetInstance<RainForestTreeTopSystem>();
            bool shouldDraw = rainForestTreeTopSystem.ShouldDrawTreeTop(i, j);

            //Only draw branches if we have a tile on the right side
            //Since it's a 1x2 tile, it'd draw twice if we didn't have this check otherwise
            if (right)
            {
                DrawBranches(i, j, spriteBatch);
            }


            //Draw Tree Tops
            if (shouldDraw && right && !up && down)
            {
                Texture2D tex = ModContent.Request<Texture2D>(Texture + "Back").Value;
                Vector2 pos = (new Vector2(i + 1, j) + VeilGen.TileAdj) * 16;

                Color color = Lighting.GetColor(i, j);

                spriteBatch.Draw(tex, pos + new Vector2(50, 40) - Main.screenPosition, null, color.MultiplyRGB(Color.Gray),
                    GetLeafSway(0, 0.05f, 0.01f), new Vector2(tex.Width / 2, tex.Height), 1, 0, 1);
                spriteBatch.Draw(tex, pos + new Vector2(-30, 80) - Main.screenPosition, null, color.MultiplyRGB(Color.DarkGray),
                    GetLeafSway(2, 0.025f, 0.012f), new Vector2(tex.Width / 2, tex.Height), 1, 0, 1);
            }

            return true;
        }


        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            if (fail || effectOnly)
                return;

            Framing.GetTileSafely(i, j).HasTile = false;

            bool left = Framing.GetTileSafely(i - 1, j).TileType == ModContent.TileType<RainforestTree>();
            bool right = Framing.GetTileSafely(i + 1, j).TileType == ModContent.TileType<RainforestTree>();
            bool up = Framing.GetTileSafely(i, j - 1).TileType == ModContent.TileType<RainforestTree>();
            bool down = Framing.GetTileSafely(i, j + 1).TileType == ModContent.TileType<RainforestTree>() ||
                Framing.GetTileSafely(i, j + 1).TileType == ModContent.TileType<RainforestTreeBase>();

            if (left)
                WorldGen.KillTile(i - 1, j);
            if (right)
                WorldGen.KillTile(i + 1, j);
            if (up)
                WorldGen.KillTile(i, j - 1);

            RainForestTreeTopSystem rainForestTreeTopSystem = ModContent.GetInstance<RainForestTreeTopSystem>();
            if (down)
            {
                rainForestTreeTopSystem.SetTreeTopData(i, j + 1, false);
            }
            rainForestTreeTopSystem.SetTreeTopData(i, j, true);
            /*
            if (down)
                WorldGen.KillTile(i, j + 1);*/
        }

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            short x = 0;
            short y = 0;

            bool left = Framing.GetTileSafely(i - 1, j).TileType == ModContent.TileType<RainforestTree>();
            bool right = Framing.GetTileSafely(i + 1, j).TileType == ModContent.TileType<RainforestTree>();
            bool up = Framing.GetTileSafely(i, j - 1).TileType == ModContent.TileType<RainforestTree>();
            bool down = Framing.GetTileSafely(i, j + 1).TileType == ModContent.TileType<RainforestTree>();

            if (up || down)
            {
                if (right)
                    x = 0;

                if (left)
                    x = 18;

                y = (short)(Main.rand.Next(3) * 18);

                if (Main.rand.NextBool(3))
                    x += 18 * 2;
            }

            Tile tile = Framing.GetTileSafely(i, j);
            tile.TileFrameX = x;
            tile.TileFrameY = y;

            return false;
        }
    }

    class RainforestTreeBase : ModTile
    {
        public override void SetStaticDefaults()
        {
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile, 2, 0);
            Main.tileAxe[Type] = true;
            TileID.Sets.PreventsTileRemovalIfOnTopOfIt[Type] = true;
            TileID.Sets.PreventsTileReplaceIfOnTopOfIt[Type] = true;
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;
            //  TileObjectData.newTile.Origin = new Point16(TileObjectData.newTile.Width / 2, TileObjectData.newTile.Height - 1);

            for (int k = 0; k < TileObjectData.newTile.Height; k++)
            {
                TileObjectData.newTile.CoordinateHeights[k] = 16;
            }

            //this breaks for some tiles: the two leads are multitiles and tiles with random styles
            TileObjectData.newTile.CoordinateHeights[TileObjectData.newTile.Height - 1] = 18;

            Main.tileSolidTop[Type] = false;
            Main.tileNoAttach[Type] = true;
            Main.tileLavaDeath[Type] = true;// This line makes NPCs not try to step up this tile during their movement. Only use this for furniture with solid tops.

            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLavaDeath[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2xX);
            TileObjectData.newTile.Height = 2;
            TileObjectData.newTile.Width = 2;

            TileObjectData.newTile.DrawXOffset = 16;

            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16 };
            TileObjectData.addTile(Type);
            LocalizedText name = CreateMapEntryName();
        }

        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            if (fail || effectOnly)
                return;

            Framing.GetTileSafely(i, j).HasTile = false;

            bool up = Framing.GetTileSafely(i, j - 1).TileType == ModContent.TileType<RainforestTree>();

            if (up)
                WorldGen.KillTile(i, j - 1);
        }
    }


    public class RainforestTreeAcorn : ModItem
    {
        public override void SetStaticDefaults()
        {
            // Tooltip.SetDefault("Super silk!");
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
            Item.value = Item.buyPrice(silver: 50);
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.createTile = ModContent.TileType<RainforestTreeSapling>();
        }
        // Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.


    }
}