using Microsoft.Xna.Framework;
using Stellamod.Common.Bases;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.UI.Systems;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Safunai.Parendine
{
    public class ParendineProj : BaseSafunaiProjectile
    {
        protected override Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.Transparent, Color.Blue, completionRatio);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {


            SoundEngine.PlaySound(SoundID.DD2_WitherBeastDeath);
            float speedX = Projectile.velocity.X * Main.rand.NextFloat(.2f, .3f) + Main.rand.NextFloat(-4f, 4f);
            float speedY = Projectile.velocity.Y * Main.rand.NextFloat(.2f, .3f) * 0.01f;
            if (Slam)
            {
                Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(Projectile.Center, 1024f, 32f);
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Parendine"));
                for (int i = 0; i < 14; i++)
                {
                    Dust.NewDustPerfect(target.Center, ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.DeepSkyBlue, 1f).noGravity = true;
                }
                for (int i = 0; i < 14; i++)
                {
                    Dust.NewDustPerfect(target.Center, ModContent.DustType<TSmokeDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.LightSkyBlue, 1f).noGravity = true;
                }


                FXUtil.GlowCircleBoom(target.Center,
                    innerColor: Color.White,
                    glowColor: Color.LightBlue,
                    outerGlowColor: Color.Blue, duration: 15, baseSize: 0.24f);
            }
            else
            {
                FXUtil.GlowCircleBoom(target.Center,
                    innerColor: Color.Cyan,
                    glowColor: Color.LightBlue,
                    outerGlowColor: Color.Blue, duration: 15, baseSize: 0.12f);
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Parendine2"));
                ShakeModSystem.Shake = 4;
                for (int i = 0; i < 8; i++)
                {
                    Dust.NewDustPerfect(target.Center, ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.Next(1, 3)).RotatedByRandom(19.0), 0, Color.DeepSkyBlue, 0.5f).noGravity = true;
                }
                for (int i = 0; i < 4; i++)
                {
                    Dust.NewDustPerfect(target.Center, ModContent.DustType<TSmokeDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.AliceBlue, 0.5f).noGravity = true;
                }
            }
        }
    }
}
