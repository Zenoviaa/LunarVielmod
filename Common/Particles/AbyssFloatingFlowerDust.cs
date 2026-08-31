using Stellamod.Core.Pixelation;
using System;
using Terraria;

namespace Stellamod.Common.Particles;

public struct AbyssFloatingFlowerDustData : IParticleData
{
    public static readonly AbyssFloatingFlowerDustData Default = new AbyssFloatingFlowerDustData { timeLeft = 240 };
    public Vector2 position;
    public Vector2 velocity;
    public float parallax;
    public int frameIndex;
    public float timeLeft;
    public bool IsActive => timeLeft > 0;
}
public class AbyssFloatingFlowerDust : ParticleUpdater<AbyssFloatingFlowerDustData>
{
    public override ParticleFrameData FrameData => base.FrameData with { FrameCount = 8 };
    public override void LoadSafe()
    {
        base.LoadSafe();
        On_Main.DrawDust += DrawParticles;
    }
    protected override void UpdateParticles()
    {
        base.UpdateParticles();
        Vector2 movement = Main.screenPosition - Main.screenLastPosition;

        for (int i = 0; i < _length; i++)
        {
            ref var particle = ref _particles[i];
            particle.position += particle.velocity;

            float xVel = (float)Math.Sin(particle.timeLeft * 0.036) * 0.48f;
            particle.velocity.X = xVel + (particle.position.Y < Main.worldSurface * 16 ? Main.windSpeedCurrent * 8 : 0);

            particle.velocity.Y = (-Math.Abs(xVel)) * 0.4f;
            particle.velocity.Y += 0.3f;
            // _particles.rotation[index] = _particles.velocity[index].X * -0.5f;
            particle.position += movement * -particle.parallax;
            particle.timeLeft--;
        }
    }
    public override void OnSpawn(ref AbyssFloatingFlowerDustData particle, in int index)
    {
        base.OnSpawn(ref particle, index);
        particle.frameIndex = Main.rand.Next(8);
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

    public override void Draw(SpriteBatch spriteBatch, ref AbyssFloatingFlowerDustData particle)
    {
        float lerpValue = Utils.GetLerpValue(0, 240, particle.timeLeft, clamped: true);
        float interpolant = EasingFunction.QuadraticBump(lerpValue);

        (Texture2D texture, Rectangle frame) = GetParticleFrame(particle.frameIndex);
        SpritebatchDrawer drawer = SpritebatchDrawer.FromTextureAsset(texture, particle.position);
        drawer.sourceRect = frame;
        drawer.CenterOrigin();
        drawer.color = Color.White * interpolant;
        drawer.rotation = particle.velocity.X * -0.5f;
        drawer.scale *= MathHelper.Lerp(1f, 0.75f, particle.parallax);
        spriteBatch.Draw(drawer);
    }

    public override int GetPoolSize()
    {
        return 350;
    }
}
