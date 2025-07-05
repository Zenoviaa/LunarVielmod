using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Stellamod.Content.Tiles.MothlightTiles
{
    public class MothlightMushroom : ModTile
    {

        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileMerge[Type][Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileMerge[TileID.Mud][Type] = true;
            Main.tileMerge[TileID.ClayBlock][Type] = true;
            Main.tileBlendAll[Type] = true;
            Main.tileLighted[Type] = true;
            Main.tileBlockLight[Type] = true;
            LocalizedText name = CreateMapEntryName();
            AddMapEntry(new Color(178, 163, 160), name);

            RegisterItemDrop(ModContent.ItemType<MothlightMushroomBlock>());
            // DustType = Main.rand.Next(110, 113);

            //MineResist = 1f;
            //MinPick = 25;
        }

        public override void RandomUpdate(int i, int j)
        {
            Tile tile = Framing.GetTileSafely(i, j);
            Tile tileBelow = Framing.GetTileSafely(i, j + 1);
            Tile tileAbove = Framing.GetTileSafely(i, j - 1);
            /*
            if (!Main.tile[i, j - 1].HasTile && Main.tile[i, j].Slope == 0 && j <= Main.worldSurface - 150)//grass
            {
                if (Main.rand.NextBool(12))
                {
                    WorldGen.PlaceTile(i, j - 1, TileType<AcidMush1>(), true);
                }
            }
            if (!Main.tile[i, j - 1].HasTile && Main.tile[i, j].Slope == 0 && j <= Main.worldSurface - 150)//grass
            {
                if (Main.rand.NextBool(12))
                {
                    WorldGen.PlaceTile(i, j - 1, TileType<AcidMush2>(), true);
                }
            }
            if (!Main.tile[i, j - 1].HasTile && Main.tile[i, j].Slope == 0 && j <= Main.worldSurface - 150)//grass
            {
                if (Main.rand.NextBool(12))
                {
                    WorldGen.PlaceTile(i, j - 1, TileType<AcidMush3>(), true);
                }
            }


            if (!Main.tile[i, j - 1].HasTile && Main.tile[i, j].Slope == 0 && j >= Main.worldSurface - 150)//grass
            {
                if (Main.rand.NextBool(3))
                {
                    WorldGen.PlaceTile(i, j - 2, TileType<AcidBush1>(), true);
                }
            }
            if (!Main.tile[i, j - 1].HasTile && Main.tile[i, j].Slope == 0 && j >= Main.worldSurface - 150)//grass
            {
                if (Main.rand.NextBool(3))
                {
                    WorldGen.PlaceTile(i, j - 2, TileType<AcidBush2>(), true);
                }
            }
            if (!Main.tile[i, j - 1].HasTile && Main.tile[i, j].Slope == 0 && j >= Main.worldSurface - 150)//grass
            {
                if (Main.rand.NextBool(3))
                {
                    WorldGen.PlaceTile(i, j - 2, TileType<AcidBush3>(), true);
                }
            }

            if (WorldGen.genRand.NextBool(2) && !tileBelow.HasTile && !(tileBelow.LiquidType == LiquidID.Lava))
            {
                if (!tile.BottomSlope)
                {
                    tileBelow.TileType = (ushort)ModContent.TileType<AcidVines>();
                    tileBelow.HasTile = true;
                    WorldGen.SquareTileFrame(i, j + 1, true);
                    if (Main.netMode == NetmodeID.Server)
                    {
                        NetMessage.SendTileSquare(-1, i, j + 1, 3, TileChangeType.None);
                    }
                }
            }

            //try place foliage
            if (WorldGen.genRand.NextBool(6) && !tileAbove.HasTile && !(tileBelow.LiquidType == LiquidID.Lava))
            {
                if (!tile.BottomSlope && !tile.TopSlope && !tile.IsHalfBlock && !tile.TopSlope)
                {
                    tileAbove.TileType = (ushort)ModContent.TileType<AcidFoliage>();
                    tileAbove.HasTile = true;
                    tileAbove.TileFrameY = 0;
                    tileAbove.TileFrameX = (short)(WorldGen.genRand.Next(8) * 18);
                    WorldGen.SquareTileFrame(i, j + 1, true);
                    if (Main.netMode == NetmodeID.Server)
                    {
                        NetMessage.SendTileSquare(-1, i, j - 1, 3, TileChangeType.None);
                    }
                }
            }
            */
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            Tile tile = Framing.GetTileSafely(i, j);
            Tile tileBelow = Framing.GetTileSafely(i, j + 1);
            Tile tileAbove = Framing.GetTileSafely(i, j - 1);

            if (!tileAbove.HasTile || !tileBelow.HasTile)
            {
                r = 0.05f;
                g = 0.15f;
                b = 0.25f;
            }
        }








        public class MothlightMushroomBlock : ModItem
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
                Item.useStyle = ItemUseStyleID.Swing;
                Item.consumable = true;
                Item.createTile = ModContent.TileType<MothlightMushroom>();
            }

            // Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
        }
    }
}