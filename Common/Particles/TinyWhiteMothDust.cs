using Stellamod.Core;
using Stellamod.Core.Pixelation;
using Terraria;

namespace Stellamod.Common.Particles;

public struct TinyWhiteMothDustData : IParticleData
{
    public Vector2 position;
    public Vector2 velocity;
    public float timeLeft;
    public bool IsActive => timeLeft > 0;
}

public class TinyWhiteMothDust : ParticleUpdater<TinyWhiteMothDustData>
{
    public override DrawLayer PixelationDrawLayer => DrawLayer.OverNPCs;
    public override int GetPoolSize()
    {
        return 250;
    }

    public override void LoadSafe()
    {
        base.LoadSafe();
        On_Main.DrawDust += DrawParticles;
    }

    public override void UnloadSafe()
    {
        base.UnloadSafe();
        On_Main.DrawDust -= DrawParticles;
    }
    private void DrawParticles(On_Main.orig_DrawDust orig, Main self)
    {
        orig(self);
        if (Main.gameMenu)
            return;
        PixelationManager.QueueSpritebatchDrawAction(Draw);
    }

    protected override void UpdateParticles()
    {
        base.UpdateParticles();
        for (int i = 0; i < _length; i++)
        {
            ref var particle = ref _particles[i];
            particle.position += particle.velocity;
            particle.velocity = particle.velocity.RotatedBy(0.01f);
            particle.velocity *= 0.96f;
            particle.timeLeft--;
        }
    }


    public override void Draw(SpriteBatch spriteBatch, Vector2 screenPos)
    {
        //base.Draw(spriteBatch, screenPos);
        for (int i = 0; i < _length; i++)
        {
            ref var particle = ref _particles[i];
            Draw(spriteBatch, ref particle);
        }

        for (int i = 0; i < _length; i++)
        {
            ref var particle = ref _particles[i];
            DrawGlow(spriteBatch, ref particle);
        }
    }

    public override void Draw(SpriteBatch spriteBatch, ref TinyWhiteMothDustData particle)
    {
        float fade = MathHelper.Clamp(particle.timeLeft / 120f, 0f, 1f);// Utils.GetLerpValue(0, 1, particle.timeLeft / 120f, true);
        fade = EasingFunction.QuadraticBump(fade);
        SpritebatchDrawer drawer = SpritebatchDrawer.FromTextureAsset(AssetReferences.Common.Particles.TinyWhiteMoth.Asset.Value, particle.position);
        drawer.color = Color.White * fade;
        int frame = (int)particle.timeLeft / 2;
        frame %= 6;
        drawer.VerticalFrame(frame, 6);
        spriteBatch.Draw(drawer);
    }

    public void DrawGlow(SpriteBatch spriteBatch, ref TinyWhiteMothDustData particle)
    {
        float fade = MathHelper.Clamp(particle.timeLeft / 120f, 0f, 1f);// Utils.GetLerpValue(0, 1, particle.timeLeft / 120f, true);
        fade = EasingFunction.QuadraticBump(fade);
        SpritebatchDrawer glowDrawer = SpritebatchDrawer.FromTextureAsset(AssetReferences.Assets.GlowMasks.SimpleGlowCircle.Asset, particle.position);
        glowDrawer.color = Color.White * 0.4f * fade;
        glowDrawer.color.A = 0;
        glowDrawer.scale *= 0.4f;
        spriteBatch.Draw(glowDrawer);
    }
}