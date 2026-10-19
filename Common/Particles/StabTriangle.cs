using Stellamod.Core.Pixelation;
using Terraria;

namespace Stellamod.Common.Particles;

public class StabTriangle : ParticleUpdater<StabTriangle.Data>
{
    public struct Data : IParticleData
    {
        public Color color;
        public Vector2 position;
        public Vector2 velocity;
        public float rotation;
        public float scale;
        public float timeLeft;
        public bool IsActive => timeLeft > 0;
    }
    public override ParticleFrameData FrameData => base.FrameData with { FrameCount = 1 };
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
            particle.velocity *= 0.96f;
            particle.timeLeft--;
        }
    }

    public override void OnSpawn(ref Data particle, in int index)
    {
        base.OnSpawn(ref particle, index);
 
    }

    private void DrawParticles(On_Main.orig_DrawDust orig, Main self)
    {
        orig(self);
        if (Main.gameMenu)
            return;
        if (_length <= 0)
            return;
        PixelationManager.QueueSpritebatchDrawAction(DrawPixelated, DrawLayer.OverNPCsAdditive);
    }

    void DrawPixelated(SpriteBatch spriteBatch, Vector2 sp)
    {
        var framing = GetParticleFrame(0);
        using(spriteBatch.Ctx(spriteBatch.Parameters with { blendState = BlendState.Additive }))
        {
            for (var i = 0; i < _length; i++)
            {
                ref var particle = ref _particles[i];
                float lerpValue = Utils.GetLerpValue(0, 30f, particle.timeLeft, clamped: true);

                var drawer = SpritebatchDrawer.FromTextureAsset(framing.texture, particle.position);
                drawer.sourceRect = framing.frame;
                drawer.CenterOrigin();
                drawer.color = particle.color; ;
                drawer.color *= lerpValue * 3f;
       
                drawer.scale *= particle.scale;
                drawer.rotation = particle.rotation;
                spriteBatch.Draw(drawer);
            }

        }

    }
    public override void Draw(SpriteBatch spriteBatch, ref Data particle)
    {

    }

    public override int GetPoolSize()
    {
        return 128;
    }
}
