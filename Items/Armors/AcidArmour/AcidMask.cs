using Stellamod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Stellamod.Items.Armors.AcidArmour
{
    [AutoloadEquip(EquipType.Head)]
    public class AcidMask : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 30;
            Item.value = 10000;
            Item.rare = ItemRarityID.Blue;
            Item.defense = 2;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage(DamageClass.Ranged) += 0.05F;
            player.GetCritChance(DamageClass.Ranged) += 8;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == Mod.Find<ModItem>("AcidBody").Type && legs.type == Mod.Find<ModItem>("AcidLegs").Type;
        }

        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawShadow = true;
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "Stand still to emit a toxic aura!";
            player.moveSpeed += 0.2f;
            player.GetModPlayer<AcidPlayer>().hasSetBonus = true;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemType<VirulentPlating>(), 5);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }
    }
}
