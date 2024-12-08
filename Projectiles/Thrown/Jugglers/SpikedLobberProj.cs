using Microsoft.Xna.Framework;
using Stellamod.Common.Bases;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Thrown.Jugglers
{
    internal class SpikedLobberProj : BaseJugglerProjectile
    {
        public override void AI()
        {
            base.AI();
            if (State == AIState.Catch)
            {
                if (Juggler.combo >= 5 && Timer % 5 == 0 && Timer < 30 && Main.myPlayer == Projectile.owner)
                {
                    //Spikes
                    Vector2 velocity = Main.rand.NextVector2Circular(16, 16);
                    velocity += new Vector2(0, -16);
                    SoundEngine.PlaySound(SoundID.Item108, Projectile.position);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity, ModContent.ProjectileType<SpikedLobberSpikeProj>(),
                        Projectile.damage, Projectile.knockBack, Projectile.owner);
                }
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            if (Juggler.combo >= 5)
            {
                Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(Projectile.Center, 1024, 4);
                SoundStyle fireBomb = new SoundStyle("Stellamod/Assets/Sounds/StormDragon_Bomb");
                SoundEngine.PlaySound(fireBomb, target.Center);

                for (int i = 0; i < 14; i++)
                {
                    Dust.NewDustPerfect(target.Center, ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.White, 1f).noGravity = true;
                }
                for (int i = 0; i < 14; i++)
                {
                    Dust.NewDustPerfect(target.Center, ModContent.DustType<TSmokeDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.DarkGray, 1f).noGravity = true;
                }

                FXUtil.GlowCircleBoom(target.Center,
                         innerColor: Color.White,
                         glowColor: Color.Gray,
                         outerGlowColor: Color.Black, duration: 25, baseSize: 0.24f);
            }
        }
    }
}
