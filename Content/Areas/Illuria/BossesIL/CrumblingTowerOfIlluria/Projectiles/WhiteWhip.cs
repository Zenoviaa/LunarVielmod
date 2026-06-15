using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Common.Shaders.MagicTrails;
using Stellamod.Core;
using Stellamod.Core.Particles;
using Stellamod.Core.Pixelation;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Illuria.BossesIL.CrumblingTowerOfIlluria.Projectiles
{
    public class WhiteWhip : ScarletProjectile
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
            Projectile.tileCollide = true;
            Projectile.extraUpdates = 1;
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

            float chargeTime = 250f;
            float completionRatio = Timer / chargeTime;
            float ease = EasingFunction.QuadraticBump(completionRatio);
            _bloomLineAlpha = MathHelper.Lerp(0f, 0.7f, ease);
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
                float numDust = 5;
                for (float f = 0; f < numDust; f++)
                {
                    Vector2 dustVelocity = Projectile.velocity;
                    dustVelocity = dustVelocity.RotatedByRandom(0.25f);
                    dustVelocity *= Main.rand.NextFloat(0.3f, 1f);
                    Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowDust>(), dustVelocity, newColor: Color.White, Scale: Main.rand.NextFloat(0.3f, 0.5f));
                }

                var donut = LegacyParticle.NewParticle<GlowDonutParticle>(Projectile.Center, -Projectile.velocity, newColor: Color.Cyan);
                SoundStyle fireSound = AssetRegistry.Sounds.Magic.AutomationCast1;
                fireSound.PitchVariance = 0.2f;
                SoundEngine.PlaySound(fireSound, Projectile.position);
            }
            if (Timer % 7 == 0)
            {
                var zap = LegacyParticle.NewParticle<ZapParticle>(Projectile.Center, Main.rand.NextVector2Circular(2, 2), newColor: Color.White, Scale: 0.75f);
                zap.Scale *= 0.2f;
                zap.innerColor = Color.White;
                zap.outerColor = Color.Cyan;
                zap.fadeToColor = Color.Purple;
            }

            if(Timer % 15 == 0)
            {
                var spark = LegacyParticle.NewParticle<SparkParticle>(Projectile.Center, Main.rand.NextVector2Circular(2, 2), newColor: Color.White, Scale: 0.75f);
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
            PixelationManager.QueuePrimitivesDrawAction(DrawPixelated);
            return false;
        }

        public void DrawPixelated(GraphicsDevice graphicsDevice)
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

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            float numDust = 3;
            for (float n = 0; n < numDust; n++)
            {
                Vector2 velocity = Main.rand.NextVector2Circular(4, 4);
                velocity += -Projectile.oldVelocity * Main.rand.NextFloat(0.5f, 1f);
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowDust>(), velocity, newColor: Color.Cyan, Scale: Main.rand.NextFloat(0.4f, 0.75f));
            }
            var part = FXUtil.GlowCircleBoom(Projectile.Center, Color.White, Color.Cyan, Color.Blue);
            part.Scale *= 0.66f;

            SoundStyle hitSound = Main.rand.NextBool(2) ? AssetRegistry.Sounds.Illuria.IceImpact1 : AssetRegistry.Sounds.Illuria.IceImpact2;
            hitSound.PitchVariance = 0.3f;
            hitSound.Volume = 0.5f;
            SoundEngine.PlaySound(hitSound, Projectile.position);
        }
    }
}
