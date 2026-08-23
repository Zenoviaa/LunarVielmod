using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Common.Shaders.MagicTrails;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Trails;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Tundra.Abyss.BossesAB.VerlianSingularity.Projectiles
{
    public class OrbitingShootingStar : VSProjectile,
        IDrawOutlines
    {
        private Color _outlineColor;
        private float _scale;
        private ref float Direction => ref Projectile.ai[2];
        public override void SetDefaults()
        {
            base.SetDefaults();
            TrailCacheLength = 16;
            Projectile.timeLeft = 400;
            Projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            base.AI();
            Timer++;

            NPC parent = GetParentNPC();
            if (Timer == 1)
            {
                SoundStyle starSingle = AssetRegistry.Sounds.Stars.Starsingle4;
                starSingle.PitchVariance = 0.2f;
                SoundEngine.PlaySound(starSingle, GetParentNPC().position);

                Vector2 velocityToParent = (parent.Center - Projectile.Center);
                velocityToParent = velocityToParent.SafeNormalize(Vector2.Zero);
                velocityToParent = velocityToParent.RotatedBy(MathHelper.ToRadians(90 * Direction));
                Projectile.velocity = velocityToParent;
            }
            
            if(Timer % 16 == 0)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 
                    ModContent.DustType<GlowDust>(), newColor: Color.White, Scale: Main.rand.NextFloat(0.2f, 0.7f));
            }
            if(Timer == 200)
            {
                Projectile.velocity *= 0.2f;
            }
            if(Projectile.velocity.Length() < 12)
            {
                Projectile.velocity *= 1.01f;
                if(Timer >= 200)
                {
                    Projectile.velocity *= 1.02f;
                }
            }

            float distanceToParent = Vector2.Distance(parent.Center, Projectile.Center);
            if (distanceToParent <= 64)
            {
                Projectile.velocity = ProjectileHelper.SimpleHomingVelocity(Projectile.Center, parent.Center, Projectile.velocity, 16);
            }
            if (distanceToParent <= 32)
            {
                _scale = MathHelper.SmoothStep(0f, 1f, distanceToParent / 32f);
            } 
            else
            {
                _scale = MathHelper.Lerp(_scale, 1f, 0.1f);
            }

            if(distanceToParent <= 8)
            {
                Projectile.Kill();
            }


            float degreesToRotate = MathHelper.Lerp(0.5f, 2f, EasingFunction.InExpo(Timer / 240f));
            Projectile.velocity = ProjectileHelper.SimpleHomingVelocity(Projectile.Center, parent.Center, Projectile.velocity, degreesToRotate);
            _outlineColor = Color.Red;
        }
        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
        }

        #region Draw Code
        public void DrawOutlines(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {

        }

        private Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.White, Color.LightCyan, completionRatio);
        }

        private float WidthFunction(float completionRatio)
        {
            float w = 12;
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
            Color drawcolor = Color.White.MultiplyRGB(lightColor);
            drawcolor.A = 0;
            spriteBatch.Draw(texture, drawPos, drawFrame, drawcolor, rotation, drawOrigin, scale, spriteEffects, 0);
            spriteBatch.Draw(texture, drawPos, drawFrame, drawcolor, rotation, drawOrigin, scale * 0.5f * ExtraMath.Osc(0.9f, 1f, speed: 16), spriteEffects, 0);
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
