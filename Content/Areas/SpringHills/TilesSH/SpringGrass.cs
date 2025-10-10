
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.SpringHills.TilesSH
{
    public class SpringGrass : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = true;
            Main.tileBlockLight[Type] = true;
            RegisterItemDrop(ModContent.ItemType<SpringGrassBlock>());
            MineResist = 1f;
            MinPick = 50;
            AddMapEntry(new Color(110, 74, 51));
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override void RandomUpdate(int i, int j)
        {
            base.RandomUpdate(i, j);
            int[] tilesToChooseFrom = new int[]
            {
                ModContent.WallType<SpringFlower>(),
                ModContent.WallType<SpringFlowerBlueBush>(),
                ModContent.WallType<SpringFlowerDarkPurple>(),
                ModContent.WallType<SpringFlowerGrass>(),
                ModContent.WallType<SpringFlowerPurpleBush>(),
                ModContent.WallType<SpringFlowerPurpleLeaf>(),
                ModContent.WallType<SpringFlowerRedBush>(),
                ModContent.WallType<SpringFlowerVine>(),
                ModContent.WallType<SpringFlowerWhite>(),
                ModContent.WallType<SpringFlowerWhiteBud>(),
                ModContent.WallType<SpringFlowerWhiteBudSmall>(),
            };

            Tile tile = Framing.GetTileSafely(i, j);
            Tile tileBelow = Framing.GetTileSafely(i, j + 1);
            if (!Main.tile[i, j - 1].HasTile && Main.tile[i, j].Slope == 0)//grass
            {
                if (Main.rand.NextBool(2))
                {
                    int wallType = tilesToChooseFrom[Main.rand.Next(0, tilesToChooseFrom.Length)];
                    if (tile.WallType == WallID.FlowerUnsafe || tile.WallType == WallID.GrassUnsafe || tile.WallType == WallID.LivingLeaf)
                    {
                        WorldGen.KillWall(i, j);
                        WorldGen.PlaceWall(i, j, wallType, true);
                    }

                }
            }
        }
    }

    public class SpringGrassBlock : ModItem
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
            Item.createTile = ModContent.TileType<SpringGrass>();
        }

        // Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
    }
}