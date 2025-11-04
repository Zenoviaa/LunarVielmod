using Stellamod.Content.Items.Materials;
using Stellamod.Items;
using Stellamod.Items.Accessories.Players;
using Stellamod.Items.Materials.Molds;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.SpringHills.AccSH
{
    [AutoloadEquip(EquipType.Neck)]
    public class IvynNecklace : ModItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = 30;
            Item.height = 40;
            Item.rare = ItemRarityID.Green;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            base.UpdateAccessory(player, hideVisual);
            DashPlayer dashPlayer = player.GetModPlayer<DashPlayer>();
            dashPlayer.MaxDashCount += 1;
        }


        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew<Ivythorn, BlankAccessory>();
        }
    }
}