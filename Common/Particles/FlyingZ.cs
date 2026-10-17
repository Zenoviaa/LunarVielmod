using Stellamod.Core.Pixelation;
using System;
using Terraria;

namespace Stellamod.Common.Particles;

public class FlyingZ : ParticleUpdater<FlyingZ.Data>
{
    public struct Data : IParticleData
    {
        public Vector2 position;
        public Vector2 velocity;
        public float timeLeft;
        public bool IsActive => timeLeft > 0;
    }

    public bool noDecrement;
    public override void LoadSafe()
    {
        base.LoadSafe();
        On_Main.DrawDust += DrawCrackImpactDusts;
    }

    public override void UnloadSafe()
    {
        base.UnloadSafe();
        On_Main.DrawDust -= DrawCrackImpactDusts;
    }

    private void DrawCrackImpactDusts(On_Main.orig_DrawDust orig, Main self)
    {
        orig(self);
        if (_length <= 0)
            return;
        PixelationManager.QueueSpritebatchDrawAction(Draw, DrawLayer.OverNPCs);
    }

    public override void OnSpawn(ref Data particle, in int index)
    {
        base.OnSpawn(ref particle, index);

    }

    protected override void UpdateParticles()
    {
        base.UpdateParticles();
        for (int i = 0; i < _length; i++)
        {
            ref var particle = ref _particles[i];
            particle.position += particle.velocity;
            particle.velocity.Y = MathHelper.Lerp(particle.velocity.Y, -0.3f, 0.1f);
            particle.velocity.X = MathHelper.Lerp(particle.velocity.X, MathF.Sin(particle.timeLeft * 0.025f) * 0.4f, 0.1f);
            particle.timeLeft--;
        }
    }


    public override void Draw(SpriteBatch spriteBatch, Vector2 screenPos)
    {
        var framing = GetParticleFrame(0);
        for (int i = 0; i < _length; i++)
        {
            ref var particle = ref _particles[i];
            float lerpValue = Utils.GetLerpValue(0, 240, particle.timeLeft, clamped: true);
            float interpolant = EasingFunction.OutSine(lerpValue);
            SpritebatchDrawer drawer = SpritebatchDrawer.FromTextureAsset(framing.texture, particle.position);
            drawer.sourceRect = framing.frame;
            drawer.CenterOrigin();
            //drawer.scale = Vector2.Lerp(Vector2.Zero, Vector2.One, lerpValue);
            drawer.color = Color.Lerp(Color.Transparent, drawer.color, lerpValue);
            spriteBatch.Draw(drawer);
        }
    }

    public override void Draw(SpriteBatch spriteBatch, ref Data particle)
    {


    }

    public override int GetPoolSize()
    {
        return 32;
    }
}

