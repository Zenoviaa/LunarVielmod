using Stellamod.Core.Pixelation;
using System;
using Terraria;

namespace Stellamod.Common.Particles;


public struct RoarDustData : IParticleData
{
    public static readonly RoarDustData Default = new RoarDustData
    {
        position = Vector2.Zero,
        color = Color.White,
        timeLeft = 120,
        scale = 1f
    };

    public Color color;
    public Vector2 position;
    public float timeLeft;
    public float scale;
    public bool IsActive => timeLeft > 0;
}
public struct FaintSmokeDustData : IParticleData
{
    public static readonly FaintSmokeDustData Default = new FaintSmokeDustData
    {
        position = Vector2.Zero,
        velocity = Vector2.Zero,
        color = Color.Lerp(Color.Gray, Color.Red, 0.5f),
        timeleft = 120,
    };

    //Colors on this one is calculated
    public Color color;
    public Vector2 position;
    public Vector2 velocity;
    public float timeleft;
    public bool IsActive => timeleft > 0;
}

public class FaintSmokeDust : ParticleUpdater<FaintSmokeDustData>
{
    public override ParticleFrameData FrameData => base.FrameData with { FrameCount = 1 };
    public override DrawLayer PixelationDrawLayer => DrawLayer.OverNPCsWithOutline;
    public override int GetPoolSize()
    {
        return 200;
    }

    public override void LoadSafe()
    {
        base.LoadSafe();
        On_Main.DrawDust += DrawParticles;
    }

    private void DrawParticles(On_Main.orig_DrawDust orig, Main self)
    {
        orig(self);
        if (_length <= 0)
            return;
        PixelationManager.QueueSpritebatchDrawAction(Draw, PixelationDrawLayer);
    }

    protected override void UpdateParticles()
    {
        base.UpdateParticles();
        for (int i = 0; i < _length; i++)
        {
            ref var particle = ref _particles[i];
            particle.position += particle.velocity;
            particle.velocity.Y -= 0.01f;
            particle.velocity.X = MathF.Sin(particle.timeleft * 0.2f + Main.GlobalTimeWrappedHourly * 8) * 0.5f;
            particle.timeleft--;
        }
    }

    public override void Draw(SpriteBatch spriteBatch, ref FaintSmokeDustData particle)
    {
        float lerpValue = Utils.GetLerpValue(0, 180, particle.timeleft, clamped: true);
        float interpolant = EasingFunction.QuadraticBump(lerpValue);

        (Texture2D texture, Rectangle frame) = GetParticleFrame(0);
        SpritebatchDrawer drawer = SpritebatchDrawer.FromTextureAsset(texture, particle.position);
        drawer.sourceRect = frame;
        drawer.CenterOrigin();
        drawer.color = Color.Lerp(Color.Transparent, particle.color, interpolant);
        drawer.color *= interpolant;
        drawer.color.A = 0;
        drawer.scale *= 1.26f;
        spriteBatch.Draw(drawer);
    }
}

