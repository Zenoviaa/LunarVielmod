using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Projectiles.IgniterExplosions;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Gun
{
    public class ClockworkBoomer : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[0];
        public override string Texture => TextureRegistry.EmptyTexture;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 64;
            Projectile.height = 64;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.friendly = true;
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            if(Timer == 1)
            {
                FXUtil.ShakeCamera(Projectile.Center, 1024, 2);
                SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, Projectile.position);
                for (float f = 0; f < 60; f++)
                {
                    Dust.NewDustPerfect(Projectile.Center, DustID.Torch,
                        (Vector2.One * Main.rand.NextFloat(0.2f, 5f)).RotatedByRandom(19.0), 0, Color.White, Main.rand.NextFloat(1f, 3f)).noGravity = true;
                }

                SoundStyle morrowExp = new SoundStyle($"Stellamod/Assets/Sounds/MorrowExp");
                morrowExp.PitchVariance = 0.3f;
                SoundEngine.PlaySound(morrowExp, Projectile.position);

                FXUtil.GlowCircleBoom(Projectile.Center,
                    innerColor: Color.White,
                    glowColor: Color.Turquoise,
                    outerGlowColor: Color.Black, duration: 25, baseSize: Main.rand.NextFloat(0.12f, 0.24f));

                for (float i = 0; i < 8; i++)
                {
                    float progress = i / 4f;
                    float rot = progress * MathHelper.ToRadians(360);
                    rot += Main.rand.NextFloat(-0.5f, 0.5f);
                    Vector2 offset = rot.ToRotationVector2() * 24;
                    var particle = FXUtil.GlowCircleDetailedBoom1(Projectile.Center,
                        innerColor: Color.White,
                        glowColor: Color.Aquamarine,
                        outerGlowColor: Color.Black,
                        baseSize: Main.rand.NextFloat(0.1f, 0.2f),
                        duration: Main.rand.NextFloat(15, 25));
                    particle.Rotation = rot + MathHelper.ToRadians(45);
                }

                for (int i = 0; i < 8; i++)
                {
                    Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<TSmokeDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.DarkGray, 1f).noGravity = true;
                }
            }

            if(Timer == 10)
            {
                for (int i = 0; i < 14; i++)
                {
                    Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlyphDust>(), (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(19.0), 0, Color.Teal, 2f).noGravity = true;
                }

                float damage = Projectile.damage;
                damage *= 0.5f;
                if (Main.myPlayer == Projectile.owner)
                {
                    var p = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero,
                        ModContent.ProjectileType<KaBoomMagic2>(), (int)damage, Projectile.knockBack, Projectile.owner);
                }


                int Sound = Main.rand.Next(1, 3);
                if (Sound == 1)
                {
                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/ClockworkCity1"), Projectile.position);
                }
                else
                {
                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/ClockworkCity2"), Projectile.position);
                }
                FXUtil.ShakeCamera(Projectile.Center, 1024, 8);
                Projectile.Kill();
            }
        }


    }
    internal class ClockworkBomb : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[0];
        private ref float Speed => ref Projectile.ai[1];
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 12;
            Projectile.friendly = true;
        }

        public override void AI()
        {
            //Projectile.spriteDirection = Projectile.velocity.X < 0 ? -1 : 1; 
            Timer++;
            if(Timer % 6 == 0)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlyphDust>(), Projectile.velocity * 0.1f, 0, Color.Teal, Main.rand.NextFloat(1f, 1.5f));
                if (Main.rand.NextBool(3))
                {
                    Dust.NewDustPerfect(Projectile.Center, DustID.CopperCoin, Projectile.velocity * 0.1f, 0, Color.White, Main.rand.NextFloat(1f, 1.5f));
                }
            }

            if(Timer <= 2 && Main.myPlayer == Projectile.owner)
            {
                Speed = Main.rand.NextFloat(0.92f, 0.98f);
                Projectile.netUpdate = true;
            }
            Projectile.velocity *= Speed;
            Projectile.rotation = Projectile.velocity.ToRotation();
            if(Projectile.velocity.Length() <= 0.1f && Projectile.active)
            {
                Projectile.Kill();
            }
            Lighting.AddLight(Projectile.Center, Color.Teal.ToVector3() * 1.75f * Main.essScale);
        }

        public override void PostDraw(Color lightColor)
        {
            base.PostDraw(lightColor);
            Texture2D texture = ModContent.Request<Texture2D>("Stellamod/Effects/Masks/DimLight").Value;
            for (int i = 0; i < 6; i++)
            {
                Color glowColor = Color.Teal;
                glowColor.A = 0;
                Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, glowColor, Projectile.rotation, new Vector2(32, 32), 0.17f * (7 + 0.6f) * VectorHelper.Osc(0f, 1f, offset: i), SpriteEffects.None, 0f);
            }
        }

        public override void OnKill(int timeLeft)
        {
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<ClockworkBoomer>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
        }
    }
}
