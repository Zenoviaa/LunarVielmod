using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Common.Shaders.MagicTrails;
using Stellamod.Helpers;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Tundra.Abyss.BossesAB.VerlianSingularity.Projectiles
{
    public class SlowFallingStar : VSProjectile
    {
        private float _scale;
        public override string Texture => TextureRegistry.EmptyTexture;
        private ref float ZigZagOffsetRadians => ref Projectile.ai[2];
        public override void SetDefaults()
        {
            base.SetDefaults();
            TrailCacheLength = 32;
            Projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            base.AI();

            NPC parent = GetParentNPC();
            Timer++;

 
            float distanceToParent = Vector2.Distance(parent.Center, Projectile.Center);
            if (distanceToParent <= 64)
            {
                Projectile.velocity *= 1.04f;
                Projectile.velocity = ProjectileHelper.SimpleHomingVelocity(Projectile.Center, parent.Center, Projectile.velocity, 16);
            }
            else
            {
                Vector2 vectorToHere = (Projectile.Center - parent.Center) * 0.995f;
                Vector2 rotated = vectorToHere.RotatedBy(MathHelper.ToRadians(-0.5f));
                Vector2 target = parent.Center + rotated;
                Vector2 velocityToRotated = (target - Projectile.Center);
                Projectile.velocity = velocityToRotated;
                Projectile.rotation = Projectile.velocity.ToRotation();
            }
            if (distanceToParent <= 32)
            {
                _scale = MathHelper.SmoothStep(0f, 1f, distanceToParent / 32f);
            }
            else
            {
                _scale = MathHelper.Lerp(_scale, 1f, 0.1f);
            }
            if (distanceToParent <= 16)
            {
                Projectile.Kill();
            }
        }

        private Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.LightCyan, Color.DarkBlue, completionRatio);
        }

        private float WidthFunction(float completionRatio)
        {
            float w = 10;
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

        public override bool PreDraw(ref Color lightColor)
        {
            DrawTrail();
            string texturePath = TextureRegistry.ZuiEffect;
            Texture2D starTexture = ModContent.Request<Texture2D>(texturePath).Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Color drawColor = Color.White;
            drawColor.A = 0;
            Vector2 drawOrigin = starTexture.Size() / 2f;
            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.Draw(starTexture, drawPosition, null, drawColor, Projectile.rotation, drawOrigin, _scale * 0.2f, SpriteEffects.None, 0);
            spriteBatch.Draw(starTexture, drawPosition, null, drawColor, Projectile.rotation, drawOrigin, _scale * 0.05f, SpriteEffects.None, 0);
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            FXUtil.GlowCircleBoom(Projectile.Center, Color.White, Color.Cyan, Color.DarkBlue);
        }
    }
}
