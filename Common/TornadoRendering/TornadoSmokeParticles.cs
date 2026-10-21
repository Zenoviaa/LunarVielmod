using Stellamod.Common.Particles;
using Terraria;

namespace Stellamod.Common.TornadoRendering;

public class TornadoSmokeParticles : ParticleUpdater<TornadoSmokeParticles.Data>
{
    public struct Data : IParticleData
    {
        public Data()
        {
            scale = 1f;
        }
        public Vector2 oldTrackingPosition;
        public Vector2 position;
        public Vector2 velocity;
        public int pointToTrack;
        public float timeLeft;
        public float scale;
        public bool stopMoving;
        public bool IsActive => timeLeft > 0;
    }

    public Vector3[] wrappedOffsets;
    public Vector2 tornadoCenter;
    public float timer;
    public float alpha;
    public float scale;
    public override int GetPoolSize()
    {
        return 384;
    }

    protected override void UpdateParticles()
    {
        base.UpdateParticles();

        for(var i = 0; i < _length; i++)
        {
            ref var particle = ref _particles[i];
            particle.position += particle.velocity;

            ref var offset = ref wrappedOffsets[particle.pointToTrack];
            if (!particle.stopMoving)
            {
                var targetCirclePos = tornadoCenter + new Vector2(offset.X * 2 * ExtraMath.Osc(0.5f, 1f, speed: 3, offset: i), offset.Y);
                var targetVelocity = (targetCirclePos - particle.position) * 0.03f;
                var sqrDistToPoint = Vector2.DistanceSquared(particle.oldTrackingPosition, targetCirclePos);
                if (particle.oldTrackingPosition != Vector2.Zero)
                {
                    if (sqrDistToPoint > 256 * 256)
                    {
                        particle.stopMoving = true;
                    }
                    else
                        particle.velocity = targetVelocity;
                }

                particle.oldTrackingPosition = targetCirclePos;
       
            }
            else
            {
                particle.velocity *= 0.97f;
            }

            particle.timeLeft--;
        }
    }
    public override void Draw(SpriteBatch spriteBatch, Vector2 screenPos)
    {

        var drawer = SpritebatchDrawer.FromTextureAsset(AssetReferences.Common.Particles.BackCloud.Asset, Vector2.Zero);
        for(var i = 0; i < _length; i++)
        {
            ref var particle = ref _particles[i];
            drawer.worldPosition = particle.position;
            drawer.rotation = Main.GlobalTimeWrappedHourly + i;
            drawer.color = Color.Lerp(Color.Transparent, Color.Lerp(Color.White, Color.Black, 0.6f) * 0.15f, EasingFunction.QuadraticBump((float)particle.timeLeft / 120f));
            drawer.color *= alpha;

            drawer.scale = Vector2.One * 2.5f * scale * particle.scale;
            spriteBatch.Draw(drawer);
        }
   
        //base.Draw(spriteBatch, screenPos);
    }

    public override void Draw(SpriteBatch spriteBatch, ref Data particle)
    {

    }
}
