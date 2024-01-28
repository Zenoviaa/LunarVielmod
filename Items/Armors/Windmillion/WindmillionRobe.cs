using Stellamod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;



namespace Stellamod.Items.Armors.Windmillion
{
    [AutoloadEquip(EquipType.Body)]
    public class WindmillionRobe : ModItem
    {

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Leather Vest");
            // Tooltip.SetDefault("Increases throwing damage by 25%");
        }

        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 30;
            Item.value = Item.sellPrice(0, 0, 20, 0);
            Item.rare = ItemRarityID.Blue;
            Item.defense = 3;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetCritChance(DamageClass.Throwing) += 2f;
        }

    }
}
