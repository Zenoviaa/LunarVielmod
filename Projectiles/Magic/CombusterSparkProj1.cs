using Microsoft.Xna.Framework;
using Stellamod.Dusts;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Magic
{
    internal class CombusterSparkProj1 : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[0];
        private ref float RotationTimer => ref Projectile.ai[1];
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.timeLeft = 60;
            Projectile.friendly = false;
            Projectile.hostile = false;
        }

        public override void AI()
        {
            Timer++;
            float rotationMulti = 1f - (Timer / 60);
            RotationTimer += rotationMulti * 5;
            Projectile.rotation = MathHelper.ToRadians(RotationTimer);
            if (Timer == 1)
            {
                Player owner = Main.player[Projectile.owner];
                for (float f = 0; f < 32; f++)
                {
                    float progress = f / 32f;
                    Vector2 pos = Vector2.Lerp(Projectile.Center, owner.Center, progress);
                    Dust.NewDustPerfect(pos, DustID.Torch, Vector2.Zero, Scale: Main.rand.NextFloat(0.5f, 1.5f));
                }
                for (float f = 0; f < 7; f++)
                {
                    Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowSparkleDust>(),
                        (Vector2.One * Main.rand.NextFloat(0.2f, 0.4f)).RotatedByRandom(19.0), 0, Color.Yellow, Main.rand.NextFloat(0.5f, 1f)).noGravity = true;
                }

                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/CombusterSnap") with { PitchVariance = 0.15f });
            }

            if (Timer == 45)
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/CombusterReady"));
            }

            if (Timer % 4 == 0)
            {
                float scaleMult = Timer / 60;
            }
        }

        public override void OnKill(int timeLeft)
        {
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero,
                ModContent.ProjectileType<CombusterExplosionProj1>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
        }
    }
}
