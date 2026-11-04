using Stellamod.Common;
using Stellamod.Common.Particles;
using Stellamod.Core.Pixelation;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Illuria.BossesIL.Niivi.Projectiles;

public class NiiviLightningStrike : ModProjectile
{
    public struct MiniLightningStrikeData
    {
        public Vector2 randScale;
        public int index;
        public float randRotation;

        public float randProgress;
    }

    public struct LightningStrikeData
    {
        public float randWidth;
        public float randRotation;
        public float randHue;
    }
    enum AIState
    {
        Telegraph,
        Strike
    }
    public override string Texture => ModType.NoTexture;
    float _randIndex;
    float Y
    {
        get
        {
            var domainExpansion = ModContent.GetInstance<DomainExpansionManager>();
            return domainExpansion.hoverPlatformY + 48;
        }
    }
    int NumStrikes => 16;
    LightningStrikeData[] StrikeData
    {
        get
        {
            if(field == null)
            {
                field = new LightningStrikeData[NumStrikes];
                for(var i = 0; i < NumStrikes; i++)
                {
                    ref var data = ref field[i];
                    data.randWidth = Main.rand.NextFloat(0.6f, 1f);
                    data.randRotation = Main.rand.NextFloat(-0.05f, 0.05f);
                    data.randHue = Main.rand.NextFloat(0f, 1f);
                }
            }
            return field;
        }
    }
    MiniLightningStrikeData[] MiniStrikeData
    {
        get
        {
            if(field == null)
            {
                field = new MiniLightningStrikeData[NumStrikes];
                for(var i = 0; i < NumStrikes; i++)
                {
                    ref var data = ref field[i];
                    data.randProgress = Main.rand.NextFloat(0f, 1f);
                    data.randScale = new Vector2(Main.rand.NextFloat(0.6f, 1f), Main.rand.NextFloat(0.8f, 1f)) * Main.rand.NextFloat(0.2f, 0.3f);
                    data.randRotation = Main.rand.NextFloat(-0.2f, 0.2f);
                    data.index = Main.rand.Next(NumStrikes);
                }
            }
            return field;
        }
    }
    float[] StrikeTimers => field ??= new float[NumStrikes];
    AIState[] StrikeStates => field ??= new AIState[NumStrikes];


