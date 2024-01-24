using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.Jack
{
    public class FallingHay : ModProjectile
    {
        public float VEL = 1;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Falling Hay");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 3;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.FrostDaggerfish);
            AIType = ProjectileID.FrostDaggerfish;
            Projectile.friendly = false;
            Projectile.hostile = true;
        }

        public override bool PreAI()
        {
            Projectile.velocity.Y *= 0.98f;
            if (Main.rand.NextBool(29))
            {
                int dust = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.Hay, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].scale = 1.5f;
            }
            Player player;
            if ((player = VectorHelper.GetNearestPlayerDirect(base.Projectile.position, Alive: true)) != null)
            {
                if (Projectile.position.Y + 5 <= player.position.Y)
                {
                    Projectile.tileCollide = false;
                }
                else
                {
                    Projectile.tileCollide = true;
                }
            }
            if (Main.rand.NextBool(29))
            {
                int dust3 = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.Hay, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
                Main.dust[dust3].noGravity = true;
                Main.dust[dust3].scale = 1.5f;
            }
            return true;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.Kill();
            for (int i = 0; i < 30; i++)
            {
                Dust.NewDustPerfect(base.Projectile.Center, 74, (Vector2.One * Main.rand.Next(1, 4)).RotatedByRandom(19.0), 0, default(Color), 1f).noGravity = true;
            }

            for (int i = 0; i < 15; i++)
            {
                Dust.NewDust(Projectile.Center, Projectile.width, Projectile.height, DustID.Hay, 0f, -2f, 0, default(Color), .8f);
                Dust.NewDust(Projectile.Center, Projectile.width, Projectile.height, DustID.Hay, 0f, -2f, 0, default(Color), .8f);
            }

            return false;
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            width = (height = 8);
            fallThrough = base.Projectile.position.Y <= base.Projectile.ai[1];
            return true;
        }
        
        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Grass, Projectile.position);
        }
    }
}