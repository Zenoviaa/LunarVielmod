using Stellamod.Buffs.Charms;
using Stellamod.Common.Bases;
using Stellamod.Items.Harvesting;
using Stellamod.Items.Materials;
using Stellamod.Items.Materials.Molds;
using Stellamod.Items.Ores;
using Stellamod.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Accessories.Brooches
{
    public class MagicalBroochA : BaseBrooch
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = 24;
            Item.height = 28;
            Item.value = Item.sellPrice(gold: 25);
            Item.rare = ItemRarityID.LightPurple;
            Item.buffType = ModContent.BuffType<MagicalBroo>();
            Item.accessory = true;
            BroochType = BroochType.Advanced;
        }

        public override void UpdateBrooch(Player player)
        {
            base.UpdateBrooch(player);
            player.GetDamage(DamageClass.Magic) *= 1.2f;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<BlankBrooch>(), 1);
            recipe.AddIngredient(ModContent.ItemType<AlcaricMush>(), 100);
            recipe.AddIngredient(ModContent.ItemType<AlcadizMetal>(), 100);
            recipe.AddIngredient(ModContent.ItemType<EldritchSoul>(), 30);
            recipe.AddIngredient(ModContent.ItemType<RadianuiBar>(), 20);
            recipe.AddIngredient(ModContent.ItemType<WickofSorcery>(), 1);
            recipe.AddIngredient(ItemID.NaturesGift, 1);
            recipe.AddTile(ModContent.TileType<BroochesTable>());
            recipe.Register();
        }
    }
}