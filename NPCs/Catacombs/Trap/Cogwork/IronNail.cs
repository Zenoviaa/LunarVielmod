using Microsoft.Xna.Framework;
using Stellamod.Helpers;
using Stellamod.Projectiles;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Catacombs.Trap.Cogwork
{
    internal class IronNail : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 16;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 58;
            Projectile.height = 16;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.timeLeft = 180;
            Projectile.tileCollide = true;
            Projectile.light = 0.25f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawHelper.DrawAdditiveAfterImage(Projectile, Color.DarkGray, Color.Transparent, ref lightColor);
            return base.PreDraw(ref lightColor);
        }

        public override void AI()
        {
            //Pretty sure projectiles automatically have regular velocity so we don't need to do anything here.
            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public override void OnKill(int timeLeft)
        {
            //Spawn Projectiles
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero,
                ModContent.ProjectileType<NailKaboom>(), 0, 0, Projectile.owner);

            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity * 0.001f,
                ModContent.ProjectileType<IronNailLodged>(), Projectile.damage, 1, Projectile.owner);

            //Effects
            SoundEngine.PlaySound(SoundID.Dig);
            for (int i = 0; i < 64; i++)
            {
                Vector2 speed = Main.rand.NextVector2CircularEdge(2f, 2f);
                var d = Dust.NewDustPerfect(Projectile.Center, DustID.Iron, speed, Scale: 2f);
                d.noGravity = true;
            }
        }
    }
}
