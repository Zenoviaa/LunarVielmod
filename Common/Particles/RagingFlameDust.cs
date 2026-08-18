using Steamworks;
using Stellamod.Core.Pixelation;
using System;
using Terraria;

namespace Stellamod.Common.Particles;

public struct RagingFlameDustData : IParticleData
{
    public static readonly RagingFlameDustData Default = new RagingFlameDustData
    {
        position = Vector2.Zero,
        velocity = Vector2.Zero,
        timeleft = 120,
        frameIndex = 0
    };

    //Colors on this one is calculated
    public Vector2 position;
    public Vector2 velocity;
    public float timeleft;
    public int frameIndex;
    public bool IsActive => timeleft > 0;
}
public class RagingFlameDust : ParticleUpdater<RagingFlameDustData>
{
    public override ParticleFrameData FrameData => base.FrameData with { FrameCount = 3 };
    public override DrawLayer PixelationDrawLayer => DrawLayer.OverNPCsWithOutline;
    public override int GetPoolSize()
    {
        return 4_000;
    }

    public override void LoadSafe()
    {
        base.LoadSafe();
        On_Main.DrawDust += DrawParticles;
    }

    public override void OnSpawn(ref RagingFlameDustData particle, in int index)
    {
        base.OnSpawn(ref particle, index);
        particle.frameIndex = Main.rand.Next(3);
    }

    protected override void UpdateParticles()
    {
        base.UpdateParticles();
        for (int i = 0; i < _length; i++)
        {
            ref RagingFlameDustData particle = ref _particles[i];
            particle.position += particle.velocity;
            particle.velocity.Y -= 0.1f;
            particle.velocity.X = MathF.Sin(particle.timeleft * 0.2f + Main.GlobalTimeWrappedHourly * 8) * 0.5f;

//float lerpValue = Utils.GetLerpValue(0, 70, particle.timeleft, clamped: true);
         //   particle.velocity.Y += MathHelper.Lerp(0.3f, 0f, lerpValue);
            particle.timeleft--;
        }
    }

    private void DrawParticles(On_Main.orig_DrawDust orig, Main self)
    {
        orig(self);
        PixelationManager.QueueSpritebatchDrawAction(Draw, PixelationDrawLayer);
    }

    public override void Draw(SpriteBatch spriteBatch, Vector2 screenPos)
    {

        base.Draw(spriteBatch, screenPos);
    }
    public override void Draw(SpriteBatch spriteBatch, ref RagingFlameDustData particle)
    {
        float lerpValue = Utils.GetLerpValue(0, 70, particle.timeleft, clamped: true);
        Color particleColor = DrawUtilities.InterpolateColorArray(EasingFunction.InSine(lerpValue), Color.Black, Color.DarkBlue, Color.Black, Color.DarkRed,  Color.Pink, Color.Lerp(Color.Red, Color.White, 0.5f));
        (Texture2D texture, Rectangle frame) = GetParticleFrame(particle.frameIndex);
        SpritebatchDrawer drawer = SpritebatchDrawer.FromTextureAsset(texture, particle.position);
        drawer.sourceRect = frame;
        drawer.CenterOrigin();
        drawer.rotation = particle.timeleft * 0.01f;
        drawer.scale = Vector2.Lerp(Vector2.Zero, Vector2.Lerp(Vector2.Zero, new Vector2(1.1f), EasingFunction.QuadraticBump(lerpValue)), lerpValue);
        drawer.color = particleColor;
        drawer.color.A = 0;
        spriteBatch.Draw(drawer);

        drawer.scale *= 4f;
        drawer.color.R = 125;
        drawer.color.B = 0;
        drawer.color.G = 0;
        drawer.color *= 0.05f;
        spriteBatch.Draw(drawer);

        drawer.scale *= 2;
        drawer.color *= 0.25f;
        spriteBatch.Draw(drawer);
    }
}

