using Microsoft.Xna.Framework;
using Stellamod.Common.Bases;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.UI.Systems;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Safunai.Kaevine
{
    public class KaevineProj : BaseSafunaiProjectile
    {
        protected override Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.Transparent, Color.GreenYellow, completionRatio);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Poisoned, 120);
            int count = 24;
            float degreesPer = 360 / (float)count;
            for (int k = 0; k < count; k++)
            {
                float degrees = k * degreesPer;
                Vector2 direction = Vector2.One.RotatedBy(MathHelper.ToRadians(degrees));
                Vector2 vel = direction * 4;
                Dust.NewDust(target.Center, 0, 0, DustID.Venom, vel.X, vel.Y);
            }

            ShakeModSystem.Shake = 4;
            SoundEngine.PlaySound(SoundID.DD2_WitherBeastDeath, target.position);
            SoundEngine.PlaySound(SoundID.Item17, target.position);
            if (Slam)
            {
                for (int i = 0; i < Main.rand.Next(1, 4); i++)
                {
                    Vector2 stingerVelocity = Vector2.One.RotatedBy(MathHelper.ToRadians(Main.rand.Next(0, 360))) * Main.rand.NextFloat(6, 8);
                    int p = Projectile.NewProjectile(Projectile.GetSource_FromThis(),
                        target.Center.X, target.Center.Y,
                        stingerVelocity.X, stingerVelocity.Y,
                        ProjectileID.QueenBeeStinger, Projectile.damage * 3, 0f, Projectile.owner);

                    Projectile stingerProj = Main.projectile[p];
                    stingerProj.hostile = false;
                    stingerProj.friendly = true;
                    stingerProj.usesLocalNPCImmunity = true;
                    stingerProj.penetrate = -1;
                    stingerProj.localNPCHitCooldown = -1;
                    stingerProj.netUpdate = true;
                }
                FXUtil.GlowCircleBoom(target.Center,
                    innerColor: Color.White,
                    glowColor: Color.Yellow,
                    outerGlowColor: Color.Red, duration: 15, baseSize: 0.12f);


                Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(Projectile.Center, 1024f, 32f);
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Vinger2"), target.position);

                for (int i = 0; i < 14; i++)
                {
                    Dust.NewDustPerfect(target.Center, ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.Green, 1f).noGravity = true;
                }
                for (int i = 0; i < 14; i++)
                {
                    Dust.NewDustPerfect(target.Center, ModContent.DustType<TSmokeDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.DarkOliveGreen, 1f).noGravity = true;
                }
                FXUtil.GlowCircleBoom(target.Center,
                    innerColor: Color.Yellow,
                    glowColor: Color.Green,
                    outerGlowColor: Color.Black, duration: 15, baseSize: 0.24f);

            }
            else
            {
                FXUtil.GlowCircleBoom(target.Center,
                    innerColor: Color.Yellow,
                    glowColor: Color.Green,
                    outerGlowColor: Color.Black, duration: 15, baseSize: 0.12f);


                for (int i = 0; i < Main.rand.Next(1, 4); i++)
                {
                    Vector2 stingerVelocity = Vector2.One.RotatedBy(MathHelper.ToRadians(Main.rand.Next(0, 360))) * Main.rand.NextFloat(4, 6);
                    int p = Projectile.NewProjectile(Projectile.GetSource_FromThis(),
                        target.Center.X, target.Center.Y,
                        stingerVelocity.X, stingerVelocity.Y,
                        ProjectileID.QueenBeeStinger, Projectile.damage * 2, 0f, Projectile.owner);

                    Projectile stingerProj = Main.projectile[p];
                    stingerProj.hostile = false;
                    stingerProj.friendly = true;
                    stingerProj.usesLocalNPCImmunity = true;
                    stingerProj.penetrate = -1;
                    stingerProj.localNPCHitCooldown = -1;
                    stingerProj.netUpdate = true;

                }
            }

        }


    }
}
