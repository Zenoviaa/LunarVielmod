using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Assets;
using Stellamod.Core;
using Stellamod.Core.Particles;
using Stellamod.Core.Pixelation;
using Stellamod.Core.Shaders;
using Stellamod.Core.Shaders.MagicTrails;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Trails;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Illuria.BossesIL.CrumblingTowerOfIlluria.Projectiles
{
    public class WhiteWhip : ScarletProjectile,
        IDrawPixelated
    {
        private float _bloomLineAlpha;
        private enum AIState
        {
            Charge,
            Fire
        }
        private ref float Timer => ref Projectile.ai[0];
        private AIState State
        {
            get => (AIState)Projectile.ai[1];
            set => Projectile.ai[1] = (float)value;
        }
        private NPC Parent
        {
            get => Main.npc[(int)Projectile.ai[2]];
        }
        public override string Texture => TextureRegistry.EmptyTexture;
        public override void SetDefaults()
        {
            base.SetDefaults();
            TrailCacheLength = 32;
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.hostile = true;
            Projectile.timeLeft = 300;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.extraUpdates = 1;
            Projectile.tileCollide = false;
        }

        public override bool ShouldUpdatePosition()
        {
            return State == AIState.Fire;
        }

        public override void AI()
        {
            base.AI();
            switch (State)
            {
                case AIState.Charge:
                    AI_Charge();
                    break;
                case AIState.Fire:
                    AI_Fire();
                    break;
            }
        }


        private void SwitchState(AIState state)
        {
            if (this.OwnedByLocalClient())
            {
                Timer = 0;
                State = state;
                Projectile.netUpdate = true;
            }
        }

        private void AI_Charge()
        {
            Timer++;
            if (Timer == 1)
            {
                float numDust = 2;
                for (float d = 0; d < numDust; d++)
                {
                    Vector2 vel = Projectile.velocity;
                    vel = vel.RotatedByRandom(0.3f);
                    vel *= Main.rand.NextFloat(0.5f, 1f);
                    Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlyphDust>(), vel, newColor: Color.White);
                }

       
            }

            float chargeTime = 80f;
            float completionRatio = Timer / chargeTime;
            float ease = EasingFunction.QuadraticBump(completionRatio);
            _bloomLineAlpha = MathHelper.Lerp(0f, 0.5f, ease);
            Projectile.Center = Parent.Center;
            if (Timer >= chargeTime)
            {
                SwitchState(AIState.Fire);
            }
        }

        private void AI_Fire()
        {
            Timer++;
            if(Timer == 1)
            {
                float numDust = 6;
                for (float f = 0; f < numDust; f++)
                {
                    Vector2 dustVelocity = Projectile.velocity;
                    dustVelocity = dustVelocity.RotatedByRandom(0.25f);
                    dustVelocity *= Main.rand.NextFloat(2, 9);
                    Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowDust>(), dustVelocity, newColor: Color.White, Scale: Main.rand.NextFloat(0.3f, 0.8f));
                }

                var donut = Particle.NewParticle<GlowDonutParticle>(Projectile.Center, -Projectile.velocity, newColor: Color.Cyan);
                SoundStyle fireSound = AssetRegistry.Sounds.Magic.AutomationCast1;
                fireSound.PitchVariance = 0.2f;
                SoundEngine.PlaySound(fireSound, Projectile.position);
            }
            if (Timer % 7 == 0)
            {
                var zap = Particle.NewParticle<ZapParticle>(Projectile.Center, Main.rand.NextVector2Circular(2, 2), newColor: Color.White, Scale: 0.75f);
                zap.Scale *= 0.2f;
                zap.innerColor = Color.White;
                zap.outerColor = Color.Cyan;
                zap.fadeToColor = Color.Purple;
            }

            if(Timer % 15 == 0)
            {
                var spark = Particle.NewParticle<SparkParticle>(Projectile.Center, Main.rand.NextVector2Circular(2, 2), newColor: Color.White, Scale: 0.75f);
                spark.innerColor = Color.White;
                spark.outerColor = Color.Cyan;
                spark.fadeToColor = Color.Purple;
            }

            Projectile.velocity *= 1.015f;
            Projectile.rotation = Projectile.velocity.ToRotation();
        }


        private Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.White, Color.SpringGreen * 0.2f, completionRatio);
        }

        private float WidthFunction(float completionRatio)
        {
            return MathHelper.SmoothStep(8, 0, completionRatio);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            float rot = Projectile.velocity.ToRotation() - MathHelper.PiOver2;
            Texture2D bloomlineTexture = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/BloomLine").Value;
            SpriteBatch spriteBatch = Main.spriteBatch;
            Vector2 drawCenter = Projectile.Center - Main.screenPosition;
            Vector2 drawOrigin = new Vector2(bloomlineTexture.Width / 2f, 0f);
            Color drawColor = Color.White;
            drawColor.A = 0;
            drawColor *= _bloomLineAlpha;

            Vector2 scale = Vector2.One;
            scale.X *= 0.2f;
            scale.Y *= 2f;
            scale.Y *= _bloomLineAlpha;
            spriteBatch.Draw(bloomlineTexture, drawCenter, null, drawColor, rot, drawOrigin, scale, SpriteEffects.None, 0);
            return false;
        }

        public void DrawPixelated()
        {

            if(State == AIState.Fire)
            {
                var shader = MagicNormalShader.Instance;
                shader.PrimaryTexture = TrailRegistry.GlowTrail;
                shader.NoiseTexture = TrailRegistry.SpikyTrail1;
                shader.BlendState = BlendState.Additive;
                shader.SamplerState = SamplerState.PointWrap;
                shader.Speed = 0.5f;
                shader.Repeats = 1f;
                //This just applis the shader changes
                TrailDrawer.Draw(Main.spriteBatch, OldCenterPos, ColorFunction, WidthFunction, shader);
            }

        }
    }
}
