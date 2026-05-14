
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Common;
using Stellamod.Content.Areas.PunkerTown.TilesPT;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.TilesNew.RainforestTiles
{
   
    public class RainforestGrass : ModTile
    {
        public override void SetStaticDefaults()
        {
            
            TileID.Sets.JungleBiome[Type] = 1;
            TileID.Sets.Grass[Type] = true;
            TileID.Sets.NeedsGrassFraming[Type] = false;
            GrassTileSystem.RegisterGrassyTile<TallGrass>(Type);

            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = true;

            //idk if i actually need to do both of these but better safe than sorry
            Main.tileMerge[Type][TileID.Mud] = true;
            Main.tileMerge[TileID.Mud][Type] = true;

            Main.tileMerge[Type][TileID.JungleGrass] = true;
            Main.tileMerge[TileID.JungleGrass][Type] = true;

            Main.tileMerge[Type][TileID.Grass] = true;
            Main.tileMerge[TileID.Grass][Type] = true;


            Main.tileBlockLight[Type] = true;
            RegisterItemDrop(ModContent.ItemType<RainforestGrassBlock>());
            // DustType = Main.rand.Next(110, 113);

            MineResist = 1f;
            MinPick = 25;

            AddMapEntry(new Color(110, 74, 51));

            // TODO: implement
            // SetModTree(new Trees.ExampleTree());
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