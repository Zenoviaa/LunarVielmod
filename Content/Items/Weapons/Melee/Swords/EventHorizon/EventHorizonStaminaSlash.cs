using Microsoft.Xna.Framework;
using Stellamod.Assets;
using Stellamod.Content.Dusts;
using Stellamod.Core.Effects.Trails;
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
    internal class EventHorizonStaminaSlash : BaseSwingProjectile
    {
        private float _starTimer;
        public MoonIceTrailer SlashTrailer { get; set; }
        public override void SetDefaults2()
        {
            base.SetDefaults2();

        }

        public override void AI()
        {
            base.AI();
            _starTimer++;
            if (_starTimer % 96 == 0)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Owner.Center, Projectile.velocity, 
                    ModContent.ProjectileType<HorizonStar>(), Projectile.damage * 2, Projectile.knockBack, Projectile.owner);
            }
        }
        private float GetTrailWidth(float interpolant)
        {
            return MathHelper.SmoothStep(100, 0, interpolant) * EasingFunction.QuadraticBump(interpolant);
        }
        public override void DefineCombo(List<ISwing> swings)
        {
            base.DefineCombo(swings);
            SlashTrailer = new();
            Trailer = SlashTrailer;
            Trailer.TrailWidthFunction = GetTrailWidth;
            useAfterImage = true;
            SoundStyle swingSound3 = AssetRegistry.Sounds.Melee.SwordSpin1;
            swingSound3.PitchVariance = 0.5f;

            swings.Add(new OvalSwing
            {
                Duration = 60,
                XSwingRadius = 80,
                YSwingRadius = 48,
                SwingDegrees = 720,
                HitCount=4,
                Easing = EasingFunction.InOutExpo,
                Sound = swingSound3,
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
        }
    }
}
