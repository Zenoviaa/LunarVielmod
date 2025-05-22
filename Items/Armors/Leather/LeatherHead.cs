using Stellamod.Items.Harvesting;
using Stellamod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Stellamod.Items.Armors.Leather
{
    [AutoloadEquip(EquipType.Head)]
    public class LeatherHead : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 30;
            Item.value = 10000;
            Item.rare = ItemRarityID.Blue;
            Item.defense = 3;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetCritChance(DamageClass.Ranged) += 4;
            player.GetDamage(DamageClass.Ranged) += 0.1f;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<LeatherBody>() && legs.type == ModContent.ItemType<LeatherLegs>();
        }

        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawShadow = true;
        }

        public override void UpdateArmorSet(Player player)
        {
            Main.LocalPlayer.GetModPlayer<MyPlayer>().Leather = true;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.Leather, 1);
            recipe.AddIngredient(ModContent.ItemType<Mushroom>(), 4);
            recipe.AddRecipeGroup(nameof(ItemID.IronBar), 3);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }
    }
}
