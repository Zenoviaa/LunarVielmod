using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Core;
using Stellamod.Core.Particles;
using Stellamod.Core.Pixelation;
using Stellamod.Core.Shaders;
using Stellamod.Core.Shaders.MagicTrails;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Trails;
using Stellamod.Visual.Particles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
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
                for(float d = 0; d < numDust; d++)
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
            _bloomLineAlpha = MathHelper.Lerp(0f, 1f, ease);
            if(Timer >= chargeTime)
            {
                SwitchState(AIState.Fire);
            }
        }

        private void AI_Fire()
        {
            Timer++;
            if(Timer % 7 == 0)
            {
                Particle.NewParticle<ZapParticle>(Projectile.Center, Main.rand.NextVector2Circular(2, 2), newColor: Color.White, Scale: 0.75f);
            }

            Projectile.velocity *= 1.01f;
            Projectile.rotation = Projectile.velocity.ToRotation();
        }


        private Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(new Color(69, 196, 182), Color.SpringGreen, completionRatio);
        }

        private float WidthFunction(float completionRatio)
        {
            return MathHelper.SmoothStep(0, 16, completionRatio);
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
            spriteBatch.Draw(bloomlineTexture, drawCenter, null, drawColor, rot, drawOrigin, 1f, SpriteEffects.None, 0);
            return false;
        }

        public void DrawPixelated()
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
