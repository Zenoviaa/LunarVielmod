using Microsoft.Xna.Framework;
using Stellamod.Items.Harvesting;
using Stellamod.Items.Materials;
using Stellamod.Projectiles.Magic;
using Stellamod.Projectiles.Spears;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Mage
{
    internal class Poya : ClassSwapItem
    {
        public int dir;
        public override DamageClass AlternateClass => DamageClass.Summon;

        public override void SetClassSwappedDefaults()
        {
            Item.damage = 50;
            Item.mana = 6;
        }
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Gladiator Spear");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 45;
            Item.width = 50;
            Item.height = 50;
            Item.mana = 60;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.noUseGraphic = false;
            Item.knockBack = 0;
            Item.value = Item.sellPrice(0, 1, 1, 29);
            Item.rare = ItemRarityID.LightPurple;
            Item.autoReuse = true;
            Item.useTurn = true;
            Item.DamageType = DamageClass.Magic;
            Item.shoot = ModContent.ProjectileType<PoyaProj>();
            Item.shootSpeed = 9f;
            Item.useAnimation = 45;
            Item.useTime = 100;
            Item.consumeAmmoOnLastShotOnly = true;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-3f, -2f);
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.AddIngredient(ModContent.ItemType<AlcaricMush>(), 23);
            recipe.AddIngredient(ModContent.ItemType<ConvulgingMater>(), 30);
            recipe.AddIngredient(ModContent.ItemType<DarkEssence>(), 9);
            recipe.AddIngredient(ModContent.ItemType<AlcadizMetal>(), 9);
            recipe.AddIngredient(ModContent.ItemType<WickofSorcery>(), 1);
            recipe.Register();
        }
    }
}
