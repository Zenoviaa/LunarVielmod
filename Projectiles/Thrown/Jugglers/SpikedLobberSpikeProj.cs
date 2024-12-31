using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Thrown.Jugglers
{
    internal class SpikedLobberSpikeProj : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.friendly = true;
        }

        public override void AI()
        {
            Projectile.velocity.Y += 0.4f;
            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            int p = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero,
                ModContent.ProjectileType<NailKaboom>(), 0, 0, Projectile.owner);
            Main.projectile[p].scale = 0.5f;
        }
    }
}
