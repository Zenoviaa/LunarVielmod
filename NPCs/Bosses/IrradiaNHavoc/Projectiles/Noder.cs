using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.IrradiaNHavoc.Projectiles
{
    public class Noder : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Verlia's Swords Dance");
            Main.projFrames[Projectile.type] = 30;
        }

        public override void SetDefaults()
        {
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.width = 28;
            Projectile.height = 28;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 30;
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
                    ModContent.ProjectileType<IrradiaCombustionBoom>(), 0, Projectile.knockBack, Projectile.owner);

            SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Built"), Projectile.position);

            if (StellaMultiplayer.IsHost)
            {
                NPC.NewNPC(Projectile.GetSource_FromThis(), (int)Projectile.Center.X, (int)Projectile.Center.Y, ModContent.NPCType<IrradiaSpikeBox>());
            }


         
        }

        public override void AI()
        {

            Vector3 RGB = new(0.89f, 2.53f, 2.55f);
            // The multiplication here wasn't doing anything
            Lighting.AddLight(Projectile.position, RGB.X, RGB.Y, RGB.Z);

            

            if (++Projectile.frameCounter >= 1)
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