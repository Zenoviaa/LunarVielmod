using Stellamod.Core.Pixelation;
using System;
using Terraria;

namespace Stellamod.Common.Particles;

public class FallingBigGoldenLeaf : ParticleUpdater<FallingBigGoldenLeaf.Data>
{
    public struct Data : IParticleData
    {
        public Vector2 position;
        public Vector2 velocity;
        public float offset;
        public float rotation;
        public float scale;
        public int frame;
        public float timeLeft;
        public bool IsActive => timeLeft > 0;
    }
    public override ParticleFrameData FrameData => base.FrameData with { FrameCount = 6 };
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
            particle.velocity.Y = MathHelper.Lerp(particle.velocity.Y, 0.5f, 0.03f);
            particle.velocity.X = MathHelper.Lerp(particle.velocity.X, ExtraMath.Osc(-0.5f, 0.5f, speed: 0.2f, particle.offset), 0.03f);
            particle.rotation += particle.velocity.X * 0.03f;
            Vector2 collisionVelocity = Collision.TileCollision(particle.position, particle.velocity, 2, 2);
            particle.velocity = collisionVelocity;
            particle.timeLeft--;
        }
    }

    public override void OnSpawn(ref Data particle, in int index)
    {
        base.OnSpawn(ref particle, index);
        particle.frame = Main.rand.Next(6);
        particle.rotation = Main.rand.NextFloat(12);
        particle.scale = Main.rand.NextFloat(0.7f, 1f);
        particle.offset = Main.rand.NextFloat(0f, 6.28f);
    }

    private void DrawParticles(On_Main.orig_DrawDust orig, Main self)
    {
        orig(self);
        if (Main.gameMenu)
            return;
        if (_length <= 0)
            return;
        PixelationManager.QueueSpritebatchDrawAction(Draw, DrawLayer.BehindTiles);
    }

    public override void Draw(SpriteBatch spriteBatch, ref Data particle)
    {
        float lerpValue = Utils.GetLerpValue(0, 420, particle.timeLeft, clamped: true);
        float outAlpha = EasingFunction.InOutSine(particle.timeLeft / 30f);
        float inAlpha = EasingFunction.InOutSine((420 - particle.timeLeft) / 30f);

        (Texture2D texture, Rectangle frame) = GetParticleFrame(particle.frame);
        SpritebatchDrawer drawer = SpritebatchDrawer.FromTextureAsset(texture, particle.position);
        drawer.sourceRect = frame;
        drawer.CenterOrigin();
        drawer.color *= inAlpha * outAlpha;
        drawer.color *= 3;
        drawer.scale *= particle.scale;
        drawer.rotation = particle.rotation;
        spriteBatch.Draw(drawer);
    }

    public override int GetPoolSize()
    {
        return 500;
    }
}
