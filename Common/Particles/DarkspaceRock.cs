using Stellamod.Core.Pixelation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace Stellamod.Common.Particles;

public class DarkspaceRock : ParticleUpdater<DarkspaceRock.Data>
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
    public override ParticleFrameData FrameData => base.FrameData with { FrameCount = 4, Texture = AssetReferences.Assets.Textures.Particle.DarkspaceRock.KEY };
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
            particle.velocity.Y += 0.06F;
            particle.velocity.X *= 0.96F;
            particle.rotation += 0.03f;
            particle.timeLeft--;
        }
    }

    public override void OnSpawn(ref Data particle, in int index)
    {
        base.OnSpawn(ref particle, index);
        particle.frame = Main.rand.Next(4);
        particle.rotation = Main.rand.NextFloat(12);
        particle.scale = Main.rand.NextFloat(1f, 1.2f);
        particle.offset = Main.rand.NextFloat(0f, 6.28f);
    }

    private void DrawParticles(On_Main.orig_DrawDust orig, Main self)
    {
        orig(self);
        if (Main.gameMenu)
            return;
        if (_length <= 0)
            return;
        PixelationManager.QueueSpritebatchDrawAction(Draw, DrawLayer.OverPlayers);
    }

    public override void Draw(SpriteBatch spriteBatch, ref Data particle)
    {
        float lerpValue = Utils.GetLerpValue(0, 180, particle.timeLeft, clamped: true);
        float outAlpha = EasingFunction.InOutSine(particle.timeLeft / 30f);
        float inAlpha = EasingFunction.InOutSine((180 - particle.timeLeft) / 30f);

        (Texture2D texture, Rectangle frame) = GetParticleFrame(particle.frame);
        SpritebatchDrawer drawer = SpritebatchDrawer.FromTextureGetLightingColor(texture, particle.position);
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
        return 128;
    }
}
