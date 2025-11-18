using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Assets;
using Stellamod.Core.Particles;
using Stellamod.Core.Shaders;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Projectiles.IgniterExplosions;
using Stellamod.Trails;
using Stellamod.UI.Systems;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.RoyalCapital.BossesRC.STARBOMBER.Projectiles
{
    public class SteamLaser : ModProjectile
    {
        private float BeamLength;
        private Vector2[] _laserPoints;
        private Vector2[] LaserPoints
        {
            get
            {
                _laserPoints ??= new Vector2[64];
                for(int  i = 0; i < _laserPoints.Length; i++)
                {
                    float f = i;
                    float numPoints = _laserPoints.Length;
                    float completionRatio = f / numPoints;
                    Vector2 point = Vector2.Lerp(Projectile.Center, Projectile.Center + Projectile.velocity, completionRatio);
                    _laserPoints[i] = point;
                }
                return _laserPoints;
            }
        }

        private ref float Timer => ref Projectile.ai[0];
        public override string Texture => TextureRegistry.EmptyTexture;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 1;
            Projectile.height = 1;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 30;
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            if (Timer == 1)
            {
                SoundStyle railgun = AssetRegistry.Sounds.STARBOMBER.STARRAILGUN;
                railgun.PitchVariance = 0.3f;
                SoundEngine.PlaySound(railgun, Projectile.position);

                for (int i = 0; i < 14; i++)
                {
                    Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.Pink, 1f).noGravity = true;
                }

                Vector2 direction = Projectile.velocity.SafeNormalize(Vector2.Zero);
                Vector2 explosionCenter = Projectile.Center + Projectile.velocity;
                Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(explosionCenter, 1024f, 32f);
                if (Main.myPlayer == Projectile.owner)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), explosionCenter, Vector2.Zero, ModContent.ProjectileType<SiriusBoom>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                }


                ShakeModSystem.Shake = 3;
                SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, explosionCenter);
                for (float f = 0; f < 16; f++)
                {
                    Dust.NewDustPerfect(explosionCenter, ModContent.DustType<GlowSparkleDust>(),
                        (Vector2.One * Main.rand.NextFloat(0.2f, 5f)).RotatedByRandom(19.0), 0, Color.Pink, Main.rand.NextFloat(1f, 3f)).noGravity = true;
                }

                SoundStyle morrowExp = new SoundStyle($"Stellamod/Assets/Sounds/MorrowExp");
                morrowExp.PitchVariance = 0.3f;
                SoundEngine.PlaySound(morrowExp, explosionCenter);

                switch (Main.rand.Next(3))
                {
                    case 0:
                        morrowExp = new SoundStyle($"Stellamod/Assets/Sounds/StarFlower1");
                        break;
                    case 1:
                        morrowExp = new SoundStyle($"Stellamod/Assets/Sounds/StarFlower1");
                        break;
                    case 2:
                        morrowExp = new SoundStyle($"Stellamod/Assets/Sounds/StarFlower3");
                        break;
                }

                morrowExp.PitchVariance = 0.3f;
                SoundEngine.PlaySound(morrowExp, explosionCenter);

                FXUtil.ShakeCamera(explosionCenter, 1024, 24);
                var b = FXUtil.GlowCircleBoom(explosionCenter,
                    innerColor: Color.White,
                    glowColor: Color.Pink,
                    outerGlowColor: Color.Blue, duration: 25, baseSize: 0.24f);
                b.Scale *= 3f;
                for (float i = 0; i < 8; i++)
                {
                    float progress = i / 4f;
                    float rot = progress * MathHelper.ToRadians(360);
                    rot += Main.rand.NextFloat(-0.5f, 0.5f);
                    Vector2 offset = rot.ToRotationVector2() * 24;
                    var particle = FXUtil.GlowCircleDetailedBoom1(explosionCenter,
                        innerColor: Color.White,
                        glowColor: Color.Pink,
                        outerGlowColor: Color.Blue,
                        baseSize: Main.rand.NextFloat(0.1f, 0.2f),
                        duration: Main.rand.NextFloat(15, 25));
                    particle.Rotation = rot + MathHelper.ToRadians(45);
                }

                for (float f = 0; f < 24; f++)
                {
                    float progress = f / 24f;
                    float rot = progress * MathHelper.ToRadians(360);
                    rot += Main.rand.NextFloat(-0.5f, 0.5f);
                    Vector2 velocity = rot.ToRotationVector2() * Main.rand.NextFloat(4f, 64f);
                    var particle = FXUtil.GlowStretch(explosionCenter, velocity);
                    particle.InnerColor = Color.White;
                    particle.GlowColor = Color.HotPink;
                    particle.OuterGlowColor = Color.Black;
                    particle.Duration = Main.rand.NextFloat(25, 50);
                    particle.BaseSize = Main.rand.NextFloat(0.09f, 0.18f);
                    particle.VectorScale *= 0.5f;

                }
                for (int i = 0; i < 8; i++)
                {
                    Dust.NewDustPerfect(explosionCenter, ModContent.DustType<TSmokeDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.DarkGray, 1f).noGravity = true;
                }
            }
         
      
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return ProjectileHelper.OldPosColliding(LaserPoints, projHitbox, targetHitbox);
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        

        private Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.DarkGray, Color.Black, completionRatio);
        }
        private float WidthFunction(float completionRatio)
        {
            return 48 * EasingFunction.QuadraticBump(Timer / 30f);
        }
        private void DrawLaser()
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            BlackFireShader shader = BlackFireShader.Instance;
            shader.PrimaryTexture = TrailRegistry.WhispyTrail;
            shader.PrimaryTexture2 = TrailRegistry.StarTrail;
            shader.InnerColor = Color.Aqua;
            shader.OuterColor = Color.LightBlue;
            shader.Distortion = 0.1f;
            shader.Time = Timer * 0.07f;
            TrailDrawer.Draw(spriteBatch, LaserPoints, ColorFunction, WidthFunction, shader, Projectile.Size / 2f);

        }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawLaser();
            return false;
        }
    }
}
