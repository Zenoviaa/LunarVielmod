
using Microsoft.Xna.Framework;
using Stellamod.Common.Players;
using Stellamod.Content.Items.Materials;
using Stellamod.Core.Bases;
using Stellamod.Core.Effects.Trails;
using Stellamod.Core.SwingSystem;
using Stellamod.Helpers;
using Stellamod.Items;
using Stellamod.Items.Materials.Molds;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.SpringHills.WeaponsSH
{
    public class WoodenSaber : BaseSwingItemV2
    {
        public override void SetDefaults2()
        {
            base.SetDefaults2();
            Item.damage = 3;
            Item.shoot = ModContent.ProjectileType<WoodenSaberSwordSlash>();
            staminaProjectileShoot = ModContent.ProjectileType<WoodenSaberStaminaSlash>();
            meleeWeaponType = MeleeWeaponType.Knives;
        }

        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew<Ivythorn, BlankSword>();
        }
    }


    public class WoodenSaberSwordSlash : BaseSwingProjectileV2
    {
        private bool _hasSpawnedSecondKnife;
        public override void DefineCombo()
        {
            base.DefineCombo();
            var SlashEffect = new SlashEffect()
            {
                BaseColor = Color.White,
                WindColor = Color.Green,
                LightColor = Color.LightGreen,
                RimHighlightColor = Color.White,
                BlendState = Microsoft.Xna.Framework.Graphics.BlendState.Additive
            };
            var SlashTrailer = new SlashTrailer();
            SlashTrailer.TrailWidthFunction = GetTrailWidth;
            SlashTrailer.Shader = SlashEffect;
            Trailer = SlashTrailer;
            SwingV2Helper.AddKnivesSwingStyle(this);
            useAfterImage = true;
        }

        private float GetTrailWidth(float interpolant)
        {
            return EasingFunction.QuadraticBump(interpolant) * 12;
        }

        public override void AI()
        {
            base.AI();
            if (!_hasSpawnedSecondKnife && ComboIndex != ComboCount - 1 && Interpolant >= 0.9f)
            {
                CloneProjectile();
                _hasSpawnedSecondKnife = true;
            }

            growScale = MathHelper.Lerp(0f, 0.3f, EasingFunction.QuadraticBump(Interpolant));
        }


        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(target, ref modifiers);
            SoundStyle spearHit = SoundRegistry.SpearHit1;
            spearHit.PitchVariance = 0.5f;
            SoundEngine.PlaySound(spearHit, Projectile.position);
            if (ComboIndex == 5)
            {
                StatModifier statModifier = new StatModifier(0.5f, 1f);
                modifiers.FinalDamage.CombineWith(statModifier);
            }
        }
    }

    public class WoodenSaberStaminaSlash : BaseSwingProjectileV2
    {
        private bool _hasSpawnedSecondKnife;
        public override void DefineCombo()
        {
            base.DefineCombo();
            var SlashEffect = new SlashEffect()
            {
                BaseColor = Color.White,
                WindColor = Color.Green,
                LightColor = Color.LightGreen,
                RimHighlightColor = Color.White,
                BlendState = Microsoft.Xna.Framework.Graphics.BlendState.Additive
            };
            var SlashTrailer = new SlashTrailer();
            SlashTrailer.TrailWidthFunction = GetTrailWidth;
            SlashTrailer.Shader = SlashEffect;
            Trailer = SlashTrailer;

            useAfterImage = true;

            SoundStyle swingSound1 = SoundRegistry.NSwordSlash2;
            swingSound1.PitchVariance = 0.5f;

            SoundStyle swingSound2 = SoundRegistry.NSwordSlash2;
            swingSound2.PitchVariance = 0.5f;
            swingSound2.Pitch = 0.5f;

            SoundStyle swingSound3 = SoundRegistry.NSwordSpin1;
            swingSound3.PitchVariance = 0.5f;

            Add(new OvalSwing
            {
                Duration = 60,
                XSwingRadius = 84,
                YSwingRadius = 42,
                SwingDegrees = 720,
                Easing = (float lerpValue) => Easing.InOutExpo(lerpValue),
                Sound = swingSound3,
            });

            Add(new OvalSwing
            {
                Duration = 60,
                XSwingRadius = 72,
                YSwingRadius = 36,
                SwingDegrees = 720,
                Easing = (float lerpValue) => Easing.InOutExpo(lerpValue),
                Sound = swingSound2,
            });


            Add(new OvalSwing
            {
                Duration = 60,
                XSwingRadius = 72,
                YSwingRadius = 36,
                SwingDegrees = 720,
                Easing = (float lerpValue) => Easing.InOutExpo(lerpValue),
                Sound = swingSound2,
            });
        }
        private float GetTrailWidth(float interpolant)
        {
            return EasingFunction.QuadraticBump(interpolant) * 12;
        }


        public override void AI()
        {
            base.AI();
            if (!_hasSpawnedSecondKnife && ComboIndex != ComboCount - 1 && Interpolant >= 0.5f)
            {
                CloneProjectile();
                _hasSpawnedSecondKnife = true;
            }

            growScale = MathHelper.Lerp(0f, 0.3f, EasingFunction.QuadraticBump(Interpolant));
        }


        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(target, ref modifiers);


            SoundStyle spearHit2 = SoundRegistry.NSwordHit1;
            spearHit2.PitchVariance = 0.2f;
            SoundEngine.PlaySound(spearHit2, Projectile.position);

            modifiers.FinalDamage *= 3;
            modifiers.Knockback *= 4;
        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            ComboPlayer comboPlayer = Owner.GetModPlayer<ComboPlayer>();
            int combo = (int)(ComboIndex + 1);
            int dir = comboPlayer.ComboDirection;
            if (ComboIndex < 2)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position, Projectile.velocity, Projectile.type, Projectile.damage, Projectile.knockBack,
                            Owner.whoAmI, ai2: combo, ai1: dir);
            }
        }
    }
}