using Stellamod.Common.Particles;
using Stellamod.Common.Shaders;
using Stellamod.Content.Areas.Jungle.ZigguratraBoss.Projectiles;
using Stellamod.Core.Pixelation;
using Stellamod.Core.Rendering.RTs;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.BackupIO;

namespace Stellamod.Common.TornadoRendering;

public class StormingTornadoV2 : ModProjectile
{
    TornadoMesh BigCylinderMesh
    {
        get
        {
            if (field == null)
            {
                field = new(height: 1024, radius: 180, heightPerBand: 256, sidesPerBand: 32, Color.White, (float f) =>
                {
                    if (f <= 0f)
                        return 64;
                    return 1;
                });
            }
            return field;
        }
    }
    TornadoMesh CylinderMesh
    {
        get
        {
            if(field == null)
            {
                field = new(height: 1024, radius: 180, heightPerBand: 256, sidesPerBand: 32, Color.White, (float f) => 1f);
            }
            return field;
        }
    }
    TornadoCircles TornadoCircles
    {
        get
        {
            if (field == null)
            {
                field = new(8, 64, 128, 16);
            }
            return field;
        }
    }
    TornadoCircles FastTornadoCircles
    {
        get
        {
            if (field == null)
            {
                field = new(8, 64, 128, 16);
            }
            return field;
        }
    }
    TornadoCircles BottomTornadoCircles
    {
        get
        {
            if (field == null)
            {
                field = new(8, 64, 128, 16);
            }
            return field;
        }
    }
    TornadoFlashParticles FlashParticles => field ??= new();
    TornadoLightningBoltParticles LightningBoltParticles => field ??= new();

    TornadoStreakLinesParticles StreakParticles
    {
        get
        {
            if (field == null)
                field = new();
            return field;
        }
    }
    TornadoSmokeParticles SmokeParticles
    {
        get
        {
            if (field == null)
                field = new();
            field.scale = 1f;
            return field;
        }
    }
    TornadoDustParticles DustParticles
    {
        get
        {
            if (field == null)
                field = new();
            return field;
        }
    }


    TornadoStreakLinesParticles BottomStreakParticles
    {
        get
        {
            if (field == null)
                field = new();
            return field;
        }
    }
    TornadoSmokeParticles BottomSmokeParticles
    {
        get
        {
            if (field == null)
                field = new();
            field.scale = 0.85f;
            return field;
        }
    }
    TornadoDustParticles BottomDustParticles
    {
        get
        {
            if (field == null)
                field = new();
            return field;
        }
    }

    Matrix BandRotationMatrix
    {
        get
        {
            return Matrix.CreateFromAxisAngle(new Vector3(0, 1, 0), Main.GlobalTimeWrappedHourly);
        }
    }
    Matrix PerspectiveMatrix
    {
        get
        {
            return Matrix.CreateFromAxisAngle(new Vector3(1, 0, 0), 0.3f + ExtraMath.Osc(-0.05f, 0.05f));

        }
    }
    Matrix TornadoMatrixBase
    {
        get
        {
            var lifeTimer = EasingFunction.InOutSine(Timer / 90f);
            lifeTimer *= EasingFunction.InOutSine((float)Projectile.timeLeft / 90f);
            var scale = MathHelper.Lerp(3f, 1f, lifeTimer);
            return Matrix.CreateScale(scale, 1f, scale) * BandRotationMatrix * PerspectiveMatrix * Matrix.CreateTranslation(TornadoCenter.X, TornadoCenter.Y, 0);
        }
    }

    Matrix TornadoCullCloseMatrix
    {
        get
        {
            return TornadoMatrixBase;
        }
    }

