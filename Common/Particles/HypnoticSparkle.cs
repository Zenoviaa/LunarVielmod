using Stellamod.Core.Pixelation;
using System;
using Terraria;

namespace Stellamod.Common.Particles;

public class HypnoticSparkle : ParticleUpdater<HypnoticSparkle.Data>
{
    public struct Data : IParticleData
    {
        public Vector2 position;
        public Vector2 velocity;
        public float rot;
        public float scale;
        public byte frame;
        public float timeLeft;
        public float frameTimer;
        public bool IsActive => timeLeft > 0;
    }
    public override ParticleFrameData FrameData => base.FrameData with { FrameCount = 4, Texture = AssetReferences.Assets.Textures.Particle.HypnoticSparkle.KEY };

    protected override void UpdateParticles()
    {
        base.UpdateParticles();
        for (int i = 0; i < _length; i++)
        {
            ref var particle = ref _particles[i];
            particle.position += particle.velocity;
            particle.frameTimer++;
            if(particle.frameTimer >= 4)
            {
                particle.frameTimer = 0;
                particle.frame++;
                particle.frame %= 4;
            }
            particle.timeLeft--;
        }
    }
    public override void LoadSafe()
    {
        base.LoadSafe();
        On_Main.DrawDust += DrawParticles;
    }
    public override void OnSpawn(ref Data particle, in int index)
    {
        base.OnSpawn(ref particle, index);
        particle.frame = (byte)MoonUtils.Rand.Next(4);
    }

    private void DrawParticles(On_Main.orig_DrawDust orig, Main self)
    {
        orig(self);
        if (Main.gameMenu || _length <= 0)
            return;
        Main.spriteBatch.Begin(SpritebatchParams.InWorldAndZoomed());
        Draw(Main.spriteBatch, Main.screenPosition);
        Main.spriteBatch.End();
    }

    public override void Draw(SpriteBatch spriteBatch, Vector2 screenPos)
    {
        for (var k = 0; k < _length; k++)
        {
            ref var particle = ref _particles[k];
            var framing = GetParticleFrame(particle.frame);
            var drawer = SpritebatchDrawer.FromTextureGetLightingColor(framing.texture, particle.position);
            drawer.sourceRect = framing.frame;
            drawer.CenterOrigin();
            drawer.rotation = particle.rot;
            drawer.scale = new Vector2(particle.scale);
            spriteBatch.Draw(drawer);
        }
    }

    public override int GetPoolSize()
    {
        return 256;
    }
}
