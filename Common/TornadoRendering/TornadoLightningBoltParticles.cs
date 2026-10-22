using Stellamod.Common.Particles;
using Terraria;

namespace Stellamod.Common.TornadoRendering;

public class TornadoLightningBoltParticles : ParticleUpdater<TornadoLightningBoltParticles.Data>
{
    public struct Data : IParticleData
    {
        public Vector2 position;
        public Vector2 velocity;
        public float power;
        public float rotation;
        public float timeLeft;
        public bool IsActive => timeLeft > 0;
    }
    float Time => 90;
    public override int GetPoolSize()
    {
        return 128;
    }

    protected override void UpdateParticles()
    {
        base.UpdateParticles();

        for (var i = 0; i < _length; i++)
        {
            ref var particle = ref _particles[i];
            particle.position += particle.velocity;
            particle.velocity *= 0.88f;
            particle.power = MathHelper.Lerp(particle.power, 0f, 0.03f);
            if (particle.timeLeft == 90)
                particle.power = 2f;

            if (particle.timeLeft == 45)
                particle.power = 5;

            if (particle.timeLeft == 15)
                particle.power = 1f;
            particle.timeLeft--;
        }
    }
    public override void Draw(SpriteBatch spriteBatch, Vector2 screenPos)
    {
        var pass = AssetReferences.Effects.Weather.TornadoLightningBolt.CreatePixelPass();
     //   pass.Parameters.time = Main.GlobalTimeWrappedHourly;
        pass.Apply();
        using(spriteBatch.Ctx(spriteBatch.Parameters with { effect = pass.Shader }))
        {

            for (var i = 0; i < _length; i++)
            {
                ref var particle = ref _particles[i];
                var drawer = SpritebatchDrawer.FromTextureAsset(AssetReferences.Assets.LaserTextures.Lightning.Asset, Vector2.Zero);
                drawer.worldPosition = particle.position;
                drawer.rotation = particle.rotation;
                drawer.scale *= 0.66f;
                drawer.scale.X *= 0.75f;
                drawer.scale.Y *= 0.25f;
                drawer.scale.Y *= MathHelper.Lerp(2f, 1f, EasingFunction.OutExpo(particle.timeLeft / Time));
                drawer.scale.Y *= MathHelper.Lerp(0f, 1f, particle.timeLeft / Time);
                drawer.LeftCenterOrigin();
                drawer.color = DrawUtilities.InterpolateColorArray(particle.timeLeft / Time, Color.DarkBlue, Color.DarkOrange, Color.White);
                drawer.color *= EasingFunction.InOutSine(particle.timeLeft / Time);
                drawer.color.A = (byte)(EasingFunction.OutSine(particle.power) * 255F);
                spriteBatch.Draw(drawer);
            }
        }
    }

}
