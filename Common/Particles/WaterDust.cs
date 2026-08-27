using Stellamod.Common.Shaders;
using Stellamod.Core.Pixelation;
using Stellamod.Core.Rendering;
using Stellamod.Effects.Generic;
using System;
using Terraria;

namespace Stellamod.Common.Particles;

public struct WaterDustData : IParticleData
{
    public static readonly WaterDustData Default = new WaterDustData
    {
        position = Vector2.Zero,
        timeLeft = 120,
        scale = 1f
    };

    public Vector2 position;
    public Vector2 velocity;
    public float timeLeft;
    public float scale;
    public bool IsActive => timeLeft > 0;
}

//This one is manually drawn by our other systems
//well actually just make a hook
public class WaterDust : ParticleUpdater<WaterDustData>
{
    public override ParticleFrameData FrameData => base.FrameData with { FrameCount = 1 };
    public override DrawLayer PixelationDrawLayer => DrawLayer.OverNPCs;
    public override int GetPoolSize()
    {
        return 250;
    }

    public override void LoadSafe()
    {
        base.LoadSafe();
        MoonWaterSystem.DrawWaterMask += DrawMask;
    }

    public override void UnloadSafe()
    {
        base.UnloadSafe();
        MoonWaterSystem.DrawWaterMask -= DrawMask;
    }

    private void DrawMask(SpriteBatch batch)
    {
        Draw(batch, Main.screenPosition);
    }


    protected override void UpdateParticles()
    {
        base.UpdateParticles();
        for (int i = 0; i < _length; i++)
        {
            ref var particle = ref _particles[i];
            particle.position += particle.velocity;
            particle.velocity.X *= 0.98f;
            particle.velocity.Y += 0.5f;
            particle.timeLeft--;
        }
    }


    public override void Draw(SpriteBatch spriteBatch, ref WaterDustData particle)
    {
        float lerpValue = Utils.GetLerpValue(0, 120, particle.timeLeft, clamped: true);
        float interpolant = EasingFunction.QuadraticBump(lerpValue);

        (Texture2D texture, Rectangle frame) = GetParticleFrame(0);
        SpritebatchDrawer drawer = SpritebatchDrawer.FromTextureAsset(texture, particle.position);
        drawer.color = Color.White;
    //    drawer.scale *= lerpValue * particle.scale;
        spriteBatch.Draw(drawer);
    }
}

