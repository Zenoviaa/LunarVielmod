using Stellamod.Content.Items.Materials;
using Stellamod.Items;
using Stellamod.Items.Materials.Molds;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.SpringHills.AccSH
{
    public class IvynSharprod : ModItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.accessory = true;
            Item.rare = ItemRarityID.Blue;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            base.UpdateAccessory(player, hideVisual);
            player.GetArmorPenetration(DamageClass.Generic) += 3;
        }
        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew<Ivythorn, BlankAccessory>();
        }
    }
}
