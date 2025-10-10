using Microsoft.Xna.Framework;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Projectiles.IgniterExplosions;
using Stellamod.UI.Systems;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Magic
{
    public class Poyashot : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[0];
        private ref float AnotherTimer => ref Projectile.ai[1];
        private ref float Timer2 => ref Projectile.ai[2];
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            Projectile.damage = 12;
            Projectile.width = 12;
            Projectile.height = 24;
            Projectile.light = 1.5f;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.ownerHitCheck = true;
            Projectile.timeLeft = 360;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void AI()
        {

            Timer++;
            if (Timer == 3)
            {
                if (Main.myPlayer == Projectile.owner)
                {
                    float speedXabc = -Projectile.velocity.X * Main.rand.NextFloat(.4f, .7f) + Main.rand.NextFloat(-8f, 8f);
                    float speedYabc = -Projectile.velocity.Y * Main.rand.Next(0, 0) * 0.01f + Main.rand.Next(-20, 21) * 0.0f;

                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X + speedXabc, Projectile.position.Y + speedYabc, speedXabc * 0, speedYabc * 0,
                        ModContent.ProjectileType<AlcaricMushBoom2>(), Projectile.damage * 1, 0f, Projectile.owner, 0f, 0f);
                }

                Timer = 0;
            }


            if (Timer2 < 60)
            {
                if (Projectile.alpha >= 10)
                {
                    Projectile.alpha -= 10;
                }

                AnotherTimer++;
                if (AnotherTimer == 1)
                {
                    Projectile.alpha = 255;
                    if (Main.myPlayer == Projectile.owner)
                    {
                        Projectile.position.X = Main.rand.NextFloat(Projectile.position.X - 120, Projectile.position.X + 120);
                        Projectile.position.Y = Main.rand.NextFloat(Projectile.position.Y - 120, Projectile.position.Y + 120);
                        Projectile.netUpdate = true;
                    }
                }
                if (AnotherTimer == 2)
                {
                    Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f + 3.14f;
                    Projectile.velocity = -Projectile.velocity;
                }
                if (AnotherTimer >= 0 && AnotherTimer <= 20)
                {
                    Projectile.velocity *= .86f;

                }
                if (AnotherTimer == 60)
                {
                    Projectile.tileCollide = true;
                }
                if (AnotherTimer == 20)
                {
                    Projectile.velocity = -Projectile.velocity;
                }
                if (AnotherTimer >= 21 && AnotherTimer <= 60)
                {
                    Projectile.velocity /= .90f;
                }
            }

            if (Timer2 < 1)
            {

                for (int j = 0; j < 2; j++)
                {
                    Vector2 vector2 = Vector2.UnitX * -Projectile.width / 2f;
                    vector2 += -Utils.RotatedBy(Vector2.UnitY, (j * 3.141591734f / 6f), default(Vector2)) * new Vector2(8f, 16f);
                    vector2 = Utils.RotatedBy(vector2, (Projectile.rotation - 1.57079637f), default(Vector2));
                    int num8 = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, ModContent.DustType<GlyphDust>(), Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
                    Main.dust[num8].scale = 1.3f;
                    Main.dust[num8].noGravity = true;
                    Main.dust[num8].position = Projectile.Center + vector2;
                    Main.dust[num8].velocity = Projectile.velocity * 0.1f;
                    Main.dust[num8].noLight = true;
                    Main.dust[num8].color = Color.Purple;
                    Main.dust[num8].velocity = Vector2.Normalize(Projectile.Center - Projectile.velocity * 3f - Main.dust[num8].position) * 1.25f;
                }

            }


            // Trying to find NPC closest to the projectile
            NPC nearest = ProjectileHelper.FindNearestEnemy(Projectile.position, 1024);
            if (nearest != null)
            {
                Projectile.velocity = ProjectileHelper.SimpleHomingVelocity(Projectile, nearest.Center, 3);
            }

            // If found, change the velocity of the projectile and turn it in the direction of the target
            // Use the SafeNormalize extension method to avoid NaNs returned by Vector2.Normalize when the vector is zero

            Projectile.rotation = Projectile.velocity.ToRotation();
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            FXUtil.GlowCircleBoom(target.Center,
                  innerColor: Color.LightPink,
                  glowColor: Color.Pink,
                  outerGlowColor: Color.Purple, duration: 25, baseSize: 0.12f);

            SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Vinger"), target.position);
            ShakeModSystem.Shake = 4;
            for (int i = 0; i < 4; i++)
            {
                Dust.NewDustPerfect(target.Center, ModContent.DustType<LumiDust>(), (Vector2.One * Main.rand.Next(1, 3)).RotatedByRandom(19.0), 170, Color.Purple, 1f).noGravity = true;
            }
            for (int i = 0; i < 4; i++)
            {
                Dust.NewDustPerfect(target.Center, ModContent.DustType<TSmokeDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.Black, 0.5f).noGravity = true;
            }
        }
    }
}
