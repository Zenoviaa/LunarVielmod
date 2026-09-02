using Stellamod.Core.Pixelation;
using System;
using Terraria;

namespace Stellamod.Common.Particles;

public struct FeatherDustData : IParticleData
{
    public static readonly FeatherDustData Default = new FeatherDustData { timeLeft = 240, scale = 1f };
    public Vector2 position;
    public Vector2 velocity;
    public float rotation;
    public float scale;
    public float timeLeft;
    public bool IsActive => timeLeft > 0;
}

public class FeatherDust : ParticleUpdater<FeatherDustData>
{
    public override ParticleFrameData FrameData => base.FrameData with { FrameCount = 1 };
    public override void LoadSafe()
    {
        base.LoadSafe();
        On_Main.DrawDust += DrawParticles;
    }
    protected override void UpdateParticles()
    {
        base.UpdateParticles();
        for (int i = 0; i < _length; i++)
        {
            ref var particle = ref _particles[i];
            particle.position += particle.velocity;
            particle.velocity.X *= 0.96f;
            particle.velocity.X += MathF.Sin(Main.GameUpdateCount * 0.03f) * 0.03f;
            particle.velocity.Y = MathHelper.Lerp(particle.velocity.Y, 0.5f, 0.03f);
            particle.rotation = Utils.AngleLerp(particle.rotation, particle.velocity.ToRotation(), 0.03f);
            particle.timeLeft--;
        }
    }

    private void DrawParticles(On_Main.orig_DrawDust orig, Main self)
    {
        orig(self);
        if (Main.gameMenu)
            return;

        if (_length <= 0)
            return;
        PixelationManager.QueueSpritebatchDrawAction(Draw, PixelationDrawLayer);
    }

    public override void Draw(SpriteBatch spriteBatch, ref FeatherDustData particle)
    {
        float lerpValue = Utils.GetLerpValue(0, 240, particle.timeLeft, clamped: true);
        float interpolant = EasingFunction.OutSine(lerpValue);

        (Texture2D texture, Rectangle frame) = GetParticleFrame(0);
        SpritebatchDrawer drawer = SpritebatchDrawer.FromTextureAsset(texture, particle.position);
        drawer.sourceRect = frame;
        drawer.CenterOrigin();
        drawer.color = Color.White * interpolant;
        drawer.color.A = 0;
        drawer.rotation = particle.rotation;
        spriteBatch.Draw(drawer);
    }

    public override int GetPoolSize()
    {
        return 100;
    }
}
