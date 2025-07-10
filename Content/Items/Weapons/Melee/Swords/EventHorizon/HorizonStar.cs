using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Stellamod.Assets;
using Stellamod.Content.Dusts;
using Stellamod.Core.Effects;
using Stellamod.Core;
using Stellamod.Core.Effects.Trails;
using Stellamod.Core.Helpers;
using Stellamod.Core.Helpers.Math;
using Stellamod.Core.Particles;
using Stellamod.Core.Visual.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Stellamod.Content.Items.Weapons.Melee.Swords.EventHorizon
{
    internal class HorizonStar : ScarletProjectile
    {
        private ref float Direction => ref Projectile.ai[0];
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 2;
            Projectile.height = 2;
            Projectile.friendly = true;
            Projectile.timeLeft = 180;
            TrailCacheLength = Main.rand.Next(12, 24);
        }

        public override void AI()
        {
            base.AI();
            if(Projectile.velocity.Length() <= 15)
            {
                Projectile.velocity *= 1.01f;
            }
            if(Main.myPlayer == Projectile.owner && Direction == 0)
            {
                Direction = Main.rand.NextFloat(-1.0f, 1.0f);
                Projectile.netUpdate = true;
            }
            Projectile.velocity = Projectile.velocity.RotatedBy(0.01f * Direction);
            Projectile.rotation = Projectile.velocity.ToRotation();

            if (Main.rand.NextBool(8))
            {
                Color color = Main.rand.NextBool(2) ? Color.Yellow : Color.LightCyan;
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.Next(1, 2)).RotatedByRandom(19.0), 0,
                    color, 0.33f).noGravity = true;
            }

        }

        public MoonIceTrailer TrailDrawer;
        public MoonSparkleShader SparkleShader;
        public override bool PreDraw(ref Color lightColor)
        {
            TrailDrawer ??= new MoonIceTrailer();
            TrailDrawer.DrawTrail(ref lightColor, OldCenterPos);

            SparkleShader ??= new MoonSparkleShader();
            SparkleShader.ApplyToEffect();
            SpriteBatch spriteBatch = Main.spriteBatch;
            //Draw to the hting
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawOrigin = tex.Size() / 2; ;

            //Draw Glow Shader
            spriteBatch.Restart(effect: SparkleShader.Effect, blendState: BlendState.Additive);
            spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, drawOrigin, Projectile.scale +
                ExtraMath.Osc(-0.1f, 0.1f, speed: 16), SpriteEffects.None, 0);
            spriteBatch.Restart(blendState: BlendState.Additive);

            //Draw Star
            spriteBatch.Restart(blendState: BlendState.AlphaBlend);
            spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, Color.LightBlue, Projectile.rotation, drawOrigin, Projectile.scale +
                ExtraMath.Osc(-0.1f, 0.1f, speed: 16) + 0.3f, SpriteEffects.None, 0);

            spriteBatch.Restart(blendState: BlendState.Additive);
            spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, drawOrigin, Projectile.scale +
                ExtraMath.Osc(-0.1f, 0.1f, speed: 16), SpriteEffects.None, 0);
            spriteBatch.RestartDefaults();
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            float boomSize = Main.rand.NextFloat(0.08f, 0.12f);

            for (float i = 0; i < 4; i++)
            {
                float progress = i / 4f;
                float rot = progress * MathHelper.ToRadians(360);
                rot += Main.rand.NextFloat(-0.5f, 0.5f);
                Vector2 offset = rot.ToRotationVector2() * 24;
                var particle = FXUtil.GlowCircleLongBoom(Projectile.Center,
                    innerColor: Color.White,
                    glowColor: Main.rand.NextBool(2) ? Color.Yellow : Color.LightCyan,
                    outerGlowColor: Color.DarkBlue,
                    baseSize: boomSize,
                    duration: Main.rand.NextFloat(15, 25));
                particle.Rotation = rot + MathHelper.ToRadians(45);
            }

            SoundStyle hitSound = Main.rand.NextBool(2) ? AssetRegistry.Sounds.Magic.HolyHit1 : AssetRegistry.Sounds.Magic.HolyHit2;
            hitSound.PitchVariance = 0.2f;
            SoundEngine.PlaySound(hitSound, Projectile.Center);
            for (int i = 0; i < 4; i++)
            {
                Color color = Main.rand.NextBool(2) ? Color.Yellow : Color.LightCyan;
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.Next(1, 3)).RotatedByRandom(19.0), 0,
                   color, 0.5f).noGravity = true;
            }
            Particle.NewParticle<SpiralBoomParticle>(Projectile.Center, Vector2.Zero, Color.LightCyan);
        }
    }
}
