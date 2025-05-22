using Microsoft.Xna.Framework;
using Stellamod.Dusts;
using Stellamod.Particles;
using Stellamod.Projectiles.IgniterExplosions;
using Stellamod.UI.Systems;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;


namespace Stellamod.Projectiles.Magic
{
    internal class CombusterSparkProj2 : ModProjectile
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
                        (Vector2.One * Main.rand.NextFloat(0.2f, 0.4f)).RotatedByRandom(19.0), 0, Color.Orange, Main.rand.NextFloat(1f, 3f)).noGravity = true;
                }
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/CombusterSnap") with { PitchVariance = 0.15f });
            }

            if (Timer == 45)
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/CombusterReady"));
            }

            if (Timer % 4 == 0)
            {

            }
        }

        public override void OnKill(int timeLeft)
        {
            Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(Projectile.position, 2048, 8);
            SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Kaboom"), Projectile.position);
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero,
                ModContent.ProjectileType<CombustionBoomMini>(), Projectile.damage * 2, Projectile.knockBack, Projectile.owner);

            for (int i = 0; i < Main.rand.Next(3, 6); i++)
            {
                Vector2 velocity = Main.rand.NextVector2Circular(16f, 16f);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity,
                    ProjectileID.WandOfSparkingSpark, Projectile.damage, 0f, Projectile.owner);
            }
        }
    }
}
