using Stellamod.Common.Particles;
using Terraria;

namespace Stellamod.Common.TornadoRendering;
public class TornadoDustParticles : ParticleUpdater<TornadoDustParticles.Data>
{
    public struct Data : IParticleData
    {
        public Vector2 oldTrackingPosition;
        public Vector2 position;
        public Vector2 velocity;
        public int pointToTrack;
        public float timeLeft;
        public float scale;
        public bool stopMoving;
        public byte frame;
        public bool IsActive => timeLeft > 0;
    }

    public Vector3[] wrappedOffsets;
    public Vector2 tornadoCenter;
    public float timer;
    public override int GetPoolSize()
    {
        return 384;
    }

    public override void OnSpawn(ref Data particle, in int index)
    {
        base.OnSpawn(ref particle, index);
        particle.frame = (byte)Main.rand.Next(4);
        particle.oldTrackingPosition = Vector2.Zero;
        particle.scale = Main.rand.NextFloat(0.25f, 1f);
    }

    protected override void UpdateParticles()
    {
        base.UpdateParticles();

        for (var i = 0; i < _length; i++)
        {
            ref var particle = ref _particles[i];
            particle.position += particle.velocity;

            ref var offset = ref wrappedOffsets[particle.pointToTrack];
            if (!particle.stopMoving)
            {
                var targetCirclePos = tornadoCenter + new Vector2(offset.X * 2 * ExtraMath.Osc(1f, 2f, speed: 3, offset: i), offset.Y);
                var targetVelocity = (targetCirclePos - particle.position) * 0.08f;
                if (particle.oldTrackingPosition != Vector2.Zero)
                {
          
                    var sqrDistToPoint = Vector2.DistanceSquared(targetCirclePos, particle.oldTrackingPosition);
                    if (sqrDistToPoint > 300 * 300)
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
                particle.timeLeft--;
                particle.velocity *= 0.98f;
            }

            particle.timeLeft--;
        }
    }
    public override void Draw(SpriteBatch spriteBatch, Vector2 screenPos)
    {
        var drawer = SpritebatchDrawer.FromTextureAsset(AssetReferences.Common.Particles.TornadoDust.Asset, Vector2.Zero);
        for (var i = 0; i < _length; i++)
        {
            ref var particle = ref _particles[i];
            drawer.worldPosition = particle.position;
            drawer.rotation = Main.GlobalTimeWrappedHourly + i;
            drawer.VerticalFrame(particle.frame, 4);
            drawer.CenterOrigin();
            drawer.color = Color.Lerp(Color.Transparent, Color.White * 0.85f, EasingFunction.QuadraticBump(particle.timeLeft / 120f));
            drawer.scale = Vector2.One * 0.85f * particle.scale;
            spriteBatch.Draw(drawer);
        }
        //base.Draw(spriteBatch, screenPos);
    }

    public override void Draw(SpriteBatch spriteBatch, ref Data particle)
    {

    }
}
