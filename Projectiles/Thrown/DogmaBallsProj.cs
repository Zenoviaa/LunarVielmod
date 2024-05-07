using Microsoft.Xna.Framework;
using ParticleLibrary;
using Stellamod.Buffs;
using Stellamod.Dusts;
using Stellamod.Gores;
using Stellamod.Helpers;
using Stellamod.Particles;
using Stellamod.Projectiles.IgniterExplosions;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Thrown
{
    internal class DogmaBallsProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 16;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 36;
            Projectile.height = 44;
            Projectile.friendly = true;
        }

        public override void AI()
        {
            Projectile.velocity.Y += 0.1f;
            Projectile.rotation += Projectile.velocity.Length() * 0.05f;
            Lighting.AddLight(Projectile.position, Color.White.ToVector3() * 0.78f);
        }


        public override bool PreDraw(ref Color lightColor)
        {
            DrawHelper.DrawAdditiveAfterImage(Projectile, Color.White, Color.Transparent, ref lightColor);
            return base.PreDraw(ref lightColor);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            target.AddBuff(BuffID.Slow, 120);
            if (Main.rand.NextBool(6))
            {
                int sentenceType;
                switch (Main.rand.Next(3))
                {
                    default:
                    case 0:
                        sentenceType = ModContent.ProjectileType<DogmaSentence1>();
                        target.AddBuff(BuffID.Daybreak, 120);
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center, Vector2.Zero,
                            ModContent.ProjectileType<CombustionBoomMini>(), Projectile.damage / 2, 1, Projectile.owner);

                        break;
                    case 1:
                        sentenceType = ModContent.ProjectileType<DogmaSentence2>();
                        target.AddBuff(ModContent.BuffType<FlamesOfIlluria>(), 120);
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center, Vector2.Zero,
                            ModContent.ProjectileType<SiriusBoom>(), Projectile.damage / 2, 1, Projectile.owner);

                        Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(Projectile.position, 1024, 32);
                        break;
                    case 2:
                        sentenceType = ModContent.ProjectileType<DogmaSentence3>();
                        target.AddBuff(ModContent.BuffType<Irradiation>(), 120);
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center, Vector2.Zero,
                            ModContent.ProjectileType<JungleBoom>(), Projectile.damage / 2, 1, Projectile.owner);

                        Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(Projectile.position, 1024, 16);
                        break;
                }

                Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center, -Vector2.UnitY * Main.rand.NextFloat(3f, 7f),
                    sentenceType, 0, 1, Projectile.owner);
            }
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 4; i++)
            {
                Vector2 velocity = Main.rand.NextVector2Circular(8, 8);
                Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center, velocity, GoreHelper.TypePaper);
            }

            for (int i = 0; i < 1; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<TSmokeDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.LightSkyBlue, 1.75f).noGravity = true;
            }

            Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(Projectile.position, 1024, 8);
            for (int i = 0; i < 2; i++)
            {
                //Get a random velocity
                Vector2 velocity = Main.rand.NextVector2Circular(4, 4);

                //Get a randoms
                float randScale = Main.rand.NextFloat(0.5f, 1.5f);
                ParticleManager.NewParticle<StarParticle2>(Projectile.Center, velocity, Color.DarkGoldenrod, randScale);
            }
        }
    }
}
