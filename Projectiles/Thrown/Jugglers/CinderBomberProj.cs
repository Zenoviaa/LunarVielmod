using Microsoft.Xna.Framework;
using Stellamod.Common.Bases;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Projectiles.IgniterExplosions;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Thrown.Jugglers
{
    internal class CinderBomberProj : BaseJugglerProjectile
    {
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            if (Juggler.combo >= 5)
            {
                Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(Projectile.Center, 1024, 4);
                SoundStyle fireBomb = new SoundStyle("Stellamod/Assets/Sounds/StormDragon_Bomb");
                SoundEngine.PlaySound(fireBomb, target.Center);

                Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center, Vector2.Zero,
                    ModContent.ProjectileType<CinderBoom>(), Projectile.damage, Projectile.knockBack, Projectile.owner);

                for (int i = 0; i < 14; i++)
                {
                    Dust.NewDustPerfect(target.Center, ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.Yellow, 1f).noGravity = true;
                }
                for (int i = 0; i < 14; i++)
                {
                    Dust.NewDustPerfect(target.Center, ModContent.DustType<TSmokeDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.Orange, 1f).noGravity = true;
                }

                FXUtil.GlowCircleBoom(target.Center,
                    innerColor: Color.White,
                    glowColor: Color.Yellow,
                    outerGlowColor: Color.Red, duration: 25, baseSize: 0.24f);

                for (int i = 0; i < 32; i++)
                {
                    Vector2 speed = Main.rand.NextVector2CircularEdge(4f, 4f);
                    var d = Dust.NewDustPerfect(target.Center, DustID.Torch, speed * 4, Scale: 1f);
                    d.noGravity = true;
                }
            }
        }
    }
}