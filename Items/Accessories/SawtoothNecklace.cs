using Stellamod.Items.Materials;
using Stellamod.Items.Materials.Molds;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Accessories
{
    internal class SawtoothNecklace : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 42;
            Item.height = 48;
            Item.value = 2500;
            Item.rare = ItemRarityID.LightRed;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            base.UpdateAccessory(player, hideVisual);

            //Increased armor pen
            player.GetArmorPenetration(DamageClass.Generic) += 12;
        }


        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew(mold: ModContent.ItemType<BlankAccessory>(), material: ModContent.ItemType<AuroreanStarI>());
        }
    }
}
