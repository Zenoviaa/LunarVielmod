using Terraria;

namespace Stellamod.Common.Particles;

public class StarDonutDust : ParticleUpdater<StarDonutDust.Data>
{
    public struct Data : IParticleData
    {
        public Color color;
        public Vector2 position;
        public float scale;
        public float timeLeft;
        public bool IsActive => timeLeft > 0;
    }
    public override int GetPoolSize()
    {
        return 150;
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
        drawer.color = Color.Lerp(Color.Transparent, particle.color, interpolant);
        drawer.color *= 0.5f;
        drawer.color.A = 0;
        drawer.scale *= MathHelper.SmoothStep(2f, 1f, lerpValue) * particle.scale;
        spriteBatch.Draw(drawer);
    }
}

