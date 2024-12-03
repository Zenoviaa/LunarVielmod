using Microsoft.Xna.Framework;
using Stellamod.Common.Bases;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Projectiles.IgniterExplosions;
using Stellamod.Projectiles.Slashers.GrailAxe;
using Stellamod.UI.Systems;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Safunai.Rinavine
{
    public class RinavineProj : BaseSafunaiProjectile
    {
        protected override Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.Transparent, Color.LightSeaGreen, completionRatio);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            ShakeModSystem.Shake = 4;
            SoundEngine.PlaySound(new SoundStyle($"{nameof(Stellamod)}/Assets/Sounds/MorrowExp"), target.position);
            float speedX = Projectile.velocity.X * Main.rand.NextFloat(.2f, .3f) + Main.rand.NextFloat(-4f, 4f);
            float speedY = Projectile.velocity.Y * Main.rand.Next(20, 35) * 0.01f + Main.rand.Next(-10, 11) * 0.2f;

            if (Slam)
            {
                
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X + speedX, Projectile.position.Y + speedY, speedX, speedY, ModContent.ProjectileType<GrailShot>(), (int)(Projectile.damage), 0f, Projectile.owner, 0f, 0f);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X + speedX, Projectile.position.Y + speedY, speedX, speedY * 3, ModContent.ProjectileType<GrailShot>(), (int)(Projectile.damage), 0f, Projectile.owner, 0f, 0f);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X + speedX, Projectile.position.Y + speedY, speedX * 0.4f, speedY, ModContent.ProjectileType<GrailShot>(), (int)(Projectile.damage), 0f, Projectile.owner, 0f, 0f);
                Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(Projectile.Center, 1024f, 32f);
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Vinger2"), target.position);
                for (int i = 0; i < 14; i++)
                {
                    Dust.NewDustPerfect(target.Center, ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.LightSeaGreen, 1f).noGravity = true;
                }
                for (int i = 0; i < 14; i++)
                {
                    Dust.NewDustPerfect(target.Center, ModContent.DustType<TSmokeDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.SeaGreen, 1f).noGravity = true;
                }

                FXUtil.GlowCircleBoom(target.Center,
                   innerColor: Color.White,
                   glowColor: Color.SeaGreen,
                   outerGlowColor: Color.DarkBlue, duration: 15, baseSize: 0.24f);
            }
            else
            {
                FXUtil.GlowCircleBoom(target.Center,
                   innerColor: Color.White,
                   glowColor: Color.SeaGreen,
                   outerGlowColor: Color.DarkBlue, duration: 15, baseSize: 0.12f);

                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Vinger"), target.position);
                ShakeModSystem.Shake = 4;
                for (int i = 0; i < 6; i++)
                {
                    Dust.NewDustPerfect(target.Center, ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.LightSeaGreen, 0.5f).noGravity = true;
                }
                for (int i = 0; i < 2; i++)
                {
                    Dust.NewDustPerfect(target.Center, ModContent.DustType<TSmokeDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.DarkSeaGreen, 0.5f).noGravity = true;
                }
            }
        }
    }
}
