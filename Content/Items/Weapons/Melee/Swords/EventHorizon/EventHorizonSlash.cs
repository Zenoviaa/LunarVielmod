using Microsoft.Xna.Framework;
using Stellamod.Assets;
using Stellamod.Content.Dusts;
using Stellamod.Core.Effects.Trails;
using Stellamod.Core.Helpers;
using Stellamod.Core.Helpers.Math;
using Stellamod.Core.Particles;
using Stellamod.Core.SwingSystem;
using Stellamod.Core.Visual.Particles;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Stellamod.Content.Items.Weapons.Melee.Swords.EventHorizon
{
    internal class EventHorizonSlash : BaseSwingProjectile
    {
        public MoonIceTrailer SlashTrailer { get; set; }
        public override void SetDefaults2()
        {
            base.SetDefaults2();

        }

        private float DefaultWidthFunction(float interpolant)
        {
            return MathHelper.SmoothStep(50, 0, interpolant) * EasingFunction.QuadraticBump(interpolant);
        }

        public override void DefineCombo(List<ISwing> swings)
        {
            base.DefineCombo(swings);
            SlashTrailer = new();
            Trailer = SlashTrailer;
            Trailer.TrailWidthFunction = DefaultWidthFunction;

            SoundStyle swingSound1 = AssetRegistry.Sounds.Melee.NormalSwordSlash1;
            swingSound1.PitchVariance = 0.25f;

            SoundStyle swingSound2 = AssetRegistry.Sounds.Melee.NormalSwordSlash2;
            swingSound2.PitchVariance = 0.25f;

            SoundStyle swingSound3 = AssetRegistry.Sounds.Melee.SwordSpin1;
            swingSound3.PitchVariance = 0.5f;
            swings.Add(new OvalSwing
            {
                Duration = 24,
                XSwingRadius = 80,
                YSwingRadius = 48,
                SwingDegrees = 270,
                Easing = EasingFunction.InOutExpo,
                Sound = swingSound1,
            });

            swings.Add(new OvalSwing
            {
                Duration = 24,
                XSwingRadius = 80,
                YSwingRadius = 48,
                SwingDegrees = 270,
                Easing = EasingFunction.InOutExpo,
                Sound = swingSound2
            });

            swings.Add(new OvalSwing
            {
                Duration = 24,
                XSwingRadius = 80,
                YSwingRadius = 48,
                SwingDegrees = 270,
                Easing = EasingFunction.InOutExpo,
                Sound = swingSound1
            });

            swings.Add(new OvalSwing
            {
                Duration = 24,
                XSwingRadius = 80,
                YSwingRadius = 48,
                SwingDegrees = 270,
                Easing = EasingFunction.InOutExpo7,
                Sound = swingSound2
            });

            swings.Add(new OvalSwing
            {
                Duration = 24,
                XSwingRadius = 80,
                YSwingRadius = 48,
                SwingDegrees = 270,
                Easing = EasingFunction.InOutExpo7,
                Sound = swingSound1
            });

            swings.Add(new OvalSwing
            {
                Duration = 40,
                XSwingRadius = 100,
                YSwingRadius = 40,
                SwingDegrees = 540,
                Easing = EasingFunction.InOutExpo7,
                Sound = swingSound3
            });
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            SoundStyle spearHit = AssetRegistry.Sounds.Melee.SpearHit1;
            spearHit.PitchVariance = 0.5f;
            SoundEngine.PlaySound(spearHit, Projectile.position);

            for (int i = 0; i < 4; i++)
            {
                Color color = Main.rand.NextBool(2) ? Color.Yellow : Color.LightCyan;
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.Next(1, 3)).RotatedByRandom(19.0), 0,
                   color, 0.5f).noGravity = true;
            }

            float boomSize = Main.rand.NextFloat(0.08f, 0.12f);

            for (float i = 0; i < 4; i++)
            {
                float progress = i / 4f;
                float rot = progress * MathHelper.ToRadians(360);
                rot += Main.rand.NextFloat(-0.5f, 0.5f);
                Vector2 offset = rot.ToRotationVector2() * 24;
                var particle = FXUtil.GlowCircleLongBoom(target.Center,
                    innerColor: Color.White,
                    glowColor: Main.rand.NextBool(2) ? Color.Yellow : Color.LightCyan,
                    outerGlowColor: Color.DarkBlue,
                    baseSize: boomSize,
                    duration: Main.rand.NextFloat(15, 25));
                particle.Rotation = rot + MathHelper.ToRadians(45);
            }

        }
    }
}
