using Microsoft.Xna.Framework;
using ParticleLibrary;
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
        private ref float ai_Timer => ref Projectile.ai[0];
        private ref float ai_RotationTimer => ref Projectile.ai[1];
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
            ai_Timer++;
            float rotationMulti = 1f - (ai_Timer / 60);
            ai_RotationTimer += rotationMulti * 5;
            Projectile.rotation = MathHelper.ToRadians(ai_RotationTimer);
            if (ai_Timer == 1)
            {
                Player owner = Main.player[Projectile.owner];
                Dust.QuickDustLine(Projectile.Center, owner.Center, 32, Color.Orange);
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/CombusterSnap") with { PitchVariance = 0.15f });
            }

            if (ai_Timer == 45)
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/CombusterReady"));
            }

            if (ai_Timer % 4 == 0)
            {
                float scaleMult = ai_Timer / 60;
                for (int i = 0; i < 6; i++)
                {
                    Vector2 vel = Main.rand.NextVector2Circular(1f, 1f);
                    Particle p = ParticleManager.NewParticle(Projectile.Center, vel, ParticleManager.NewInstance<BurnParticle2>(),
                        Color.OrangeRed, Vector2.One * scaleMult * 1.5f);
                    p.rotation = Projectile.rotation;
                    p.timeLeft = 8;
                }
            }
        }

        public override void OnKill(int timeLeft)
        {
            Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(Projectile.position, 2048, 8);
            SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Kaboom"), Projectile.position);
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero,
                ModContent.ProjectileType<CombustionBoomMini>(), Projectile.damage * 2, Projectile.knockBack, Projectile.owner);
            for (int i = 0; i < 6; i++)
            {
                Vector2 velocity = Main.rand.NextVector2Circular(16f, 16f);
                ParticleManager.NewParticle(Projectile.Center, velocity, ParticleManager.NewInstance<UnderworldParticle1>(),
                    Color.HotPink, Main.rand.NextFloat(0.2f, 0.8f));
            }

            for (int i = 0; i < Main.rand.Next(3, 6); i++)
            {
                Vector2 velocity = Main.rand.NextVector2Circular(16f, 16f);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity,
                    ProjectileID.WandOfSparkingSpark, Projectile.damage, 0f, Projectile.owner);
            }
        }
    }
}
