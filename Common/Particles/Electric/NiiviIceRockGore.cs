
using Stellamod.Common.Particles;
using Stellamod.Core.Pixelation;
using System;
using Terraria;

public class NiiviIceRockGore : ParticleUpdater<NiiviIceRockGore.Data>
{
    public override ParticleFrameData FrameData => base.FrameData with { FrameCount = 4, Texture = AssetReferences.Assets.Textures.Particle.NiiviIceRockGore.KEY };
    public struct Data : IParticleData
    {
        public Color color;
        public Vector2 position;
        public Vector2 velocity;
        public Vector2 scale;
        public float rotation;
        public float timeLeft;
        public byte frame;
        public bool IsActive => timeLeft > 0;
    }

    public override int GetPoolSize()
    {
        return 64;
    }

    public override void LoadSafe()
    {
        base.LoadSafe();
        On_Main.DrawDust += DrawParticles;
    }

    protected override void UpdateParticles()
    {
        base.UpdateParticles();
        for (var i = 0; i < _length; i++)
        {
            ref var particle = ref _particles[i];
            particle.position += particle.velocity;
            particle.velocity.X *= 0.97F;
            particle.velocity.Y += 0.18F;
            particle.rotation += MathF.Sign(particle.velocity.X) * 0.05F;
            particle.timeLeft--;
        }
    }

    public override void OnSpawn(ref Data particle, in int index)
    {
        base.OnSpawn(ref particle, index);
        particle.frame = (byte)Main.rand.Next(4);
        particle.rotation = Main.rand.NextFloat(6F);
    }
    private void DrawParticles(On_Main.orig_DrawDust orig, Main self)
    {
        orig(self);
        if (Main.gameMenu)
            return;
        if (_length <= 0)
            return;
        PixelationManager.QueueSpritebatchDrawAction(Draw, DrawLayer.OverNPCs);
    }

    public override void Draw(SpriteBatch spriteBatch, Vector2 screenPos)
    {
        for (var i = 0; i < _length; i++)
        {
            ref var particle = ref _particles[i];
            (var texture, var frame) = GetParticleFrame(particle.frame); 
            var fade = EasingFunction.InOutSine(particle.timeLeft / 60f);
            var drawer = SpritebatchDrawer.FromTextureGetLightingColor(texture, particle.position);
            drawer.color *= fade;
            drawer.sourceRect = frame;
            drawer.rotation = particle.rotation;
            drawer.scale *= particle.scale;
            drawer.CenterOrigin();
            spriteBatch.Draw(drawer);
        }
    }
}
