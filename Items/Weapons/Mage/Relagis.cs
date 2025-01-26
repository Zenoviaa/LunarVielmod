using Stellamod.Items.Ores;
using Stellamod.Projectiles.Magic;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Mage
{
    public class Relagis : ClassSwapItem
    {
        public override DamageClass AlternateClass => DamageClass.Ranged;

        public override void SetClassSwappedDefaults()
        {
            Item.damage = 40;
            Item.mana = 0;
            Item.useAmmo = AmmoID.Bullet;
        }

        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 80;
            Item.DamageType = DamageClass.Magic;
            Item.width = 32;
            Item.mana = 7;
            Item.height = 25;
            Item.useTime = 16;
            Item.useAnimation = 16;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 2;
            Item.rare = ItemRarityID.Orange;
            Item.autoReuse = false;
            Item.shootSpeed = 30f;
            Item.shoot = ModContent.ProjectileType<RadiantOrb>();
            Item.noMelee = true; // The projectile will do the damage and not the item
            Item.value = Item.buyPrice(gold: 95);
            Item.noUseGraphic = true;
            Item.channel = true;
        }

        public override bool CanUseItem(Player player)
        {
            return player.ownedProjectileCounts[Item.shoot] < 1;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.ChlorophyteBar, 10);
            recipe.AddIngredient(ModContent.ItemType<RadianuiBar>(), 20);
            recipe.Register();
        }


    }
}