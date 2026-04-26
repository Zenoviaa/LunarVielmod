using Stellamod.Content.Areas.Terror.TilesTR;
using Stellamod.WorldG;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Helpers
{
    public class TreeGrower : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 62;
            Item.height = 32;
            Item.scale = 0.9f;
            Item.rare = ItemRarityID.Green;
            Item.useTime = 2;
            Item.useAnimation = 2;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.autoReuse = false;
        }

        public override bool? UseItem(Player player)
        {
            int i = (int)Main.MouseWorld.X / 16;
            int j = (int)Main.MouseWorld.Y / 16;
            WorldGen.GrowTree(i, j);
            return true;
        }
    }
    public class TreeGrower2 : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 62;
            Item.height = 32;
            Item.scale = 0.9f;
            Item.rare = ItemRarityID.Green;
            Item.useTime = 2;
            Item.useAnimation = 2;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.autoReuse = false;
        }

        public override bool? UseItem(Player player)
        {
            Main.NewText(Main.worldSurface);
            int i = (int)Main.MouseWorld.X / 16;
            int j = (int)Main.MouseWorld.Y / 16;
            int height = Main.rand.Next(12, 45);
            VeilGen.PlaceBigTrees<BigDeadTree, BigDeadTreeTop>(i, j, height);
            return true;
        }
    }
}
