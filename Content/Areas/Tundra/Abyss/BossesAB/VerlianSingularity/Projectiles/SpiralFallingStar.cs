using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Common.Shaders.MagicTrails;
using Stellamod.Helpers;
using Stellamod.Trails;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Tundra.Abyss.BossesAB.VerlianSingularity.Projectiles
{
    public class SpiralFallingStar : VSProjectile
    {
        private Color _outlineColor;
        private float _scale;
        public override void SetDefaults()
        {
            base.SetDefaults();
            TrailCacheLength = 16;
            Projectile.extraUpdates = 2;
        }

        public override void AI()
        {
            base.AI();
            Timer++;

            NPC parent = GetParentNPC();
            if (Timer == 1)
            {
                SoundStyle starSingle = AssetRegistry.Sounds.Stars.Starsingle5;
                starSingle.PitchVariance = 0.2f;
                SoundEngine.PlaySound(starSingle, GetParentNPC().position);

                Vector2 velocityToParent = (parent.Center - Projectile.Center);
                velocityToParent = velocityToParent.SafeNormalize(Vector2.Zero);
                Projectile.velocity = velocityToParent;
            }

            if (Projectile.velocity.Length() <= 15)
            {
                Projectile.velocity *= 1.004f;
            }
            float distanceToParent = Vector2.Distance(parent.Center, Projectile.Center);
            if(distanceToParent <= 64)
            {
                Projectile.velocity *= 1.04f;
                Projectile.velocity = ProjectileHelper.SimpleHomingVelocity(Projectile.Center, parent.Center, Projectile.velocity, 16);
            }
            if (distanceToParent <= 32)
            {
                _scale = MathHelper.SmoothStep(0f, 1f, distanceToParent / 32f);
            }
            else
            {
                _scale = MathHelper.Lerp(_scale, 1f, 0.1f);
                Projectile.velocity = ProjectileHelper.SimpleHomingVelocity(Projectile.Center, parent.Center, Projectile.velocity, 1);
            }
            if (distanceToParent <= 16)
            {
                SoundStyle starSingle2 = AssetRegistry.Sounds.Stars.Starsingle1;
                starSingle2.PitchVariance = 0.2f;
                SoundEngine.PlaySound(starSingle2, Projectile.position);
                Projectile.Kill();
            }


        }

        #region Draw Code
        public void DrawOutlines(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            /*
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            float outlineOffset = 2;
            Vector2 left = drawPos + Vector2.UnitX * -outlineOffset;
            Vector2 right = drawPos + Vector2.UnitX * outlineOffset;
            Vector2 up = drawPos + Vector2.UnitY * -outlineOffset;
            Vector2 down = drawPos + Vector2.UnitY * outlineOffset;
            Color drawColor = _outlineColor;
            drawColor.A = 0;

            SpriteEffects spriteEffects = Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Rectangle drawFrame = Projectile.Frame();
            Vector2 drawOrigin = drawFrame.Size() / 2;
            float scale = Projectile.scale * _scale;
            float rotation = Projectile.rotation;

            spriteBatch.Draw(texture, left, drawFrame, drawColor, rotation, drawOrigin, scale, spriteEffects, 0);
            spriteBatch.Draw(texture, right, drawFrame, drawColor, rotation, drawOrigin, scale, spriteEffects, 0);
            spriteBatch.Draw(texture, up, drawFrame, drawColor, rotation, drawOrigin, scale, spriteEffects, 0);
            spriteBatch.Draw(texture, down, drawFrame, drawColor, rotation, drawOrigin, scale, spriteEffects, 0);*/
        }

        private Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.White, Color.LightCyan, completionRatio);
        }

        private float WidthFunction(float completionRatio)
        {
            float w = 5;
            float ew = w / 10;
            float width = w;

            float p = completionRatio / 0.5f;
            float ep = EasingFunction.OutCirc(p);
            float circleWidth = MathHelper.Lerp(0, w, ep);
            float trailWidth = MathHelper.Lerp(width, 0, EasingFunction.OutCirc(completionRatio));
            return MathHelper.Lerp(circleWidth, trailWidth, EasingFunction.OutExpo(completionRatio)) * _scale;
        }

        private void DrawTrail()
        {
            var shader = MagicNormalShader.Instance;
            shader.PrimaryTexture = TrailRegistry.GlowTrail;
            shader.NoiseTexture = TrailRegistry.SpikyTrail1;
            shader.BlendState = BlendState.Additive;
            shader.SamplerState = SamplerState.PointWrap;
            shader.Speed = 0.5f;
            shader.Repeats = 1f;
            TrailDrawer.Draw(Main.spriteBatch, OldCenterPos, ColorFunction, WidthFunction, shader);
        }

        private void DrawMainSprite(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            float outlineOffset = 2;
            Vector2 left = Vector2.UnitX * -outlineOffset;
            Vector2 right = Vector2.UnitX * outlineOffset;
            Vector2 up = Vector2.UnitY * -outlineOffset;
            Vector2 down = Vector2.UnitY * outlineOffset;
            SpriteEffects spriteEffects = Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            SpriteBatch spriteBatch = Main.spriteBatch;
            Rectangle drawFrame = Projectile.Frame();
            Vector2 drawOrigin = drawFrame.Size() / 2;
            float scale = Projectile.scale * _scale * 0.3f;
            float rotation = Projectile.rotation;
            Color drawcolor = Color.Lerp(Color.White, Color.Red, ExtraMath.Osc(0f, 1f, speed: 12)).MultiplyRGB(lightColor);
            drawcolor.A = 0;
            spriteBatch.Draw(texture, drawPos, drawFrame, drawcolor, rotation, drawOrigin, scale, spriteEffects, 0);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawTrail();
            DrawMainSprite(ref lightColor);
            return false;
        }
        #endregion
    }
}
