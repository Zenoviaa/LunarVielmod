using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Stellamod.TilesNew.MothlightTiles
{
    public class MothlightBrick : ModTile
    {

        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileMerge[Type][Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileBlendAll[Type] = true;
            Main.tileLighted[Type] = true;
            Main.tileBlockLight[Type] = true;
            TileID.Sets.CanBeClearedDuringOreRunner[Type] = false;
            LocalizedText name = CreateMapEntryName();
            AddMapEntry(new Color(178, 163, 190), name);

            RegisterItemDrop(ModContent.ItemType<MothlightBrickBlock>());
            // DustType = Main.rand.Next(110, 113);

            MineResist = 1f;
            MinPick = 145;
        }
        public override bool CanExplode(int i, int j) => false;

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








        public class MothlightBrickBlock : ModItem
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
                Item.createTile = ModContent.TileType<MothlightBrick>();
            }

            // Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
        }
    }
}