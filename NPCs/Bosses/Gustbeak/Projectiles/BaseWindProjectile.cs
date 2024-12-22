using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Common.DrawEffects;
using Stellamod.Helpers;
using Stellamod.Trails;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.Gustbeak.Projectiles
{
    public abstract class BaseWindProjectile : ModProjectile
    {
        private CommonWind _wind;

        protected CommonWind Wind
        {
            get
            {
                _wind ??= new CommonWind();
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
        public float WidthFunction(float completionRatio)
        {
            float baseWidth = Projectile.scale * Projectile.width * 0.62f;
            return MathHelper.SmoothStep(32, baseWidth, completionRatio);
        }

        public Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.Transparent, Color.White, Easing.SpikeOutCirc(completionRatio));
        }

        protected virtual void DrawWindTrail(ref Color lightColor)
        {
            Trail ??= new PrimDrawer(WidthFunction, ColorFunction, GameShaders.Misc["VampKnives:SuperSimpleTrail"]);
            GameShaders.Misc["VampKnives:SuperSimpleTrail"].SetShaderTexture(TrailRegistry.Dashtrail);
            Trail.DrawPrims(Projectile.oldPos, -Main.screenPosition + Projectile.Size / 2, totalTrailPoints: 155);
        }

        protected virtual void DrawWindSlashes(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            Wind.Draw(spriteBatch, lightColor);
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
            drawColor *= 0.75f;
            float drawRotation = Projectile.rotation;
            float drawScale = 0.5f * DrawScale;
            spriteBatch.Draw(texture, drawPos, frame, drawColor, drawRotation, drawOrigin, drawScale, SpriteEffects.None, layerDepth: 0);
        }
        protected virtual void DrawBackShadow(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            Texture2D texture = TextureRegistry.BasicGlow.Value;
            Vector2 shadowDrawPos = Projectile.Center - Main.screenPosition;
            Vector2 shadowDrawOrigin = texture.Size() / 2f;
            float drawScale = 0.66f * DrawScale;
            Color drawColor = Color.Black.MultiplyRGB(lightColor) * ShadowScale;
            spriteBatch.Draw(texture, shadowDrawPos, null, drawColor, 0, shadowDrawOrigin, drawScale, SpriteEffects.None, layerDepth: 0);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawBackShadow(ref lightColor);
            DrawWindTrail(ref lightColor);
            DrawWindSlashes(ref lightColor);
            return false;
        }
    }
}
