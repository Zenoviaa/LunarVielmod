using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Assets;
using Stellamod.Core;
using Stellamod.Core.Shaders;
using Stellamod.Helpers;
using Stellamod.Trails;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.PunkerTown.BossesPT.PunkerPrime
{
    public class PrimeSawblade : ScarletProjectile,
        IDrawOutlines
    {
        private float _flashAlpha;
        private ref float Timer => ref Projectile.ai[0];
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.projFrames[Type] = 3;
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            TrailCacheLength = 16;
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 300;
            Projectile.extraUpdates = 1;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            base.AI();
            Timer++;

            if(Timer < 30)
            {
                Projectile.velocity *= 0.995f;
            }

            if(Timer >= 60)
            {
                Projectile.velocity *= 0.998f;
            }
            if (Timer % 15 == 0)
            {
                var d = Dust.NewDustPerfect(Projectile.Center, DustID.FireworkFountain_Yellow, Scale: Main.rand.NextFloat(0.5f, 1f));
                d.noGravity = false;
            }

            var closest = PlayerHelper.FindClosestPlayer(Projectile.position, 1000);
            if (Timer == 60)
            {
                SoundStyle mechSaw = AssetRegistry.Sounds.SteamPunking.MechSaw;
                mechSaw.PitchVariance = 0.3f;
                SoundEngine.PlaySound(mechSaw, Projectile.position);
                _flashAlpha = 1f;
            }
            else
            {
                _flashAlpha *= 0.9f;
            }


            if (Timer >= 60 && Timer < 150 && closest != null)
            {
                float distToTarget = Vector2.Distance(Projectile.Center, closest.Center);
                float degreesToRotate = MathHelper.Lerp(1f, 4f, distToTarget / 1000f);
                Vector2 homingVelocity = ProjectileHelper.SimpleHomingVelocity(Projectile, closest.Center, degreesToRotate);
                Projectile.velocity = homingVelocity;
            }
            DrawHelper.AnimateTopToBottom(Projectile, 4);
        }


        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return false;
        }

        private void DrawAfterImage(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Rectangle frame = Projectile.Frame();
            Vector2 drawOrigin = frame.Size() / 2f;
            float length = TrailCacheLength;
            for (int i = 0; i < TrailCacheLength; i++)
            {
                float f = i;
                float completionRatio = f / length;
                Vector2 drawCenter = OldCenterPos[i] - screenPos;
                Color afterImageColor = Color.White;

                float alpha = MathHelper.SmoothStep(1f, 0f, completionRatio);
                afterImageColor *= alpha;
                afterImageColor *= 0.3f;

                spriteBatch.Draw(texture, drawCenter, frame, afterImageColor, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            }
        }

        private void DrawBlade(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Rectangle frame = Projectile.Frame();
            Vector2 drawOrigin = frame.Size() / 2f;
            Color finalColor = Color.White.MultiplyRGB(lightColor);
            spriteBatch.Draw(texture, Projectile.Center - screenPos, frame, finalColor, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);

            if (_flashAlpha > 0)
            {
                Color redColor = Color.Red;
                redColor.A = 0;
                redColor *= _flashAlpha;
                spriteBatch.Draw(texture, Projectile.Center - screenPos, frame, redColor, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            }


        }
        public void DrawOutlines(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            float outlineOffset = 2;
            Vector2 h = Vector2.UnitX * outlineOffset;
            Vector2 v = Vector2.UnitY * outlineOffset;
            DrawBlade(spriteBatch, screenPos + h, Color.Red);
            DrawBlade(spriteBatch, screenPos - h, Color.Red);
            DrawBlade(spriteBatch, screenPos + v, Color.Red);
            DrawBlade(spriteBatch, screenPos - v, Color.Red);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            DrawAfterImage(spriteBatch, Main.screenPosition, lightColor);
            DrawBlade(spriteBatch, Main.screenPosition, lightColor);
            return false;
        }
    }
}
