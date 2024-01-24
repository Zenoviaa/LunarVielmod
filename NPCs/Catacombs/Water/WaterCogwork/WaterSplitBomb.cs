using Microsoft.Xna.Framework;
using ParticleLibrary;
using Stellamod.Dusts;
using Stellamod.Particles;
using Stellamod.Projectiles;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Catacombs.Water.WaterCogwork
{
    internal class WaterSplitBomb : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 9;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.timeLeft = 300;
            Projectile.friendly = false;
            Projectile.hostile = true;
        }

        public override void AI()
        {
            Visuals();
        }

        private void Visuals()
        {
            for (int i = 0; i < Main.rand.Next(2, 4); i++)
            {
                Vector2 position = Projectile.Center + Main.rand.NextVector2Circular(4, 4);
                float size = Main.rand.NextFloat(1f, 1.25f);
                ParticleManager.NewParticle(position, Vector2.Zero, ParticleManager.NewInstance<WaterParticle>(),
                    default(Color), size);
            }

            if (Main.rand.NextBool(8))
            {
                Dust.NewDust(Projectile.Center, 2, 2, ModContent.DustType<BubbleDust>());
            }
        }

        public override void OnKill(int timeLeft)
        {
            if (Main.rand.NextBool(2))
            {
                SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/DeathShotBomb"));
            }
            else
            {
                SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/DeathShotBomb2"));
            }

            if(Main.myPlayer == Projectile.owner)
            {
                for (int i = 0; i < Main.rand.Next(8, 12); i++)
                {
                    Vector2 velocity = Vector2.One.RotatedBy(MathHelper.ToRadians(Main.rand.Next(0, 360))) * Main.rand.NextFloat(3, 4);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(),
                        Projectile.Center.X, Projectile.Center.Y,
                        velocity.X, velocity.Y,
                        ModContent.ProjectileType<WaterBolt>(), Projectile.damage, 0f, Projectile.owner);
                }
            }

            int count = 64;
            for (int i = 0; i < count; i++)
            {
                Vector2 speed = Main.rand.NextVector2CircularEdge(5f, 5f);
                ParticleManager.NewParticle(Projectile.Center, speed, ParticleManager.NewInstance<WaterParticle>(),
                    default(Color), 1 / 2f);
            }

            for (int i = 0; i < count; i++)
            {
                Vector2 speed = Main.rand.NextVector2CircularEdge(3f, 3f);
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<BubbleDust>(), speed);
            }
        }
    }
}
