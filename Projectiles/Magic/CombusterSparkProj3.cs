using Microsoft.Xna.Framework;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Projectiles.IgniterExplosions;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Magic
{
    internal class CombusterSparkProj3 : ModProjectile
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
                for (float f = 0; f < 32; f++)
                {
                    float progress = f / 32f;
                    Vector2 pos = Vector2.Lerp(Projectile.Center, owner.Center, progress);
                    Dust.NewDustPerfect(pos, ModContent.DustType<GlyphDust>(), Vector2.Zero, newColor: Color.Red, Scale: Main.rand.NextFloat(0.5f, 1.5f));
                    Dust.NewDustPerfect(pos, DustID.Torch, Vector2.Zero, Scale: Main.rand.NextFloat(0.5f, 1.5f));
                }
                for (float f = 0; f < 7; f++)
                {
                    Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowSparkleDust>(),
                        (Vector2.One * Main.rand.NextFloat(0.2f, 0.4f)).RotatedByRandom(19.0), 0, Color.Red, Main.rand.NextFloat(1f, 3f)).noGravity = true;
                }
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/CombusterSnap") with { PitchVariance = 0.15f });
            }

            if (ai_Timer == 45)
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/CombusterReady"));
            }

            if (ai_Timer % 4 == 0)
            {
                float scaleMult = ai_Timer / 60;
            }
        }

        public override void OnKill(int timeLeft)
        {
            Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(Projectile.position, 2048, 32);
            SoundEngine.PlaySound(SoundRegistry.CombusterBoom, Projectile.position);
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero,
                ModContent.ProjectileType<CombustionBoom>(), Projectile.damage * 8, Projectile.knockBack * 2, Projectile.owner);
            for (float f = 0; f < 64; f++)
            {
                Color glyphColor = Color.Red;
                switch (Main.rand.Next(3))
                {
                    case 0:
                        glyphColor = Color.Red;
                        break;
                    case 1:
                        glyphColor = Color.OrangeRed;
                        break;
                    case 2:
                        glyphColor = Color.Yellow;
                        break;
                }
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlyphDust>(),
                    (Vector2.One * Main.rand.NextFloat(0.2f, 25f)).RotatedByRandom(19.0), 0, glyphColor, Main.rand.NextFloat(1f, 3f)).noGravity = true;
            }

            SoundStyle morrowExp = new SoundStyle($"Stellamod/Assets/Sounds/MorrowExp");
            morrowExp.PitchVariance = 0.3f;
            SoundEngine.PlaySound(morrowExp, Projectile.position);
            for (float i = 0; i < 4; i++)
            {
                float progress = i / 4f;
                float rot = progress * MathHelper.ToRadians(360);
                Vector2 offset = rot.ToRotationVector2() * 24;
                var particle = FXUtil.GlowCircleDetailedBoom1(Projectile.Center,
                    innerColor: Color.White,
                    glowColor: Color.Yellow,
                    outerGlowColor: Color.Red,
                    baseSize: 0.2f);
                particle.Rotation = rot + MathHelper.ToRadians(45);
            }

            for (int i = 0; i < 8; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<TSmokeDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.DarkGray, 1f).noGravity = true;
            }

            for (int i = 0; i < Main.rand.Next(10, 15); i++)
            {
                Vector2 velocity = Main.rand.NextVector2Circular(16f, 16f);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity,
                    ProjectileID.WandOfSparkingSpark, Projectile.damage, 0f, Projectile.owner);
            }

            for (int i = 0; i < Main.rand.Next(3, 7); i++)
            {
                Vector2 velocity = Main.rand.NextVector2Circular(16f, 16f);
                int index = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity,
                    ProjectileID.GreekFire3, Projectile.damage, 0f, Projectile.owner);
                Main.projectile[index].friendly = true;
                Main.projectile[index].hostile = false;
            }
        }
    }
}
