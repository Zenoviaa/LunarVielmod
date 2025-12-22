using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using Terraria;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Stellamod.NPCs.Bosses.GothiviaTheSun.GOS.Projectiles
{
    public class BlinkingStar : ModProjectile
    {

        public override string Texture => TextureRegistry.EmptyTexture;
        //texture
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Rune Spawn Effect");
        }
        public float Rot;
        public override void SetDefaults()
        {
            Projectile.aiStyle = 0;
            Projectile.alpha = 255;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.penetrate = 10;
            Projectile.timeLeft = 100;
            Projectile.height = 256;
            Projectile.width = 256;
            Projectile.extraUpdates = 1;
        }

        private float alphaCounter = 5;
        public override void AI()
        {


            Projectile.rotation = Projectile.velocity.ToRotation();
            NPC npc = Main.npc[(int)Projectile.ai[1]];
            Projectile.Center = npc.Center;





            Projectile.rotation -= 0.4f;

            alphaCounter -= 0.1f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D4 = Request<Texture2D>("Stellamod/Assets/NoiseTextures/Extra_63").Value;
            Vector2 drawOrigin = texture2D4.Size() / 2f;
            Color color = Color.White;
          //  color *= alphaCounter;
            color.A = 0;


            float scale = 0.4f * (alphaCounter + 0.2f);
            if (scale < 0)
                scale = 0;
            Main.spriteBatch.Draw(texture2D4, Projectile.Center - Main.screenPosition, null, color, Projectile.rotation, drawOrigin, scale, SpriteEffects.None, 0f);
         
            return true;
        }
    }
}