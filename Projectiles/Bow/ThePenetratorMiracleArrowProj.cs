using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Stellamod.Helpers;
using ParticleLibrary;
using Stellamod.Particles;
using Stellamod.Trails;

namespace Stellamod.Projectiles.Bow
{
    internal class ThePenetratorMiracleArrowProj : ModProjectile
    {
        internal PrimitiveTrail BeamDrawer;
        ref float Timer => ref Projectile.ai[0];

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 24;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.width = 92;
            Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.extraUpdates = 3;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 360;
            Projectile.penetrate = 3;
        }

        public override void AI()
        {
            Timer++;
            if(Timer % 16 == 0)
            {
                if (Main.rand.NextBool(3))
                {
                    Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.BoneTorch);
                    Vector2 speed = Main.rand.NextVector2Circular(0.5f, 0.5f);
                    ParticleManager.NewParticle(Projectile.Center, speed * 1, ParticleManager.NewInstance<VoidParticle>(), Color.RosyBrown, Main.rand.NextFloat(0.2f, 0.8f));
                }
            }

            Projectile.velocity *= 1.001f;
            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Projectile projectile = Projectile;
            Color startColor = ColorFunctions.MiracleVoid;
            Color endColor = Color.Transparent;

            Texture2D texture = TextureAssets.Projectile[projectile.type].Value;
            int projFrames = Main.projFrames[projectile.type];
            int frameHeight = texture.Height / projFrames;
            int startY = frameHeight * projectile.frame;

            Rectangle sourceRectangle = new Rectangle(0, startY, texture.Width, frameHeight);
            Vector2 drawOrigin = sourceRectangle.Size() / 2f;
            //drawOrigin.X = projectile.spriteDirection == 1 ? sourceRectangle.Width - offsetX : offsetX;
            for (int k = 0; k < projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = projectile.oldPos[k] - Main.screenPosition + drawOrigin;// + new Vector2(0f, projectile.gfxOffY);
                Color color = projectile.GetAlpha(Color.Lerp(startColor, endColor, 1f / projectile.oldPos.Length * k) * (1f - 1f / projectile.oldPos.Length * k));
                Main.spriteBatch.Draw(texture, drawPos, sourceRectangle, color, projectile.oldRot[k], drawOrigin, projectile.scale, SpriteEffects.None, 0f);
            }
            return base.PreDraw(ref lightColor);
        }

        public float WidthFunction(float completionRatio)
        {
            float baseWidth = Projectile.scale * 16;
            return MathHelper.SmoothStep(baseWidth, 3.5f, completionRatio);
        }

        public Color ColorFunction(float completionRatio)
        {
            Color startColor = Color.White;
            Color endColor = Color.Transparent;
            return Color.Lerp(startColor, endColor, completionRatio);
        }
    }
}
