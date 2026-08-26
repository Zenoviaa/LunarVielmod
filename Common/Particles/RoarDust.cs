using Stellamod.Common.Shaders;
using Stellamod.Core.Pixelation;
using Stellamod.Effects.Generic;
using Terraria;

namespace Stellamod.Common.Particles;

public class RoarDust : ParticleUpdater<RoarDustData>
{
    public override ParticleFrameData FrameData => base.FrameData with { FrameCount = 1 };
    public override DrawLayer PixelationDrawLayer => DrawLayer.OverNPCs;
    public override int GetPoolSize()
    {
        return 25;
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
            particle.timeLeft--;
        }
    }

    public override void Draw(SpriteBatch spriteBatch, Vector2 screenPos)
    {
        var roarShader = ShaderContent.GetInstance<RoarShader>();
        /*
       */
        SpritebatchParams roarParameters = SpritebatchParams.InWorldAndZoomed() with { effect = roarShader.Effect, blendState = BlendState.Additive };
        using (new SpritebatchContext(spriteBatch, roarParameters))
        {

        }
        base.Draw(spriteBatch, screenPos);

    }
    public override void Draw(SpriteBatch spriteBatch, ref RoarDustData particle)
    {
        float lerpValue = Utils.GetLerpValue(0, 24, particle.timeLeft, clamped: true);
        float interpolant = EasingFunction.QuadraticBump(lerpValue);

        (Texture2D texture, Rectangle frame) = GetParticleFrame(0);
        SpritebatchDrawer drawer = SpritebatchDrawer.FromTextureAsset(texture, particle.position);
        drawer.sourceRect = frame;
        drawer.CenterOrigin();
        drawer.color = Color.Lerp(Color.Transparent, particle.color, interpolant);
        drawer.color *= 0.5f;
        drawer.color.A = 0;
        drawer.scale *= MathHelper.Lerp(5f, 0.1f, lerpValue) * particle.scale;
        spriteBatch.Draw(drawer);
    }
}