    Vector2 Top => Projectile.Center + new Vector2(0, -StrikeHeight);
    Vector2 TopLeft => Top + new Vector2(-StrikeRadius, 0);
    Vector2 TopRight => Top + new Vector2(StrikeRadius, 0);
    float StrikeHeight => 1444;
    float StrikeRadius => 1024;
    float StrikeTime => 30;
    ref float TelegraphTime => ref Projectile.ai[2];
    AIState State
    {
        get => (AIState)Projectile.ai[1];
        set => Projectile.ai[1] = (float)value;
    }
    ref float Timer => ref Projectile.ai[0];
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.hostile = true;
        Projectile.timeLeft = 1200;
        Projectile.light = 0.5f;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
    }

    public override bool ShouldUpdatePosition()
    {
        return false;
    }

    public override void OnSpawn(IEntitySource source)
    {
        base.OnSpawn(source);
        _randIndex = Main.rand.Next(NumStrikes / 2 - 4, NumStrikes / 2 + 4);
    }

    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {
        for (var i = 0; i < NumStrikes; i++)
        {
            ref var state = ref StrikeStates[i];
            if (_randIndex == i)
                continue;
            if (state == AIState.Telegraph)
                continue;
            if (state == AIState.Telegraph)
                continue;

            ref var timer = ref StrikeTimers[i];
            if (timer >= StrikeTime * 0.3f)
                continue;

            var ratio = i / (float)NumStrikes;
            var startPOsition = Vector2.Lerp(TopLeft, TopRight, ratio);
            var endPosition = startPOsition;
            endPosition.Y = Y;
            if (CollisionHelper.LaserCollision(projHitbox, targetHitbox, startPOsition, endPosition, 128))
                return true;
        }
        return false;
    }

    public override bool CanHitPlayer(Player target)
    {
        return base.CanHitPlayer(target);
    }


    bool NoStrikesLeft()
    {
        for (var k = 0; k < StrikeStates.Length; k++)
        {
            ref var state = ref StrikeStates[k];
            ref var timer = ref StrikeTimers[k];
            if (state == AIState.Telegraph)
                return false;
            if (state == AIState.Strike && timer < StrikeTime)
                return false;
        }
        return true;
    }
    public override void AI()
    {
        base.AI();
        if (NoStrikesLeft())
            Projectile.Kill();
        Projectile.Center = new Vector2(Projectile.Center.X, Y);
        Timer++;
        if (Timer == 1)
        {
            for (var k = 0; k < StrikeStates.Length; k++)
            {
                ref var state = ref StrikeStates[k];
                ref var timer = ref StrikeTimers[k];
                timer -= k * 2;
            }

            var zap = AssetReferences.Assets.Sounds.StormDragon_LightingRain.Asset with { PitchVariance = 0.4f };
            SoundEngine.PlaySound(zap);
        }

        for (var k = 0; k < StrikeStates.Length; k++)
        {
            ref var state = ref StrikeStates[k];
            ref var timer = ref StrikeTimers[k];
            timer++;
            switch (state)
            {
                case AIState.Telegraph:
                    {
                        if (timer >= TelegraphTime)
                        {
                            timer = 0;
                            state = AIState.Strike;
                        }
                    }

                    break;
                case AIState.Strike:
                    {
                        if (_randIndex == k)
                            continue;
                        if (timer == 1)
                        {

                            var zap = AssetReferences.Assets.Sounds.StormDragon_LightingZap.Asset with { PitchVariance = 0.4f };
                            SoundEngine.PlaySound(zap);

                            var ratio = k / (float)NumStrikes;
                            var startPosition = Vector2.Lerp(TopLeft, TopRight, ratio);
                            var endPosition = startPosition + new Vector2(0, StrikeHeight);
                            var vel = (endPosition - startPosition).SafeNormalize(Vector2.Zero) * 4;


                            void Blast()
                            {
                                var currentPosition = Projectile.Center;
                                var fx = FXUtil.GlowCircleBoom(endPosition, DrawUtilities.Prismatic, DrawUtilities.Prismatic, Color.Black, 24);
                                fx.Scale *= 2.2F;

                                var fx2 = FXUtil.GlowCircleBoom(endPosition, Color.White, DrawUtilities.Prismatic, Color.Black, 24);
                                fx2.Scale *= 1.5f;

                                var pos = Main.rand.NextVector2Circular(7, 7) + endPosition;
                                var color = DrawUtilities.Prismatic.ToVector4();
                                for (var i = 0; i < 10; i++)
                                {
                                    Particles.SwirlingFlameDust.Spawn(BitDustFactory.SlowingOverTime with
                                    {
                                        position = pos,
                                        velocity = -Vector2.UnitY.RotatedByRandom(3.4f) * Main.rand.NextFloat(20, 35),
                                        timeLeft = 60,
                                        innerColor = Vector4.Lerp(color, Vector4.One, 0.65f),
                                        outerColor = color,
                                        scale = new Vector2(Main.rand.NextFloat(0.3f, 0.8f))
                                    });
                                }

                                Particles.DiamondSparkle.Spawn(new()
                                {
                                    position = endPosition + Main.rand.NextVector2Circular(7, 7),
                                    timeLeft = 30,
                                    color = DrawUtilities.Prismatic
                                });

                                foreach (PositionVelocity posVel in new CircleOrientation(endPosition, 32, 8))
                                {
                                    Particles.GlowDust.Spawn(new()
                                    {
                                        position = posVel.position,
                                        velocity = posVel.velocity * Main.rand.NextFloat(0.5f, 1f) * 0.3f,
                                        color = DrawUtilities.Prismatic,
                                        timeLeft = Main.rand.Next(45, 120),
                                        scale = Main.rand.NextFloat(0.15f, 0.25f) * 2
                                    });
                                }

                                for(var i = 0; i < 3; i++)
                                {
                                    var vel = -Vector2.UnitY.RotatedByRandom(1f) * 8;
                                    var fxs = FXUtil.GlowStretch(endPosition, vel);
                                    fxs.InnerColor = Color.White;
                                    fxs.OuterGlowColor = Color.Black;
                                    fxs.GlowColor = Color.DarkGray;
                                    fxs.VectorScale.X *= 0.5F;
                                }

                                Particles.NiiviLightningImpact.Spawn(new()
                                {
                                    position = endPosition,
                                    timeLeft = 30,
                                    color = Color.Lerp(Color.White, DrawUtilities.Prismatic, 0.3f),
                                    scale = new Vector2(Main.rand.NextFloat(0.86f, 1f), 1f)
                                });
                            }

                            if(timer == 15)
                            {
                                var soundStyle = AssetReferences.Assets.Sounds.LeviathanEel.LeviZap3.Asset;
                                var randIndex = Main.rand.Next(2);
                                switch (randIndex)
                                {
                                    case 1:
                                        {
                                            soundStyle = AssetReferences.Assets.Sounds.LeviathanEel.LeviZap4.Asset;
                                        }
                                        break;
                                }

                                soundStyle = soundStyle with { PitchVariance = 0.5F, MaxInstances = 3, Volume = 0.5F };
                                SoundEngine.PlaySound(soundStyle, endPosition);
                                var fx = FXUtil.GlowCircleBoom(endPosition, DrawUtilities.Prismatic * 0.6F, DrawUtilities.Prismatic * 0.6F, Color.Black, 24);
                                fx.Scale *= 1f;


                            }

                            Blast();
                            FXUtil.ShakeCamera(Projectile.Center, 1024, 10);
                            for (var j = 0; j < 24; j++)
                            {
                                var pos = Vector2.Lerp(startPosition, endPosition, Main.rand.NextFloat(0f, 1f));
                                pos += Main.rand.NextVector2Circular(32, 32);
                                Particles.SwirlingFlameDust.Spawn(BitDustFactory.SlowingOverTime with
                                {
                                    position = pos,
                                    velocity = vel.RotatedByRandom(1.5F) * 6,
                                    timeLeft = 60,
                                    innerColor = Color.White.ToVector4(),
                                    outerColor = DrawUtilities.Prismatic.ToVector4(),
                                    scale = new Vector2(Main.rand.NextFloat(0.3f, 1f)),
                                });
                            }
                        }
                    }
                    break;
            }
        }
    }
    void DrawPixelatedTelegraph(SpriteBatch sb, Vector2 sp)
    {

        var drawer = SpritebatchDrawer.FromTextureGetLightingColor(AssetReferences.Assets.Textures.Illuria.NPCs.HappyBrick.Asset, Projectile.Center);
        drawer.scale *= 0.75F;
        //drawer.scale.Y *= 2f;
        drawer.BottomCenterOrigin();

        var halfTime = TelegraphTime * 0.5f;

        using (sb.Ctx(sb.Parameters with { blendState = BlendState.Additive }))
        {
            for (var i = 0; i < NumStrikes; i++)
            {
                if (_randIndex == i)
                    continue;
                ref var timer = ref StrikeTimers[i];
                ref var state = ref StrikeStates[i];
                if (state == AIState.Strike)
                    continue;
                ref var strikeData = ref StrikeData[i];
                var ratio = i / (float)NumStrikes;
                var startPosition = Vector2.Lerp(TopLeft, TopRight, ratio);
                startPosition.Y = Y;



                var scrollingColor = Color.Red;
                scrollingColor.ScrollHue(i * 24 + timer);

                var osc = ExtraMath.Osc(0.92f, 1f, speed: 32);
                var osc2 = ExtraMath.Osc(0.7f, 2f, speed: 6, offset: i);
                var osc3 = osc * osc2;
                scrollingColor *= osc3;
                drawer.color = scrollingColor * MathHelper.Lerp(1f, 0f, EasingFunction.InOutSine(timer / TelegraphTime)) * ExtraMath.Osc(0.6f, 1f, speed: 16);

                var telegraphRatio = timer / halfTime;
                var a = MathHelper.Lerp(0f, 1f, EasingFunction.QuadraticBump(telegraphRatio)) * 2f;
                drawer.color *= a;
                //   drawer.color.A = 0;
                drawer.worldPosition = startPosition;
                sb.Draw(drawer);

                var lineDrawer = SpritebatchDrawer.FromTextureGetLightingColor(AssetReferences.Assets.GlowMasks.BlastPillar.Asset, startPosition);
                lineDrawer.BottomCenterOrigin();
                lineDrawer.worldPosition.Y -= 52;
                lineDrawer.color = scrollingColor * a * osc3 * 0.85F;
                // lineDrawer.color.A = 0;
                lineDrawer.scale.X *= 0.1f;
                sb.Draw(lineDrawer);

            }
        }

    }

    void DrawPixelatedStrike(SpriteBatch sb, Vector2 sp)
    {
        var drawer = SpritebatchDrawer.FromTextureWhite(AssetReferences.Assets.GlowMasks.SingleLightningStrike.Asset);
        drawer.LeftCenterOrigin();

        var halfTime = TelegraphTime * 0.5f;
        var pass = AssetReferences.Effects.Niivi.PrismLightningStrike.CreatePixelPass();
        pass.Parameters.time = Main.GlobalTimeWrappedHourly;
        pass.Apply();
        using (sb.Ctx(sb.Parameters with { blendState = BlendState.Additive, effect = pass.Shader }))
        {
            for(var i = 0; i < MiniStrikeData.Length; i++)
            {
                ref var miniStrikeData = ref MiniStrikeData[i];
                if (miniStrikeData.index == _randIndex)
                    continue;
                ref var timer = ref StrikeTimers[miniStrikeData.index];
                ref var state = ref StrikeStates[miniStrikeData.index];
                if (state == AIState.Telegraph)
                    continue;

                var ratio = (float)miniStrikeData.index / (float)NumStrikes;
                var startPosition = Vector2.Lerp(TopLeft, TopRight, ratio);
                var scrollingColor = Color.White;

                ref var strikeData = ref StrikeData[miniStrikeData.index];

                var hueOffset = ((float)i * 24F + timer + strikeData.randHue * 6F);
                hueOffset /= 360F;
                hueOffset %= 1.0F;

                var osc = ExtraMath.Osc(0.92f, 1f, speed: 32);
                var osc2 = ExtraMath.Osc(0.7f, 2f, speed: 6, offset: i);
                var osc3 = osc * osc2;

                var telegraphRatio = timer / halfTime;
                var a = MathHelper.Lerp(0f, 1f, EasingFunction.QuadraticBump(telegraphRatio)) * 2f;
                var alpha = a * osc3 * MathHelper.Lerp(1f, 0f, EasingFunction.InOutSine(timer / TelegraphTime)) * ExtraMath.Osc(0.6f, 1f, speed: 16);
                var strikeColor = new Color(1.0f, 1.0f, hueOffset, alpha);
                drawer.color = strikeColor;
                drawer.worldPosition = startPosition + new Vector2(16, 0);
                drawer.worldPosition.Y += MathHelper.Lerp(0f, StrikeHeight, miniStrikeData.randProgress);

                drawer.scale.X = StrikeHeight / drawer.Width;
                drawer.scale.X *= strikeData.randWidth;

                drawer.scale.X *= MathHelper.Lerp(1.06f, 1f, EasingFunction.OutExpo(timer / StrikeTime));
                drawer.scale.Y = MathHelper.Lerp(2f, 1f, EasingFunction.OutSine(timer / StrikeTime));
                drawer.scale *= miniStrikeData.randScale;

                drawer.rotation = Vector2.UnitY.ToRotation();
                drawer.rotation += strikeData.randRotation;
                drawer.rotation += miniStrikeData.randRotation;

                if (i % 2 == 0)
                {
                    drawer.spriteEffects = SpriteEffects.FlipVertically;
                }
                else
                {
                    drawer.spriteEffects = SpriteEffects.None;
                }

                var bloomDrawer = drawer;
                bloomDrawer.scale.Y *= 1.5f;
                bloomDrawer.color.A -= 50;
                sb.Draw(bloomDrawer);
                sb.Draw(drawer);
            }

            for (var i = 0; i < NumStrikes; i++)
            {
                if (_randIndex == i)
                    continue;
                ref var timer = ref StrikeTimers[i];
                ref var state = ref StrikeStates[i];
                if (state == AIState.Telegraph)
                    continue;

                var ratio = i / (float)NumStrikes;
                var startPosition = Vector2.Lerp(TopLeft, TopRight, ratio);
                var scrollingColor = Color.White;

                ref var strikeData = ref StrikeData[i];
       
                var hueOffset = ((float)i * 24F + timer + strikeData.randHue * 6F);
                hueOffset /= 360F;
                hueOffset %= 1.0F;

                var osc = ExtraMath.Osc(0.92f, 1f, speed: 32);
                var osc2 = ExtraMath.Osc(0.7f, 2f, speed: 6, offset: i);
                var osc3 = osc * osc2;

                var telegraphRatio = timer / halfTime;
                var a = MathHelper.Lerp(0f, 1f, EasingFunction.QuadraticBump(telegraphRatio)) * 2f;
                var alpha = a *  osc3 * MathHelper.Lerp(1f, 0f, EasingFunction.InOutSine(timer / TelegraphTime)) * ExtraMath.Osc(0.6f, 1f, speed: 16);
                var strikeColor = new Color(1.0f, 1.0f, hueOffset, alpha);
                drawer.color = strikeColor;
                drawer.worldPosition = startPosition + new Vector2(16, 0);
                drawer.scale.X = StrikeHeight / drawer.Width;
                drawer.scale.X *= strikeData.randWidth;
                
                drawer.scale.X *= MathHelper.Lerp(1.06f, 1f, EasingFunction.OutExpo(timer / StrikeTime));
                drawer.scale.Y = MathHelper.Lerp(2f, 1f, EasingFunction.OutSine(timer / StrikeTime));

                drawer.rotation = Vector2.UnitY.ToRotation();
                drawer.rotation += strikeData.randRotation;
                if (i % 2 == 0)
                {
                    drawer.spriteEffects = SpriteEffects.FlipVertically;
                }
                else
                {
                    drawer.spriteEffects = SpriteEffects.None;
                }

                var bloomDrawer = drawer;
                bloomDrawer.scale.Y *= 1.5f;
                bloomDrawer.color.A -= 50;
                sb.Draw(bloomDrawer);
                sb.Draw(drawer);
            }
        }
    }

    public override bool PreDraw(ref Color lightColor)
    {
        PixelationManager.QueueSpritebatchDrawAction(DrawPixelatedTelegraph);
        PixelationManager.QueueSpritebatchDrawAction(DrawPixelatedStrike);
        return false;
    }
}
