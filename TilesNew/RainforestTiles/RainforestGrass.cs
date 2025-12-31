
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Core.Grass;
using Stellamod.Helpers;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.TilesNew.RainforestTiles
{
    public class RainforestGrass : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = true;
            Main.tileBlockLight[Type] = true;
            RegisterItemDrop(ModContent.ItemType<RainforestGrassBlock>());
            // DustType = Main.rand.Next(110, 113);

            MineResist = 1f;
            MinPick = 25;

            AddMapEntry(new Color(110, 74, 51));

            // TODO: implement
            // SetModTree(new Trees.ExampleTree());
        }
        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
   
            if(j != 0)
            {
                //Render grass above the tile
                Tile tile = Main.tile[i, j - 1];
                if (!tile.HasTile)
                {
                    //Render grass
                    Vector2 worldPosition = new Point(i, j).ToWorldCoordinates();
                    GrassRenderer grassRenderer = ModContent.GetInstance<GrassRenderer>();
                    Color grassColor = new Color(80, 107, 26);
                    Color lightColor = Lighting.GetColor(i, j);
                    grassColor = grassColor.MultiplyRGB(lightColor);
                    float height = ExtraMath.Osc(0.5f, 1f, 0, i * 3);
                    float width = ExtraMath.Osc(0.5f, 1f, 0, i * 3);
                    float h = height * 80;
                    float w = width * 4.5f;
                    worldPosition.Y += 8 * ExtraMath.Osc(0f, 1f, 0, i * 3);


                    int num = (int)(4 * ExtraMath.Osc(0f, 1f, 0, i * 0.3f)) + 2;
                    grassRenderer.AddGrassPatch(grassColor, worldPosition, -Vector2.UnitY, h, w, num);
                }
            }

            return base.PreDraw(i, j, spriteBatch);
        }
        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }
        public override bool CanExplode(int i, int j) => false;
    }

    public class RainforestGrassBlock : ModItem
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
            Item.createTile = ModContent.TileType<RainforestGrass>();
        }

        // Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
    }
}