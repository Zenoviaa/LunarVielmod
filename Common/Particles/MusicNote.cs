using Stellamod.Core;
using Stellamod.Core.Pixelation;
using System;
using Terraria;

namespace Stellamod.Common.Particles;

public class MusicNote : ParticleUpdater<MusicNote.Data>
{
    public struct Data : IParticleData
    {
        public Color color;
        public Vector2 position;
        public Vector2 velocity;
        public float timeLeft;
        public byte frame;
        public bool IsActive => timeLeft > 0;
    }

    public override ParticleFrameData FrameData => base.FrameData with { FrameCount = 3 };
    public override DrawLayer PixelationDrawLayer => DrawLayer.OverNPCs;
    public override int GetPoolSize()
    {
        return 128;
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
            particle.velocity *= 0.96f;
            particle.velocity.X += MathF.Sin(particle.timeLeft * 0.01f) * 0.03f;
            particle.timeLeft--;
        }
    }

    public override void Draw(SpriteBatch spriteBatch, Vector2 screenPos)
    {
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

    public override void OnSpawn(ref Data particle, in int index)
    {
        base.OnSpawn(ref particle, index);
        particle.frame = (byte)Main.rand.Next(3);
    }
    public override void Draw(SpriteBatch spriteBatch, ref Data particle)
    {
        float fade = MathHelper.Clamp(particle.timeLeft / 120f, 0f, 1f);
        fade = EasingFunction.QuadraticBump(fade);
        var framing = GetParticleFrame(particle.frame);
        var drawer = SpritebatchDrawer.FromTextureAsset(framing.texture, particle.position);
        drawer.color = particle.color * fade;
        drawer.sourceRect = framing.frame;
        drawer.CenterOrigin();
        drawer.scale *= fade;
        spriteBatch.Draw(drawer);
    }

    public void DrawGlow(SpriteBatch spriteBatch, ref Data particle)
    {
        float fade = MathHelper.Clamp(particle.timeLeft / 120f, 0f, 1f);
        fade = EasingFunction.QuadraticBump(fade);
        SpritebatchDrawer glowDrawer = SpritebatchDrawer.FromTextureAsset(AssetReferences.Assets.GlowMasks.SimpleGlowCircle.Asset, particle.position);
        glowDrawer.color = particle.color * 0.4f * fade;
        glowDrawer.color.A = 0;
        glowDrawer.scale *= 0.14f;
        glowDrawer.scale *= fade;
        spriteBatch.Draw(glowDrawer);
    }
}