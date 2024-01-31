using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Magic
{
    public class CombusterExplosionProj1 : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("FrostShotIN");
            Main.projFrames[Projectile.type] = 15;
        }

        public override void SetDefaults()
        {
            Projectile.localNPCHitCooldown = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.friendly = true;
            Projectile.width = 128;
            Projectile.height = 128;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 30;
            Projectile.scale = 1f;
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

        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            switch (Main.rand.Next(0, 4))
            {
                case 0:
                    target.AddBuff(BuffID.OnFire3, 120);
                    break;
                case 1:
                    target.AddBuff(BuffID.ShadowFlame, 120);
                    break;
                case 2:
                    target.AddBuff(BuffID.CursedInferno, 120);
                    break;
                case 3:
                    target.AddBuff(BuffID.Daybreak, 60);
                    break;
            }
        }
    }
}