using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Core.DrawEffects;
using Stellamod.Helpers;
using Stellamod.Trails;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;
using Stellamod.Common.Shaders;
using Stellamod.Common.Shaders.MagicTrails;
using Stellamod.Core.Pixelation;

namespace Stellamod.Content.Areas.Collosseum.BossesCL.Gustbeak.Projectiles
{
    public abstract class BaseWindProjectile : ModProjectile,
        IDrawOutlines
    {
        private CoreWind _wind;

        protected CoreWind Wind
        {
            get
            {
                _wind ??= new CoreWind();
                return _wind;
            }
        }

        protected ref float Timer => ref Projectile.ai[0];
        protected float DrawScale = 1f;
        protected float ShadowScale = 1f;
        public PrimDrawer Trail { get; set; }
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.projFrames[Type] = 4;
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            Wind.AI(Projectile.Center);
            Projectile.rotation += 0.025f;
            DrawHelper.AnimateTopToBottom(Projectile, 2);
        }

        protected virtual void DrawWindTrail()
        {
            var shader = MagicRadianceShader.Instance;
            shader.PrimaryTexture = TrailRegistry.GlowTrail;
            shader.NoiseTexture = TrailRegistry.CloudsSmall;
            shader.OutlineTexture = TrailRegistry.DottedTrailOutline;
            shader.PrimaryColor = Color.Lerp(Color.White, Color.LightGray, 0.5f);
            shader.NoiseColor = Color.LightGray;
            shader.OutlineColor = Color.Transparent;
            shader.BlendState = BlendState.Additive;
            shader.SamplerState = SamplerState.PointWrap;
            shader.Speed = 5.2f;
            shader.Distortion = 0.15f;
            shader.Power = 0.25f;
            TrailDrawer.Draw(Main.spriteBatch, Projectile.oldPos, Projectile.oldRot, StripColors, StripWidth, shader, offset: Projectile.Size / 2);
        }

        public virtual Color StripColors(float progressOnStrip)
        {
            //  return Color.Lerp(Color.LightGoldenrodYellow, Color.White, Utils.GetLerpValue(0f, 0.7f, progressOnStrip, clamped: true)) * (1f - Utils.GetLerpValue(0f, 0.98f, progressOnStrip));
            Color result = Color.Lerp(Color.LightGray, Color.White,
                Utils.GetLerpValue(0f, 0.7f, progressOnStrip, clamped: true)) * (1f - Utils.GetLerpValue(0f, 0.98f, progressOnStrip));
            //     result.A /= 2;
            return result;
        }

        public virtual float StripWidth(float progressOnStrip)
        {
            return MathHelper.Lerp(26f, 32f, Utils.GetLerpValue(0f, 0.2f, progressOnStrip, clamped: true)) * Utils.GetLerpValue(0f, 0.07f, progressOnStrip, clamped: true);
        }


        protected virtual void DrawWindSlashes(ref Color lightColor)
        {
            PixelationManager.QueuePrimitivesDrawAction(DrawPixelatedWind);
        }

        public void DrawPixelatedWind(GraphicsDevice graphicsDevice)
        {
            DrawWindTrail();
            Wind.Draw(graphicsDevice);
        }

        protected virtual void DrawWindBall(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            Rectangle frame = Projectile.Frame();
            Vector2 drawOrigin = frame.Size() / 2f;
            Color drawColor = Color.White.MultiplyRGB(lightColor);
            float drawRotation = Projectile.rotation;
            float drawScale = 1f * DrawScale;
            spriteBatch.Draw(texture, drawPos, frame, drawColor, drawRotation, drawOrigin, drawScale, SpriteEffects.None, layerDepth: 0);
        }

        protected virtual void DrawWindBall(Vector2 drawPos, ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Rectangle frame = Projectile.Frame();
            Vector2 drawOrigin = frame.Size() / 2f;
            Color drawColor = Color.White.MultiplyRGB(lightColor);
            drawColor.A = 0;
            drawColor *= 0.35f;

            float drawRotation = Projectile.rotation;
            float drawScale = 0.5f * DrawScale;
            spriteBatch.Draw(texture, drawPos, frame, drawColor, drawRotation, drawOrigin, drawScale, SpriteEffects.None, layerDepth: 0);
        }

        protected virtual void DrawBackShadow(Vector2 screenPos, Color spriteColor, ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            Texture2D texture = TextureRegistry.BasicGlow.Value;
            Vector2 shadowDrawPos = Projectile.Center - screenPos;
            Vector2 shadowDrawOrigin = texture.Size() / 2f;
            float drawScale = 0.66f * DrawScale;
            Color drawColor = spriteColor.MultiplyRGB(lightColor) * ShadowScale;
            spriteBatch.Draw(texture, shadowDrawPos, null, drawColor, 0, shadowDrawOrigin, drawScale, SpriteEffects.None, layerDepth: 0);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawBackShadow(Main.screenPosition, Color.Black, ref lightColor);
            DrawWindSlashes(ref lightColor);
            return false;
        }

        public void DrawOutlines(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            Vector2 v = Vector2.UnitY * 2;
            Vector2 h = Vector2.UnitX * 2;
            DrawBackShadow(screenPos + v, Color.Red, ref lightColor);
            DrawBackShadow(screenPos - v, Color.Red, ref lightColor);
            DrawBackShadow(screenPos + h, Color.Red, ref lightColor);
            DrawBackShadow(screenPos - h, Color.Red, ref lightColor);
        }
    }
}
