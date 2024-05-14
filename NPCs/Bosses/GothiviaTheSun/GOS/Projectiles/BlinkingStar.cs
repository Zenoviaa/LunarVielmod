using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using Terraria;
using Terraria.ID;
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
            Texture2D texture2D4 = Request<Texture2D>("Stellamod/Effects/Masks/Extra_64").Value;
            Main.spriteBatch.Draw(texture2D4, Projectile.Center - Main.screenPosition, null, new Color((int)(85f * alphaCounter), (int)(85f * alphaCounter), (int)(85f * alphaCounter), 0), Projectile.rotation, new Vector2(512, 512), 0.4f * (alphaCounter + 0.2f), SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture2D4, Projectile.Center - Main.screenPosition, null, new Color((int)(55f * alphaCounter), (int)(55f * alphaCounter), (int)(55f * alphaCounter), 0), Projectile.rotation, new Vector2(512, 512), 0.3f * (alphaCounter + 0.2f), SpriteEffects.None, 0f);
            return true;
        }
    }
}