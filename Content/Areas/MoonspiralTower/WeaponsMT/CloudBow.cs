using Microsoft.Xna.Framework;
using Stellamod.Buffs;
using Stellamod.Core.Bases;
using Stellamod.Items;
using Stellamod.Items.Materials;
using Stellamod.Items.Materials.Molds;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.MoonspiralTower.WeaponsMT

{
    public class CloudBow : BaseCrossbowItem
    {

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 18;
            Item.knockBack = 6;
            Item.rare = ItemRarityID.LightRed;
        }

        public override void ShootBow(Player player, EntitySource_ItemUse_WithAmmo source, ShootParams shootParams)
        {
            base.ShootBow(player, source, shootParams);
            float numberProjectiles = 3;
            float rotation = MathHelper.ToRadians(10);

            Vector2 velocity = shootParams.velocity * shootParams.speed;
            velocity *= 3;
            velocity *= shootParams.chargeStrength;
            Vector2 position = shootParams.position;
            float bowDamage = shootParams.damage * shootParams.chargeStrength;
            for (int i = 0; i < numberProjectiles; i++)
            {
                var EntitySource = player.GetSource_FromThis();
                Vector2 perturbedSpeed = new Vector2(velocity.X, velocity.Y).RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numberProjectiles - 1))) * .4f; // This defines the projectile roatation and speed. .4f == projectile speed
                Projectile.NewProjectile(EntitySource, position.X, position.Y, perturbedSpeed.X, perturbedSpeed.Y,
                    shootParams.projToShoot, (int)bowDamage, Item.knockBack, player.whoAmI);
            }
        }

        public override void StaminaShootBow(Player player, EntitySource_ItemUse_WithAmmo source, ShootParams shootParams)
        {
            base.StaminaShootBow(player, source, shootParams);

            Vector2 fireVelocity = shootParams.velocity * shootParams.speed;
            fireVelocity *= 3;
            fireVelocity *= shootParams.chargeStrength;

            float bowDamage = shootParams.damage * shootParams.chargeStrength;
            Projectile crossShot = Projectile.NewProjectileDirect(source, shootParams.position, fireVelocity,
               ModContent.ProjectileType<CloudArrow>(), (int)bowDamage, shootParams.knockBack, player.whoAmI, ai0: shootParams.projToShoot);
            crossShot.GetGlobalProjectile<CrossbowGlobalProjectile>().CrossbowShot = true;
        }

        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew(mold: ModContent.ItemType<BlankBow>(), material: ModContent.ItemType<PearlescentScrap>());
        }
    }










    public class CloudArrow : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[0];
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ProjectileID.Sets.TrailCacheLength[Type] = 16;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.friendly = true;
        }
        public override void AI()
        {
            base.AI();
            Timer++;
            if (Timer % 6 == 0)
            {
                Dust.NewDustPerfect(Projectile.Center, DustID.Cloud, Projectile.velocity * 0.1f, 0, Color.White, Main.rand.NextFloat(0.5f, 1f)).noGravity = true;
            }
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(90);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            if (Main.rand.NextBool(2))
                target.AddBuff(ModContent.BuffType<Clouded>(), 240);
        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            for (int i = 0; i < 4; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, DustID.Cloud, Projectile.velocity.RotatedByRandom(MathHelper.PiOver4) * 0.1f, 0, Color.White, Main.rand.NextFloat(1f, 2f)).noGravity = true;
            }
        }
    }
}