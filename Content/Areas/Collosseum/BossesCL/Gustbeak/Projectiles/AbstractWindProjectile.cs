using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Common.Shaders.MagicTrails;
using Stellamod.Core.DrawEffects;
using Stellamod.Core.Pixelation;
using Stellamod.Helpers;
using Stellamod.Trails;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Collosseum.BossesCL.Gustbeak.Projectiles
{
    public abstract class AbstractWindProjectile : ModProjectile,
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
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.projFrames[Type] = 4;
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            if(Timer == 1 && Projectile.velocity.Length() > 2)
            {
                CrescentSlashParticle.Spawn(Projectile.Center, Projectile.velocity * 3);
             //   CrescentSlashParticle.Spawn(Projectile.Center, Projectile.velocity.RotatedByRandom(4f) * Main.rand.NextFloat(0.5f, 1f));
            }

            Wind.AI(Projectile.Center);
            Projectile.rotation += 0.025f;
            DrawHelper.AnimateTopToBottom(Projectile, 2);
            if (Main.rand.NextBool(6))
            {
                WindStormParticle wsp = WindStormParticle.Spawn(Projectile.Center, Vector2.Zero);
                wsp.parent = Projectile;
            }

            if (Main.rand.NextBool(16))
            {
                WindDebrisParticle wsp = WindDebrisParticle.SpawnInAlphaLayer(Projectile.Center, Vector2.Zero, Color.White);
                wsp.parent = Projectile;
            }
            if (Timer % 24 == 0)
            {
                SwirlParticle wsp = SwirlParticle.Spawn(Projectile.Center, Vector2.Zero, Color.White);
                wsp.gravity = 0;
                wsp.color *= 0.5f;
            }

            if (Main.rand.NextBool(8))
            {
                DustParticle wsp = DustParticle.Spawn(Projectile.Center, -Projectile.velocity.RotatedByRandom(1f) * Main.rand.NextFloat(0.5f, 1f));
                wsp.gravity = 0f;
                wsp.Scale *= 0.5f;
            }

            if (Main.rand.NextBool(8))
            {
                var sp = SmokeParticle.SpawnInAlphaLayer(Projectile.Center, Vector2.Zero, Color.Tan);
                sp.initialColor = Color.DarkGray;
                sp.Scale *= 0.3f;
            }
        }

        protected virtual void DrawWindTrail()
        {
            var shader = MagicRadianceShader.Instance;
            shader.PrimaryTexture = TrailRegistry.GlowTrail;
            shader.NoiseTexture = TrailRegistry.CloudsSmall;
            shader.OutlineTexture = TrailRegistry.DottedTrailOutline;

            Color primaryColor = Color.LightGray;
            primaryColor.A = 0;
            shader.PrimaryColor = primaryColor;
            shader.NoiseColor = primaryColor;
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
            result.A /= 2;

            return result;
        }

        public virtual float StripWidth(float progressOnStrip)
        {
            return MathHelper.Lerp(26f, 48, Utils.GetLerpValue(0f, 0.2f, progressOnStrip, clamped: true)) * Utils.GetLerpValue(0f, 0.07f, progressOnStrip, clamped: true);
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
            /*
            SpriteBatch spriteBatch = Main.spriteBatch;
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Rectangle frame = Projectile.Frame();
            Vector2 drawOrigin = frame.Size() / 2f;
            Color drawColor = Color.White.MultiplyRGB(lightColor);
            drawColor.A = 0;
            drawColor *= 0.35f;

            float drawRotation = Projectile.rotation;
            float drawScale = 0.5f * DrawScale;
            spriteBatch.Draw(texture, drawPos, frame, drawColor, drawRotation, drawOrigin, drawScale, SpriteEffects.None, layerDepth: 0);*/
        }

        protected virtual void DrawBackShadow(Vector2 screenPos, Color spriteColor, ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            Texture2D texture = TextureRegistry.BasicGlow.Value;
            Vector2 shadowDrawPos = Projectile.Center - screenPos;
            Vector2 shadowDrawOrigin = texture.Size() / 2f;
            float drawScale = DrawScale;
            drawScale *= MathHelper.SmoothStep(3f, 0.75f, EasingFunction.InOutSine(Timer / 30f));
            Color drawColor = spriteColor.MultiplyRGB(lightColor) * ShadowScale;
            drawColor *= MathHelper.SmoothStep(0f, 1f, EasingFunction.InOutSine(Timer / 30f));
            spriteBatch.Draw(texture, shadowDrawPos, null, drawColor, 0, shadowDrawOrigin, drawScale, SpriteEffects.None, layerDepth: 0);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawBackShadow(Main.screenPosition, Color.Black, ref lightColor);
            DrawWindSlashes(ref lightColor);
            DrawFaintGlow();
            DrawSpiralVortex();

            return false;
        }

        private void DrawFaintGlow()
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            Texture2D texture = AssetManager.GlowMask.SimpleGlowCircle.Value;
            Vector2 shadowDrawPos = Projectile.Center - Main.screenPosition;
            Vector2 shadowDrawOrigin = texture.Size() / 2f;
            float drawScale = DrawScale;
            Color drawColor = Color.DarkGray;
            drawColor.A = 0;
            spriteBatch.Draw(texture, shadowDrawPos, null, drawColor * 0.1f, 0, shadowDrawOrigin, drawScale * 0.4f, SpriteEffects.None, layerDepth: 0);

        }
        private void DrawSpiralVortex()
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            Texture2D texture = AssetManager.GlowMask.SpiralVortex.Value;
            Vector2 shadowDrawPos = Projectile.Center - Main.screenPosition;
            Vector2 shadowDrawOrigin = texture.Size() / 2f;
            float drawScale = DrawScale;
            drawScale *= MathHelper.Lerp(3f, 1f, EasingFunction.InOutSine(Timer / 30f));

            Color drawColor = Color.White;
            drawColor.A = 0;
            drawColor *= MathHelper.Lerp(0f, 1f, EasingFunction.InOutSine(Timer / 30f));
            float rotation = Main.GlobalTimeWrappedHourly * 12;
            spriteBatch.Draw(texture, shadowDrawPos, null, drawColor * 0.1f, rotation, shadowDrawOrigin, drawScale * 0.7f, SpriteEffects.None, layerDepth: 0);
            spriteBatch.Draw(texture, shadowDrawPos, null, drawColor * 0.2f, rotation * 1.5f, shadowDrawOrigin, drawScale * 0.15f, SpriteEffects.None, layerDepth: 0);
        }
        public void DrawOutlines(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            /*
            Vector2 v = Vector2.UnitY * 2;
            Vector2 h = Vector2.UnitX * 2;
            DrawBackShadow(screenPos + v, Color.Red, ref lightColor);
            DrawBackShadow(screenPos - v, Color.Red, ref lightColor);
            DrawBackShadow(screenPos + h, Color.Red, ref lightColor);
            DrawBackShadow(screenPos - h, Color.Red, ref lightColor);*/
        }
    }
}
