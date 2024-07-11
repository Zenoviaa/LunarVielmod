using Microsoft.Xna.Framework;
using Stellamod.Helpers;
using Stellamod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Stellamod.Items.Armors.Terric
{
    [AutoloadEquip(EquipType.Head)]
    public class TerricHead : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Terric Helmet");
            // Tooltip.SetDefault("Increases magic damage by 13%");
        }
        public override void SetDefaults()
        {
            Item.Size = new Vector2(18);
            Item.value = Item.sellPrice(silver: 26);
            Item.rare = ItemRarityID.Green;
            Item.defense = 6;

        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemType<TerrorFragments>(), 5);
            recipe.AddIngredient(ItemType<DreadFoil>(), 9);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }
        public override void UpdateEquip(Player player)
        {
            player.GetDamage(DamageClass.Magic) += 0.13f;
        }
        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawShadow = true;
        }
        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ItemType<TerricBody>() && legs.type == ItemType<TerricLegs>();
        }

        public override void UpdateArmorSet(Player player)
        {
            Main.LocalPlayer.GetModPlayer<MyPlayer>().Teric = true;
            player.setBonus = LangText.SetBonus(this);//"You build up magical crit chance over time!\n" + "+20 Max Life");
            player.statLifeMax2 += 20;
        }
    }
}