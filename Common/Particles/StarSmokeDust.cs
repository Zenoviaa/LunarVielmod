using Terraria;

namespace Stellamod.Common.Particles;

public class StarSmokeDust : ParticleUpdater<StarSmokeDust.Data>
{
    public struct Data : IParticleData
    {
        public Color color;
        public Vector2 position;
        public float timeLeft;
        public bool IsActive => timeLeft > 0;
    }
    public override int GetPoolSize()
    {
        return 250;
    }
    protected override void UpdateParticles()
    {
        base.UpdateParticles();
        for (int i = 0; i < _length; i++)
        {
            ref var particle = ref _particles[i];
            particle.timeLeft--;
        }
    }

    public override void Draw(SpriteBatch spriteBatch, ref Data particle)
    {
        float lerpValue = Utils.GetLerpValue(0, 120, particle.timeLeft, clamped: true);
        float interpolant = EasingFunction.QuadraticBump(lerpValue);

        (Texture2D texture, Rectangle frame) = GetParticleFrame(0);
        SpritebatchDrawer drawer = SpritebatchDrawer.FromTextureAsset(texture, particle.position);
        drawer.worldPosition += Main.screenPosition;
        drawer.sourceRect = frame;
        drawer.CenterOrigin();
        drawer.color = Color.Lerp(Color.Transparent, particle.color, interpolant) * 1.3f;
        //.color *= 0.5f;
        drawer.scale *= MathHelper.SmoothStep(3f, 2f, lerpValue);
        spriteBatch.Draw(drawer);
    }
}

