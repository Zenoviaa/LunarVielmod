using Microsoft.Xna.Framework;
using Stellamod.Assets;
using Stellamod.Core.Effects.Trails;
using Stellamod.Core.Helpers;
using Stellamod.Core.Helpers.Math;
using Stellamod.Core.Particles;
using Stellamod.Core.SwingSystem;
using Stellamod.Core.Visual.Particles;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Items.Weapons.Melee.Spears
{
    public class CrystalPointer : BaseSwingItem
    {
        public override void SetDefaults2()
        {
            Item.damage = 8;
            Item.DamageType = DamageClass.Melee;
            Item.width = 40;
            Item.height = 40;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.useTime = 60;
            Item.useAnimation = 60;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 6;
            Item.value = Item.buyPrice(silver: 1);
            Item.rare = ItemRarityID.Blue;
            Item.shootSpeed = 10;
            Item.shoot = ModContent.ProjectileType<CrystalPointerStab>();
            Item.autoReuse = true;
            staminaProjectileShoot = ModContent.ProjectileType<CrystalPointerStaminaStab>();
        }
    }

    public class CrystalPointerStab : BaseSwingProjectile
    {
        public bool Hit;
        public SlashEffect SlashEffect { get; set; }
        public SlashTrailer SlashTrailer { get; set; }
        private float GetTrailWidth(float interpolant)
        {
            return EasingFunction.QuadraticBump(interpolant) * 20;
        }

        public override void DefineCombo(List<ISwing> swings)
        {
            base.DefineCombo(swings);

            //Setup the slash effect
            SlashEffect = new SlashEffect()
            {
                BaseColor = Color.LightBlue,
                WindColor = Color.DarkBlue,
                LightColor = Color.LightCyan,
                RimHighlightColor = Color.White,
                BlendState = Microsoft.Xna.Framework.Graphics.BlendState.Additive
            };
            SlashTrailer = new();
            SlashTrailer.TrailWidthFunction = GetTrailWidth;
            SlashTrailer.Shader = SlashEffect;
            Trailer = SlashTrailer;

            SoundStyle spearSlash1 = AssetRegistry.Sounds.Melee.SpearSlash1;
            SoundStyle spearSlash2 = AssetRegistry.Sounds.Melee.SpearSlash2;
            SoundStyle nSpin = AssetRegistry.Sounds.Melee.SwordSpin1;
            spearSlash1.PitchVariance = 0.25f;
            spearSlash2.PitchVariance = 0.25f;
            nSpin.PitchVariance = 0.2f;



            swings.Add(new OvalSwing
            {
                Duration = 28,
                XSwingRadius = 100,
                YSwingRadius = 50,
                SwingDegrees = 90,
                Easing = EasingFunction.InOutExpo,
                Sound = spearSlash1
            });

            swings.Add(new OvalSwing
            {
                Duration = 28,
                XSwingRadius = 100,
                YSwingRadius = 50,
                SwingDegrees = 90,
                Easing = EasingFunction.InOutExpo,
                Sound = spearSlash1
            });

            swings.Add(new ThrustSwing
            {
                Duration = 12,
                ThrowDistance = 90,
                Easing = EasingFunction.QuadraticBump,
                Sound = spearSlash2
            });

            swings.Add(new ThrustSwing
            {
                Duration = 12,
                ThrowDistance = 90,
                Easing = EasingFunction.QuadraticBump,
                Sound = spearSlash2
            });



            swings.Add(new OvalSwing
            {
                Duration = 24,
                XSwingRadius = 100,
                YSwingRadius = 50,
                SwingDegrees = 90,
                Easing = EasingFunction.InOutExpo,
                Sound = spearSlash1
            });

            swings.Add(new OvalSwing
            {
                Duration = 24,
                XSwingRadius = 100,
                YSwingRadius = 50,
                SwingDegrees = 90,
                Easing = EasingFunction.InOutExpo,
                Sound = spearSlash1
            });


            swings.Add(new OvalSwing
            {
                Duration = 60,
                XSwingRadius = 64,
                YSwingRadius = 64,
                SwingDegrees = 360 * 4,
                HitCount = 5,
                Easing = EasingFunction.None,
                Sound = nSpin
            });

            swings.Add(new ThrustSwing
            {
                Duration = 30,
                ThrowDistance = 128,
                Easing = EasingFunction.QuadraticBump,
                Sound = spearSlash2
            });

            swings.Add(new ThrustSwing
            {
                Duration = 60,
                ThrowDistance = 200,
                Easing = EasingFunction.QuadraticBump,
                Sound = spearSlash2
            });
        }

        public override void AI()
        {
            base.AI();

        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            if (!Hit)
            {
                FXUtil.ShakeCamera(target.Center, 1024, 8f);
                Particle.NewParticle<IceStrikeParticle>(target.Center, Vector2.Zero, Color.White);

                Hit = true;
            }
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(target, ref modifiers);
            SoundStyle spearHit = AssetRegistry.Sounds.Melee.SpearHit1;
            spearHit.PitchVariance = 0.1f;
            SoundEngine.PlaySound(spearHit, Projectile.position);

            if (ComboIndex == 7)
            {
                modifiers.FinalDamage *= 2;
            }

            if (ComboIndex == 8)
            {
                modifiers.FinalDamage *= 3;
            }
        }
    }

    public class CrystalPointerStaminaStab : BaseSwingProjectile
    {
        public bool Hit;
        public SlashEffect SlashEffect { get; set; }
        public SlashTrailer SlashTrailer { get; set; }
        private float GetTrailWidth(float interpolant)
        {
            return EasingFunction.QuadraticBump(interpolant) * 32;
        }

        public override void DefineCombo(List<ISwing> swings)
        {
            base.DefineCombo(swings);
            //Setup the slash effect
            SlashEffect = new SlashEffect()
            {
                BaseColor = Color.LightBlue,
                WindColor = Color.DarkBlue,
                LightColor = Color.LightCyan,
                RimHighlightColor = Color.White,
                BlendState = Microsoft.Xna.Framework.Graphics.BlendState.Additive
            };
            SlashTrailer = new();
            SlashTrailer.TrailWidthFunction = GetTrailWidth;
            SlashTrailer.Shader = SlashEffect;
            Trailer = SlashTrailer;


            SoundStyle spearSlash1 = AssetRegistry.Sounds.Melee.SpearSlash1;
            SoundStyle spearSlash2 = AssetRegistry.Sounds.Melee.SpearSlash2;
            SoundStyle nSpin = AssetRegistry.Sounds.Melee.SwordSpin1;
            spearSlash1.PitchVariance = 0.25f;
            spearSlash2.PitchVariance = 0.25f;
            nSpin.PitchVariance = 0.2f;

            swings.Add(new OvalSwing
            {
                Duration = 60,
                XSwingRadius = 128,
                YSwingRadius = 48,
                SwingDegrees = 2100,
                Easing = EasingFunction.None,
                HitCount=16,
                Sound = nSpin,
            });

            swings.Add(new ThrustSwing
            {
                Duration = 20,
                ThrowDistance = 250,
                Easing = EasingFunction.QuadraticBump,
                Sound = spearSlash2
            });

        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            SwingPlayer comboPlayer = Owner.GetModPlayer<SwingPlayer>();
            int combo = (int)(ComboIndex + 1);
            int dir = comboPlayer.ComboDirection;


            if (ComboIndex < 1)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position, Main.MouseWorld - Owner.Center, Projectile.type, Projectile.damage, Projectile.knockBack,
                            Owner.whoAmI, ai2: combo, ai1: dir);
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            if (!Hit)
            {
                FXUtil.ShakeCamera(target.Center, 1024, 8f);
                Particle.NewParticle<IceStrikeParticle>(target.Center, Vector2.Zero, Color.White);
                Hit = true;
            }
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(target, ref modifiers);

            if (ComboIndex == 0)
            {
                SoundStyle spearHit = AssetRegistry.Sounds.Melee.SpearHit1;
                spearHit.PitchVariance = 0.1f;
                SoundEngine.PlaySound(spearHit, Projectile.position);
            }

            if (ComboIndex == 1)
            {
                SoundStyle spearHit2 = AssetRegistry.Sounds.Melee.NormalSwordHit1;
                spearHit2.PitchVariance = 0.2f;
                SoundEngine.PlaySound(spearHit2, Projectile.position);
                modifiers.FinalDamage *= 3;
            }
        }
    }
}