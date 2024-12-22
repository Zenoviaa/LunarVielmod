using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Trails;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.IgniterExplosions
{
    public class ScatterBoom : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("FrostShotIN");
            Main.projFrames[Projectile.type] = 6;
        }

        public override void SetDefaults()
        {
            Projectile.tileCollide = false;
            Projectile.localNPCHitCooldown = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.friendly = true;
            Projectile.width = 520 / 2;
            Projectile.height = 688 / 2;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 18;
            Projectile.hostile = true;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.position, Color.Red.ToVector3() * 1.5f);
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public override bool PreAI()
        {

            Projectile.tileCollide = false;
            if (++Projectile.frameCounter >= 3)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 6)
                {
                    Projectile.frame = 0;
                }
            }
            return true;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Color drawColor = Color.White;
            //	drawColor = drawColor.MultiplyRGB(lightColor);
            drawColor.A = 0;
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, Projectile.Frame(), drawColor, Projectile.velocity.ToRotation() - MathHelper.PiOver2, Projectile.Frame().Size() / 2f, 1f, SpriteEffects.None, 0);
            return false;
        }


    }

}