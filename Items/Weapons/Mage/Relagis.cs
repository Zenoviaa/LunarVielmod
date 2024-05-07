using Stellamod.Items.Materials;
using Stellamod.Items.Ores;
using Stellamod.Items.Quest.BORDOC;
using Stellamod.Projectiles.Crossbows.Ultras;
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
            Item.damage = 85;
            Item.mana = 0;
            Item.useAmmo = AmmoID.Bullet;
        }

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Wooden Crossbow"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
            /* Tooltip.SetDefault("Use a small crossbow and shoot three bolts!"
                + "\n'Triple Threat!'"); */
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;

        }

        public override void SetDefaults()
        {
            Item.damage = 80;
            Item.DamageType = DamageClass.Magic;
            Item.width = 32;
            Item.mana = 7;
            Item.height = 25;
            Item.useTime = 69;
            Item.useAnimation = 70;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 2;
            Item.rare = ItemRarityID.Orange;
            Item.autoReuse = false;
            Item.shootSpeed = 30f;
            Item.shoot = ModContent.ProjectileType<RadiantOrb>();
            Item.scale = 0.8f;
            Item.noMelee = true; // The projectile will do the damage and not the item
            Item.value = Item.buyPrice(gold: 95);
            Item.noUseGraphic = true;
            Item.channel = true;


        }



        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.ChlorophyteBar, 10);
            recipe.AddIngredient(ModContent.ItemType<RadianuiBar>(), 20);
            recipe.AddIngredient(ModContent.ItemType<RottenHeart>(), 1);
            recipe.Register();
        }


    }
}