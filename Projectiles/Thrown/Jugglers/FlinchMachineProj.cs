using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Stellamod.Common.Bases;
using Stellamod.Dusts;
using Stellamod.Gores;
using Stellamod.Helpers;
using Stellamod.Particles;
using Stellamod.Trails;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Thrown.Jugglers
{
    internal class FlinchMachineProj : BaseJugglerProjectile
    {
        private ref float HitCount => ref Projectile.ai[2];
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 16;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = Projectile.height = 64;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 600;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);

            HitCount++;
            bool extremeHitEffect = false;
            if (Juggler.combo >= 5)
            {
                if(HitCount >= 3)
                {
                    HitCount = 0;
                    extremeHitEffect = true;
                }
            }

            float catchCount = Juggler.combo;
            float pitch = MathHelper.Clamp(catchCount * 0.05f, 0f, 1f);
            if (extremeHitEffect)
            {
                SoundStyle fanHit2 = SoundRegistry.FanHit2;
                fanHit2.PitchVariance = 0.1f;
                SoundEngine.PlaySound(fanHit2, Projectile.position);
                Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(Projectile.position, 2048, 64);
                target.SimpleStrikeNPC(Projectile.damage * 5, hit.HitDirection, damageType: Projectile.DamageType);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center, Vector2.Zero, 
                    ModContent.ProjectileType<FlinchMachineExplosionProj>(), Projectile.damage, Projectile.knockBack, Projectile.owner);

                FXUtil.GlowCircleBoom(target.Center,
                     innerColor: Color.White,
                     glowColor: Color.Black,
                     outerGlowColor: Color.Black, duration: 25, baseSize: 0.4f);

                for (int i = 0; i < 7; i++)
                {
                    Gore.NewGore(Projectile.GetSource_FromThis(), target.position, Projectile.velocity.RotatedByRandom(MathHelper.TwoPi), GoreHelper.Fan1);
                    Gore.NewGore(Projectile.GetSource_FromThis(), target.position, Projectile.velocity.RotatedByRandom(MathHelper.TwoPi), GoreHelper.Fan2);
                    Gore.NewGore(Projectile.GetSource_FromThis(), target.position, Projectile.velocity.RotatedByRandom(MathHelper.TwoPi), GoreHelper.Fan3);
                }

                for (int i = 0; i < 16; i++)
                {
                    //Get a random velocity
                    Vector2 velocity = Main.rand.NextVector2CircularEdge(16, 16);

                    //Get a random
                    float randScale = Main.rand.NextFloat(0.5f, 1.5f);
                                    }
            }
            else
            {
                SoundStyle fanHit = SoundRegistry.FanHit1;
                fanHit.Pitch = pitch;
                fanHit.PitchVariance = 0.1f;
                fanHit.Volume = 0.85f;
                SoundEngine.PlaySound(fanHit, Projectile.position);

                for (int i = 0; i < 1; i++)
                {
                    Gore.NewGore(Projectile.GetSource_FromThis(), target.position, Projectile.velocity.RotatedByRandom(MathHelper.TwoPi), GoreHelper.Fan1);
                    Gore.NewGore(Projectile.GetSource_FromThis(), target.position, Projectile.velocity.RotatedByRandom(MathHelper.TwoPi), GoreHelper.Fan2);
                    Gore.NewGore(Projectile.GetSource_FromThis(), target.position, Projectile.velocity.RotatedByRandom(MathHelper.TwoPi), GoreHelper.Fan3);
                }

                for (int i = 0; i < 14; i++)
                {
                    Dust.NewDustPerfect(target.Center, ModContent.DustType<TSmokeDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.LightGray, 1f).noGravity = true;
                }

                for (int i = 0; i < 2; i++)
                {
                    Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Electric);
                }
            }

            for (int i = 0; i < 4; i++)
            {
                //Get a random velocity
                Vector2 velocity = Main.rand.NextVector2Circular(4, 4);

                //Get a random
                float randScale = Main.rand.NextFloat(0.5f, 1.5f);
                            }
        }

    }
}
