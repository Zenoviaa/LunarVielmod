using Stellamod.Items.Materials.Molds;
using Stellamod.Items.Ores;
using Stellamod.Projectiles.Paint;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Ranged
{

    public class Photobomb : ClassSwapItem
    {

        public override DamageClass AlternateClass => DamageClass.Magic;

        public override void SetClassSwappedDefaults()
        {
            Item.damage = 4;
            Item.mana = 2;
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
            Item.damage = 9;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 32;
            Item.height = 25;
            Item.useTime = 98;
            Item.useAnimation = 98;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 10;
            Item.rare = ItemRarityID.LightPurple;
            Item.autoReuse = false;
            Item.shootSpeed = 30f;
            Item.shoot = ModContent.ProjectileType<PhotobombProj>();
            Item.scale = 0.8f;
            Item.noMelee = true; // The projectile will do the damage and not the item
            Item.value = Item.buyPrice(silver: 7);
            Item.noUseGraphic = true;
            Item.channel = true;
            Item.noMelee = true;

        }


        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew(mold: ModContent.ItemType<BlankGun>(), material: ModContent.ItemType<KaleidoscopicInk>());
        }
    }
}