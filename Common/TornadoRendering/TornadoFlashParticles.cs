using Stellamod.Common.Particles;
using Terraria;

namespace Stellamod.Common.TornadoRendering;

public class TornadoFlashParticles : ParticleUpdater<TornadoFlashParticles.Data>
{
    public struct Data : IParticleData
    {
        public Color color;
        public Vector2 flashOffset;
        public float timeLeft;
        public float scale;
        public bool IsActive => timeLeft > 0;
    }
    public Vector2 tornadoCenter;
    public override int GetPoolSize()
    {
        return 128;
    }

    public override void OnSpawn(ref Data particle, in int index)
    {
        base.OnSpawn(ref particle, index);
        particle.scale = Main.rand.NextFloat(0.65f, 1f);
    }

    protected override void UpdateParticles()
    {
        base.UpdateParticles();

        for (var i = 0; i < _length; i++)
        {
            ref var particle = ref _particles[i];
            particle.timeLeft--;
        }
    }
    public override void Draw(SpriteBatch spriteBatch, Vector2 screenPos)
    {
        var divisor = 25F;
        using(spriteBatch.Ctx(SpritebatchParams.InWorldAndZoomed() with
        {
            matrix = Matrix.Identity,
            blendState = CustomBlendStates.Max }))
        {
            var drawer = SpritebatchDrawer.FromTextureAsset(AssetReferences.Assets.GlowMasks.JumbledGlowCircle.Asset, Vector2.Zero);
            for (var i = 0; i < _length; i++)
            {
                ref var particle = ref _particles[i];
                var position = tornadoCenter + particle.flashOffset;
                drawer.CenterOrigin();
                drawer.color = particle.color * MathHelper.Lerp(0f, 1f, EasingFunction.OutSine(particle.timeLeft / divisor));
                drawer.color.A = 0;
                drawer.scale = Vector2.One * 0.85f * particle.scale * MathHelper.Lerp(0f, 1f, EasingFunction.OutSine(particle.timeLeft / divisor));

                drawer.worldPosition = position;
                spriteBatch.Draw(drawer);

                drawer.scale *= 1.5f;
                drawer.color *= 0.5f;
                spriteBatch.Draw(drawer);
            }
        }

    }

    public override void Draw(SpriteBatch spriteBatch, ref Data particle)
    {

    }
}
