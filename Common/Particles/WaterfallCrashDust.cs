using Stellamod.Core.Pixelation;
using System;
using Terraria;

namespace Stellamod.Common.Particles;

public struct WaterfallCrashDustData : IParticleData
{
    public static readonly WaterfallCrashDustData Default = new WaterfallCrashDustData { timeLeft = 240, scale = 1f };
    public Vector2 position;
    public Vector2 velocity;
    public int frame;
    public float rotation;
    public float scale;
    public float timeLeft;
    public bool IsActive => timeLeft > 0;
}
public class WaterfallCrashDust : ParticleUpdater<WaterfallCrashDustData>
{
    public override ParticleFrameData FrameData => base.FrameData with { FrameCount = 3 };
    public override void LoadSafe()
    {
        base.LoadSafe();

        On_Main.DrawInfernoRings += DrawParticles;
    }

    private void DrawParticles(On_Main.orig_DrawInfernoRings orig, Main self)
    {
        orig(self);
        if (Main.gameMenu)
            return;

        if (_length <= 0)
            return;
        using(new SpritebatchContext(Main.spriteBatch, SpritebatchParams.InWorldAndZoomed() with { blendState = BlendState.Additive }))
        {
            Draw(Main.spriteBatch, Main.screenPosition);
        }

    }

    protected override void UpdateParticles()
    {
        base.UpdateParticles();
        for (int i = 0; i < _length; i++)
        {
            ref var particle = ref _particles[i];
            particle.position += particle.velocity;
            particle.velocity *= 0.94f;
            particle.timeLeft--;
        }
    }

    public override void OnSpawn(ref WaterfallCrashDustData particle, in int index)
    {
        base.OnSpawn(ref particle, index);
        particle.frame = Main.rand.Next(3);
    }

    public override void Draw(SpriteBatch spriteBatch, ref WaterfallCrashDustData particle)
    {
        float lerpValue = Utils.GetLerpValue(0, 240, particle.timeLeft, clamped: true);
        float interpolant = EasingFunction.OutSine(lerpValue);

        (Texture2D texture, Rectangle frame) = GetParticleFrame(particle.frame);
        SpritebatchDrawer drawer = SpritebatchDrawer.FromTextureAsset(texture, particle.position);
        drawer.sourceRect = frame;
        drawer.CenterOrigin();
        drawer.color = Color.Lerp(Color.Transparent, Color.White, interpolant);
        drawer.scale *= particle.scale;
        drawer.rotation = particle.rotation;
        spriteBatch.Draw(drawer);
    }

    public override int GetPoolSize()
    {
        return 255;
    }
}
