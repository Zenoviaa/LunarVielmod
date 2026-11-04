using Stellamod.Common;
using Stellamod.Common.Particles;
using Stellamod.Common.Shaders;
using Stellamod.Core.Pixelation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Illuria.BossesIL.Niivi.Projectiles;

public class NiiviLightningStrike : ModProjectile
{
    enum AIState
    {
        Telegraph,
        Strike
    }
    public override string Texture => ModType.NoTexture;
    float _telegraphAlpha;
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
        for(var i = 0; i < NumStrikes; i++)
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
      
            var ratio = (float)i / (float)NumStrikes;
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
        for(var k = 0; k < StrikeStates.Length; k++)
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
            for(var k = 0; k < StrikeStates.Length; k++)
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
                        if(timer == 1)
                        {
                 
                            var zap = AssetReferences.Assets.Sounds.StormDragon_LightingZap.Asset with { PitchVariance = 0.4f };
                            SoundEngine.PlaySound(zap);

                            var ratio = (float)k / (float)NumStrikes;
                            var startPosition = Vector2.Lerp(TopLeft, TopRight, ratio);
                            var endPosition = startPosition + new Vector2(0, StrikeHeight);
                            var vel = (endPosition - startPosition).SafeNormalize(Vector2.Zero) * 4;


                            void Blast()
                            {
   
                                var currentPosition = Projectile.Center;
                                var fx = FXUtil.GlowCircleBoom(endPosition, DrawUtilities.Prismatic, DrawUtilities.Prismatic, Color.Black, 24);
                                fx.Scale *= 2;

                                var fx2 = FXUtil.GlowCircleBoom(endPosition, Color.White, DrawUtilities.Prismatic, Color.Black, 24);
                                fx2.Scale *= 1.5f;

                                var pos = Main.rand.NextVector2Circular(7, 7) + endPosition;
                                var color = DrawUtilities.Prismatic.ToVector4();
                                for (var i = 0; i < 12; i++)
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
                            }

                            Blast();
                            FXUtil.ShakeCamera(Projectile.Center, 1024, 8);
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

    void SwitchState(AIState state)
    {
        if (this.OwnedByLocalClient())
        {
            Timer = 0;
            State = state;
            Projectile.netUpdate = true;
        }
    }
    void AI_Telegraph()
    {

        _telegraphAlpha = Timer / TelegraphTime;
        if (Timer >= TelegraphTime)
        {
            SwitchState(AIState.Strike);
        }
    }

    void AI_Strike()
    {     

     
        Timer++;
        if(Timer == 1)
        {
            var zap = AssetReferences.Assets.Sounds.StormDragon_LightingZap.Asset with { PitchVariance = 0.4f };
            SoundEngine.PlaySound(zap);

            for (var i = 0; i < NumStrikes; i++)
            {
                if (_randIndex == i)
                    continue;
                var ratio = (float)i / (float)NumStrikes;
                var startPosition = Vector2.Lerp(TopLeft, TopRight, ratio);
                var endPosition = startPosition + new Vector2(0, StrikeHeight);
                var vel = (endPosition - startPosition).SafeNormalize(Vector2.Zero) * 4;

                for (var j = 0; j < 24; j++)
                {
                    var pos = Vector2.Lerp(startPosition, endPosition, Main.rand.NextFloat(0f, 1f));
                    pos += Main.rand.NextVector2Circular(32, 32);
                    Particles.SwirlingFlameDust.Spawn(BitDustFactory.SlowingOverTime with
                    {
                        position = pos,
                        velocity = vel.RotatedByRandom(1.5F) * 6,
                        timeLeft = 60,
                        innerColor = DrawUtilities.Prismatic.ToVector4(),
                        outerColor = DrawUtilities.Prismatic.ToVector4(),
                        scale = new Vector2(Main.rand.NextFloat(0.3f, 0.5F)),
                    });
                }

            }

        }
        _telegraphAlpha = MathHelper.Lerp(1f, 0f, Timer / StrikeTime);
        if (Timer >= StrikeTime)
        {
            Projectile.Kill();
        }
    }

    float GetTrailWidth(float ratio)
    {
        return MathHelper.Lerp(MathHelper.Lerp(0, 154, EasingFunction.OutQuad(ratio)), 0, EasingFunction.OutSine(Timer / StrikeTime));
    }

    float GetTrailWidth2(float ratio)
    {
        return GetTrailWidth(ratio) * 1.3f;
    }

    Color GetTrailColor(float ratio)
    {
        return Color.White;
    }

    Color GetTrailColor2(float ratio)
    {
        Color prismatic = Color.Red;
        prismatic.ScrollHue(Timer * 24 + ratio * 255);
        prismatic = Color.Lerp(prismatic, Color.White, 0.9F);
        return prismatic * 0.4f;
    }

    void DrawPixelatedTelegraph(SpriteBatch sb, Vector2 sp)
    {

        var drawer = SpritebatchDrawer.FromTextureAsset(AssetReferences.Assets.Textures.Illuria.NPCs.HappyBrick.Asset, Projectile.Center);
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
                var ratio = (float)i / (float)NumStrikes;
                var startPosition = Vector2.Lerp(TopLeft, TopRight, ratio);
                startPosition.Y =Y;
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

                var lineDrawer = SpritebatchDrawer.FromTextureAsset(AssetReferences.Assets.GlowMasks.BlastPillar.Asset, startPosition);
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

        var drawer = SpritebatchDrawer.FromTextureAsset(AssetReferences.Assets.GlowMasks.SingleLightningStrike.Asset, Projectile.Center);
        //drawer.scale.Y *= 2f;
        drawer.LeftCenterOrigin();

        var halfTime = TelegraphTime * 0.5f;
        var pass = AssetReferences.Effects.Niivi.PrismLightningStrike.CreatePixelPass();
        pass.Parameters.time = Main.GlobalTimeWrappedHourly;
        pass.Apply();
        using (sb.Ctx(sb.Parameters with { blendState = BlendState.Additive, effect = pass.Shader }))
        {
            for (var i = 0; i < NumStrikes; i++)
            {
                if (_randIndex == i)
                    continue;
                ref var timer = ref StrikeTimers[i];
                ref var state = ref StrikeStates[i];
                if (state == AIState.Telegraph)
                    continue;

                var ratio = (float)i / (float)NumStrikes;
                var startPosition = Vector2.Lerp(TopLeft, TopRight, ratio);
                var scrollingColor = Color.White;
             
                var osc = ExtraMath.Osc(0.92f, 1f, speed: 32);
                var osc2 = ExtraMath.Osc(0.7f, 2f, speed: 6, offset: i);
                var osc3 = osc * osc2;
                scrollingColor *= osc3;
                drawer.color = scrollingColor * MathHelper.Lerp(1f, 0f, EasingFunction.InOutSine(timer / StrikeTime)) * ExtraMath.Osc(0.6f, 1f, speed: 16);

                var telegraphRatio = timer / halfTime;
                var a = MathHelper.Lerp(0f, 1f, EasingFunction.QuadraticBump(telegraphRatio)) * 2f;
                drawer.color *= a;
                //   drawer.color.A = 0;
                drawer.worldPosition = startPosition + new Vector2(16, 0);
                drawer.scale.X = StrikeHeight / (float)drawer.Width;
                drawer.scale.X *= MathHelper.Lerp(1.06f, 1f, EasingFunction.OutExpo(timer / StrikeTime));
                drawer.scale.Y = MathHelper.Lerp(2f, 1f, EasingFunction.OutSine(timer / StrikeTime));

                drawer.rotation = Vector2.UnitY.ToRotation();
                if(i % 2== 0)
                {
                    drawer.spriteEffects = SpriteEffects.FlipVertically;
                }
                else
                {
                    drawer.spriteEffects = SpriteEffects.None;
                }
                var bloomDrawer = drawer;
                bloomDrawer.scale.Y *= 1.5f;
                bloomDrawer.color *= 0.6F;
                sb.Draw(bloomDrawer);
                sb.Draw(drawer);

                var flickerDrawer = drawer;
                drawer.color = Color.Lerp(Color.Black, Color.White, EasingFunction.InExpo(timer / StrikeTime));
                sb.Draw(flickerDrawer);
            }
        }

    }

    void DrawPixelatedBeam(GraphicsDevice gDevice)
    {
        int beamIndex = 0;

        float GetTrailWidth(float ratio)
        {
            var timer = StrikeTimers[beamIndex];
            return MathHelper.Lerp(MathHelper.Lerp(0, 154, EasingFunction.OutQuad(ratio)), 0, EasingFunction.OutSine(timer / StrikeTime));
        }

        float GetTrailWidth2(float ratio)
        {
            return GetTrailWidth(ratio) * 1.3f;
        }

        Color GetTrailColor(float ratio)
        {
            return Color.White;
        }

        Color GetTrailColor2(float ratio)
        {
            var timer = StrikeTimers[beamIndex];
            Color prismatic = Color.Red;
            prismatic.ScrollHue(Timer * 24 + ratio * 255);
            prismatic = Color.Lerp(prismatic, Color.White, 0.9F);
            return prismatic * 0.4f;
        }

        BloomTrailShader bloomTrailShader = BloomTrailShader.Instance;
        bloomTrailShader.InnerColor = Main.DiscoColor;
        bloomTrailShader.OuterColor = Main.DiscoColor;

        var offset = 24;
        TrailDrawer.ClearPrimitives();
        for (var i = 0; i < NumStrikes; i++)
        {
            ref var state = ref StrikeStates[i];
            if (state == AIState.Telegraph)
                continue;
            if (_randIndex == i)
                continue;
            beamIndex = i;
            var ratio = (float)i / (float)NumStrikes;
            var startPosition = Vector2.Lerp(TopLeft, TopRight, ratio);
            var endPosition = startPosition;
            endPosition.Y = Y + offset;
            var points = DrawUtilities.InterpolateBetweenPoints(startPosition, endPosition, 32);
            TrailDrawer.PreparePrimitives(points, GetTrailColor2, GetTrailWidth2);
        }

        TrailDrawer.DrawCached(bloomTrailShader);

        var pass = AssetReferences.Effects.Niivi.PrismaticBreath.CreatePrimitivesPass();
        pass.Parameters.transformMatrix = TrailDrawer.WorldViewPoint2;
        pass.Parameters.time = Main.GlobalTimeWrappedHourly;
        pass.Parameters.distortionStrength = 0.03f;
        pass.Apply();
        gDevice.Register(0, AssetReferences.Assets.LaserTextures.Lightning2.Asset.Value, SamplerState.LinearWrap);
        gDevice.Register(1, AssetReferences.Assets.Noise.PerlinBlurred.Asset.Value, SamplerState.LinearWrap);

        TrailDrawer.ClearPrimitives();
        for (var i = 0; i < NumStrikes; i++)
        {
            ref var state = ref StrikeStates[i];
            if (state == AIState.Telegraph)
                continue;
            if (_randIndex == i)
                continue;
            beamIndex = i;
            var ratio = (float)i / (float)NumStrikes;
            var startPosition = Vector2.Lerp(TopLeft, TopRight, ratio);
            var endPosition = startPosition;
            endPosition.Y = Y + offset;
            var points = DrawUtilities.InterpolateBetweenPoints(startPosition, endPosition, 32);
            TrailDrawer.PreparePrimitives(points, GetTrailColor, GetTrailWidth);
        }
        TrailDrawer.DrawCached(pass.Shader);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        PixelationManager.QueueSpritebatchDrawAction(DrawPixelatedTelegraph);
        PixelationManager.QueueSpritebatchDrawAction(DrawPixelatedStrike);
        //     PixelationManager.QueuePrimitivesDrawAction(DrawPixelatedBeam);
        return false;
    }
}
