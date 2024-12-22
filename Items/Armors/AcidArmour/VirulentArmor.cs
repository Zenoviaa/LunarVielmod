using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Armors.AcidArmour
{
    [AutoloadEquip(EquipType.Body)]
    public class VirulentArmor : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("AcidBody");
            // Tooltip.SetDefault("Increases ranged damage by 13% and ranged speed by 10%");
        }

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 22;
            Item.value = 80000;
            Item.rare = ItemRarityID.Blue;
            Item.defense = 5;
        }

        public override void UpdateEquip(Player player)
        {
            player.lifeRegen += 3;
        }
    }
}
