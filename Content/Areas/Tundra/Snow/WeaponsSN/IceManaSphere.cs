using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Common.WeaponTypes;
using Stellamod.Content.Areas.Dungeon.BossesDG.Bisinine.Projectiles;
using Stellamod.Content.CommonMaterials;
using Stellamod.Content.Gores;
using Stellamod.Core.Bases;
using Stellamod.Core.Particles;
using Stellamod.Core.Pixelation;
using Stellamod.Core.Utilities;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Items;
using Stellamod.Visual.Particles;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Tundra.Snow.WeaponsSN
{
    public class IceManaSphere : ModItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.DefaultToManaSphere(ModContent.ProjectileType<IceManaSphereHold>(), ModContent.ProjectileType<IceManaOrb>());
            Item.shoot = ModContent.ProjectileType<IceManaBlast>();
            Item.shootSpeed = 15;
            Item.damage = 9;
            Item.UseSound = SoundID.DD2_BetsyFireballShot;
        }

        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew(mold: ModContent.ItemType<BlankOrb>(), material: ModContent.ItemType<WinterbornShard>());
        }
    }

    public class IceCometBoom : ModProjectile
    {
        private float _lightningPower;
        private bool _drawLightning;
        public override string Texture => TextureRegistry.EmptyTexture;
        private ref float Timer => ref Projectile.ai[0];
        private ref float ForceLightning => ref Projectile.ai[1];
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 60;
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public override bool CanHitPlayer(Player target)
        {
            return false;
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            if (Timer == 1)
            {
                _lightningPower = 0.9f;
                int[] gores = AutoGoreLoader.FindGores("GrayRock");
                foreach (int g in gores)
                {
                    if (Main.rand.NextBool(3))
                        continue;
                    Gore.NewGore(Projectile.GetSource_FromThis(),
                        Projectile.Center,
                        -Vector2.UnitY.RotatedByRandom(MathHelper.ToRadians(20)) * Main.rand.NextFloat(5f, 15f), g, Main.rand.NextFloat(0f, 1f));
                }

                for (float f = 0; f < 1; f++)
                {
                    var fp = SmokeParticle.SpawnInAlphaLayer(Projectile.Center, -Vector2.UnitY.RotatedByRandom(MathHelper.ToRadians(45)) * 5 * Main.rand.NextFloat(0.2f, 0.6f), Color.White);
                    fp.initialColor = Color.White;
                }
                for (float f = 0; f < 4f; f++)
                {
                    DustParticleSpawnParams spawnParams = DustParticleSpawnParams.Default;
                    spawnParams.outerColor = Color.SkyBlue;
                    DustParticle.Spawn(Projectile.Center, -Vector2.UnitY.RotatedByRandom(MathHelper.ToRadians(45)) * 5, spawnParams);
                }


                FXUtil.ShakeCamera(Projectile.Center, 1024, 8);
                ShakeScreenPosition.Shake = 2;

                for (float f = 0; f < 4f; f++)
                {
                    Vector2 pos = Projectile.Center;
                    pos += Main.rand.NextVector2Circular(80, 80);
                    var zap = LegacyParticle.NewParticle<ZapParticle>(pos, Vector2.UnitY.RotatedByRandom(10) * Main.rand.NextFloat(2, 15));
                    zap.innerColor = Color.Gray;
                    zap.outerColor = Color.Blue;
                    zap.fadeToColor = Color.Black;
                    zap.Scale *= Main.rand.NextFloat(0f, 0.5f);
                    zap.Rotation = Main.rand.NextFloat(0f, 3f);
                }

                SoundStyle smashSound;
                int sound = Main.rand.Next(3);
                switch (sound)
                {
                    default:
                    case 0:
                        smashSound = Main.rand.NextBool(2) ? SoundRegistry.HammerHit1 : SoundRegistry.HammerHit2;
                        break;
                    case 1:
                        smashSound = AssetRegistry.Sounds.Bishinine.Comet1;
                        break;
                    case 2:
                        smashSound = AssetRegistry.Sounds.Bishinine.Comet2;
                        foreach (int g in gores)
                        {
                            Gore.NewGore(Projectile.GetSource_FromThis(),
                                Projectile.Center,
                                -Vector2.UnitY.RotatedByRandom(MathHelper.ToRadians(20)) * Main.rand.NextFloat(5f, 15f), g, Main.rand.NextFloat(0f, 1f));
                        }
                        FXUtil.ShakeCamera(Projectile.Center, 1024, 32);
                        _drawLightning = true;
                        var p3 = FXUtil.GlowCircleBoom(Projectile.Center,
                           innerColor: Color.Gray,
                           glowColor: Color.LightBlue,
                           outerGlowColor: Color.DarkBlue, duration: 15, baseSize: .09f);
                        break;
                }

                if (ForceLightning > 0)
                {
                    _drawLightning = true;
                }

                smashSound.PitchVariance = 0.2f;
                SoundEngine.PlaySound(smashSound, Projectile.position);


                var part = LegacyParticle.NewParticle<GlowDonutParticle>(Projectile.Center, Vector2.Zero, Color.White);
                part.fadeToColor = Color.Black;
                part.outerColor = Color.Gray;
                part.noStretch = true;
                part.shrink = true;

                var part2 = LegacyParticle.NewParticle<GlowDonutParticle>(Projectile.Center, Vector2.Zero, Color.White);
                part2.fadeToColor = Color.Black;
                part2.outerColor = Color.Gray;
                part2.noStretch = true;
                part2.color *= 0.5f;

                var soundStyle = AssetRegistry.Sounds.Stars.Starsingle5;
                soundStyle.PitchVariance = 0.3f;
                SoundEngine.PlaySound(soundStyle, Projectile.position);
                float boomSize = Main.rand.NextFloat(0.06f, 0.08f);
                FXUtil.GlowCircleBoom(Projectile.Center,
                   innerColor: Color.Gray,
                   glowColor: Color.LightBlue,
                   outerGlowColor: Color.DarkBlue, duration: 15, baseSize: boomSize * 2);
            }

            if (Timer == 15)
            {
                _lightningPower = 5;
            }

            if (Timer == 15)
            {
                _lightningPower = 30;
            }
            if (_drawLightning)
            {

                if (Timer > 35)
                {
                    _lightningPower = MathHelper.Lerp(_lightningPower, 10, 0.1f);
                }

                if (Timer == 42)
                {
                    _lightningPower = 1.5f;
                }
                if (Timer == 42)
                {
                    var part = FXUtil.GlowCircleBoom(Projectile.Center,
                                      innerColor: Color.Gray,
                                      glowColor: Color.GhostWhite,
                                      outerGlowColor: Color.Blue, duration: 6, baseSize: 0.12f);
                }
                if (Timer == 52)
                {
                    _lightningPower = 2.3f;
                }
                if (Timer == 52)
                {
                    var part = FXUtil.GlowCircleBoom(Projectile.Center,
                                      innerColor: Color.Gray,
                                      glowColor: Color.GhostWhite,
                                      outerGlowColor: Color.Blue, duration: 6, baseSize: 0.07f);
                }

                if (Timer == 58)
                {
                    SoundStyle zap = SoundID.DD2_LightningBugZap;
                    zap.PitchVariance = 0.3f;
                    SoundEngine.PlaySound(zap, Projectile.position);

                    for (float f = 0; f < 2; f++)
                    {
                        Vector2 pVelocity = -Vector2.UnitY.RotatedByRandom(MathHelper.PiOver4);
                        pVelocity *= Main.rand.NextFloat(0.5f, 1f);
                        var spark = LegacyParticle.NewParticle<ZapParticle>(Projectile.Center + Main.rand.NextVector2Circular(64, 64), pVelocity);
                        spark.Scale *= 0.5f;
                        spark.Rotation = Main.rand.NextFloat(0f, 3.14f);
                        spark.outerColor = Color.Blue;
                    }
                }
            }
        }
    }
    public class IceManaOrb : ModProjectile
    {
        public override string Texture => TextureRegistry.EmptyTexture;
        private ref float Timer => ref Projectile.ai[0];
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 64;
            Projectile.height = 64;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 180;
            Projectile.ignoreWater = true;
        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            if (this.OwnedByLocalClient())
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero,
                    ModContent.ProjectileType<IcicleManaBlastSpawner>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            }

            float boomSize = Main.rand.NextFloat(0.03f, 0.04f);
            for (float n = 0; n < 8; n++)
            {
                var spawnParams = new DustParticleSpawnParams();
                spawnParams.innerColor = Color.LightSkyBlue;
                spawnParams.outerColor = Color.DarkBlue;
                spawnParams.scaleRange = new Vector2(0.3f, 1f);
                DustParticle.Spawn(Projectile.Center, -Projectile.oldVelocity.RotatedByRandom(1.5f) * Main.rand.NextFloat(0.5f, 1f), spawnParams);
            }

            SmokeParticle sp = Particle<SmokeParticle>.SpawnInAlphaLayer(Projectile.Center, -Vector2.UnitY, Color.White, Scale: 1f);
            sp.initialColor = Color.White * 0.14f;
            if (this.OwnedByLocalClient())
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.oldVelocity,
                    ModContent.ProjectileType<IceCometBoom>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            }
        }

        public override void AI()
        {
            base.AI();

            Projectile.velocity *= 1.02f;
            Timer++;
            if (Main.rand.NextBool(3))
            {
                FlakeParticle dp = Particle<FlakeParticle>.Spawn(Projectile.Center, Main.rand.NextVector2Circular(2.5f, 2.5f), Scale: Main.rand.NextFloat(0.2f, 0.35f));
                //   dp.innerColor = Color.Goldenrod;
                // dp.outerColor = Color.Red;
                dp.parent = Projectile;
                dp.gravity = 0f;
                dp.dampening = 0.05f;
                // dp.fast = true;
                dp.Scale *= 0.8f;

            }

            if (Main.rand.NextBool(5))
            {
                switch (Main.rand.Next(2))
                {
                    case 0:
                        DustParticle sp = Particle<DustParticle>.Spawn(Projectile.Center + Main.rand.NextVector2Circular(32, 32), -Projectile.velocity.SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(0.3f, 8), Scale: Main.rand.NextFloat(0.5f, 1.5f));
                        sp.gravity = 0f;
                        sp.fast = true;
                        sp.dampening = 0.1f;
                        sp.Scale *= 0.25f;
                        break;
                    case 1:
                        FlakeParticle sp2 = Particle<FlakeParticle>.Spawn(Projectile.Center + Main.rand.NextVector2Circular(32, 32), -Projectile.velocity.SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(1f, 8), Scale: Main.rand.NextFloat(0.1f, 0.2f));
                        sp2.gravity = 0f;
                        //sp2.fast = true;
                        sp2.dampening = 0.1f;

                        break;
                }

            }
            if (Main.rand.NextBool(4))
            {
                DustParticle sp = Particle<DustParticle>.Spawn(Projectile.Center + Main.rand.NextVector2Circular(32, 32), 
                    -Projectile.velocity.SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(0.4f, 3f));
                sp.outerColor = Color.SkyBlue;
                sp.gravity = 0;

                    }

            if (Main.rand.NextBool(8))
            {
                FlameSparksParticle sp = Particle<FlameSparksParticle>.Spawn(Projectile.Center + Main.rand.NextVector2Circular(32, 32), -Projectile.velocity.SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(0.6f, 8f),
                    color: Color.White, Scale: Main.rand.NextFloat(0.35f, 0.75f));
                sp.gravity = 0f;
                sp.fast = true;
                sp.dampening = 0.1f;
                sp.Scale *= 0.25f;
            }

            if(Timer >= 60)
            {
                Projectile.tileCollide = true;
            }

        }
        public float WidthFunction(float completionRatio)
        {
            return MathHelper.SmoothStep(16, 0f, completionRatio);
        }

        public Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.LightCyan, Color.DarkBlue, completionRatio);
        }
        private void DrawPixelatedFlames(GraphicsDevice graphicsDevice)
        {
            var shader = RichLaserShader.Instance;
            shader.LaserColor = Color.LightCyan * 0.5f;
            shader.InnerColor = Color.Cyan * 0.5f;
            shader.OuterColor = Color.Blue * 0.5f;
            shader.LaserTexture = TrailRegistry.SpikyTrail1;
            shader.BloomTexture = AssetManager.LaserTextures.SnowflakeLaser;
            TrailDrawer.Draw(Main.spriteBatch, Projectile.oldPos, ColorFunction, WidthFunction, shader, Projectile.Size / 2f);
        }

        private void DrawPixelatedFrozenOrb(SpriteBatch spriteBatch, Vector2 screenPos)
        {
            SpritebatchDrawer orbTexture = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SimpleGlowCircle, Projectile.Center);
            orbTexture.blackIsTransparency = true;
            orbTexture.color = Color.Cyan;
            orbTexture.color *= 0.5f;
            orbTexture.color *= ExtraMath.Osc(0.75f, 1f, speed: 8);
            orbTexture.scale = Vector2.One * 0.35f;
            spriteBatch.Draw(orbTexture);

            SpritebatchDrawer spiralVortexTexture = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SpiralVortex, Projectile.Center);
            spiralVortexTexture.blackIsTransparency = true;
            spiralVortexTexture.color = Color.SkyBlue;
            spiralVortexTexture.rotation = Main.GlobalTimeWrappedHourly * 4;
            spiralVortexTexture.scale = Vector2.One * 0.35f;
            spriteBatch.Draw(spiralVortexTexture);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            PixelationManager.QueueSpritebatchDrawAction(DrawPixelatedFrozenOrb);
            PixelationManager.QueuePrimitivesDrawAction(DrawPixelatedFlames);
            return false;
        }
    }
    public class IcicleManaBlastSpawner : ModProjectile
    {
        private Player Owner => Main.player[Projectile.owner];
        private ref float Timer => ref Projectile.ai[0];
        public override string Texture => TextureRegistry.EmptyTexture;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = Projectile.height = 1;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 60;
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            if (Timer % 3 == 0)
            {
                if (this.OwnedByLocalClient())
                {
                    Vector2 velocity = (Projectile.Center - Owner.Center).SafeNormalize(Vector2.Zero) * 15;
                    Projectile.NewProjectile(Owner.GetSource_FromThis(), Projectile.Center - Vector2.UnitY * 1250 + Main.rand.NextVector2Circular(1000, 64), Vector2.UnitY * 15, 
                        ModContent.ProjectileType<IcicleManaBlast>(), Projectile.damage, Projectile.knockBack, Owner.whoAmI);
                }

            }
        }
    }
    public class IcicleManaBlast : ModProjectile
    {
        private Vector2 _initialVelocity;
        private Vector2 HomingTarget;
        private ref float Timer => ref Projectile.ai[0];
        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
            writer.WriteVector2(_initialVelocity);
            writer.WriteVector2(HomingTarget);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
            _initialVelocity = reader.ReadVector2();
            HomingTarget = reader.ReadVector2();
        }
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ProjectileID.Sets.TrailCacheLength[Type] = 16;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 200;
            Projectile.extraUpdates = 1;
        }
        public override void AI()
        {
            base.AI();
            Timer++;
            if (Timer == 1)
            {
                _initialVelocity = Projectile.velocity;

            }

            if (Timer == 5)
            {
                ShockOvalSpawnParams spawnParams = new ShockOvalSpawnParams
                {
                    innerColor = Color.LightSkyBlue,
                    outerColor = Color.DarkBlue
                };
                ShockOvalParticle sp = ShockOvalParticle.Spawn(Projectile.Center, -Projectile.velocity * 0.4f, spawnParams);
                sp.color *= 0.85f;
                sp.Scale *= 0.6f;

                sp = ShockOvalParticle.Spawn(Projectile.Center, -Projectile.velocity * 0.2f, spawnParams);
                sp.color *= 0.85f;
                sp.Scale *= 0.3f;
            }
            if (Projectile.velocity.Length() < 25)
            {
                Projectile.velocity *= 1.01f;
            }

            if (Main.rand.NextBool(3))
            {
                FlakeParticle dp = Particle<FlakeParticle>.Spawn(Projectile.Center, Main.rand.NextVector2Circular(2.5f, 2.5f), Scale: Main.rand.NextFloat(0.2f, 0.35f));
                //   dp.innerColor = Color.Goldenrod;
                // dp.outerColor = Color.Red;
                dp.parent = Projectile;
                dp.gravity = 0f;
                dp.dampening = 0.05f;
                // dp.fast = true;
                dp.Scale *= 0.8f;

            }

            if (Main.rand.NextBool(5))
            {
                switch (Main.rand.Next(2))
                {
                    case 0:
                        DustParticle sp = Particle<DustParticle>.Spawn(Projectile.Center + Main.rand.NextVector2Circular(32, 32), -Projectile.velocity.SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(0.3f, 8), Scale: Main.rand.NextFloat(0.5f, 1.5f));
                        sp.gravity = 0f;
                        sp.fast = true;
                        sp.dampening = 0.1f;
                        sp.Scale *= 0.25f;
                        break;
                    case 1:
                        FlakeParticle sp2 = Particle<FlakeParticle>.Spawn(Projectile.Center + Main.rand.NextVector2Circular(32, 32), -Projectile.velocity.SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(1f, 8), Scale: Main.rand.NextFloat(0.1f, 0.2f));
                        sp2.gravity = 0f;
                        //sp2.fast = true;
                        sp2.dampening = 0.1f;

                        break;
                }

            }

            if (Main.rand.NextBool(8))
            {
                FlameSparksParticle sp = Particle<FlameSparksParticle>.Spawn(Projectile.Center + Main.rand.NextVector2Circular(32, 32), -Projectile.velocity.SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(0.6f, 8f),
                    color: Color.White, Scale: Main.rand.NextFloat(0.35f, 0.75f));
                sp.gravity = 0f;
                sp.fast = true;
                sp.dampening = 0.1f;
                sp.Scale *= 0.25f;
            }


            if (Main.myPlayer == Projectile.owner)
            {
                HomingTarget = Main.MouseWorld;


                Projectile.netUpdate = true;
            }
            if (Timer < 30f)
            {
                Vector2 homingVelocity = ProjectileHelper.SimpleHomingVelocity(Projectile, HomingTarget);
                Projectile.velocity = Vector2.Lerp(_initialVelocity, homingVelocity, EasingFunction.InOutSine(Timer / 30f));
            }
            if (Timer > 60)
                Projectile.tileCollide = true;

        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;

            Texture2D icicle = TextureAssets.Projectile[Type].Value;
            spriteBatch.Draw(icicle, drawPos, null, lightColor, Projectile.velocity.ToRotation(), icicle.Size() / 2f, Vector2.One, SpriteEffects.None, 0);

            Vector2 drawScale = Vector2.One * 0.1f;

            Texture2D glowMask = AssetManager.GlowMask.SimpleGlowCircle.Value;
            Vector2 glowDrawOrigin = glowMask.Size() / 2f;
            Color glowColor = Color.Lerp(Color.LightCyan, Color.DarkBlue, ExtraMath.Osc(0f, 1f, speed: 8));
            glowColor.A = 0;
            spriteBatch.Draw(glowMask, drawPos, null, glowColor, 0, glowDrawOrigin, drawScale * Projectile.scale * ExtraMath.Osc(0.9f, 1.2f, speed: 8) * 0.3f, SpriteEffects.None, 0);

            glowMask = AssetManager.GlowMask.SpiralVortex.Value;
            glowDrawOrigin = glowMask.Size() / 2f;
            glowColor = Color.SkyBlue;
            glowColor.A = 0;
            spriteBatch.Draw(glowMask, drawPos, null, glowColor, Main.GlobalTimeWrappedHourly * 8, glowDrawOrigin, drawScale * Projectile.scale * ExtraMath.Osc(0.99f, 1.01f, speed: 8) * 0.6f, SpriteEffects.None, 0);
            glowMask = AssetManager.GlowMask.SimpleGlowCircle.Value;
            glowDrawOrigin = glowMask.Size() / 2f;
            glowColor = Color.SkyBlue;
            glowColor.A = 0;
            glowColor *= 0.5f;
            Vector2 stretch = new Vector2(3f, 1.3f) * 0.75f;
            spriteBatch.Draw(glowMask, drawPos - Projectile.velocity * 1.5f, null, glowColor, Projectile.velocity.ToRotation(), glowDrawOrigin, drawScale * Projectile.scale * ExtraMath.Osc(0.99f, 1.01f, speed: 8) * stretch, SpriteEffects.None, 0);
            PixelationManager.QueuePrimitivesDrawAction(DrawPixelatedFlames);
            return false;
        }
        public float WidthFunction(float completionRatio)
        {
            return MathHelper.SmoothStep(16, 0f, completionRatio);
        }

        public Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.LightCyan, Color.DarkBlue, completionRatio);
        }
        private void DrawPixelatedFlames(GraphicsDevice graphicsDevice)
        {
            var shader = RichLaserShader.Instance;
            shader.LaserColor = Color.LightCyan * 0.5f;
            shader.InnerColor = Color.Cyan * 0.5f;
            shader.OuterColor = Color.Blue * 0.5f;
            shader.LaserTexture = TrailRegistry.SpikyTrail1;
            shader.BloomTexture = AssetManager.LaserTextures.SnowflakeLaser;
            TrailDrawer.Draw(Main.spriteBatch, Projectile.oldPos, ColorFunction, WidthFunction, shader, Projectile.Size / 2f);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return base.OnTileCollide(oldVelocity);
        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            float boomSize = Main.rand.NextFloat(0.03f, 0.04f);
            for (float n = 0; n < 2f; n++)
            {
                var spawnParams = new DustParticleSpawnParams();
                spawnParams.innerColor = Color.LightSkyBlue;
                spawnParams.outerColor = Color.DarkBlue;
                spawnParams.scaleRange = new Vector2(0.1f, 1f);
                DustParticle.Spawn(Projectile.Center, -Projectile.oldVelocity.RotatedByRandom(1.5f) * Main.rand.NextFloat(0.5f, 1f), spawnParams);
            }

            SmokeParticle sp = Particle<SmokeParticle>.SpawnInAlphaLayer(Projectile.Center, -Vector2.UnitY, Color.White, Scale: 1f);
            sp.initialColor = Color.White * 0.14f;

            if (this.OwnedByLocalClient())
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.oldVelocity,
                    ModContent.ProjectileType<IceCometBoom>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            }
        }
    }
    public class IceManaBlast : ModProjectile
    {
        private Vector2 _initialVelocity;
        private Vector2 HomingTarget;
        private ref float Timer => ref Projectile.ai[0];
        public override string Texture => TextureRegistry.EmptyTexture;
        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
            writer.WriteVector2(_initialVelocity);
            writer.WriteVector2(HomingTarget);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
            _initialVelocity = reader.ReadVector2();
            HomingTarget = reader.ReadVector2();
        }
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ProjectileID.Sets.TrailCacheLength[Type] = 4;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 200;
        }
        public override void AI()
        {
            base.AI();
            Timer++;
            if (Timer == 1)
            {
                _initialVelocity = Projectile.velocity;

            }

            if (Timer == 5)
            {
                ShockOvalSpawnParams spawnParams = new ShockOvalSpawnParams
                {
                    innerColor = Color.LightSkyBlue,
                    outerColor = Color.DarkBlue
                };
                ShockOvalParticle sp = ShockOvalParticle.Spawn(Projectile.Center, -Projectile.velocity * 0.4f, spawnParams);
                sp.color *= 0.85f;
                sp.Scale *= 0.6f;

                sp = ShockOvalParticle.Spawn(Projectile.Center, -Projectile.velocity * 0.2f, spawnParams);
                sp.color *= 0.85f;
                sp.Scale *= 0.3f;
            }
            if (Projectile.velocity.Length() < 25)
            {
                Projectile.velocity *= 1.01f;
            }

            if (Main.rand.NextBool(3))
            {
                FlakeParticle dp = Particle<FlakeParticle>.Spawn(Projectile.Center, Main.rand.NextVector2Circular(2.5f, 2.5f), Scale: Main.rand.NextFloat(0.2f, 0.35f));
                //   dp.innerColor = Color.Goldenrod;
                // dp.outerColor = Color.Red;
                dp.parent = Projectile;
                dp.gravity = 0f;
                dp.dampening = 0.05f;
                // dp.fast = true;
                dp.Scale *= 0.8f;

            }

            if (Main.rand.NextBool(5))
            {
                switch (Main.rand.Next(2))
                {
                    case 0:
                        DustParticle sp = Particle<DustParticle>.Spawn(Projectile.Center + Main.rand.NextVector2Circular(32, 32), -Projectile.velocity.SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(0.3f, 8), Scale: Main.rand.NextFloat(0.5f, 1.5f));
                        sp.gravity = 0f;
                        sp.fast = true;
                        sp.dampening = 0.1f;
                        sp.Scale *= 0.25f;
                        break;
                    case 1:
                        FlakeParticle sp2 = Particle<FlakeParticle>.Spawn(Projectile.Center + Main.rand.NextVector2Circular(32, 32), -Projectile.velocity.SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(1f, 8), Scale: Main.rand.NextFloat(0.1f, 0.2f));
                        sp2.gravity = 0f;
                        //sp2.fast = true;
                        sp2.dampening = 0.1f;

                        break;
                }

            }

            if (Main.rand.NextBool(8))
            {
                FlameSparksParticle sp = Particle<FlameSparksParticle>.Spawn(Projectile.Center + Main.rand.NextVector2Circular(32, 32), -Projectile.velocity.SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(0.6f, 8f),
                    color: Color.White, Scale: Main.rand.NextFloat(0.35f, 0.75f));
                sp.gravity = 0f;
                sp.fast = true;
                sp.dampening = 0.1f;
                sp.Scale *= 0.25f;
            }


            if (Main.myPlayer == Projectile.owner)
            {
                HomingTarget = Main.MouseWorld;


                Projectile.netUpdate = true;
            }
            if (Timer < 30f)
            {
                Vector2 homingVelocity = ProjectileHelper.SimpleHomingVelocity(Projectile, HomingTarget);
                Projectile.velocity = Vector2.Lerp(_initialVelocity, homingVelocity, EasingFunction.InOutSine(Timer / 30f));
            }
            if (Timer > 30f)
                Projectile.tileCollide = true;
        }

        public override bool PreDraw(ref Color lightColor)
        {
      
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            Vector2 drawScale = Vector2.One * 0.1f;
            SpriteBatch spriteBatch = Main.spriteBatch;
            Texture2D glowMask = AssetManager.GlowMask.SimpleGlowCircle.Value;
            Vector2 glowDrawOrigin = glowMask.Size() / 2f;
            Color glowColor = Color.Lerp(Color.LightCyan, Color.DarkBlue, ExtraMath.Osc(0f, 1f, speed: 8));
            glowColor.A = 0;
            spriteBatch.Draw(glowMask, drawPos, null, glowColor, 0, glowDrawOrigin, drawScale * Projectile.scale * ExtraMath.Osc(0.9f, 1.2f, speed: 8) * 0.3f, SpriteEffects.None, 0);

            glowMask = AssetManager.GlowMask.SpiralVortex.Value;
            glowDrawOrigin = glowMask.Size() / 2f;
            glowColor = Color.SkyBlue;
            glowColor.A = 0;
            spriteBatch.Draw(glowMask, drawPos, null, glowColor, Main.GlobalTimeWrappedHourly * 8, glowDrawOrigin, drawScale * Projectile.scale * ExtraMath.Osc(0.99f, 1.01f, speed: 8) * 0.6f, SpriteEffects.None, 0);
            glowMask = AssetManager.GlowMask.SimpleGlowCircle.Value;
            glowDrawOrigin = glowMask.Size() / 2f;
            glowColor = Color.SkyBlue;
            glowColor.A = 0;
            glowColor *= 0.5f;
            Vector2 stretch = new Vector2(3f, 1.3f) * 0.75f;
            spriteBatch.Draw(glowMask, drawPos - Projectile.velocity * 1.5f, null, glowColor, Projectile.velocity.ToRotation(), glowDrawOrigin, drawScale * Projectile.scale * ExtraMath.Osc(0.99f, 1.01f, speed: 8) * stretch, SpriteEffects.None, 0);
            PixelationManager.QueuePrimitivesDrawAction(DrawPixelatedFlames);
            return false;
        }
        public float WidthFunction(float completionRatio)
        {
            return MathHelper.SmoothStep(16, 0f, completionRatio);
        }

        public Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.LightCyan, Color.DarkBlue, completionRatio);
        }
        private void DrawPixelatedFlames(GraphicsDevice graphicsDevice)
        {
            var shader = RichLaserShader.Instance;
            shader.LaserColor = Color.LightCyan;
            shader.InnerColor = Color.Cyan;
            shader.OuterColor = Color.Blue;
            shader.LaserTexture = TrailRegistry.SpikyTrail1;
            shader.BloomTexture = AssetManager.LaserTextures.SnowflakeLaser;
            TrailDrawer.Draw(Main.spriteBatch, Projectile.oldPos, ColorFunction, WidthFunction, shader, Projectile.Size / 2f);
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return base.OnTileCollide(oldVelocity);
        }
        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);

            float boomSize = Main.rand.NextFloat(0.03f, 0.04f);
            for (float n = 0; n < 2f; n++)
            {
                var spawnParams = new DustParticleSpawnParams();
                spawnParams.innerColor = Color.LightSkyBlue;
                spawnParams.outerColor = Color.DarkBlue;
                spawnParams.scaleRange = new Vector2(0.1f, 1f);
                DustParticle.Spawn(Projectile.Center, -Projectile.oldVelocity.RotatedByRandom(1.5f) * Main.rand.NextFloat(0.5f, 1f), spawnParams);
            }

            SmokeParticle sp = Particle<SmokeParticle>.SpawnInAlphaLayer(Projectile.Center, -Vector2.UnitY, Color.White, Scale: 1f);
            sp.initialColor = Color.White * 0.14f;
        }
    }

    public class IceManaSphereHold : AbstractManaSphereHold
    {
        public override string Texture => ModContent.GetInstance<IceManaSphere>().Texture;
        public override void AI_OrbitPlayer()
        {
            base.AI_OrbitPlayer();

        }

        public override void DustEffects()
        {
            base.DustEffects();
            if (Timer % 5 == 0)
            {
                FlakeParticle dp = Particle<FlakeParticle>.Spawn(Projectile.Center, Main.rand.NextVector2Circular(2.5f, 2.5f), Scale: Main.rand.NextFloat(0.2f, 0.35f));
                //   dp.innerColor = Color.Goldenrod;
                // dp.outerColor = Color.Red;
                dp.parent = Projectile;
                dp.gravity = 0f;
                dp.dampening = 0.05f;
                // dp.fast = true;
                dp.Scale *= 0.8f;
            }

        }
        public override bool PreDraw(ref Color lightColor)
        {
            DrawHead(Main.spriteBatch, Main.screenPosition);
            PixelationManager.QueuePrimitivesDrawAction(DrawPixelatedFlames);
            return false;
        }
        public float WidthFunction(float completionRatio)
        {
            return MathHelper.SmoothStep(16, 0f, completionRatio);
        }

        public Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.Goldenrod, Color.DarkRed, completionRatio);
        }
        private void DrawPixelatedFlames(GraphicsDevice graphicsDevice)
        {
            var shader = RichLaserShader.Instance;
            shader.LaserColor = Color.LightCyan;
            shader.InnerColor = Color.Cyan;
            shader.OuterColor = Color.Blue;
            shader.LaserTexture = TrailRegistry.SpikyTrail1;
            shader.BloomTexture = AssetManager.LaserTextures.SnowflakeLaser;
            TrailDrawer.Draw(Main.spriteBatch, Projectile.oldPos, ColorFunction, WidthFunction, shader, Projectile.Size / 2f);
        }
        private void DrawHead(SpriteBatch spriteBatch, Vector2 screenPos)
        {

            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            Vector2 drawScale = Vector2.One * 0.1f;

            Texture2D glowMask = AssetManager.GlowMask.SimpleGlowCircle.Value;
            Vector2 glowDrawOrigin = glowMask.Size() / 2f;
            Color glowColor = Color.Lerp(Color.LightCyan, Color.DarkBlue, ExtraMath.Osc(0f, 1f, speed: 8));
            glowColor.A = 0;
            spriteBatch.Draw(glowMask, drawPos, null, glowColor, 0, glowDrawOrigin, drawScale * Projectile.scale * ExtraMath.Osc(0.9f, 1.2f, speed: 8) * 0.3f, SpriteEffects.None, 0);

            glowMask = AssetManager.GlowMask.SpiralVortex.Value;
            glowDrawOrigin = glowMask.Size() / 2f;
            glowColor = Color.SkyBlue;
            glowColor.A = 0;
            spriteBatch.Draw(glowMask, drawPos, null, glowColor, Main.GlobalTimeWrappedHourly * 8, glowDrawOrigin, drawScale * Projectile.scale * ExtraMath.Osc(0.99f, 1.01f, speed: 8) * 0.6f, SpriteEffects.None, 0);
            glowMask = AssetManager.GlowMask.SimpleGlowCircle.Value;
            glowDrawOrigin = glowMask.Size() / 2f;
            glowColor = Color.SkyBlue;
            glowColor.A = 0;
            glowColor *= 0.5f;
            Vector2 stretch = new Vector2(1.5f, 1.3f);
            spriteBatch.Draw(glowMask, drawPos - Projectile.velocity * 1.5f, null, glowColor, Projectile.velocity.ToRotation(), glowDrawOrigin, drawScale * Projectile.scale * ExtraMath.Osc(0.99f, 1.01f, speed: 8) * stretch, SpriteEffects.None, 0);
        }
    }
}
