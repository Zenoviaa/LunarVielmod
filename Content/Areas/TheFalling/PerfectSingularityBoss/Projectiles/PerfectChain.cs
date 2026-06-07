using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Core.Pixelation;
using Stellamod.Core.Utilities;
using Stellamod.Effects.RoyalMagic;
using Stellamod.Helpers;
using Stellamod.Trails;
using Stellamod.Visual.Particles;
using System;
using System.IO;
using System.Net;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.TheFalling.PerfectSingularityBoss.Projectiles;

public class PerfectChain : ModProjectile,
    IDrawToRenderTarget
{
    private enum AIState
    {
        ChainWhip,
        ChainJail,
        ChainLinger
    }

    private int _drawOffset;
    private bool _impactGround;
    private ref float Timer => ref Projectile.ai[0];
    private AIState State
    {
        get => (AIState)Projectile.ai[1];
        set => Projectile.ai[1] = (float)value;
    }
    private ref float AttackCycle => ref Projectile.ai[2];

    private float DistanceInterpolant;
    private float SwingInterpolant;
    private float TimeMult => 1f;
    private int NumPoints => (int)(Projectile.velocity.Length() / 28f) * 2;
    private float[] _chainRotations;
    private Vector2[] _chainVelocities;
    private Vector2[] _chainPoints;
    private Vector2[] _chainSwingPos;
 
    private Vector2 _movementDirection;
    private Vector2 ChainWhip_Start => Projectile.Center;
    private Vector2 ChainWhip_End => Projectile.Center + Projectile.velocity;
    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {
        return base.Colliding(projHitbox, targetHitbox);
    }
    public override bool CanHitPlayer(Player target)
    {
        return base.CanHitPlayer(target);
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 16;
        Projectile.height = 16;
        Projectile.hostile = false;
        Projectile.timeLeft = 300;
        Projectile.penetrate = -1;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
    }
    public override void SendExtraAI(BinaryWriter writer)
    {
        base.SendExtraAI(writer);
        writer.WriteVector2(_movementDirection);
    }
    public override void ReceiveExtraAI(BinaryReader reader)
    {
        base.ReceiveExtraAI(reader);
        _movementDirection = reader.ReadVector2();
    }
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
    }
    public override bool ShouldUpdatePosition()
    {
        return false;
    }
    public override void AI()
    {
        base.AI();
       
        switch (State)
        {
            case AIState.ChainWhip:
                AI_ChainWhip();
                break;
        }
    }
    private Vector2[] InterpolateChainVelocities(Vector2[] oldVelocities, float targetRotation)
    {
        Vector2[] velocities = new Vector2[oldVelocities.Length];
        for (int i = 0; i < velocities.Length; i++)
        {
            ref Vector2 chainVelocity = ref velocities[i];
            ref Vector2 oldVelocity = ref oldVelocities[i];
            float progress = (float)i / (float)velocities.Length;
            progress = 1f - progress;
            progress = MathHelper.Lerp(0.06f, 0.25f, progress);
        //    progress = EasingFunction.InSine(progress);

            float oldRotation = oldVelocity.ToRotation();
            float newRotation = Utils.AngleLerp(oldRotation, targetRotation, progress);
            chainVelocity = newRotation.ToRotationVector2();
        }

        return velocities;
    }
    private void SimulateChainWhip(float maxTime, float swingDegrees)
    {
        if(_chainVelocities == null)
        {
            _chainVelocities = new Vector2[NumPoints];
            for(int i = 0; i < _chainVelocities.Length; i++)
            {
                ref Vector2 initialVelocity = ref _chainVelocities[i];
                initialVelocity = Projectile.velocity.SafeNormalize(Vector2.Zero);
            }
        }

        float timestep = Timer / maxTime;
        float easeIn = EasingFunction.InExpo(timestep);
        float easeOut = EasingFunction.OutExpo(timestep);
        timestep = easeIn * easeOut;

        float swingRange = MathHelper.ToRadians(swingDegrees);
        float halfSwingRange = swingRange * 0.5f;
        float initialRotation = Projectile.velocity.ToRotation();

        initialRotation -= halfSwingRange;
        float targetRotation = initialRotation + swingRange;
        
        float interpolatedRotation = MathHelper.Lerp(initialRotation, targetRotation, timestep);
   
        _chainVelocities = InterpolateChainVelocities(_chainVelocities, interpolatedRotation);
    }
    private void PrepareChainWhip(float maxTime, float swingDegrees)
    {
        if (_chainVelocities == null)
        {
            _chainVelocities = new Vector2[NumPoints];
            for (int i = 0; i < _chainVelocities.Length; i++)
            {
                ref Vector2 initialVelocity = ref _chainVelocities[i];
                initialVelocity = Projectile.velocity.SafeNormalize(Vector2.Zero);
            }
        }

        float swingRange = MathHelper.ToRadians(swingDegrees);
        float halfSwingRange = swingRange * 0.5f;
        float initialRotation = Projectile.velocity.ToRotation();

        initialRotation -= halfSwingRange;
        float targetRotation = initialRotation + swingRange;

   

        float progress = Timer / maxTime;
        float ease = EasingFunction.InOutQuad(progress);
        float realRotation = MathHelper.Lerp(Projectile.velocity.ToRotation(), initialRotation, ease);
        _chainVelocities = InterpolateChainVelocities(_chainVelocities, realRotation);
    }
    private void SimulateChainPoke(float maxTime, float swingDegrees)
    {
        if (_chainVelocities == null)
        {
            _chainVelocities = new Vector2[NumPoints];
            for (int i = 0; i < _chainVelocities.Length; i++)
            {
                ref Vector2 initialVelocity = ref _chainVelocities[i];
                initialVelocity = Projectile.velocity.SafeNormalize(Vector2.Zero);
            }
        }
    }


    private void AI_ChainWhip()
    {
        float swingAmount = 450;
        Timer++;
        switch (AttackCycle)
        {
            case 0:
                {
                    float swingTime = 24 * TimeMult;
                    float in1 = EasingFunction.InExpo(Timer / swingTime);
                    float out1 = EasingFunction.OutExpo(Timer / swingTime);
                    float ease = in1 * out1;
       
                    DistanceInterpolant = ease;
                    SimulateChainPoke(maxTime: 60, swingDegrees: swingAmount);
                    if(Timer == swingTime)
                    {
                        FXUtil.ShakeCamera(ChainWhip_End, 1024, 4);
                        FXUtil.CreateRipple(ChainWhip_End);
                    }
                    if(Timer >= swingTime + 30)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 1:
                {
                    float prepTime = 30 * TimeMult;
                    PrepareChainWhip(maxTime: prepTime, swingDegrees: swingAmount* 0.5f);
                    if(Timer >= prepTime)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 2:
                {

                    Projectile.velocity = Projectile.velocity.Resize(Projectile.velocity.Length() * 1.01f);
                    Projectile.hostile = true;
                    float swingTime = 37 * TimeMult;
                    SimulateChainWhip(maxTime: swingTime, swingDegrees: swingAmount * 0.5f);
                    SwingInterpolant = EasingFunction.QuadraticBump(Timer / (swingTime * 1.75f));
                    if (Timer == 1)
                    {
                        FXUtil.CreateRipple(Projectile.Center);
                    }
                    if(Timer >= swingTime)
                    {
                        DistanceInterpolant *= 0.98f;
                    }
                    if(Timer >= swingTime * 1.75f)
                    {
                        Projectile.Kill();
                    }
                }
                break;
        }
        if (_chainVelocities == null)
            return;

        if(_chainPoints == null || _chainPoints.Length != _chainVelocities.Length)
        {
            _chainPoints = new Vector2[_chainVelocities.Length];
            
        }

        if(_chainSwingPos == null)
        {
            _chainSwingPos = new Vector2[32];
        }
        Vector2 start = ChainWhip_Start;
        Vector2 end = ChainWhip_End;
        float dist = Vector2.Distance(start, end) * DistanceInterpolant;
        for (float f = 0; f < _chainVelocities.Length; f++)
        {
            Vector2 chainVelocity = _chainVelocities[(int)f];
            ref Vector2 chainPoint = ref _chainPoints[(int)f];
            Vector2 newEnd = start + chainVelocity * dist;
            chainPoint = Vector2.Lerp(start, newEnd, f / (float)_chainVelocities.Length);
        }

        for(int i = _chainSwingPos.Length - 1; i > 0; i--)
        {
            _chainSwingPos[i] = _chainSwingPos[i - 1];
        }
        _chainSwingPos[0] = _chainPoints[_chainPoints.Length - 2];
    }

    public override bool PreDraw(ref Color lightColor) => false;

    private Vector2[] CalculateChainVelocities(float timestep, float initialRotation, float targetRotation, int numSteps)
    {
        Vector2[] velocities = new Vector2[numSteps];
        for(int i = 0; i < velocities.Length; i++)
        {
            ref Vector2 chainVelocity = ref velocities[i];
            float progress = (float)i / (float)velocities.Length;
            progress = 1f - progress;

            float interpolatedRotation = MathHelper.Lerp(initialRotation, targetRotation, timestep * progress);
            chainVelocity = interpolatedRotation.ToRotationVector2();
        }

        return velocities;
    }

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);

        if (_chainVelocities == null)
            return;

        Vector2 start = ChainWhip_Start;
        Vector2 end = ChainWhip_End;

        float dist = Vector2.Distance(start, end);
        Vector2 p = start;
        for (float f = 0; f < _chainVelocities.Length; f++)
        {
            Vector2 chainVelocity = _chainVelocities[(int)f];
            Vector2 newEnd = start + chainVelocity * dist;
            Vector2 chainPoint = Vector2.Lerp(start, newEnd, f / (float)_chainVelocities.Length);

            for (int i = 0; i < 3; i++)
            {
  

                float ratio = Main.rand.NextFloat(0f, 100f) / 100f;
                Vector2 pos = chainPoint;
                Vector2 vel = Main.rand.NextVector2Circular(4, 4) * Main.rand.NextFloat(5f, 15f);
                var dp = DustParticle.Spawn(pos + Main.rand.NextVector2Circular(4, 4), vel);
                dp.dampening = 0.25f;
                dp.gravity = 0;
                dp.noTileCollide = true;
                dp.outerColor = Color.DarkGray;
                dp.Scale *= Main.rand.NextFloat(0.7f, 1.6f);
                dp.superFast = true;
            }

        }
    }

    private void DrawWhipTrail(GraphicsDevice gDevice)
    {
        if (_chainVelocities == null)
            return;

        Func<float, Color> getTrailColor = (float completionRatio) =>
        {
            Color drawColor = Color.White;
            return Color.White;
        };

        Func<float, float> getTrailWidth = (float completionRatio) =>
        {
            return 64;
        };


        Color drawColor = Color.White;
        Vector2 start = ChainWhip_Start;
        Vector2 end = ChainWhip_End;

        float dist = Vector2.Distance(start, end) * EasingFunction.OutQuad(Timer / 60f);
        Vector2 p = start;
        Vector2[] chainPoints = new Vector2[_chainVelocities.Length];
        for (float f = 0; f < _chainVelocities.Length; f++)
        {
            Vector2 chainVelocity = _chainVelocities[(int)f];
            ref Vector2 chainPoint = ref chainPoints[(int)f];
            Vector2 newEnd = start + chainVelocity * dist;
            chainPoint = Vector2.Lerp(start, newEnd, f / (float)_chainVelocities.Length);
        }

        PerfectEyesShader eyesShader = ShaderContent.GetInstance<PerfectEyesShader>();
        eyesShader.Texture = TextureAssets.Projectile[Type].Value;
        eyesShader.Time = Main.GlobalTimeWrappedHourly * -6;
        eyesShader.TransformMatrix = TrailDrawer.WorldViewPoint2;
        eyesShader.DistortionStrength = MathHelper.Lerp(0.025f, 0.2f, EasingFunction.QuadraticBump(Timer / 60f));
        TrailDrawer.Draw(chainPoints, getTrailColor, getTrailWidth, eyesShader);
    }

    private void DrawSlashTrail(GraphicsDevice gDevice)
    {
        if (_chainSwingPos == null)
            return;

        Func<float, Color> getTrailColor = (float completionRatio) =>
        {
            Color drawColor = Color.White;
            drawColor *= EasingFunction.QuadraticBump(completionRatio);
            return drawColor;
        };

        Func<float, float> getTrailWidth = (float completionRatio) =>
        {
            return MathHelper.SmoothStep(80, 0, completionRatio) * SwingInterpolant;
        };


        AlcadSlashShader shader = ShaderContent.GetInstance<AlcadSlashShader>();
        shader.ScrollingLaser = TrailRegistry.Beamlight.Value;
        shader.Noise = AssetManager.Noise.Whirly.Value;
        shader.Slash = AssetManager.GlowMask.SwordSlash.Value;
        shader.BloomColor = Color.Purple;
        shader.Time = Main.GlobalTimeWrappedHourly * 24;
        shader.TransformMatrix = TrailDrawer.WorldViewPoint2;
        shader.Distortion = 0.15f;
        TrailDrawer.Draw(_chainSwingPos, getTrailColor, getTrailWidth, shader);

    }
    private void DrawWhipSprites(SpriteBatch sb, Vector2 screenPos, Color? overrideColor = null)
    {
        if (_chainVelocities == null)
            return;
        if (_chainPoints == null)
            return;

        Color drawColor = Color.White;
        drawColor = overrideColor != null ? overrideColor.Value : drawColor;

        Vector2 start = ChainWhip_Start;
        Vector2 end = ChainWhip_End;

        float dist = Vector2.Distance(start, end) * DistanceInterpolant; 
        Vector2 p = start;

        Vector2[] chainPoints = _chainPoints;
        SpritebatchDrawer chainDrawer = SpritebatchDrawer.FromProjectile(Projectile);
        chainDrawer.VerticalFrame(0 + _drawOffset, 4);
        chainDrawer.LeftCenterOrigin();
        chainDrawer.color = drawColor;
        chainDrawer.scale = Vector2.One * 0.86f;
        float chainLength = chainDrawer.sourceRect.Value.Width;
        for(int i = 0; i < chainPoints.Length - 1; i++)
        {
            Vector2 currentPoint = chainPoints[i];
            Vector2 nextPoint = chainPoints[i + 1];
            float distanceBetweenPoints = Vector2.Distance(currentPoint, nextPoint);
            float numPoints = distanceBetweenPoints / chainLength;

            Vector2 drawPoint = currentPoint;
            Vector2 dir = (nextPoint - currentPoint).SafeNormalize(Vector2.Zero);
            float chainRotation = dir.ToRotation();
            if(i == chainPoints.Length - 2)
            {
                chainDrawer.VerticalFrame(1 + _drawOffset, 4);
                chainDrawer.scale = Vector2.One * 1.2f;
                chainRotation = (currentPoint - chainPoints[i - 1]).ToRotation();
            }

            for (float j = 0; j < numPoints; j++)
            {
         

                chainDrawer.worldPosition = drawPoint;
                chainDrawer.rotation = chainRotation;
                sb.Draw(chainDrawer);
                drawPoint += dir;
            }
        }
    }

    private void DrawWhipInner(SpriteBatch sb, Color? overridecolor = null)
    {

    }
    private void DrawWhipOutline(SpriteBatch sb)
    {

    }
    private void DrawPixelatedWhip(SpriteBatch sb, Vector2 screenPos)
    {
        _drawOffset = 2;
        Color outlineColor = Projectile.hostile ? Color.Red : Color.Yellow;
        DrawWhipSprites(sb, screenPos, outlineColor);
        _drawOffset = 0;
        DrawWhipSprites(sb, screenPos, Color.White);
    }
    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        return base.OnTileCollide(oldVelocity);
    }

    public void DrawToRenderTargets()
    {
        //OutlineRenderer.Queue(DrawWhipOutline);
     //   PixelationManager.QueuePrimitivesDrawAction(DrawWhipTrail, DrawLayer.OverNPCsAdditive);
           
        PixelationManager.QueueSpritebatchDrawAction(DrawPixelatedWhip);
        PixelationManager.QueuePrimitivesDrawAction(DrawSlashTrail);
    }
}
