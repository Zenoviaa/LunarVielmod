using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Bases;
using Stellamod.Core.Effects.Trails;
using Stellamod.Core.SwingSystem;
using Stellamod.Helpers;
using Stellamod.Items;
using Stellamod.Items.Materials;
using Stellamod.Trails;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.WondrousDarkspace.WeaponsWD
{
    public class HypnoticScythe : BaseSwingItemV2
    {
        public override void SetDefaults2()
        {
            base.SetDefaults2();
            Item.damage = 14;
            Item.shoot = ModContent.ProjectileType<HypnoticScytheSlash>();
            staminaProjectileShoot = ModContent.ProjectileType<HypnoticScytheStaminaSlash>();
            meleeWeaponType = MeleeWeaponType.Scythe;
        }

        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew<HypnotizedSoul, BlankSword>();
        }
    }

    public class HypnoticScytheSlash : BaseSwingProjectileV2
    {
        public override void DefineCombo()
        {
            base.DefineCombo();
            SwingV2Helper.AddScytheSwingStyle(this);
            BlackFireShader blackFireShader = new BlackFireShader();
            blackFireShader.SetDefaults();
            blackFireShader.InnerColor = Color.LightBlue;
            blackFireShader.OuterColor = Color.DarkBlue;
            blackFireShader.InnerEmitColor = Color.LightBlue * 0.2f;
            blackFireShader.OuterEmiteColor = Color.Purple;
            blackFireShader.BloomTexture = TrailRegistry.StarTrail;
            //blackFireShader.PrimaryTexture2 = TrailRegistry.StarTrail;
            SlashTrailer devilsPeak = new SlashTrailer
            {
                Shader = blackFireShader,
                TrailWidthFunction = (interpolant) =>
                {
                    return MathHelper.SmoothStep(8, 32, interpolant) * EasingFunction.QuadraticBump(Interpolant);
                },
                TrailColorFunction = (interpolant) =>
                {
                    return Color.Lerp(Color.DeepSkyBlue, Color.White, interpolant) * EasingFunction.QuadraticBump(Interpolant);
                }

            };

         //   outlineColor = Color.Yellow;

            //Bloom
            useBloom = true;
            bloom.innerBloomColor = Color.LightBlue;
            bloom.outerBloomColor = Color.Purple;
            bloom.bloomWidthFunction = GetBloomWidth;
            bloom.bloomColorFunction = GetBloomColor;
            outlineColor = Color.White;
            additive = true;
            Trailer = devilsPeak;
            useAfterImage = true;
        }
        private float GetBloomWidth(float ratio)
        {
            return MathHelper.SmoothStep(8, 36, ratio) * 1.5f * MathHelper.SmoothStep(1f, 0f, EasingFunction.InExpo(Interpolant));
        }
        private Color GetBloomColor(float ratio)
        {
            return Color.Lerp(Color.Purple * 0.9f, Color.Lerp(Color.DeepSkyBlue, Color.Violet, ExtraMath.Osc(0f, 1f, speed: 24)), ratio);
        }
        public override void AI()
        {
            base.AI();

            if (Timer % 16 == 0)
            {
                int index = (int)(Interpolant * swingTrailCache.Length) % swingTrailCache.Length;
                Vector2 spawnPos = swingTrailCache[index];
                SparkleParticle dp = SparkleParticle.Spawn(spawnPos, Vector2.Zero, Scale: 0.3f);
                dp.color = Color.Lerp(Color.Lerp(Color.Black, Color.Red, 0.1f), Color.Black, Main.rand.NextFloat(0f, 1f));
                dp.innerColor = Color.White;
                dp.outerColor = Color.Blue;
                dp.gravity = 0;
                dp.noTileCollide = true;
                dp.fast = true;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            float damage = Projectile.damage * 0.3f;
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center, Vector2.Zero,
                ModContent.ProjectileType<HypnotizingAura>(), (int)damage, Projectile.knockBack, Projectile.owner);
        }

        public override void RenderSwingTrail(ref Color lightColor, Vector2[] points)
        {
            base.RenderSwingTrail(ref lightColor, points);
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(target, ref modifiers);
            if (ComboIndex == ComboCount - 1)
            {
                modifiers.FinalDamage *= 2;
            }
            SoundStyle spearHit = SoundRegistry.SpearHit1;
            spearHit.PitchVariance = 0.5f;
            SoundEngine.PlaySound(spearHit, Projectile.position);

            SoundStyle scytheHit;

            int rand = Main.rand.Next(0, 3);
            switch (rand)
            {
                default:
                case 0:
                    scytheHit = AssetRegistry.Sounds.Melee.ScytheHit1;
                    break;
                case 1:
                    scytheHit = AssetRegistry.Sounds.Melee.ScytheHit2;
                    break;
            }

            scytheHit.PitchVariance = 0.5f;
            SoundEngine.PlaySound(scytheHit, Projectile.position);
        }
    }

    public class HypnoticScytheStaminaSlash : BaseSwingProjectileV2
    {
        private bool _grantedBuff;
        public override void DefineCombo()
        {
            base.DefineCombo();
            SoundStyle chargeSound = AssetRegistry.Sounds.Melee.ScythePull;
            chargeSound.PitchVariance = 0.1f;
            Add(new ThrustSwing
            {
                Duration = 64,
                Easing = EasingFunction.InOutExpo,
                OverrideVelocity = -Vector2.UnitY,
                ThrowDistance = 128,
                Sound = chargeSound,
            });

        }

        public override void AI()
        {
            base.AI();
            if (!_grantedBuff && Interpolant >= 0.5f)
            {
                Owner.AddBuff(ModContent.BuffType<TranceBuff>(), 180);
                SoundStyle chargeSound = AssetRegistry.Sounds.Melee.ScytheHit3;
                chargeSound.PitchVariance = 0.1f;
                SoundEngine.PlaySound(chargeSound, Projectile.position);

                float boomSize = Main.rand.NextFloat(0.08f, 0.12f);
                FXUtil.GlowCircleBoom(Projectile.Center,
                    innerColor: Color.Cyan,
                    glowColor: Color.LightBlue,
                    outerGlowColor: Color.Blue, duration: 25, baseSize: boomSize);



                for (float i = 0; i < 4; i++)
                {
                    float progress = i / 4f;
                    float rot = progress * MathHelper.ToRadians(360);
                    rot += Main.rand.NextFloat(-0.5f, 0.5f);
                    Vector2 offset = rot.ToRotationVector2() * 24;
                    var particle = FXUtil.GlowCircleLongBoom(Projectile.Center,
                        innerColor: Color.White,
                        glowColor: Color.LightBlue,
                        outerGlowColor: Color.DarkBlue,
                        baseSize: Main.rand.NextFloat(0.1f, 0.2f),
                        duration: Main.rand.NextFloat(15, 25));
                    particle.Rotation = rot + MathHelper.ToRadians(45);
                }
                if (Main.myPlayer == Projectile.owner)
                {
                    Vector2 velocity = -Vector2.UnitX * 2;
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity,
                        ModContent.ProjectileType<PotOfGreedMinionProjBat>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, -velocity,
                        ModContent.ProjectileType<PotOfGreedMinionProjBat>(), Projectile.damage, Projectile.knockBack, Projectile.owner);

                }
                for (float f = 0; f < 24; f++)
                {
                    Vector2 vel = Main.rand.NextVector2Circular(8, 8);
                    Dust.NewDustPerfect(Projectile.Center, DustID.GemAmethyst, vel, Scale: 0.2f);
                }

                _grantedBuff = true;
            }
        }

        public override void RenderSwingTrail(ref Color lightColor, Vector2[] points)
        {
            base.RenderSwingTrail(ref lightColor, points);
        }
    }


    public class TranceBuff : ModBuff
    {
        public override void Update(Player player, ref int buffIndex)
        {
            base.Update(player, ref buffIndex);
            player.GetAttackSpeed(DamageClass.Generic) += 1;
        }
    }
}
