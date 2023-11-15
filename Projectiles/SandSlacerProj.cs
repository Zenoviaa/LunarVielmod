using Microsoft.Xna.Framework;
using ParticleLibrary;
using Stellamod.Particles;
using Stellamod.UI.Systems;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Projectiles
{
    public class SandSlacerProj : ModProjectile
    {
        public override void SetDefaults()
        {
            
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.height = 56;
            Projectile.width = 28;
            Projectile.friendly = true;
            Projectile.scale = 0.5f;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 5;
            Projectile.timeLeft = 100;    
        }

        public float Timer
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            float rotation = Projectile.rotation;
            Timer++;

            player.RotatedRelativePoint(Projectile.Center);
            Projectile.rotation  -= 0.5f;
            Projectile.velocity *= 1.03f;
            ParticleManager.NewParticle(Projectile.Center, Projectile.velocity * 0, ParticleManager.NewInstance<DustaParticle>(), Color.Purple, 0.4f, Projectile.whoAmI);

            if (Timer == 1)
            {
                ShakeModSystem.Shake = 1;
            }
            if (Timer < 30)
            {
                if (Main.mouseLeft)
                {
                    Projectile.velocity = Projectile.DirectionTo(Main.MouseWorld) * Projectile.Distance(Main.MouseWorld) / 12;

                }

                player.heldProj = Projectile.whoAmI;
                player.ChangeDir(Projectile.velocity.X < 0 ? -1 : 1);
                player.itemTime = 10;
                player.itemAnimation = 10;
                player.itemRotation = rotation * player.direction;
            }

            Vector3 RGB = new(2.55f, 2.55f, 0.94f);
            // The multiplication here wasn't doing anything
            Lighting.AddLight(Projectile.Center, RGB.X, RGB.Y, RGB.Z);

            //Projectile.netUpdate = true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
       




        }
    }
}

