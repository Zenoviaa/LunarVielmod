using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.GothiviaTheSun.GOS.Projectiles
{
    public class SwirlingKick : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Verlia's Swords Dance");
            Main.projFrames[Projectile.type] = 60;
        }

        public override void SetDefaults()
        {
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.width = 89 / 2;
            Projectile.height = 89 / 2;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 120;
            Projectile.scale = 2f;

        }
        public float Timer
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        public override void AI()
        {

            Vector3 RGB = new(0.89f, 2.53f, 2.55f);
            // The multiplication here wasn't doing anything
            Lighting.AddLight(Projectile.position, RGB.X, RGB.Y, RGB.Z);

            NPC npc = Main.npc[(int)Projectile.ai[1]];
            Projectile.Center = npc.Center;

            if (++Projectile.frameCounter >= 1)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 60)
                {
                    Projectile.frame = 0;
                }
            }
            Projectile.alpha += 2;

        }

        public override bool PreAI()
        {
            Projectile.tileCollide = false;




            return true;


        }


    }

}