using Stellamod.Common;
using Stellamod.Common.Shaders;
using Stellamod.Core.Pixelation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
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
    int NumStrikes => 16;

    Vector2 Top => Projectile.Center + new Vector2(0, -StrikeHeight);
    Vector2 TopLeft => Top + new Vector2(-StrikeRadius, 0);
    Vector2 TopRight => Top + new Vector2(StrikeRadius, 0);
    float _bottomY;
    float StrikeHeight => 768;
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
        _randIndex = Main.rand.Next(NumStrikes / 2 - 4, NumStrikes / 2 + 2);
    }

    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {
        for(var i = 0; i < NumStrikes; i++)
        {
            if (_randIndex == i)
                continue;
            var ratio = (float)i / (float)NumStrikes;
            var startPOsition = Vector2.Lerp(TopLeft, TopRight, ratio);
            var endPosition = startPOsition + new Vector2(0, _bottomY);
            if (CollisionHelper.LaserCollision(projHitbox, targetHitbox, startPOsition, endPosition, 32))
                return true;
        }
        return false;
    }

    public override bool CanHitPlayer(Player target)
    {
        return base.CanHitPlayer(target) && State == AIState.Strike;
    }


    public override void AI()
    {
        base.AI();
        switch (State)
        {
            case AIState.Telegraph:
                AI_Telegraph();
                break;
            case AIState.Strike:
                AI_Strike();
                break;
        }
    }

    void SwitchState(AIState state)
    {
        if (this.OwnedByLocalClient())
        {
            Projectile.netUpdate = true;
            Timer = 0;
            State = state;
        }
    }
    void AI_Telegraph()
    {

        Timer++;
        _telegraphAlpha = Timer / TelegraphTime;
        if (Timer >= TelegraphTime)
        {
            SwitchState(AIState.Strike);
        }
    }

    void AI_Strike()
    {
        
        Timer++;
        _telegraphAlpha = MathHelper.Lerp(1f, 0f, Timer / StrikeTime);
        if (Timer >= StrikeTime)
        {
            Projectile.Kill();
        }
    }
    float GetTrailWidth(float ratio)
    {
        return MathHelper.Lerp(MathHelper.Lerp(0, 96, EasingFunction.OutQuad(ratio)), 0, EasingFunction.OutSine(Timer / StrikeTime));
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
    void DrawPixelatedBeam(GraphicsDevice gDevice)
    {

        BloomTrailShader bloomTrailShader = BloomTrailShader.Instance;
        bloomTrailShader.InnerColor = Main.DiscoColor;
        bloomTrailShader.OuterColor = Main.DiscoColor;

        TrailDrawer.ClearPrimitives();
        for (var i = 0; i < NumStrikes; i++)
        {
            if (_randIndex == i)
                continue;
            var ratio = (float)i / (float)NumStrikes;
            var startPOsition = Vector2.Lerp(TopLeft, TopRight, ratio);
            var endPosition = startPOsition + new Vector2(0, _bottomY);
            var points = DrawUtilities.InterpolateBetweenPoints(startPOsition, endPosition, 16);
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
            if (_randIndex == i)
                continue;
            var ratio = (float)i / (float)NumStrikes;
            var startPOsition = Vector2.Lerp(TopLeft, TopRight, ratio);
            var endPosition = startPOsition + new Vector2(0, _bottomY);
            var points = DrawUtilities.InterpolateBetweenPoints(startPOsition, endPosition, 16);
            TrailDrawer.PreparePrimitives(points, GetTrailColor, GetTrailWidth);
        }
        TrailDrawer.DrawCached(pass.Shader);
    }


    public override bool PreDraw(ref Color lightColor)
    {
        if(State == AIState.Strike)
            PixelationManager.QueuePrimitivesDrawAction(DrawPixelatedBeam);
        return false;
    }
}
