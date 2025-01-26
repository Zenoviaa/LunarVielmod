using Microsoft.Xna.Framework;
using Stellamod.Items.Harvesting;
using Stellamod.Items.Materials.Molds;
using Stellamod.Projectiles.Crossbows.Lasers;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Ranged
{
    internal class TheTraumatizer : ClassSwapItem
    {

        public override DamageClass AlternateClass => DamageClass.Magic;

        public override void SetClassSwappedDefaults()
        {
            Item.damage = 14;
            Item.mana = 10;
        }
        public override void SetDefaults()
        {
            Item.damage = 28;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 80;
            Item.height = 38;
            Item.useTime = 45;
            Item.useAnimation = 45;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 2;
            Item.rare = ItemRarityID.LightRed;
            Item.autoReuse = false;
            Item.shootSpeed = 30f;
            Item.shoot = ModContent.ProjectileType<TheTraumatizerHold>();
            Item.scale = 0.8f;
            Item.noMelee = true; // The projectile will do the damage and not the item
            Item.value = Item.sellPrice(0, 5, 0, 0);
            Item.noUseGraphic = true;
            Item.channel = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<TheTraumatizerHold>(), damage, knockback, player.whoAmI, 1);
            Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<TraumatizingRay>(), damage, knockback, player.whoAmI, 1);
            return false;
        }
        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew(mold: ModContent.ItemType<BlankGun>(), material: ModContent.ItemType<Cinderscrap>());
        }
    }
}
