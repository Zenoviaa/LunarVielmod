using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Armors.Elagent
{
    [AutoloadEquip(EquipType.Legs)]
    public class ElagentLegs : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Shade Wraith Legs");
            // Tooltip.SetDefault("Increases melee critical strike chance by 8% and movement speed by 10%");
        }

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 22;
            Item.value = 10000;
            Item.rare = ItemRarityID.Orange;
            Item.defense = 3;
        }

        public override void UpdateEquip(Player player)
        {
            player.moveSpeed += 0.5f;
            player.maxMinions += 2;
            player.GetDamage(DamageClass.Summon) *= 1.1f;
        }
    }
}
