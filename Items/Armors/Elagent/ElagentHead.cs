using Stellamod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Stellamod.Items.Armors.Elagent
{
    [AutoloadEquip(EquipType.Head)]
    public class ElagentHead : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 30;
            Item.value = 10000;
            Item.rare = ItemRarityID.Orange;
            Item.defense = 3;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage(DamageClass.Summon) *= 1.2f;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == Mod.Find<ModItem>("ElagentBody").Type && legs.type == Mod.Find<ModItem>("ElagentLegs").Type;
        }

        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawShadow = true;
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "+1 Max Minions\n" +
                "+45 Max Life";
            player.maxMinions += 1;
            player.statLifeMax2 += 45;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemType<StarSilk>(), 8);
            recipe.AddIngredient(ItemType<PearlescentScrap>(), 8);
            recipe.AddIngredient(ItemID.Cloud, 7);
            recipe.AddIngredient(ItemID.Bone, 5);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }
    }
}
