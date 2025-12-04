using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using Stellamod.Trails;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Ishtar.BossesIS.SanguineSingularity.Projectiles
{
    public class BloodyHallucination : ModProjectile
    {
        private float _alpha;
        private ref float Timer => ref Projectile.ai[0];
        private Player Owner => Main.player[Projectile.owner];
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.projFrames[Type] = 5;
            ProjectileID.Sets.TrailCacheLength[Type] = 15;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.hostile = true;
            Projectile.timeLeft = 180;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
        }

        public override bool CanHitPlayer(Player target)
        {
            return target.whoAmI == Owner.whoAmI;
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            if(Timer % 12 == 0)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Lava, Scale: Main.rand.NextFloat(0.2f, 0.5f));
            }

            Player target = Owner;

            float inAlpha = EasingFunction.InOutSine(Timer / 30f);
            float outAlpha = ((float)Projectile.timeLeft) / 30f;
            _alpha = inAlpha * outAlpha;
            Vector2 targetVelocity = Projectile.velocity.Length() * (target.Center - Projectile.Center).SafeNormalize(Vector2.Zero);
            Projectile.velocity = Vector2.Lerp(Projectile.velocity, targetVelocity, 0.1f);
            DrawHelper.AnimateTopToBottom(Projectile, 4);
        }
        private void DrawAfterImage(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Rectangle frame = Projectile.Frame();
            Vector2 drawOrigin = frame.Size() / 2f;
            float numAfterImages = Projectile.oldPos.Length;
            for(int i = 0; i < numAfterImages; i++)
            {
                float a = i;
                float completionRatio = a / numAfterImages;
                Color afterImageColor = Color.Lerp(Color.White, Color.Transparent, MathHelper.SmoothStep(0f, 1f, completionRatio));
                afterImageColor *= 0.15f;
                afterImageColor *= _alpha;

                Vector2 drawCenter = Projectile.oldPos[i] + Projectile.Size / 2f - screenPos;
                SpriteEffects flip = Projectile.velocity.X < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
                spriteBatch.Draw(texture, drawCenter, frame, afterImageColor, Projectile.rotation, drawOrigin, Projectile.scale, flip, 0);
            }
        }
        private void DrawSprite(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Rectangle frame = Projectile.Frame();
            Vector2 drawOrigin = frame.Size() / 2f;
            Vector2 drawCenter = Projectile.Center - screenPos;
            Color finalColor = Color.White.MultiplyRGB(lightColor);
            finalColor *= _alpha;
            finalColor *= 0.5f;
            SpriteEffects flip = Projectile.velocity.X < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            spriteBatch.Draw(texture, drawCenter, frame, finalColor, Projectile.rotation, drawOrigin, Projectile.scale, flip, 0);
        }


        public override bool PreDraw(ref Color lightColor)
        {
            //This will make it so only the person who owns the projectile can see it
            if(Main.LocalPlayer.whoAmI != Owner.whoAmI)
                return false;
            DrawAfterImage(Main.spriteBatch, Main.screenPosition, lightColor);
            DrawSprite(Main.spriteBatch, Main.screenPosition, lightColor);
            return false;
        }
    }
}
