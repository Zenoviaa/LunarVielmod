using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Core.Bases;
using Stellamod.Core.Effects.Trails;
using Stellamod.Core.Particles;
using Stellamod.Core.SwingSystem;
using Stellamod.Helpers;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Cinderspark.WeaponsCS
{
    public class FlamelashChakrams : BaseSwingItemV2
    {
        public override void SetDefaults2()
        {
            base.SetDefaults2();
            Item.DamageType = DamageClass.Summon;
            Item.shoot = ModContent.ProjectileType<FlamelashChakramsSlash>();
        }
    }

    public class FlamelashChakramsSlash : BaseSwingProjectileV2
    {
        private bool _hit;
        private bool _spawnedClone;
        public override void DefineCombo()
        {
            base.DefineCombo();
            ComboBuilder comboBuilder = new ComboBuilder();

            //Ok for chakrams uhh
            //1. spin around once
            comboBuilder.AddChakramSpin();

            //2. spin around twice
            comboBuilder.AddChakramSpin();

            //3. throw one forward
            comboBuilder.AddChakramThrow();

            //4. throw the other forward
            //5. spin around again
            comboBuilder.AddChakramSpin();

            //6. throw them up
            comboBuilder.AddChakramThrow();

            //7. one more spin
            comboBuilder.AddChakramSpin();
            comboBuilder.AddToProjectile(this);


            BlackFireShader blackFireShader = new BlackFireShader();
            blackFireShader.SetDefaults();

            SlashTrailer devilsPeak = new SlashTrailer
            {
                Shader = blackFireShader,
                TrailWidthFunction = (interpolant) =>
                {
                    return EasingFunction.QuadraticBump(interpolant) * 48;
                },
                TrailColorFunction = (interpolant) =>
                {
                    Color lerp1 = Color.Lerp(Color.OrangeRed, Color.RosyBrown, interpolant);
                    return Color.Lerp(lerp1, Color.Transparent, EasingFunction.InExpo(interpolant));
                }
            };

            Trailer = devilsPeak;
            useAfterImage = true;
        }
        public override void AI()
        {
            base.AI();
            if (!_spawnedClone)
            {
                if (IsFinishingSwing())
                {
                    Owner.velocity += Projectile.velocity;
                }
                MirrorProjectile();
            }
            glowColor = Color.Lerp(Color.Transparent, Color.Red * 0.5f, EasingFunction.QuadraticBump(Interpolant));
            growScale = MathHelper.Lerp(0f, 0.3f, EasingFunction.QuadraticBump(Interpolant));

        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            Main.player[Projectile.owner].MinionAttackTargetNPC = target.whoAmI;
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
                        innerColor: Color.Yellow,
                        outerColor: Color.Orange,
                        fadeToColor: Color.Red,
                        distortOut: true);


                }

                _hit = true;
            }
            if (ComboIndex == ComboCount - 1)
            {
                SoundStyle fireSound = AssetRegistry.Sounds.Magic.RadiantCast1;
                fireSound.PitchVariance = 0.2f;
                SoundEngine.PlaySound(fireSound, Projectile.position);
                for (float f = 0; f < 8; f++)
                {
                    Vector2 vel = Main.rand.NextVector2Circular(4, 4);
                    LegacyParticle.NewParticle<EmberParticle>(Owner.Center, vel);
                }

            }

            target.AddBuff(BuffID.OnFire, 120);
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(target, ref modifiers);
            SoundStyle spearHit = SoundRegistry.SpearHit1;
            spearHit.PitchVariance = 0.5f;
            SoundEngine.PlaySound(spearHit, Projectile.position);
            if (IsFinishingSwing())
            {
                DamageHelper.PercentIncreasedamage(ref modifiers, 0.5f);
            }
        }
    }
}