    float TornadoBigScale => 1.54f;
    Matrix TornadoCullCloseBigMatrix
    {
        get
        {
            return Matrix.CreateScale(TornadoBigScale, 1f, TornadoBigScale) * TornadoMatrixBase;
        }
    }
    Matrix TornadoCullFarMatrix
    {
        get
        {
            return TornadoMatrixBase;
        }
    }
    Matrix TornadoCullFarBigMatrix
    {
        get
        {
            return Matrix.CreateScale(TornadoBigScale, 1f, TornadoBigScale) * TornadoMatrixBase;
        }
    }
    Color FlashColor => Color.LightGoldenrodYellow with { A = 0 };
    Color LightGoldenColor => Color.Lerp(Color.Gold, Color.Black, 0.5f);
    Color DarkGoldenColor => Color.Lerp(Color.DarkGoldenrod, Color.Black, 0.5f);
    Color BackColor => Color.Lerp(Color.Lerp(Color.White, Color.Black, 0.75f), FlashColor, _glowStrikeAlpha * 0.3f);
    Color FrontColor => Color.Lerp(Color.White, FlashColor, _glowStrikeAlpha * 0.3f);
    float _glowStrikeAlpha;
    ref float Timer => ref Projectile.ai[0];
    public override string Texture => TextureRegistry.EmptyTexture;
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.tileCollide = false;
        Projectile.timeLeft = 600;
    }
    public override bool ShouldUpdatePosition()
    {
        return true;
    }


    Vector2 TornadoTarget => Main.player[Projectile.owner].Center;
    Vector2 TornadoCenter => Projectile.Center + new Vector2(0, 64);
    Vector2 TornadoBottom => Projectile.Center + new Vector2(0, 64) + CalculateBandWarpOffsets()[0].XY();
    Vector3[] CalculateBandWarpOffsets()
    {
        Vector3[] centerOffsets = new Vector3[8];
        for (var i = 0; i < centerOffsets.Length; i++)
        {
            var mwOffset = TornadoTarget - Projectile.Center;
            mwOffset = mwOffset.SafeNormalize(Vector2.Zero) * Vector2.Distance(Projectile.Center, TornadoTarget) * 0f;
            var mwOffset3 = new Vector3(mwOffset.X, mwOffset.Y, 0);
            var ratio = (1f - ((float)i / (float)centerOffsets.Length));
            ratio = MathHelper.Lerp(ratio, 0.5f, 1f);
            centerOffsets[i] = Vector3.Lerp(Vector3.Zero, mwOffset3, ratio * 0.9f);
            centerOffsets[i].X += ExtraMath.Osc(-16, 16, speed: 6);
            centerOffsets[i].X += ExtraMath.Osc(-32, 32, speed: 6, offset: i);

        }
        return centerOffsets;
    }
    void UpdateBottomTornado()
    {
        BottomTornadoCircles.perspectiveMatrix = Matrix.CreateFromAxisAngle(new Vector3(1, 0, 0), 1.3f);

        BottomTornadoCircles.upwardVelocity = -7;
        BottomTornadoCircles.Update(Timer);

        BottomSmokeParticles.wrappedOffsets = BottomTornadoCircles.WrappedOffsets;
        BottomDustParticles.wrappedOffsets = BottomTornadoCircles.WrappedOffsets;
        BottomStreakParticles.wrappedOffsets = BottomTornadoCircles.WrappedOffsets;

        if (Projectile.timeLeft > 120)
        {
            {
                var pointToTrack = BottomTornadoCircles.GetRandPointToTrack();
                ref var offset = ref BottomTornadoCircles.WrappedOffsets[pointToTrack];
                var targetCirclePos = BottomSmokeParticles.tornadoCenter + new Vector2(offset.X, offset.Y);

                BottomSmokeParticles.Spawn(new()
                {
                    position = targetCirclePos + Main.rand.NextVector2Circular(128, 32),
                    pointToTrack = pointToTrack,
                    timeLeft = 120,
                });
            }


            {

                if (Main.rand.NextBool(6))
                {
                    var pointToTrack = BottomTornadoCircles.GetRandPointToTrack();
                    ref var offset = ref BottomTornadoCircles.WrappedOffsets[pointToTrack];
                    var targetCirclePos = BottomSmokeParticles.tornadoCenter + new Vector2(offset.X, offset.Y);

                    BottomSmokeParticles.Spawn(new()
                    {
                        position = targetCirclePos + Main.rand.NextVector2Circular(128, 32),
                        pointToTrack = pointToTrack,
                        scale = 1.5f,
                        timeLeft = 120,
                    });
                }

            }

            {
                for (var i = 0; i < 1; i++)
                {
                    var pointToTrack = Main.rand.Next(0, BottomTornadoCircles.WrappedOffsets.Length);
                    ref var offset = ref BottomTornadoCircles.WrappedOffsets[pointToTrack];
                    var targetCirclePos = BottomDustParticles.tornadoCenter + new Vector2(offset.X, offset.Y) + Main.rand.NextVector2Circular(48, 48);

                    BottomDustParticles.Spawn(new()
                    {
                        position = targetCirclePos,
                        pointToTrack = pointToTrack,
                        timeLeft = 120,
                    });
                }

            }
        }

        BottomTornadoCircles.ApplyWarp(CalculateBandWarpOffsets());
        BottomSmokeParticles.tornadoCenter = TornadoCenter;
        BottomSmokeParticles.Update();

        BottomDustParticles.tornadoCenter = TornadoCenter;
        BottomDustParticles.Update();

        BottomStreakParticles.tornadoCenter = new Vector3(TornadoCenter.X, TornadoCenter.Y, 0);
        BottomStreakParticles.Simulate();
    }

    void SimulateGroundParticles()
    {
        if (Main.netMode == NetmodeID.Server)
            return;

        var ground = TileUtilities.FallToSolidTileOrPlatformOrWater(TornadoBottom);
        var dist = Vector2.DistanceSquared(ground, TornadoBottom);
        if (dist > 232 * 232)
            return;

        if (Projectile.timeLeft <= 120)
            return;

        //Simulate ground particles at the ground position
        //First let's dust spam slightly
        var groundTile = ground.ToTileCoordinates();
        Vector2 spawnVelocity = Vector2.UnitY * Main.rand.NextFloat(-5, -15);
        int d = WorldGen.KillTile_MakeTileDust(groundTile.X, groundTile.Y, Framing.GetTileSafely(groundTile));
        Dust dust = Main.dust[d];
        dust.velocity = spawnVelocity;
        dust.noLightEmittence = true;
        dust.position.X += Main.rand.Next(-32, 32);

        ground.Y -= Main.rand.Next(32, 64);
        TornadoDestructionRenderer.DestructionMaskParticleSystem.Spawn(new()
        {
            position = ground + Main.rand.NextVector2Circular(32, 8),
            upPosition = ground + Main.rand.NextVector2Circular(32, 32) + new Vector2(0,-16),
            scale = Vector2.One * new Vector2(Main.rand.NextFloat(0.6f, 1f), Main.rand.NextFloat(1f, 5f)),
            rotation = Main.rand.NextFloat(-0.5f, 0.5f),
            color = Color.White * Main.rand.NextFloat(0.2f, 1f),
            timeLeft = 129
        });

        if (Main.rand.NextBool(12))
        {
            FXUtil.MakeSoilParticle(ground, spawnVelocity.RotatedByRandom(0.75f));
        }

        
        Particles.Particles.BitDust.Spawn(BitDustFactory.SlowingOverTime with
        {
            position = ground + new Vector2(0, 24),
            velocity = -Vector2.UnitY.RotatedByRandom(1.2f) * Main.rand.NextFloat(5, 25),
            innerColor = Color.White.ToVector4() * 0.64f,
            outerColor = Color.White.ToVector4() * 0.64f,
            scale = new Vector2(Main.rand.NextFloat(0.4f, 1.6F)),
            velocityPerTickMult = 0.85f,
            timeLeft=15
        });
        //  Particles.Particles.CrackDust.Spawn(CrackImpactDust.Data.Default with { position = ground + Main.rand.NextVector2Circular(32, 32), timeLeft = 200, color = Color.DarkOrange });
    }

    public override void AI()
    {
        base.AI();
        Timer++;
        var dist = Vector2.Distance(Projectile.Center, TornadoTarget);
        var alpha = dist / 384f;
        alpha = MathHelper.Clamp(alpha, 0f, 1f);
        alpha = 1f - alpha;
        ShakeScreenPosition.Shake = alpha * 3;
        Projectile.velocity = Vector2.Lerp(Projectile.velocity, (TornadoTarget - Projectile.Center).SafeNormalize(Vector2.Zero) * 6, 0.1f);
        Vector3[] warpOffsets = CalculateBandWarpOffsets();
        TornadoCircles.ApplyWarp(warpOffsets);
        Vector3[] warpOffsets2 = CalculateBandWarpOffsets();
        for (var i = 0; i < warpOffsets2.Length; i++)
            warpOffsets2[i].X *= 0.5f;
        FastTornadoCircles.ApplyWarp(warpOffsets2);
        TornadoCircles.perspectiveMatrix = FastTornadoCircles.perspectiveMatrix = Matrix.CreateFromAxisAngle(new Vector3(1, 0, 0), 0.4f);

        StreakParticles.wrappedOffsets = FastTornadoCircles.WrappedOffsets;
        DustParticles.wrappedOffsets = TornadoCircles.WrappedOffsets;
        SmokeParticles.wrappedOffsets = TornadoCircles.WrappedOffsets;


        StreakParticles.wrappedOffsets = FastTornadoCircles.WrappedOffsets;
        DustParticles.wrappedOffsets = TornadoCircles.WrappedOffsets;
        SmokeParticles.wrappedOffsets = TornadoCircles.WrappedOffsets;



        UpdateBottomTornado();
        SimulateGroundParticles();
        TornadoCircles.upwardVelocity = -7;
        TornadoCircles.angularVelocity = 0.1f;
        TornadoCircles.Update(Timer);

        FastTornadoCircles.upwardVelocity = -6;
        FastTornadoCircles.angularVelocity = 0.15f;
        FastTornadoCircles.Update(Timer);
        if(Projectile.timeLeft > 120)
        {
            LightningBoltParticles.Update();
            //TornadoCircles.Update(Timer);
            if (Timer % 1 == 0)
            {
                var pointToTrack = TornadoCircles.GetRandPointToTrack();
                ref var offset = ref TornadoCircles.WrappedOffsets[pointToTrack];
                var targetCirclePos = SmokeParticles.tornadoCenter + new Vector2(offset.X, offset.Y);

                SmokeParticles.Spawn(new()
                {
                    position = targetCirclePos + Main.rand.NextVector2Circular(128, 32),
                    pointToTrack = pointToTrack,
                    timeLeft = 120,
                });
            }
            if (Timer % 1 == 0)
            {
                for (var i = 0; i < 2; i++)
                {
                    var pointToTrack = Main.rand.Next(0, TornadoCircles.WrappedOffsets.Length);
                    ref var offset = ref TornadoCircles.WrappedOffsets[pointToTrack];
                    var targetCirclePos = DustParticles.tornadoCenter + new Vector2(offset.X, offset.Y) + Main.rand.NextVector2Circular(48, 48);

                    DustParticles.Spawn(new()
                    {
                        position = targetCirclePos,
                        pointToTrack = pointToTrack,
                        timeLeft = 120,
                    });
                }

            }
            if (Timer % 2 == 0)
            {
                var pointToTrack = FastTornadoCircles.GetRandPointToTrack();
                ref var offset = ref FastTornadoCircles.WrappedOffsets[pointToTrack];
                var targetCirclePos = DustParticles.tornadoCenter + new Vector2(offset.X, offset.Y) + Main.rand.NextVector2Circular(48, 48);
                var targetCirclePos3 = new Vector3(targetCirclePos + Main.rand.NextVector2CircularEdge(32, 32), 0);

                StreakParticles.Spawn(new()
                {
                    position = targetCirclePos3,
                    pointToTrack = pointToTrack,
                    timeLeft = 120,
                });
            }
            if (Main.rand.NextBool(7))
            {
                var pos = TornadoCenter + Main.rand.NextVector2Circular(275, 512);
                pos.Y -= 256;
                var vel = -Vector2.UnitY;
                vel *= 3 * Main.rand.NextFloat(1f, 1.6f);
                vel = vel.RotatedBy(Main.rand.NextFloat(0, 6.28f));

                var color = Color.Lerp(LightGoldenColor, DarkGoldenColor, Main.rand.NextFloat(0f, 1f));
                color = Color.Lerp(color, Color.White, 0.6f);


                Common.Particles.Particles.LightningSpark.Spawn(new()
                {
                    position = pos,
                    velocity = vel,
                    color = color,
                    scale = Main.rand.NextFloat(0.7f, 1.5f) * 0.3f,
                    timeLeft = Main.rand.Next(25, 45)
                });

            }

            if (Main.rand.NextBool(1))
            {
                var pos = TornadoCenter + Main.rand.NextVector2Circular(275, 512);
                pos.Y -= 256;
                var vel = -Vector2.UnitY;
                vel *= 2;
                vel = vel.RotatedBy(Main.rand.NextFloat(0, 6.28f));

                var color = Color.Lerp(LightGoldenColor, DarkGoldenColor, Main.rand.NextFloat(0f, 1f));
                color = Color.Lerp(color, Color.White, 0.6f);


                Common.Particles.Particles.LightningSpikeySpark.Spawn(new()
                {
                    position = pos,
                    velocity = vel,
                    scale = Main.rand.NextFloat(0.4f, 0.8f),
                    color = color,
                    timeLeft = 25
                });
            }

            if (Main.rand.NextBool(4))
            {
                var pos = TornadoCenter + Main.rand.NextVector2Circular(275, 512);
                pos.Y -= 512;
                var vel = -Vector2.UnitY;
                vel *= 6;
                vel = vel.RotatedBy(Main.rand.NextFloat(0, 6.28f));

                var color = Color.Lerp(Color.Gold, Color.DarkOrange, Main.rand.NextFloat(0.6f, 1f));
                FlashParticles.Spawn(new()
                {
                    flashOffset = (pos - TornadoCenter  ),
                    timeLeft = 25,
                    color = color,
                    scale = 1f
                });

                color = Color.Lerp(LightGoldenColor, DarkGoldenColor, Main.rand.NextFloat(0f, 1f));
                color = Color.Lerp(color, Color.White, 0.6f);


                for(var i = 0; i < 3; i++)
                {
                    Common.Particles.Particles.LightningSpikeySpark.Spawn(new()
                    {
                        position = pos + Main.rand.NextVector2Circular(97, 98),
                        velocity = vel.RotatedByRandom(0.5f),
                        scale = Main.rand.NextFloat(0.4f, 0.8f),
                        color = color,
                        timeLeft = 50
                    });
                }
                if (Main.rand.NextBool(4))
                {
                    FXUtil.GlowCircleBoom(pos, Color.Gold, Color.DarkOrange, Color.DarkBlue, 20, baseSize: 0.18f);
                    var fx2 = FXUtil.GlowCircleBoom(pos, Color.Gold, Color.DarkOrange, Color.DarkBlue, 20, baseSize: 0.18f);
                    fx2.VectorScale *= 8;
                    fx2.GlowColor *= 0.2f;
                    fx2.InnerColor *= 0.2f;
                    fx2.OuterGlowColor *= 0.2f;
                    LightningBoltParticles.Spawn(new()
                    {
                        position = pos,
                        velocity = vel,
                        timeLeft = 90,
                        power = 1,
                        rotation = vel.ToRotation()
                    });
                    _glowStrikeAlpha = 1f;
                }
            
            }
        }
        _glowStrikeAlpha = MathHelper.Lerp(_glowStrikeAlpha, 0f, 0.03f);
        FlashParticles.tornadoCenter = TornadoCenter;
        FlashParticles.Update();

        SmokeParticles.tornadoCenter = TornadoCenter;
        SmokeParticles.Update();

        DustParticles.tornadoCenter = TornadoCenter;
        DustParticles.Update();

        StreakParticles.tornadoCenter = new Vector3(TornadoCenter.X, TornadoCenter.Y, 0);
        StreakParticles.Simulate();

        LightningBoltParticles.Update();
    }


    void DrawTornado()
    {
        var spriteBatch = Main.spriteBatch;
        spriteBatch.EndOut(out var oldParameters);

        //First we draw the tornado to a render target
        using var tornadoRT = RT.Context(RenderTargets.ScreenTarget);
        using var flashRT = RT.Context(RenderTargets.ScreenTarget);
        using(RT.Clear(flashRT, Color.Transparent))
        {
            //Now we draw the flash particles over top
            FlashParticles.Draw(Main.spriteBatch, Main.screenPosition);
        }
        using(RT.Clear(tornadoRT, Color.Transparent))
        {
            var gDevice = spriteBatch.GraphicsDevice;
            gDevice.RasterizerState = RasterizerState.CullNone;
            gDevice.BlendState = BlendState.AlphaBlend;
            gDevice.SamplerStates[0] = SamplerState.LinearWrap;

            var pass = AssetReferences.Effects.Weather.ThunderingTornado.CreatePrimitivesPass();
            var ditherTexture = AssetReferences.Assets.Dithering.Dither8x8DoubleScaled.Asset;

            var warpOffsets = CalculateBandWarpOffsets();
            pass.Parameters.warps = warpOffsets;
            pass.Parameters.wvpMatrix = TrailDrawer.WorldViewPointCullClose;

            var lifeTimer = EasingFunction.InOutSine(Timer / 90f);
            lifeTimer *= EasingFunction.InOutSine((float)Projectile.timeLeft / 90f);
            pass.Parameters.lifeTimer = lifeTimer;
            pass.Parameters.time = Main.GlobalTimeWrappedHourly;
            pass.Parameters.transformMatrix = TornadoCullCloseBigMatrix;
            pass.Parameters.rotationMatrix = BandRotationMatrix;
            pass.Parameters.tintColor = BackColor.ToVector4() * 0.15f;
            pass.Parameters.dir = 1f;
            pass.Parameters.ditherTexelSize = ditherTexture.GetTexelSize();
            pass.Parameters.spriteSize = AssetReferences.Assets.Noise.TornadoNoise.Asset.Value.Size() / new Vector2(16, 8);
            pass.Parameters.ditherSampler = new()
            {
                Texture = ditherTexture.Value,
                Sampler = SamplerState.PointWrap
            };
            pass.Apply();
            Main.graphics.GraphicsDevice.Textures[0] = AssetReferences.Assets.Noise.TornadoNoise.Asset.Value;
            BigCylinderMesh.Draw(gDevice);

            pass.Parameters.time = Main.GlobalTimeWrappedHourly;
            pass.Parameters.transformMatrix = TornadoCullCloseMatrix;
            pass.Parameters.rotationMatrix = BandRotationMatrix;
            pass.Parameters.tintColor = BackColor.ToVector4();
            pass.Parameters.dir = 1f;
            pass.Parameters.ditherTexelSize = ditherTexture.GetTexelSize();
            pass.Parameters.spriteSize = AssetReferences.Assets.Noise.TornadoNoise.Asset.Value.Size() / new Vector2(16, 8);
            pass.Parameters.ditherSampler = new()
            {
                Texture = ditherTexture.Value,
                Sampler = SamplerState.PointWrap
            };
            pass.Apply();
            CylinderMesh.Draw(gDevice);

            pass.Parameters.time = Main.GlobalTimeWrappedHourly;
            pass.Parameters.wvpMatrix = TrailDrawer.WorldViewPointCullFarAway;
            pass.Parameters.transformMatrix = TornadoCullFarMatrix;
            pass.Parameters.rotationMatrix = BandRotationMatrix;
            pass.Parameters.tintColor = FrontColor.ToVector4();
            pass.Parameters.dir = -0.65F;
            pass.Apply();
            CylinderMesh.Draw(gDevice);

            pass.Parameters.time = Main.GlobalTimeWrappedHourly;
            pass.Parameters.transformMatrix = TornadoCullFarBigMatrix;
            pass.Parameters.rotationMatrix = BandRotationMatrix;
            pass.Parameters.tintColor = FrontColor.ToVector4() * 0.39f;
            pass.Parameters.dir = -0.65F;
            pass.Apply();
            BigCylinderMesh.Draw(gDevice);

        }



        spriteBatch.Begin(oldParameters);
        spriteBatch.Draw(tornadoRT, Vector2.Zero, Color.White);
        spriteBatch.End();

        MaskCombineShader maskCombine = MaskCombineShader.Instance;
        maskCombine.MixTexture = flashRT;
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone,
           maskCombine.Effect, Main.GameViewMatrix.TransformationMatrix);



        spriteBatch.Draw(tornadoRT, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        spriteBatch.End();
        spriteBatch.Begin(oldParameters);
    }

    void DrawPixelated(SpriteBatch sb, Vector2 sp)
    {
        SmokeParticles.alpha = 0.145f;
        SmokeParticles.Draw(Main.spriteBatch, Main.screenPosition);
        BottomSmokeParticles.alpha = 0.145f;
        BottomSmokeParticles.Draw(Main.spriteBatch, Main.screenPosition);
        DrawTornado();
        DustParticles.Draw(Main.spriteBatch, Main.screenPosition);
        BottomDustParticles.Draw(Main.spriteBatch, Main.screenPosition);
        SmokeParticles.alpha = 0.145f;
        SmokeParticles.Draw(Main.spriteBatch, Main.screenPosition);
        BottomSmokeParticles.alpha = 0.145f;
        BottomSmokeParticles.Draw(Main.spriteBatch, Main.screenPosition);
        LightningBoltParticles.Draw(Main.spriteBatch, Main.screenPosition);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        PixelationManager.QueueSpritebatchDrawAction(DrawPixelated, DrawLayer.OverNPCs);
        PixelationManager.QueuePrimitivesDrawAction(StreakParticles.DrawStreaks, DrawLayer.OverPlayers);
        return false;
    }
}
