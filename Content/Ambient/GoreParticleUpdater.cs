using Stellamod.Common.Particles;
using Stellamod.Core.Pixelation;
using Terraria;

namespace Stellamod.Content.Ambient;

public abstract class GoreParticleUpdater : ParticleUpdater<GoreParticleUpdater.Data>
{
    public struct Data : IParticleData
    {
        public Vector2 position;
        public Vector2 velocity;
        public byte frame;
        public float timeLeft;
        public bool IsActive => timeLeft > 0;
    }

    public override ParticleFrameData FrameData => base.FrameData with
    {
        FrameCount = 4,
        Texture = AssetReferences.Assets.Textures.Particle.WoodenCrateFragment.KEY
    };

    public override void LoadSafe()
    {
        base.LoadSafe();
        On_Main.DrawDust += DrawCrateShards;
    }
    public override void OnSpawn(ref Data particle, in int index)
    {
        base.OnSpawn(ref particle, index);
        particle.frame = (byte)Main.rand.Next(4);
    }

    protected override void UpdateParticles()
    {
        for (var i = 0; i < _length; i++)
        {
            ref var particle = ref _particles[i];
            particle.position += particle.velocity;
            particle.velocity.X *= 0.96f;
            particle.velocity.Y += 0.5f;
            particle.velocity = Collision.TileCollision(particle.position, particle.velocity, 4, 4);
            particle.timeLeft--;
        }
    }

    private void DrawCrateShards(On_Main.orig_DrawDust orig, Main self)
    {
        orig(self);
        if (_length <= 0)
            return;
        PixelationManager.QueueSpritebatchDrawAction(Draw, DrawLayer.OverPlayers);
    }
    public override void Draw(SpriteBatch spriteBatch, Vector2 screenPos)
    {
        for (var i = 0; i < _length; i++)
        {
            ref var particle = ref _particles[i];
            var framign = GetParticleFrame(particle.frame);
            var drawer = SpritebatchDrawer.FromTextureAsset(framign.texture, particle.position);
            drawer.rotation = particle.velocity.ToRotation();
            spriteBatch.Draw(drawer);
        }
    }
}
