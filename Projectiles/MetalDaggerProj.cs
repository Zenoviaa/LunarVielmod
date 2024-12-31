using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Projectiles
{
    public class MetalDaggerProj : ModProjectile
    {
        public override void SetDefaults()
        {           
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.height = 36;
            Projectile.width = 36;
            Projectile.friendly = true;
            Projectile.scale = 1f;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.timeLeft = 200;     
        }

        public float Timer
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            //float rotation = Projectile.rotation;
            Timer++;

            player.RotatedRelativePoint(Projectile.Center);
            Projectile.rotation  -= 0.2f;

            Vector3 RGB = new(2.55f, 2.55f, 0.94f);
            // The multiplication here wasn't doing anything
            Lighting.AddLight(Projectile.Center, RGB.X, RGB.Y, RGB.Z);
        }
    }
}

