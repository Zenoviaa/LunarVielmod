using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.IrradiaNHavoc.Projectiles
{
    public class IrradiaBuilds : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Verlia's Swords Dance");
            Main.projFrames[Projectile.type] = 30;
        }

        public override void SetDefaults()
        {
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.width = 38;
            Projectile.height = 36;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 180;
            Projectile.scale = 1f;


        }
        public float Timer
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        public override void OnKill(int timeLeft)
        {
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero,
                    ModContent.ProjectileType<IrradiaCombustionBoom>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
        }

        public override void AI()
        {

            Vector3 RGB = new(0.89f, 2.53f, 2.55f);
            // The multiplication here wasn't doing anything
            Lighting.AddLight(Projectile.position, RGB.X, RGB.Y, RGB.Z);

            NPC npc = Main.npc[(int)Projectile.ai[1]];
            Projectile.Center = npc.Center;

            if (++Projectile.frameCounter >= 2)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 30)
                {
                    Projectile.frame = 0;
                }
            }

        }

        public override bool PreAI()
        {
            Projectile.tileCollide = false;




            return true;


        }
   


    }

}