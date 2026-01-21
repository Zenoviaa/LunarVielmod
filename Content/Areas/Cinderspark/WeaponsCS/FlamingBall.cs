using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Bases;
using Stellamod.Core.Particles;
using Stellamod.Helpers;
using Stellamod.Items;
using Stellamod.Items.Harvesting;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Cinderspark.WeaponsCS
{
    public class FlamingBall : BaseChainedBallItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 21;
            Item.shoot = ModContent.ProjectileType<FlamingBallProj>();
        }

        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew<Cinderscrap, BlankOrb>();
        }
    }

    public class FlamingBallProj : BaseChainedBallProjectile
    {
        private bool _hit;
        public override void SetDefaults()
        {
            base.SetDefaults();
            //Just having this here in case
            //Iron Ball is just gonna use default stuff htough

            //Variables
            //Easing
            Easer = (float lerpValue) => Easing.InOutExpo(lerpValue, 7);

            //How far it drags behind you
            DragDistance = 126;

            //Swing Range (IT USES OVAL SWING)
            SwingRange = MathHelper.ToRadians(360);

            //Offst for theoval swing
            OvalRotOffset = MathHelper.ToRadians(-90);

            //Max X Swing Radius
            SwingXRadius = 512;

            //Y Swing  Radius
            SwingYRadius = 80;

            //How long it takes to swing
            BaseSwingTime = 48;

            //Glowing stuff
            GlowDistanceOffset = 4;
            GlowRotationSpeed = 0.005f;

            //Damage multiplier for hitting the tip
            TipDamageMultiplier = 2;
        }

        protected override void SetSlingDefaults()
        {
            base.SetSlingDefaults();

            //Reset the hit
            _hit = false;
        }


        public override void AI()
        {
            base.AI();

            if (Main.rand.NextBool(8))
            {
                switch (Main.rand.Next(2))
                {
                    case 0:
                        DustParticle sp = Particle<DustParticle>.Spawn(Projectile.Center + Main.rand.NextVector2Circular(32, 32), -Projectile.velocity.SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(0.3f, 16), Scale: Main.rand.NextFloat(0.5f, 1.5f));
                        sp.gravity = 0f;
                        sp.fast = true;
                        sp.dampening = 0.1f;
                        sp.Scale *= 0.33f;
                        break;
                    case 1:
                        FlameParticle sp2 = Particle<FlameParticle>.Spawn(Projectile.Center + Main.rand.NextVector2Circular(32, 32), -Projectile.velocity.SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(1f, 16), Scale: Main.rand.NextFloat(0.1f, 0.2f));
                        sp2.gravity = 0f;
                        sp2.fast = true;
                        sp2.dampening = 0.1f;
                        sp2.Scale *= 0.13f;
                        break;
                }
            }
        }

        private float GetTrailWidth(float completionRatio)
        {
            return MathHelper.Lerp(0, 32, completionRatio) * EasingFunction.QuadraticBump(UnEasedLerpValue);
        }

        private Color GetTrailColor(float p)
        {
            Color trailColor = Color.Lerp(Color.White, Color.LightBlue, p);
            return trailColor;
        }

        protected override void DrawSlashTrail(ref Color lightColor, Vector2[] slashPos)
        {
            //   base.DrawSlashTrail(ref lightColor, slashPos);
            RichLaserShader laserShader = RichLaserShader.Instance;
            laserShader.LaserColor = Color.Goldenrod;
            laserShader.InnerColor = Color.Red;
            laserShader.OuterColor = Color.DarkRed;
            TrailDrawer.Draw(Main.spriteBatch, slashPos, GetTrailColor, GetTrailWidth, laserShader);
        }

        protected override void DrawBallSprite(ref Color lightColor)
        {
            base.DrawBallSprite(ref lightColor);
            Texture2D glowTexture = AssetManager.GlowMask.SimpleGlowCircle.Value;
            Vector2 drawOrigin = glowTexture.Size() * 0.5f;
            SpriteBatch spriteBatch = Main.spriteBatch;
            Vector2 drawCenter = Projectile.Center - Main.screenPosition;
            Color glowColor = Color.OrangeRed;
            glowColor.A = 0;
            spriteBatch.Draw(glowTexture, drawCenter, null, glowColor, 0, drawOrigin, Projectile.scale * 0.15f, SpriteEffects.None, 0);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);


            if (!_hit)
            {
                float numDust = 6;
                for (float n = 0; n < numDust; n++)
                {
                    DustParticleSpawnParams spawnParams = new DustParticleSpawnParams
                    {
                        innerColor = Color.OrangeRed,
                        outerColor = Color.DarkRed
                    };
                    DustParticle.Spawn(target.Center, -Vector2.UnitY.RotatedByRandom(1.5f) * Main.rand.NextFloat(2f, 8f), spawnParams);
                }
                SoundStyle hitSound;
                switch (Main.rand.Next(2))
                {
                    default:
                    case 0:
                        hitSound = AssetManager.GetSound("Fire/FireballShoot1");
                        break;
                    case 1:
                        hitSound = AssetManager.GetSound("Fire/FireballShoot2");
                        break;
                }

                hitSound.PitchVariance = 0.3f;
                hitSound.Volume = 0.66f;
                SoundEngine.PlaySound(hitSound, target.Center);
                FXUtil.ShakeCamera(target.Center, 1024, 2);
                _hit = true;
            }
            if (Main.rand.NextBool(2))
                target.AddBuff(BuffID.OnFire, 120);
        }
    }
}
