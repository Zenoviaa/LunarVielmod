using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Helpers.Mathin;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Govheil
{
    public class GovheilGear : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[0];
        private ref float Direction => ref Projectile.ai[1];
        private ref float Step => ref Projectile.ai[2];
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ProjectileID.Sets.TrailCacheLength[Type] = 16;
            ProjectileID.Sets.TrailingMode[Type] = 2;

        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.tileCollide = true;
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.hostile = true;
            Projectile.timeLeft = 180;
            Projectile.penetrate = -1;
        }
        public override void AI()
        {
            base.AI();

            Timer++;
            if(Timer == 1)
            {

                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/GhostExcalibur1") with { PitchVariance = 0.3f}, Projectile.position);
                for (int i = 0; i < 16; i++)
                {
                    Vector2 speed = Main.rand.NextVector2CircularEdge(4f, 4f);
                    var d = Dust.NewDustPerfect(Projectile.Center, DustID.GemTopaz, speed, Scale: 1.5f);
                    d.noGravity = true;
                }
            }


            Projectile.rotation +=  Projectile.velocity.Length() * MathF.Sign(Projectile.velocity.X) * 0.05f;
            Projectile.velocity *= 1.01f;
            Projectile.velocity.Y += 0.25f;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.velocity = Projectile.velocity.RotatedBy(-0.145f * MathF.Sign(Projectile.velocity.X));

            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            float scale = Easing.InOutSine((float)Projectile.timeLeft / 30f);
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Vector2 origin = new Vector2(texture.Width, texture.Height) * 0.5f;
            SpriteBatch spriteBatch = Main.spriteBatch;
            Vector2 position = Projectile.Center - Main.screenPosition;

            var projectile = Projectile;
            int projFrames = Main.projFrames[projectile.type];
            int frameHeight = texture.Height / projFrames;
            int startY = frameHeight * projectile.frame;

            SpriteEffects direction = projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Rectangle sourceRectangle = new Rectangle(0, startY, texture.Width, frameHeight);
            Vector2 drawOrigin = sourceRectangle.Size() / 2f;
            //drawOrigin.X = projectile.spriteDirection == 1 ? sourceRectangle.Width - offsetX : offsetX;
            for (int k = 0; k < projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = projectile.oldPos[k] - Main.screenPosition + Projectile.Size * 0.5F;// + new Vector2(0f, projectile.gfxOffY);
                Color color = projectile.GetAlpha(Color.Lerp(Color.Brown, Color.Transparent, 1f / projectile.oldPos.Length * k) * (1f - 1f / projectile.oldPos.Length * k));
                color *= 0.45f;
                color.A = 0;
                Main.spriteBatch.Draw(texture, drawPos, sourceRectangle, color, projectile.oldRot[k], drawOrigin, projectile.scale  *scale, direction, 0f);
            }

            spriteBatch.Draw(texture, position, null, lightColor, Projectile.rotation, origin, scale, SpriteEffects.None, 0);
            return false;
            //return base.PreDraw(ref lightColor);
        }
        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            for (int i = 0; i < 14; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<TSmokeDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.DarkGoldenrod, 1f).noGravity = true;
            }
        }
    }
}