using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Buffs.Scorpion;
using Stellamod.Common.ScorpionMountSystem;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Trails;
using Stellamod.UI.Systems;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Scorpions.OreKingdom
{
    internal class OreKingdomScorpionPlayer : ModPlayer
    {
        public override void OnHitNPCWithProj(Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPCWithProj(proj, target, hit, damageDone);
            if(Player.HeldItem.type == ModContent.ItemType<OreKingdomScorpion>() && proj.type == ProjectileID.Bullet)
            {
                if (!target.HasBuff<RoyalPalaceScorpionTagDebuff>())
                {
                    //Spawn Cool Effect Here

                }
                target.AddBuff(ModContent.BuffType<RoyalPalaceScorpionTagDebuff>(), 180);
            }
        }
    }
    internal class OreKingdomScorpionGun : BaseScorpionGun
    {

        public override void SetDefaults()
        {
            base.SetDefaults();
            UseTime = 60;

        }
        protected override void Shoot()
        {
            base.Shoot();
            if (Main.myPlayer == Projectile.owner)
            {
                Vector2 fireVelocity = Projectile.velocity * 10f;
                fireVelocity = fireVelocity.RotatedByRandom(MathHelper.ToRadians(8));
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, fireVelocity,
                ModContent.ProjectileType<GLUX>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            }

            int Sound = Main.rand.Next(1, 3);
            if (Sound == 1)
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/GunBigNew1"), Projectile.position);
            }
            else
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/GunBigNew1"), Projectile.position);
            }

            Vector2 velocity = Projectile.velocity;
            float rot = velocity.ToRotation();
            float spread = 0.4f;

            for (int k = 0; k < 7; k++)
            {
                Vector2 direction = velocity.RotatedByRandom(spread);
                Dust.NewDustPerfect(Projectile.position + velocity * 43, ModContent.DustType<Dusts.GlowDust>(), direction * Main.rand.NextFloat(8), 125, Color.Red, Main.rand.NextFloat(0.2f, 0.5f));
            }
            Dust.NewDustPerfect(Projectile.position + velocity * 43, ModContent.DustType<Dusts.GlowDust>(), new Vector2(0, 0), 125, Color.DarkRed, 1);
        }

    }


     internal class GLUX : ModProjectile,
            IPixelPrimitiveDrawer
     {
            //Don't change the sample points, 3 is good enough
            private const int NumSamplePoints = 3;

            private const float MaxBeamLength = 2400f;

            public float BeamLength;
            public List<Vector2> BeamPoints;
            internal PrimitiveTrail BeamDrawer;

            //No texture for this
            public override string Texture => TextureRegistry.EmptyTexture;

            float Timer;
            public override void SetDefaults()
            {
                Projectile.width = 150;
                Projectile.height = 16;
                Projectile.friendly = true;
                Projectile.hostile = false;
                Projectile.tileCollide = true;
                Projectile.penetrate = -1;
                Projectile.timeLeft = 20;
                Projectile.usesLocalNPCImmunity = true;
                Projectile.localNPCHitCooldown = 10;
                BeamPoints = new List<Vector2>();
            }

            public override void AI()
            {
                float targetBeamLength = PerformBeamHitscan();
                BeamLength = targetBeamLength;
                Timer++;
                if (Timer == 1)
                {
                    Vector2 direction = Projectile.velocity.SafeNormalize(Vector2.Zero);
                    Vector2 explosionCenter = Projectile.Center + direction * BeamLength;

                    for (int i = 0; i < 5; i++)
                    {
                        Dust.NewDustPerfect(explosionCenter, ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.NextFloat(0.00f, 1.00f)).RotatedByRandom(19.0), 0, Color.DarkBlue, 1f).noGravity = true;
                    }
                }
            }

            public override bool OnTileCollide(Vector2 oldVelocity)
            {
                return false;
            }

            public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
            {
                float _ = 0f;
                float width = Projectile.width * 0.8f * 0.4f;
                Vector2 start = Projectile.Center;

                Vector2 direction = Projectile.velocity.SafeNormalize(Vector2.Zero);
                Vector2 end = start + direction * (BeamLength - 80f);
                return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, width, ref _);
            }

            private float PerformBeamHitscan()
            {
                // By default, the hitscan interpolation starts at the Projectile's center.
                // If the host Prism is fully charged, the interpolation starts at the Prism's center instead.
                Vector2 samplingPoint = Projectile.Center;

                // Perform a laser scan to calculate the correct length of the beam.
                // Alternatively, if you want the beam to ignore tiles, just set it to be the max beam length with the following line.
                // return MaxBeamLength;
                float[] laserScanResults = new float[NumSamplePoints];


                Vector2 direction = Projectile.velocity.SafeNormalize(Vector2.Zero);
                Collision.LaserScan(samplingPoint, direction, 0 * Projectile.scale, MaxBeamLength, laserScanResults);
                float averageLengthSample = 0f;
                for (int i = 0; i < laserScanResults.Length; ++i)
                {
                    averageLengthSample += laserScanResults[i];
                }
                averageLengthSample /= NumSamplePoints;
                return averageLengthSample;
            }


            public float WidthFunction(float completionRatio)
            {
                float osc = VectorHelper.Osc(0.5f, 1f);

                float width = (float)Projectile.timeLeft / 20f;
                return (Projectile.width * Projectile.scale) * osc * width * 0.9f;
            }

            public Color ColorFunction(float completionRatio)
            {
                Color color = Color.Lerp(Color.Turquoise, Color.DarkBlue, VectorHelper.Osc(0, 1));
                return color;
            }

            public override bool PreDraw(ref Color lightColor) => false;
            public override bool ShouldUpdatePosition() => false;
            public void DrawPixelPrimitives(SpriteBatch spriteBatch)
            {
                BeamDrawer ??= new PrimitiveTrail(WidthFunction, ColorFunction, null, true, TrailRegistry.LaserShader);

                TrailRegistry.LaserShader.UseColor(Color.Lerp(Color.SpringGreen, Color.AliceBlue, VectorHelper.Osc(0, 1)));
                TrailRegistry.LaserShader.SetShaderTexture(TrailRegistry.WhispyTrail);

                //Put in the points
                //This is just a straight beam that collides with tiles
                BeamPoints.Clear();
                Vector2 direction = Projectile.velocity.SafeNormalize(Vector2.Zero);
                for (int i = 0; i <= 8; i++)
                {
                    BeamPoints.Add(Vector2.Lerp(Projectile.Center, Projectile.Center + direction * BeamLength, i / 8f));
                }

                BeamDrawer.DrawPixelated(BeamPoints, -Main.screenPosition, 32);
                Main.spriteBatch.ExitShaderRegion();
            }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            ShakeModSystem.Shake = 4;
            SoundEngine.PlaySound(new SoundStyle($"{nameof(Stellamod)}/Assets/Sounds/MorrowExp"), target.position);
            float speedX = Projectile.velocity.X * Main.rand.NextFloat(.2f, .3f) + Main.rand.NextFloat(-4f, 4f);
            float speedY = Projectile.velocity.Y * Main.rand.Next(20, 35) * 0.01f + Main.rand.Next(-10, 11) * 0.2f;

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
                   outerGlowColor: Color.DarkBlue, duration: 25, baseSize: 0.24f);
            
 
                FXUtil.GlowCircleBoom(target.Center,
                   innerColor: Color.White,
                   glowColor: Color.SeaGreen,
                   outerGlowColor: Color.DarkBlue, duration: 25, baseSize: 0.12f);

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

