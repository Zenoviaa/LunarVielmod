using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using Stellamod.Trails;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Illuria.BossesIL.CrumblingTowerOfIlluria.Projectiles
{
    public class HomingWhiteMoth : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[0];
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
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 120;
        }

        public override void AI()
        {
            base.AI();
            Timer++;


            Player closest = PlayerHelper.FindClosestPlayer(Projectile.position, 2000);
            if(closest != null)
            {
                if(Timer < 30f)
                {
                    Vector2 homingVelocity = ProjectileHelper.SimpleHomingVelocity(Projectile, closest.Center);
                    Projectile.velocity = homingVelocity;
                }
            }

            if(Timer % 8 == 0)
            {
                Dust.NewDustPerfect(Projectile.Center, DustID.GemDiamond, Main.rand.NextVector2Circular(1, 1));
            }

            float inTime = 30f;
            float inRatio = Timer / inTime;
            float ease = EasingFunction.InOutSine(inRatio);
            float inScale = MathHelper.Lerp(0f, 1f, ease);
            Projectile.scale = inScale;
            Projectile.rotation = Projectile.velocity.X * 0.05f;
            DrawHelper.AnimateTopToBottom(Projectile, 4);
        }


        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Rectangle frame = Projectile.Frame();
            for(int i = 0;  i < Projectile.oldPos.Length; i++)
            {
                float f = i;
                float completionRatio = f / (float)Projectile.oldPos.Length;
                Color drawColor = Color.Lerp(Color.White, Color.Transparent, completionRatio);
                Vector2 drawPosition = Projectile.oldPos[i] + Projectile.Size / 2f;
                Vector2 drawOrigin = frame.Size() / 2f;
                drawColor *= 0.2f;
                spriteBatch.Draw(texture, drawPosition, frame, drawColor, Projectile.oldRot[i], drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            }
            return base.PreDraw(ref lightColor);
        }


        public override void PostDraw(Color lightColor)
        {
            base.PostDraw(lightColor);
            SpriteBatch spriteBatch = Main.spriteBatch;
            Texture2D glowingBallTexture = ModContent.Request<Texture2D>(TextureRegistry.ZuiEffect).Value;
            Vector2 drawOrigin = glowingBallTexture.Size() / 2f;
            Vector2 drawCenter = Projectile.Center - Main.screenPosition;
            Color glowColor = Color.White;
            glowColor.A = 0;
            spriteBatch.Draw(glowingBallTexture, drawCenter, null, glowColor, Projectile.rotation, drawOrigin, Projectile.scale * 0.3f, SpriteEffects.None, 0);
        }
    }
}
