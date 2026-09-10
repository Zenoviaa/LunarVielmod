using Stellamod.Core.Particles;
using Stellamod.Core.Pixelation;
using System;
using System.Diagnostics;
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

    public override void Draw(SpriteBatch spriteBatch, Vector2 screenPos)
    {
        (Texture2D texture, Rectangle frame) = GetParticleFrame(0);
        SpritebatchDrawer drawer = SpritebatchDrawer.FromTextureAsset(texture,Vector2.Zero);
        drawer.sourceRect = frame;
        drawer.CenterOrigin();
        for (int i = 0; i < _length; i++)
        {
            ref var particle = ref _particles[i];
            float interpolant = EasingFunction.OutSine((float)particle.timeLeft / 240f);
            drawer.color = Color.Lerp(Color.Transparent, Color.White, interpolant);
            drawer.scale = Vector2.One *particle.scale;
            drawer.rotation = particle.rotation;
            drawer.worldPosition = particle.position;
            drawer.VerticalFrame(particle.frame, 3);
            spriteBatch.Draw(drawer);
        }
    }


    public override int GetPoolSize()
    {
        return 500;
    }

    public override void Draw(SpriteBatch spriteBatch, ref WaterfallCrashDustData particle)
    {
        //throw new NotImplementedException();
    }
}
