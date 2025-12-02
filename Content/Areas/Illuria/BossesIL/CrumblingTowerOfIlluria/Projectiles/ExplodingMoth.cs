using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Core.Shaders;
using Stellamod.Core.Shaders.MagicTrails;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Trails;
using Stellamod.UI.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Illuria.BossesIL.CrumblingTowerOfIlluria.Projectiles
{
    public class MothExplosion : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[0];
        public override string Texture => TextureRegistry.EmptyTexture;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.timeLeft = 4;
            Projectile.hostile = true;
            Projectile.width = 64;
            Projectile.height = 64;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            if(Timer == 1)
            {
                ShakeModSystem.Shake = 4;
                FXUtil.ShakeCamera(Projectile.position, 1024, 4);
                float numDust = 8;
                for (float n = 0; n < numDust; n++)
                {
                    Vector2 vel = Main.rand.NextVector2Circular(16, 16);
                    Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowDust>(), vel, newColor: Color.LightCyan, Scale: Main.rand.NextFloat(0.5f, 1f));
                }
                for (float i = 0; i < 8; i++)
                {
                    float progress = i / 4f;
                    float rot = progress * MathHelper.ToRadians(360);
                    rot += Main.rand.NextFloat(-0.5f, 0.5f);
                    Vector2 offset = rot.ToRotationVector2() * 24;
                    var particle = FXUtil.GlowCircleDetailedBoom1(Projectile.Center,
                        innerColor: Color.White,
                        glowColor: Color.LightCyan,
                        outerGlowColor: Color.Blue,
                        baseSize: Main.rand.NextFloat(0.1f, 0.2f),
                        duration: Main.rand.NextFloat(15, 25));
                    particle.Rotation = rot + MathHelper.ToRadians(45);
                }
            }
        }
    }
    public class ExplodingMoth : ModProjectile,
         IDrawOutlines
    {
        private float _explosionAlpha;
        private Color _outlineColor;
        private enum AIState
        {
            Hover,
            Explode
        }
        private ref float Timer => ref Projectile.ai[0];
        private AIState State
        {
            get => (AIState)Projectile.ai[1];
            set => Projectile.ai[1] = (float)value;
        }
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.projFrames[Type] = 4;
            ProjectileID.Sets.TrailCacheLength[Type] = 16;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 600;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            switch (State)
            {
                case AIState.Hover:
                    AI_Idle();
                    break;
                case AIState.Explode:
                    AI_Prime();
                    break;
            }

 
            DrawHelper.AnimateTopToBottom(Projectile, 4);
        }


        private void SwitchState(AIState state)
        {
            State = state;
            Timer = 0;
            Projectile.netUpdate = true;
        }

        private void AI_Idle()
        {
            _outlineColor = Color.Yellow;

            Projectile.velocity.X = 0;
            Projectile.velocity.Y = MathF.Sin(Timer * 0.05f) * 0.5f;

            float inTime = 30f;
            float inRatio = Timer / inTime;
            float ease = EasingFunction.InOutSine(inRatio);
            float inScale = MathHelper.Lerp(0f, 1f, ease);
            float outScale = (float)Projectile.timeLeft / 30f;
            outScale = EasingFunction.InOutSine(outScale);
            Projectile.scale = inScale * outScale;
            Projectile.rotation = Projectile.velocity.X * 0.05f;

            Player closest = PlayerHelper.FindClosestPlayer(Projectile.position, 4000);
            if (closest != null)
            {
                float distanceToTarget = Vector2.Distance(closest.Center, Projectile.Center);
                if (distanceToTarget < 150)
                {
                    if (this.OwnedByLocalClient() && Timer >= 60)
                    {
                        SwitchState(AIState.Explode);
                    }
                }
            }
        }

        private void AI_Prime()
        {

            _outlineColor = Color.Yellow;
            float explodingTime = 120;
            float completionRatio = Timer / explodingTime;
            _explosionAlpha = EasingFunction.Anticipation(completionRatio);
            Projectile.velocity.Y *= 0.9f;
            Projectile.scale *= 1.001f;
            Projectile.timeLeft = 2;
            if (Timer == 119)
            {
                if (this.OwnedByLocalClient())
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero,
                        ModContent.ProjectileType<MothExplosion>(), Projectile.damage, 1, Projectile.owner);
                }
                Projectile.Kill();
            }
        }

        private Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.White, Color.SpringGreen, completionRatio);
        }

        private float WidthFunction(float completionRatio)
        {
            return MathHelper.SmoothStep(8, 0, completionRatio);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            var shader = MagicNormalShader.Instance;
            shader.PrimaryTexture = TrailRegistry.GlowTrail;
            shader.NoiseTexture = TrailRegistry.SpikyTrail1;
            shader.BlendState = BlendState.Additive;
            shader.SamplerState = SamplerState.PointWrap;
            shader.Speed = 0.5f;
            shader.Repeats = 1f;
            //This just applis the shader changes
            TrailDrawer.Draw(Main.spriteBatch, Projectile.oldPos, ColorFunction, WidthFunction, shader, offset: Projectile.Size / 2f);
            SpriteBatch spriteBatch = Main.spriteBatch;
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Rectangle frame = Projectile.Frame();
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                float f = i;
                float completionRatio = f / (float)Projectile.oldPos.Length;
                Color drawColor = Color.Lerp(Color.White, Color.Transparent, completionRatio);
                Vector2 drawPosition = Projectile.oldPos[i] + Projectile.Size / 2f;
                Vector2 drawOrigin = frame.Size() / 2f;
                drawColor *= 0.1f;
                spriteBatch.Draw(texture, drawPosition - Main.screenPosition, frame, drawColor, Projectile.oldRot[i], drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            }
            DrawSprite(spriteBatch, Main.screenPosition, lightColor);

            DrawSprite(spriteBatch, Main.screenPosition, Color.Yellow * _explosionAlpha * ExtraMath.Osc(0f, 1f, speed: 64));
            return false;
        }

        private void DrawSprite(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Rectangle frame = Projectile.Frame();
            spriteBatch.Draw(texture, Projectile.Center - screenPos, frame, drawColor, Projectile.rotation, frame.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
        }


        public override void PostDraw(Color lightColor)
        {
            base.PostDraw(lightColor);
            SpriteBatch spriteBatch = Main.spriteBatch;
            Texture2D glowingBallTexture = ModContent.Request<Texture2D>(TextureRegistry.ZuiEffect).Value;
            Vector2 drawOrigin = glowingBallTexture.Size() / 2f;
            Vector2 drawCenter = Projectile.Center - Main.screenPosition;
            Color glowColor = Color.Cyan;
            glowColor.A = 0;
            glowColor *= 0.5f;
            glowColor *= ExtraMath.Osc(0.3f, 0.8f, speed: 8);
            spriteBatch.Draw(glowingBallTexture, drawCenter, null, glowColor, Projectile.rotation, drawOrigin, Projectile.scale * 0.6f, SpriteEffects.None, 0);

            glowColor = Color.Yellow;
            glowColor.A = 0;
            glowColor *= 0.5f;
            glowColor *= ExtraMath.Osc(0.3f, 0.8f, speed: 8);
            spriteBatch.Draw(glowingBallTexture, drawCenter, null, glowColor * _explosionAlpha, Projectile.rotation, drawOrigin, Projectile.scale * 0.6f, SpriteEffects.None, 0);
        }

        public void DrawOutlines(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            float outlineOffset = 2;
            Vector2 v = Vector2.UnitY * outlineOffset;
            Vector2 h = Vector2.UnitX * outlineOffset;
            DrawSprite(spriteBatch, screenPos + v, _outlineColor);
            DrawSprite(spriteBatch, screenPos - v, _outlineColor);
            DrawSprite(spriteBatch, screenPos + h, _outlineColor);
            DrawSprite(spriteBatch, screenPos - h, _outlineColor);
        }
    }
}
