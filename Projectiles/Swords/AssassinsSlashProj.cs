using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Swords
{
    public class AssassinsSlashProj : ModProjectile
    {
        public float VEL = 1;
        public override void SetStaticDefaults()
        {

            Main.projFrames[Projectile.type] = 7;
        }
        public override void SetDefaults()
        {
            Projectile.width = 400;
            Projectile.height = 400;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = 110;
            Projectile.timeLeft = 900;
            Projectile.tileCollide = false;
            Projectile.aiStyle = -1;
        }

        public override bool PreAI()
        {
            Projectile.ai[0]++;
            Projectile.alpha -= 40;
            if (Projectile.alpha < 0)
                Projectile.alpha = 0;
            if (Projectile.ai[0] <= 1)
            {
                int Sound = Main.rand.Next(1, 5);
                if (Sound == 1)
                {
                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/AssassinsSlash"), Projectile.position);
                }
                if (Sound == 2)
                {
                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/AssassinsSlashProj2"), Projectile.position);
                }
                if (Sound == 3)
                {
                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/AssassinsSlashProj3"), Projectile.position);
                }
                if (Sound == 4)
                {
                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/AssassinsSlashProj4"), Projectile.position);
                }

                Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(base.Projectile.Center, 512f, 32f);

                Projectile.rotation = Main.rand.Next(0, 360);
            }
            Projectile.spriteDirection = Projectile.direction;
            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 2)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
                if (Projectile.frame >= 7)
                {
                    Projectile.active = false;
                }


            }
            return true;
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 20; i++)
            {
                Dust.NewDust(Projectile.Center, Projectile.width, Projectile.height, DustID.Dirt, 0, 60, 133);
            }
        }
        public override Color? GetAlpha(Color lightColor) => Color.White;

        public override void AI()
        {
        }

    }
}