using Stellamod.Core.Pixelation;
using Terraria;

namespace Stellamod.Common.Particles;

public class FallingDarkspaceStar : ParticleUpdater<FallingDarkspaceStar.Data>
{
    public struct Data : IParticleData
    {
        public Color color;
        public Vector2 position;
        public Vector2 velocity;
        public float scale;
        public float rotation;
        public float timer;
        public float timeLeft;
        public bool fast;
        public bool IsActive => timeLeft > 0;
    }
    public override ParticleFrameData FrameData => base.FrameData with
    {
        FrameCount = 1,
        Texture = AssetReferences.Assets.Textures.Particle.DarkspaceStarParticle.KEY
    };
    public override void LoadSafe()
    {
        base.LoadSafe();
        On_Main.DrawDust += DrawParticles;
    }

    public override void OnSpawn(ref Data particle, in int index)
    {
        base.OnSpawn(ref particle, index);
        particle.scale = Main.rand.NextFloat(0.13f, 0.7f);
        particle.rotation = Main.rand.NextFloat(-MathHelper.TwoPi, MathHelper.TwoPi);
    }

    private void DrawParticles(On_Main.orig_DrawDust orig, Main self)
    {
        orig(self);
        if (Main.gameMenu)
            return;
        if (_length <= 0)
            return;
        PixelationManager.QueueSpritebatchDrawAction(Draw, DrawLayer.OverWater);
    }
    public override int GetPoolSize()
    {
        return 256;
    }
    protected override void UpdateParticles()
    {
        base.UpdateParticles();
        for (var i = 0; i < _length; i++)
        {
            ref var particle = ref _particles[i];
            particle.position += particle.velocity;
            particle.velocity *= 0.98f;
            particle.velocity.Y += 0.0125f;
            particle.velocity = particle.velocity.RotatedByRandom(0.09f);
            particle.rotation += 0.01f;

            float denom = 180f;
            if (particle.fast)
                denom *= 0.25f;
      
            particle.color = Color.Lerp(Color.Transparent, Color.White, EasingFunction.QuadraticBump(particle.timer / denom));
            particle.timer++;
            particle.timeLeft--;
        }
    }

    public override void Draw(SpriteBatch spriteBatch, Vector2 screenPos)
    {
        var framing = GetParticleFrame(0);
        var drawer = SpritebatchDrawer.FromTextureWhite(framing.texture);
        drawer.CenterOrigin();
        for (var i = 0; i < _length; i++)
        {
            ref var particle = ref _particles[i];
            drawer.worldPosition = particle.position;
            drawer.rotation = particle.rotation;
            drawer.color = particle.color;
            drawer.color.A = 0;
            drawer.scale = new Vector2(particle.scale);
            spriteBatch.Draw(drawer);
        }
    }
}
