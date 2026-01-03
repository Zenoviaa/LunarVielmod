using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core;
using Stellamod.Core.Bases;
using Stellamod.Core.Effects;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Items;
using Stellamod.Items.Materials;
using Stellamod.Trailing;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.WondrousDarkspace.WeaponsWD
{
    public class Hypnotizer : BaseCrossbowItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 2;
            staminaProjectileShoot = ModContent.ProjectileType<HypnotizerStaminaHold>();
        }

        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew<HypnotizedSoul, BlankBow>();
        }
    }







    public class HypnotizerHold : CrossbowHold
    {
        public override void Shoot(Vector2 position, Vector2 velocity)
        {
            base.Shoot(position, velocity);
            if (Owner.PickAmmo(Owner.HeldItem, out int projToShoot, out float speed, out int damage, out float knockBack, out int useAmmoItemId)
                && Main.myPlayer == Projectile.owner)
            {
                Vector2 fireVelocity = velocity * speed;
                fireVelocity *= 2f;
                fireVelocity *= ChargeStrength;

                float bowDamage = damage * ChargeStrength;
                Projectile crossShot = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), position, fireVelocity,
                    projToShoot,
                    (int)bowDamage, knockBack, Projectile.owner, ai0: projToShoot);
                crossShot.GetGlobalProjectile<CrossbowGlobalProjectile>().CrossbowShot = true;
                crossShot.GetGlobalProjectile<CrossbowGlobalProjectile>().Trailer = TrailPresets.StarringBalls;
            }
        }
    }
















    public class HypnotizerStaminaHold : CrossbowHold
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            BurstCount = 3;
        }
        public override void Shoot(Vector2 position, Vector2 velocity)
        {
            base.Shoot(position, velocity);
            if (Owner.PickAmmo(Owner.HeldItem, out int projToShoot, out float speed, out int damage, out float knockBack, out int useAmmoItemId)
                && Main.myPlayer == Projectile.owner)
            {
                Vector2 fireVelocity = velocity * speed;
                fireVelocity *= 2f;
                fireVelocity *= ChargeStrength;
           
                float bowDamage = damage * ChargeStrength * 2;
                Projectile crossShot = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), position, fireVelocity,
                    ModContent.ProjectileType<HypnotizerArrow>(),
                    damage, knockBack, Projectile.owner);
            }
        }
    }

    public class HypnotizerArrow : ScarletProjectile
    {
        private ITrailer _trailer;
        private ref float Timer => ref Projectile.ai[0];
        public override void SetDefaults()
        {
            base.SetDefaults();
            TrailCacheLength = 24;
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.timeLeft = 200;
        }

        public override void AI()
        {
            base.AI();
            Projectile.velocity *= 1.01f;
            Projectile.rotation = Projectile.velocity.ToRotation();
            Timer++;
            if (Timer % 20 == 0)
            {
                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(8, 8), ModContent.DustType<GlowDust>(), Scale: Main.rand.NextFloat(0.25f, 0.5f));
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            _trailer ??= TrailPresets.StarringBalls;
            _trailer.DrawTrail(ref lightColor, OldCenterPos);
            this.DrawCentered(ref lightColor);
            return false;
        }

        public override void PostDraw(Color lightColor)
        {
            base.PostDraw(lightColor);
            Texture2D texture2D4 = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/DimLight").Value;
            Main.spriteBatch.Draw(texture2D4, Projectile.Center - Main.screenPosition, null, new Color(255, 128, 125, 0), Projectile.rotation, new Vector2(32, 32), 0.17f * (7 + 0.6f), SpriteEffects.None, 0f);
            Lighting.AddLight(Projectile.Center, Color.White.ToVector3() * 0.5f * Main.essScale);
        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<HypnotizerStaminaShotExplosionProjectile>(),
                Projectile.damage, Projectile.knockBack, Projectile.owner);
        }
    }


    public class HypnotizerStaminaShotExplosionProjectile : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[0];
        public override string Texture => TextureRegistry.EmptyTexture;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 80;
            Projectile.height = 80;
            //Projectile.hostile = true;
            Projectile.timeLeft = 4;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            if (Timer == 1)
            {
                //EXPLODE
                float boomSize = Main.rand.NextFloat(0.15f, 0.2f);
                FXUtil.GlowCircleBoom(Projectile.Center,
                    innerColor: Color.White,
                    glowColor: Color.Pink,
                    outerGlowColor: Color.Blue, duration: 25, baseSize: boomSize);

                FXUtil.ShakeCamera(Projectile.position, 1024, 32);

                SoundStyle explosionSound = new SoundStyle("Stellamod/Assets/Sounds/ExplosionCrystalShard");
                explosionSound.PitchVariance = 0.2f;
                SoundEngine.PlaySound(explosionSound, Projectile.position);

                for (float i = 0; i < 4; i++)
                {
                    float progress = i / 4f;
                    float rot = progress * MathHelper.ToRadians(360);
                    rot += Main.rand.NextFloat(-0.5f, 0.5f);
                    Vector2 offset = rot.ToRotationVector2() * 24;
                    var particle = FXUtil.GlowCircleLongBoom(Projectile.Center,
                        innerColor: Color.White,
                        glowColor: Color.Pink,
                        outerGlowColor: Color.DarkBlue,
                        baseSize: Main.rand.NextFloat(0.1f, 0.2f),
                        duration: Main.rand.NextFloat(15, 25));
                    particle.Rotation = rot + MathHelper.ToRadians(45);
                }


                for (float f = 0f; f < 16; f++)
                {
                    float rot = Main.rand.NextFloat(0f, 1f) * MathHelper.TwoPi;
                    Vector2 velocity = rot.ToRotationVector2() * Main.rand.NextFloat(3f, 8f);
                    Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowDust>(), velocity, newColor: Color.Pink, Scale: Main.rand.NextFloat(0.5f, 1f));
                }
            }
        }
    }
}
