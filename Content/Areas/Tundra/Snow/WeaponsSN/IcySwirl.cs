using Microsoft.Xna.Framework;
using Stellamod.Common.Shaders;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Bases;
using Stellamod.Core.Effects.Trails;
using Stellamod.Core.Particles;
using Stellamod.Core.SwingSystem;
using Stellamod.Helpers;
using Stellamod.Items;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Tundra.Snow.WeaponsSN
{
    public class IcySwirl : BaseSwingItemV2
    {
        public override void SetDefaults2()
        {
            base.SetDefaults2();
            Item.damage = 10;
            Item.shoot = ModContent.ProjectileType<IcySwirlSlash>();
            staminaProjectileShoot = ModContent.ProjectileType<IcySwirlStaminaSlash>();
            meleeWeaponType = MeleeWeaponType.Sword;
            staminaCost = 1;
        }
        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew(
                mold: ModContent.ItemType<BlankSword>(),
                material: ModContent.ItemType<WinterbornShard>());
        }
    }

    public class IcySwirlMist : ModProjectile
    {

        private ref float Timer => ref Projectile.ai[0];
        public override string Texture => TextureRegistry.EmptyTexture;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 128;
            Projectile.height = 128;
            Projectile.timeLeft = 180;
            Projectile.penetrate = -1;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 15;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
        }
        public override void AI()
        {
            base.AI();
            Projectile.velocity *= 0.95f;

            Timer++;
            if(Timer % 8 == 0)
            {
                Vector2 position = Projectile.position;
                position.X += Main.rand.Next(0, Projectile.width);
                position.Y += Main.rand.Next(0, Projectile.height);
                Particle<TexturedCloudParticle>.Spawn(position, Main.rand.NextVector2Circular(0.5f, 0.5f), Color.White, Scale: 2f);
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            target.AddBuff(BuffID.Frostburn, 120);
        }
        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
        }
    }
    public class IcySwirlStaminaSlash : BaseSwingProjectileV2
    {
        private bool _hit;
        private bool _throwProjectile;
        public override void DefineCombo()
        {
            base.DefineCombo();
            ComboBuilder comboBuilder = new ComboBuilder();
            comboBuilder.AddSpinningSwordSlash(duration: 60, xSwingRadius: 64, ySwingRadius: 64, swingDegrees: 2000, hitCount: 4);


            comboBuilder.AddToProjectile(this);
        }

        public float WidthFunction(float completionRatio)
        {
            return EasingFunction.QuadraticBump(completionRatio) * 32;
        }

        public Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.White, Color.Transparent, completionRatio);
        }

        public override void RenderSwingTrail(ref Color lightColor, Vector2[] points)
        {
            base.RenderSwingTrail(ref lightColor, points);
            var shader = RichLaserShader.Instance;
            shader.LaserColor = Color.White * 0.3f;
            shader.InnerColor = Color.Lerp(Color.Cyan, Color.Blue, 0.75f) * 0.3f;
            shader.OuterColor = Color.Cyan * 0.3f;
            TrailDrawer.Draw(Main.spriteBatch, points, ColorFunction, WidthFunction, shader);
        }
        public override void AI()
        {
            base.AI();
            glowColor = Color.Lerp(Color.Transparent, Color.LightCyan * 0.5f, EasingFunction.QuadraticBump(Interpolant));
            growScale = MathHelper.Lerp(0f, 0.3f, EasingFunction.QuadraticBump(Interpolant));

            if (Timer % 16 == 0 && Interpolant >= 0.3f)
            {
                DustParticle dp = Particle<DustParticle>.Spawn(Projectile.Center, Main.rand.NextVector2Circular(1, 1), Color.White, Main.rand.NextFloat(0.3f, 0.8f));
                dp.outerColor = Color.Cyan;
            }

            if ( Interpolant >= 0.3f)
            {
                if (!_throwProjectile)
                {
                    SoundStyle soundStyle = SoundRegistry.IceyWind;
                    soundStyle.PitchVariance = 0.33f;
                    SoundEngine.PlaySound(soundStyle, Projectile.position);

                    if (this.OwnedByLocalClient())
                    {
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Owner.Center, Projectile.velocity,
                                    ModContent.ProjectileType<IcySwirlMist>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                    }
        
                    _throwProjectile = true;
                }
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            if (!_hit)
            {
                FXUtil.ShakeCamera(target.Center, 1024, 4);
                Vector2 position = target.Center;
                Vector2 lvelocity = Projectile.velocity.SafeNormalize(Vector2.Zero) * 4;
                for (float f = 0; f < 4; f++)
                {
                    Vector2 pVelocity = lvelocity.RotatedByRandom(MathHelper.PiOver4 / 3f);
                    pVelocity *= Main.rand.NextFloat(0.5f, 2f);
                    var frag = LegacyParticle.NewParticle<GlowFragmentParticle>(position, pVelocity);
                    FXUtil.GlowFragmentParticle(position, pVelocity,
                        innerColor: Color.White,
                        outerColor: Color.Cyan,
                        fadeToColor: Color.DarkBlue,
                        distortOut: true);
                }

                _hit = true;
            }

            target.AddBuff(BuffID.Frostburn, 120);
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(target, ref modifiers);
            if (ComboIndex == 5)
            {
                modifiers.FinalDamage += 0.5f;
            }
            SoundStyle spearHit = SoundRegistry.SpearHit1;
            spearHit.PitchVariance = 0.5f;
            SoundEngine.PlaySound(spearHit, Projectile.position);
        }
    }
    public class IcySwirlSlash : BaseSwingProjectileV2
    {
        private bool _hit;
        public override void DefineCombo()
        {
            base.DefineCombo();
            //Icy swirl has a bit more personality, having multiple swirls in its combo and hitting multiple times
            //Pretty cool weapon
            ComboBuilder comboBuilder = new ComboBuilder();
            comboBuilder.AddSwordSlash1(duration: 17)
                    .AddSwordSlash2(duration: 17)
                .AddSpinningSwordSlash(duration: 30, xSwingRadius: 64, ySwingRadius: 64, swingDegrees: 720, hitCount: 2)
            
                .AddSpinningSwordSlash(duration: 30, xSwingRadius: 64, ySwingRadius: 64, swingDegrees: 720, hitCount: 2)
                .AddSwordSlash3(duration: 38, swingDegress: 720, hitCount: 3);
            comboBuilder.AddToProjectile(this);
            useAfterImage = true;
        }

        public override void RenderSwingTrail(ref Color lightColor, Vector2[] points)
        {
            base.RenderSwingTrail(ref lightColor, points);
            var shader = RichLaserShader.Instance;
            shader.LaserColor = Color.White * 0.3f;
            shader.InnerColor = Color.Lerp(Color.Cyan, Color.Blue, 0.75f) * 0.3f;
            shader.OuterColor = Color.Cyan * 0.3f;
            TrailDrawer.Draw(Main.spriteBatch, points, ColorFunction, WidthFunction, shader);
        }

        public float WidthFunction(float completionRatio)
        {
            return EasingFunction.QuadraticBump(completionRatio) * 24;
        }

        public Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.White, Color.Green, completionRatio) * completionRatio;
        }

        public override void AI()
        {
            base.AI();
            glowColor = Color.Lerp(Color.Transparent, Color.LightCyan * 0.5f, EasingFunction.QuadraticBump(Interpolant));
            growScale = MathHelper.Lerp(0f, 0.3f, EasingFunction.QuadraticBump(Interpolant));

            if (Timer % 16 == 0 && Interpolant >= 0.3f)
            {
                DustParticle dp = Particle<DustParticle>.Spawn(Projectile.Center, Main.rand.NextVector2Circular(1, 1), Color.White, Main.rand.NextFloat(0.3f, 0.8f));
                dp.outerColor = Color.Cyan;
            }
            if (Timer % 32 == 0 && Interpolant >= 0.3f)
            {
                SwirlParticle dp = Particle<SwirlParticle>.Spawn(Projectile.Center, Main.rand.NextVector2Circular(1, 1), Color.White, Main.rand.NextFloat(0.3f, 0.5f));
                dp.innerColor = Color.Lerp(Color.White, Color.Cyan, Main.rand.NextFloat(0f, 1f));
                dp.outerColor = Color.Lerp(Color.White, Color.Cyan, Main.rand.NextFloat(0f, 1f));
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            if (!_hit)
            {
                FXUtil.ShakeCamera(target.Center, 1024, 4);
                Vector2 position = target.Center;
                Vector2 lvelocity = Projectile.velocity.SafeNormalize(Vector2.Zero) * 4;
                for (float f = 0; f < 4; f++)
                {
                    Vector2 pVelocity = lvelocity.RotatedByRandom(MathHelper.PiOver4 / 3f);
                    pVelocity *= Main.rand.NextFloat(0.5f, 2f);
                    var frag = LegacyParticle.NewParticle<GlowFragmentParticle>(position, pVelocity);
                    FXUtil.GlowFragmentParticle(position, pVelocity,
                        innerColor: Color.White,
                        outerColor: Color.Cyan,
                        fadeToColor: Color.DarkBlue,
                        distortOut: true);
                }

                _hit = true;
            }

            target.AddBuff(BuffID.Frostburn, 120);
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(target, ref modifiers);
            if (ComboIndex == 5)
            {
                modifiers.FinalDamage += 0.5f;
            }
            SoundStyle spearHit = SoundRegistry.SpearHit1;
            spearHit.PitchVariance = 0.5f;
            SoundEngine.PlaySound(spearHit, Projectile.position);
        }
    }

}
