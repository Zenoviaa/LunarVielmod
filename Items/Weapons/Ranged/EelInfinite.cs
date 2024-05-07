using Microsoft.Xna.Framework;
using Stellamod.Items.Materials.Tech;
using Stellamod.Projectiles.Gun;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Ranged
{
    internal class EelInfinite : ClassSwapItem
    {
        public override DamageClass AlternateClass => DamageClass.Magic;

        public override void SetClassSwappedDefaults()
        {
            Item.damage = 36;
            Item.mana = 4;
        }

        public override void SetDefaults()
        {
            Item.width = 114;
            Item.height = 36;
            Item.rare = ItemRarityID.LightRed;
            Item.damage = 24;
            Item.DamageType = DamageClass.Ranged;
            Item.useAnimation = 9;
            Item.useTime = 9;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.shoot = ModContent.ProjectileType<EelLightningBolt>();
            Item.shootSpeed = 1;
            Item.value = Item.sellPrice(gold: 2);
            Item.UseSound = SoundID.DD2_LightningAuraZap;
            Item.noMelee = true;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-24, 0);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            //Dust Burst Towards Mouse
            int count = 2;
            for (int k = 0; k < count; k++)
            {
                Vector2 newVelocity = (velocity*20).RotatedByRandom(MathHelper.ToRadians(15));
                newVelocity *= 1f - Main.rand.NextFloat(0.3f);
                Dust.NewDust(position, 0, 0, DustID.Electric, newVelocity.X * 0.5f, newVelocity.Y * 0.5f);
            }

            int numProjectiles = Main.rand.Next(1, 2);
            for (int p = 0; p < numProjectiles; p++)
            {
                // Rotate the velocity randomly by 30 degrees at max.
                Vector2 newVelocity = velocity.RotatedByRandom(MathHelper.ToRadians(6));
                newVelocity *= 1f - Main.rand.NextFloat(0.3f);
                Projectile.NewProjectileDirect(source, position, newVelocity, type, damage, knockback, player.whoAmI);
            }

            return true;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<BasicGunParts>());
            recipe.AddIngredient(ModContent.ItemType<MetallicOmniSource>(), 5);
            recipe.AddIngredient(ModContent.ItemType<WeaponDrive>(), 10);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.Register();
        }
    }
}
