using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Content.Gores;
using Stellamod.Core.Particles;
using Stellamod.Core.Utilities;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Visual.Particles;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Collosseum.BossesCL.Gustbeak.Projectiles
{
    public class WindStormDebris : AbstractWindProjectile,
        IDrawOutlines
    {
        private Vector2 _scale;
        private ref float FallDownTime => ref Projectile.ai[1];

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.projFrames[Type] = 1;
            ProjectileID.Sets.TrailCacheLength[Type] = 32;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
        }

        public override void AI()
        {
            base.AI();
            _scale = Vector2.Lerp(_scale, Vector2.One, 0.1f);

            if(Timer % 8 == 0)
            {
                ShockOvalSpawnParams spawnParams = new ShockOvalSpawnParams
                {
                    innerColor = Color.White,
                    outerColor = Color.DarkGray
                };
                ShockOvalParticle sp = ShockOvalParticle.Spawn(Projectile.Center, -Projectile.velocity * 0.4f, spawnParams);
                sp.color *= 0.45f;
                sp.Scale *= 0.6f;

                sp = ShockOvalParticle.Spawn(Projectile.Center, -Projectile.velocity * 0.2f, spawnParams);
                sp.color *= 0.45f;
                sp.Scale *= 0.3f;
            }
            if (Timer == 1 && Main.myPlayer == Projectile.owner)
            {
                FallDownTime = Main.rand.NextFloat(15, 100);
                Projectile.netUpdate = true;
            }

            Projectile.rotation += 0.02f;
            Projectile.rotation -= Projectile.velocity.Length() * 0.025f;
            if (Timer > FallDownTime)
            {
                Projectile.tileCollide = true;
                if (Projectile.velocity.Y < 16)
                    Projectile.velocity.Y += 1f;
            }
            else
            {
                Projectile.velocity.Y += MathF.Sin(Timer * 0.2f) * 0.1f;
            }
        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            for (float f = 0; f < 12; f++)
            {
                float rot = f / 12f * MathHelper.TwoPi;
                Vector2 velOffset = rot.ToRotationVector2() * 4;
                Dust.NewDustPerfect(Projectile.Center, DustID.GemDiamond, velOffset, Scale: 1f);
            }
            for (float f = 0; f < 4; f++)
            {
                Vector2 velocity = Main.rand.NextVector2Circular(16, 16);
                FXUtil.GlowStretch(Projectile.Center, velocity);
            }

            FXUtil.ShakeCamera(Projectile.position, 1024, 3);

            int[] gores = AutoGoreLoader.FindGores("GrayRock");
            foreach (int g in gores)
            {
                Gore.NewGore(Projectile.GetSource_FromThis(),
                    Projectile.Center,
                    -Vector2.UnitY.RotatedByRandom(MathHelper.ToRadians(20)) * Main.rand.NextFloat(5f, 15f), g, Main.rand.NextFloat(0f, 1f));
            }


            var sear = LegacyParticle.NewParticle<SearParticle>(Projectile.Center, Vector2.Zero);
            sear.innerColor = Color.Gray;
            sear.outerColor = Color.Blue;
            sear.fadeToColor = Color.Black;
            FXUtil.ShakeCamera(Projectile.Center, 1024, 8);
            ShakeScreenPosition.Shake = 2;


            for (float f = 0; f < 4f; f++)
            {
                Vector2 pos = Projectile.Center;
                pos += Main.rand.NextVector2Circular(80, 80);
                var zap = LegacyParticle.NewParticle<ZapParticle>(pos, Vector2.UnitY.RotatedByRandom(10) * Main.rand.NextFloat(2, 15));
                zap.innerColor = Color.Gray;
                zap.outerColor = Color.Blue;
                zap.fadeToColor = Color.Black;
                zap.Scale *= Main.rand.NextFloat(0f, 0.5f);
                zap.Rotation = Main.rand.NextFloat(0f, 3f);
            }

            SoundStyle smashSound;
            int sound = Main.rand.Next(3);
            switch (sound)
            {
                default:
                case 1:
                    smashSound = new SoundStyle("Stellamod/Assets/Sounds/RockBreak1");
                    break;
                case 2:
                    smashSound = new SoundStyle("Stellamod/Assets/Sounds/RockBreak2");
                    foreach (int g in gores)
                    {
                        Gore.NewGore(Projectile.GetSource_FromThis(),
                            Projectile.Center,
                            -Vector2.UnitY.RotatedByRandom(MathHelper.ToRadians(20)) * Main.rand.NextFloat(5f, 15f), g, Main.rand.NextFloat(0f, 1f));
                    }
                    FXUtil.ShakeCamera(Projectile.Center, 1024, 32);
                    var p3 = FXUtil.GlowCircleBoom(Projectile.Center,
                       innerColor: Color.Gray,
                       glowColor: Color.LightBlue,
                       outerGlowColor: Color.DarkBlue, duration: 15, baseSize: .09f);
                    p3.Scale *= 2;
                    break;
            }


            smashSound.PitchVariance = 0.2f;
            SoundEngine.PlaySound(smashSound, Projectile.position);


            var part = LegacyParticle.NewParticle<GlowDonutParticle>(Projectile.Center, Vector2.Zero, Color.White);
            part.fadeToColor = Color.Black;
            part.outerColor = Color.Gray;
            part.noStretch = true;
            part.shrink = true;

            var part2 = LegacyParticle.NewParticle<GlowDonutParticle>(Projectile.Center, Vector2.Zero, Color.White);
            part2.fadeToColor = Color.Black;
            part2.outerColor = Color.Gray;
            part2.noStretch = true;
            part2.color *= 0.5f;
            for (float f = 0; f < 3; f++)
            {
                Vector2 vel = Main.rand.NextVector2Circular(16, 16);
                vel.Y -= 10;
                var d = Dust.NewDustPerfect(Projectile.Center,
                    ModContent.DustType<GlowSparkleDust>(), newColor: Color.Gray, Scale: Main.rand.NextFloat(0f, 2f), Velocity: vel);

            }

            float boomSize = Main.rand.NextFloat(0.06f, 0.08f);
            FXUtil.GlowCircleBoom(Projectile.Center,
               innerColor: Color.Gray,
               glowColor: Color.LightBlue,
               outerGlowColor: Color.DarkBlue, duration: 15, baseSize: boomSize);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawWindSlashes(ref lightColor);
            this.DrawCentered(ref lightColor, _scale);
            return false;
        }

        public override float StripWidth(float progressOnStrip)
        {
            return base.StripWidth(progressOnStrip) * 0.66f;
        }

        public new void DrawOutlines(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            this.OutlineNoRestart(Color.Red, ref lightColor, _scale);
        }
    }
}
