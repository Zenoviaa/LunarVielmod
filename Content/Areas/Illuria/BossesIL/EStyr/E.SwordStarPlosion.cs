using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Core;
using Stellamod.Core.Shaders;
using Stellamod.Helpers;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Illuria.BossesIL.EStyr
{
    public class DarkStarMini : ScarletProjectile,
        IDrawBlackStar,
        IDrawOutlines
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
        }
        public override void AI()
        {
            base.AI();
        }
        public override bool PreDraw(ref Color lightColor)
        {
            return base.PreDraw(ref lightColor);
        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
        }
        public void DrawBlackStar(SpriteBatch spriteBatch)
        {
            //   throw new NotImplementedException();
        }

        public void DrawOutlines(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            //   throw new NotImplementedException();
        }
    }
    public class ExplodingStarBuster : ScarletProjectile,
        IDrawBlackStar,
        IDrawOutlines
    {
        private float _scale;
        private ref float Timer => ref Projectile.ai[0];
        public override void SetDefaults()
        {
            base.SetDefaults();

            TrailCacheLength = 8;
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 1;
        }
        public override void AI()
        {
            base.AI();
            Timer++;
            if (Timer < 60)
            {
                if (Projectile.velocity.Length() < 20)
                {
                    Projectile.velocity *= 1.065f;
                }
            }
            else
            {
                Projectile.velocity *= 0.9f;
                if (Projectile.velocity.Length() < 2)
                {
                    Projectile.Kill();
                }
            }

            Projectile.rotation = Projectile.velocity.ToRotation();
            float inTime = 15;
            float completionRatio = Timer / inTime;
            float ease = EasingFunction.InOutSine(completionRatio);
            _scale = MathHelper.Lerp(0f, 1f, ease);
        }

        private void DrawSprite(SpriteBatch spriteBatch)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawOrigin = texture.Size() / 2f;
            Vector2 drawCenter = Projectile.Center - Main.screenPosition;
            float rotation = Projectile.rotation;
            float scale = Projectile.scale;
            Vector2 drawScale = new Vector2(1f, MathHelper.Lerp(0.8f, 1f, _scale)) * _scale * 0.85f;
            spriteBatch.Draw(texture, drawCenter, null, Color.White, rotation, drawOrigin, drawScale, SpriteEffects.None, 0);
        }

        private void DrawAfterImages(SpriteBatch spriteBatch)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawOrigin = texture.Size() / 2f;
            for (int i = 0; i < TrailCacheLength; i++)
            {
                float completionRatio = (float)i / (float)TrailCacheLength;

                Vector2 drawCenter = OldCenterPos[i] - Main.screenPosition;
                float rotation = OldCenterRot[i];
                float scale = Projectile.scale;
                Color drawColor = Color.Lerp(Color.White, Color.Transparent, completionRatio);
                drawColor *= 0.3f;
                Vector2 drawScale = new Vector2(1f, MathHelper.Lerp(0.8f, 1f, _scale)) * _scale * 0.85f;
                spriteBatch.Draw(texture, drawCenter, null, drawColor, rotation, drawOrigin, drawScale, SpriteEffects.None, 0);
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {

            return false;
        }

        public void DrawBlackStar(SpriteBatch spriteBatch)
        {
            DrawAfterImages(spriteBatch);
            DrawSprite(spriteBatch);
        }
        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
        }

        public void DrawOutlines(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {

        }
    }
    public partial class E
    {
        /*
         * 
         * Jumps over top of you and charges his sword, 
         * slashing downwards at you with a big slash until it meets half way and a bunch of stars explode
         */

        private void AI_SwordStarPlosionStart()
        {

        }

        private void AI_SwordStarPlosionCharge()
        {

        }

        private void AI_SwordStarPlosionSwing()
        {

        }

        private void AI_SwordStarPlosion_End()
        {

        }
    }
}
