using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Bases;
using Stellamod.Core.Particles;
using Stellamod.Core.Pixelation;
using Stellamod.Core.SwingSystem;
using Stellamod.Helpers;
using Stellamod.Items.Materials;
using Stellamod.UI.Systems;
using Stellamod.Visual.Particles;

using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Melee
{
    public class SkyrageShasher : BaseSwingItemV2
    {
        public override void SetDefaults2()
        {
            base.SetDefaults2();
            Item.damage = 27;
            Item.shoot = ModContent.ProjectileType<SkyrageShasherSlash>();
            staminaProjectileShoot = ModContent.ProjectileType<SkyrageShasherStaminaSlash>();
            staminaCost = 2;
            meleeWeaponType = MeleeWeaponType.Spear;
        }

        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew(mold: ModContent.ItemType<BlankSword>(), material: ModContent.ItemType<PearlescentScrap>());
        }
    }

    public class SkyrageBeam : ModProjectile
    {
        private float BeamLength;
        private Vector2[] _beamPoints;
        private Vector2[] BeamPoints
        {
            get
            {
                _beamPoints ??= new Vector2[32];

                for (int i = 0; i < _beamPoints.Length; i++)
                {
                    float ratio = (float)i / (float)_beamPoints.Length;
                    _beamPoints[i] = Vector2.Lerp(Projectile.Center, Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.Zero) * BeamLength, ratio);
                }
                return _beamPoints;
            }
        }
        public override string Texture => TextureRegistry.EmptyTexture;
        private ref float Timer => ref Projectile.ai[0];

        private float Lifetime => Projectile.ai[1];
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return ProjectileHelper.OldPosColliding(BeamPoints, projHitbox, targetHitbox);
        }

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = (int)(30 / 3f);
            Projectile.timeLeft = 30;
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            Projectile.scale = EasingFunction.QuadraticBump(Timer / Lifetime);
            ShakeModSystem.Shake = MathHelper.SmoothStep(5, 0, Timer / Lifetime);
            float targetBeamLength = ProjectileHelper.PerformBeamHitscan(Projectile.Center, Projectile.velocity, 2000);
            BeamLength = targetBeamLength;

            if(Main.rand.NextBool(8))
            {
                int index = Main.rand.Next(0, BeamPoints.Length);
                Vector2 b = BeamPoints[index];
                var sp = SparkleParticle.Spawn(b, Projectile.velocity.RotatedByRandom(1f) * Main.rand.NextFloat(0.25f, 0.5f));
                sp.Scale *= 0.85f;
                sp.dampening = 0.1f;
                sp.flickering = true;
                sp.innerColor = Color.White;
                sp.outerColor = Color.Blue;
                sp.gravity = 0f;
                sp.fast = true;
            }


            if (Timer == 19)
            {
                for(int i = 0; i < BeamPoints.Length; i++)
                {
                    Vector2 beamPoint = BeamPoints[i];
                    if (Main.rand.NextBool(2))
                    {
                        DustParticleSpawnParams spawnParams = new DustParticleSpawnParams
                        {
                            innerColor = Color.LightCyan,
                            outerColor = Color.DarkBlue,
                            gravity = 0f,
                            scaleRange = new Vector2(1f, 3f)
                        };
                        var dp = DustParticle.Spawn(beamPoint, Projectile.velocity.RotatedByRandom(0.5f) * Main.rand.NextFloat(0.1f, 0.5f), spawnParams);
                        dp.dampening = 0.1f;
                    }
                    if (Main.rand.NextBool(3))
                    {
                        var dp = SparkleParticle.Spawn(beamPoint, Projectile.velocity.RotatedByRandom(1f) * Main.rand.NextFloat(0.25f, 0.5f));
                        dp.Scale *= 0.85f;
                        dp.dampening = 0.1f;
                        dp.flickering = true;
                        dp.innerColor = Color.White;
                        dp.outerColor = Color.Blue;
                        dp.gravity = 0f;
                    }
                }
            }
            if (Timer == 1)
            {
                for (int i = 0; i < BeamPoints.Length; i++)
                {
                    Vector2 beamPoint = BeamPoints[i];
                    if (Main.rand.NextBool(6))
                    {
                        GlowDonutParticle glowDonutParticle = LegacyParticle.NewParticle<GlowDonutParticle>(beamPoint, Projectile.velocity.SafeNormalize(Vector2.Zero).RotatedBy(1.5f) * Main.rand.NextFloat(0.2f, 0.6f), Scale: 0.4f);
                        glowDonutParticle.rotOffset = MathHelper.PiOver2;

                        float ratio = (float)i / (float)BeamPoints.Length;
                        glowDonutParticle.Scale *= MathHelper.Lerp(1f, 0.25f, ratio);
            
                    }
                }
                SoundStyle sound = AssetRegistry.Sounds.SteamPunking.DescendingBoom;
                sound.PitchVariance = 0.3f;
                sound.Volume = 0.5f;
                SoundEngine.PlaySound(sound, Projectile.position);

                Vector2 direction = Projectile.velocity.SafeNormalize(Vector2.Zero);
                Vector2 explosionCenter = Projectile.Center + direction * BeamLength;
                for (float f = 0; f < 16; f++)
                {
                    Vector2 initialVelocity = -Projectile.velocity * 4;
                    initialVelocity = initialVelocity.RotatedByRandom(MathHelper.ToRadians(60));
                    initialVelocity *= Main.rand.NextFloat(0.5f, 1f);

                    DustParticle dustParticle = Particle<DustParticle>.Spawn(explosionCenter, initialVelocity, Color.White, Scale: Main.rand.NextFloat(0.6f, 2f));
                    dustParticle.innerColor = Color.SkyBlue;
                    dustParticle.outerColor = Color.Violet;
                }

                for (float f = 0; f < 6; f++)
                {
                    Vector2 initialVelocity = -Vector2.UnitY;
                    initialVelocity *= 12;
                    initialVelocity = initialVelocity.RotatedByRandom(MathHelper.ToRadians(360));
                    initialVelocity *= Main.rand.NextFloat(0.5f, 1f);

                    SparkParticle dustParticle = LegacyParticle.NewParticle<SparkParticle>(explosionCenter, initialVelocity, Color.White, Scale: Main.rand.NextFloat(0.6f, 2f));
                    dustParticle.innerColor = Color.SkyBlue;
                    dustParticle.outerColor = Color.Violet;
                }

                for (float f = 0; f < 6; f++)
                {
                    Vector2 initialVelocity = -Projectile.velocity.SafeNormalize(Vector2.Zero);
                    initialVelocity *= 4;
                    initialVelocity = initialVelocity.RotatedByRandom(MathHelper.ToRadians(60));
                    initialVelocity *= Main.rand.NextFloat(0.15f, 1f);

                    SmokeParticle smokeParticle = Particle<SmokeParticle>.SpawnInAlphaLayer(explosionCenter + initialVelocity,
                        initialVelocity, Color.White, Scale: Main.rand.NextFloat(0.6f, 1.3f));
                    smokeParticle.initialColor = Color.Lerp(Color.White, Color.Black, 0.4f);
                    smokeParticle.extraUpdates = Main.rand.Next(0, 1);
                    smokeParticle.fadeToColor = Color.Black;
                }

                FXUtil.GlowCircleBoom(explosionCenter,
                    innerColor: Color.White,
                    glowColor: Color.LightSkyBlue,
                    outerGlowColor: Color.Blue, duration: 25, baseSize: 0.06f);

                for (float f = 0; f < 3f; f++)
                {
                    float progress = f / 3f;
                    float rot = progress * MathHelper.ToRadians(360);
                    rot += Main.rand.NextFloat(-0.5f, 0.5f);
                    Vector2 velocity = rot.ToRotationVector2() * Main.rand.NextFloat(4f, 25f);
                    var particle = FXUtil.GlowStretch(explosionCenter, velocity);
                    particle.InnerColor = Color.White;
                    particle.GlowColor = Color.LightCyan;
                    particle.OuterGlowColor = Color.Black;
                    particle.Duration = Main.rand.NextFloat(25, 50);
                    particle.BaseSize = Main.rand.NextFloat(0.045f, 0.09f);
                    particle.VectorScale *= 0.5f;

                }
            }
        }

        private Color GetTrailColor(float completionRatio)
        {
            return Color.Lerp(Color.White, Color.Aqua, completionRatio);
        }

        private float GetTrailWidth(float completionRatio)
        {
            return 24 * EasingFunction.QuadraticBump(Timer / Lifetime);
        }

        private void DrawPixelGlows(SpriteBatch spriteBatch, Vector2 screenPos)
        {
            Texture2D glow = AssetManager.GlowMask.SimpleGlowCircle.Value;
            Vector2 drawOrigin = glow.Size() * 0.5f;
            for(int i = 0; i < BeamPoints.Length; i++)
            {
                if(i % 2 == 0)
                {
                    Vector2 pos = BeamPoints[i];
                    Color color = Color.DarkBlue;
                    color.A = 0;
                    spriteBatch.Draw(glow, pos - screenPos, null, color, 0, drawOrigin, 0.25f * new Vector2(1.5f, 1f) * Projectile.scale, SpriteEffects.None, 0);
                }

            }
            Color muzzleColor = Color.DarkBlue;
            muzzleColor.A = 0;
            spriteBatch.Draw(glow, Projectile.Center - screenPos, null, muzzleColor, 0, drawOrigin, 0.15f * new Vector2(1f, 1.75f) * Projectile.scale, SpriteEffects.None, 0);

            muzzleColor = Color.White;
            muzzleColor.A = 0;
            spriteBatch.Draw(glow, Projectile.Center - screenPos, null, muzzleColor, 0, drawOrigin, 0.1f * new Vector2(1f, 1.75f) * Projectile.scale, SpriteEffects.None, 0);


            muzzleColor = Color.DarkBlue;
            muzzleColor.A = 0;
            Vector2 offset = Projectile.velocity.SafeNormalize(Vector2.Zero) * BeamLength;
            spriteBatch.Draw(glow, Projectile.Center + offset  - screenPos, null, muzzleColor, 0, drawOrigin, 0.15f * new Vector2(1.75f, 1.75f) * Projectile.scale, SpriteEffects.None, 0);

            muzzleColor = Color.White;
            muzzleColor.A = 0;
            spriteBatch.Draw(glow, Projectile.Center + offset - screenPos, null, muzzleColor, 0, drawOrigin, 0.1f * new Vector2(1.75f, 1.75f) * Projectile.scale, SpriteEffects.None, 0);


        }


        private void DrawPixelatedBeam(GraphicsDevice graphicsDevice)
        {
            var shader2 = RichLaserShader.Instance;
            shader2.LaserColor = Color.White;
            shader2.InnerColor = Color.Turquoise * 0.5f;
            shader2.OuterColor = Color.Blue;
            TrailDrawer.Draw(Main.spriteBatch, BeamPoints, GetTrailColor, GetTrailWidth, shader2);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            PixelationManager.QueueSpritebatchDrawAction(DrawPixelGlows);
            PixelationManager.QueuePrimitivesDrawAction(DrawPixelatedBeam);
            return false;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
        }
    }
    public class SkyrageShasherStaminaSlash : BaseSwingProjectileV2
    {
        private bool _firedBeam;
        public override void DefineCombo()
        {
            base.DefineCombo();
            ComboBuilder comboBuilder = new ComboBuilder();
            comboBuilder
                .AddSpearThrust1(duration: 60, throwDistance: 32);
            comboBuilder.AddToProjectile(this);
            useAfterImage = true;
        }
        public override void AI()
        {
            base.AI();
            if (Main.rand.NextBool(128))
            {
                Vector2 position = Projectile.Center;
                position += Main.rand.NextVector2Circular(48, 48);
                SmokeParticle smokeParticle = SmokeParticle.SpawnInAlphaLayer(position, -Vector2.UnitY, Color.White, Scale: 0.5f);
                smokeParticle.initialColor = Color.White;
            }

            if (!_firedBeam && Interpolant > 0.1f && this.OwnedByLocalClient())
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity,
                    ModContent.ProjectileType<SkyrageBeam>(), Projectile.damage * 2, Projectile.knockBack, Projectile.owner, ai1: 30);
                _firedBeam = true;
            }
            glowColor = Color.Lerp(Color.Transparent, Color.LightBlue, EasingFunction.QuadraticBump(Interpolant));
            growScale = MathHelper.Lerp(0f, 0.15f, EasingFunction.QuadraticBump(Interpolant));
        }

    }
    public class SkyrageShasherSlash : BaseSwingProjectileV2
    {
        private bool _firedBeam;
        public override void DefineCombo()
        {
            base.DefineCombo();
            ComboBuilder comboBuilder = new ComboBuilder();
            comboBuilder.AddSpearSlash1().
                AddSpearSlash1()
                .AddSpearThrust1()
                .AddSpearThrust1()
                .AddSpearSlash1()
                .AddSpearSlash1()
                .AddSpearSpin1(duration: 30, swingDegrees: 720)
                .AddSpearSpin1(duration: 30, swingDegrees: 720)
                .AddSpearThrust1(duration: 30, throwDistance: 32);
            comboBuilder.AddToProjectile(this);
            useAfterImage = true;
        }

        private Color GetTrailColor(float completionRatio)
        {
            return Color.Lerp(Color.White, Color.Aqua, completionRatio);
        }

        private float GetTrailWidth(float completionRatio)
        {
            return MathHelper.SmoothStep(0, 24, completionRatio);
        }

        public override void RenderSwingTrail(ref Color lightColor, Vector2[] points)
        {
            base.RenderSwingTrail(ref lightColor, points);
            var shader2 = RichLaserShader.Instance;
            shader2.LaserColor = Color.White;
            shader2.InnerColor = Color.Turquoise * 0.5f;
            shader2.OuterColor = Color.Blue;
            TrailDrawer.Draw(Main.spriteBatch, points, GetTrailColor, GetTrailWidth, shader2);
        }

        public override void AI()
        {
            base.AI();
            if (Main.rand.NextBool(128))
            {
                Vector2 position = Projectile.Center;
                position += Main.rand.NextVector2Circular(48, 48);
                SmokeParticle smokeParticle = SmokeParticle.SpawnInAlphaLayer(position, -Vector2.UnitY, Color.White, Scale: 1f);
                smokeParticle.initialColor = Color.White;
                smokeParticle.fast = true;
            }
            if (IsFinishingSwing())
            {
                if (!_firedBeam && Interpolant > 0.1f && this.OwnedByLocalClient())
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity.SafeNormalize(Vector2.Zero),
                        ModContent.ProjectileType<SkyrageBeam>(), Projectile.damage, Projectile.knockBack, Projectile.owner, ai1: 20);
                    _firedBeam = true;
                }
            }
            glowColor = Color.Lerp(Color.Transparent, Color.LightBlue * 0.3f, EasingFunction.QuadraticBump(Interpolant));
            growScale = MathHelper.Lerp(0f, 0.15f, EasingFunction.QuadraticBump(Interpolant));
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(target, ref modifiers);
            SoundStyle spearHit = SoundRegistry.SpearHit1;
            spearHit.PitchVariance = 0.5f;
            SoundEngine.PlaySound(spearHit, Projectile.position);
            if (IsFinishingSwing())
            {
                DamageHelper.PercentIncreasedamage(ref modifiers, 0.5f);
            }
        }
    }
}