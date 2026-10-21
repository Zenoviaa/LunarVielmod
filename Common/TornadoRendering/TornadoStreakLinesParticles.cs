using Stellamod.Common.Particles;
using Stellamod.Common.Shaders;
using System;
using Terraria;

namespace Stellamod.Common.TornadoRendering;

public class TornadoStreakLinesParticles : ParticleUpdater<TornadoStreakLinesParticles.Data>
{
    public struct Data : IParticleData
    {
        public Vector3 position;
        public Vector3 velocity;
        public Vector3 oldTrackingPosition;
        public int pointToTrack;
        public bool stopMoving;
        public float timeLeft;
        public float inTimer;
        public bool IsActive => timeLeft > 0;
    }
    public int TrailCacheLength => 24;
    public VertexPositionColor[] VertexBuffer;
    public short[] IndexBuffer;
    public Vector3[] OldPos;
    public Vector3[] wrappedOffsets;
    public Vector3 tornadoCenter;
    public TornadoStreakLinesParticles() : base()
    {
        OldPos = new Vector3[TrailCacheLength * GetPoolSize()];
        VertexBuffer = new VertexPositionColor[TrailCacheLength * GetPoolSize() * 2];
        IndexBuffer = DrawUtilities.TexturedQuadUpDownPattern(VertexBuffer.Length);

    }

    public override int GetPoolSize()
    {
        return 64;
    }
    public override void OnSpawn(ref Data particle, in int index)
    {
        base.OnSpawn(ref particle, index);
        var trailIndex = index * TrailCacheLength;
        var endTrailIndex = trailIndex + TrailCacheLength;
        for(var i = trailIndex; i < endTrailIndex; i++)
        {
            OldPos[i] = particle.position;
        }
      
    }

    public void Simulate()
    {
        for (var i = 0; i < _length; i++)
        {
            ref var particle = ref _particles[i];
            do
            {
                var trailIndex = i * TrailCacheLength;
                var endTrailIndex = trailIndex + TrailCacheLength;
                for (var j = endTrailIndex - 1; j > trailIndex; j--)
                {
                    OldPos[j] = OldPos[j - 1];
                }
                OldPos[trailIndex] = particle.position;


                ref var offset = ref wrappedOffsets[particle.pointToTrack];
                if (!particle.stopMoving)
                {
                    var targetCirclePos = tornadoCenter + new Vector3(offset.X * 4, offset.Y + ExtraMath.Osc(-32, 32, 0, i), offset.Z);
                    var targetVelocity = (targetCirclePos - particle.position) * 0.5f;
                    var sqrDistToPoint = MathF.Abs(targetCirclePos.Y - particle.oldTrackingPosition.Y);
                    if (particle.oldTrackingPosition != Vector3.Zero)
                    {
                        if (sqrDistToPoint > 64)
                        {
                            particle.stopMoving = true;
                        }
                        else
                            particle.velocity = Vector3.Lerp(particle.velocity, targetVelocity, 0.2f);
                    }

                    particle.oldTrackingPosition = targetCirclePos;

                }
                else
                {
                    particle.timeLeft -= 2;
                    particle.velocity *= 0.98f;
                }

                particle.position += particle.velocity;
                particle.timeLeft--;
                particle.inTimer++;
            }
            while (particle.inTimer < 16);
        }
        for (int i = 0; i < _length; i++)
        {
            ref Data particleData = ref _particles[i];
            if (!particleData.IsActive)
            {
                KillParticle2(i);
                i--;
            }
        }
    }
    void KillParticle2(in int index)
    {
        //Swap with the last active particle and set the data to default
        //Order does not matter for when they get updated, so we can do it like this :)
        _particles[index] = _particles[_length - 1];

        var startIndex = index * TrailCacheLength;
        var endIndex = startIndex + TrailCacheLength;
        var oldStartIndex = (_length - 1) * TrailCacheLength;
        var offset = oldStartIndex - startIndex;

        for(var i = startIndex; i < endIndex; i++)
        {
            OldPos[i] = OldPos[i + offset];
        }


        _particles[_length - 1] = default;

        _length--;
    }
    protected override void UpdateParticles()
    {
        base.UpdateParticles();

  
    }
    public void DrawStreaks(GraphicsDevice gDevice)
    {
        var length = 0;
        var quadCount = 0;
        for(var i = 0; i < _length; i++)
        {
            ref var particle = ref _particles[i];
            var trailIndex = i * TrailCacheLength;
            var endTrailIndex = trailIndex + TrailCacheLength;
 
            //prepare vertices
            for(var j = trailIndex; j < endTrailIndex; j++)
            {
                var ratio = (float)(j - trailIndex) / (float)(TrailCacheLength - 1);
                var color = Color.Lerp(Color.Transparent, Color.White, EasingFunction.QuadraticBump(ratio));
                color *= EasingFunction.InOutSine(particle.timeLeft / 90f);
                color *= (120 - particle.timeLeft) / 60f;
                color *= 0.2f;

                var width = EasingFunction.QuadraticBump(ratio);
                width *= 4 * (120 - particle.timeLeft) / 60f;
                ref var position = ref OldPos[j];
                if(j == trailIndex)
                {
                    ref var nextPosition = ref OldPos[j + 1];
      

                    var direction2 = (nextPosition.XY() - position.XY());
                    var rot = direction2.ToRotation();
                    var perp = (rot + MathHelper.PiOver2).ToRotationVector2();
                    Vector3 perpendicularNormalized = new Vector3(perp, 0);

                    ref var topVertex = ref VertexBuffer[length++];
                    topVertex.Position = position - perpendicularNormalized * width;
                    topVertex.Color = color;

                    ref var bottomVertex = ref VertexBuffer[length++];
                    bottomVertex.Position = position + perpendicularNormalized * width;
                    bottomVertex.Color = color;


                    /*
                     * 
                     * 
                     * 0 1 3 0 2 3
                     * tl tr br 
                     * tl bl br
                     * 0 2 3 0 1 3
                var topLeftVertex = new VertexPositionColorTexture(topLeft, nextColor, topLeftCoord); 0 
                var topRightVertex = new VertexPositionColorTexture(topRight, nextColor, topRightCoord); 1 
                var bottomLeftVertex = new VertexPositionColorTexture(bottomLeft, color, bottomLeftCoord); 2 
                var bottomRightVertex = new VertexPositionColorTexture(bottomRight, color, bottomRightCoord); 3
                     */
                    quadCount++;
                }
                else
                {
                    ref var prevPosition = ref OldPos[j - 1];
                    var direction2 = (position.XY() - prevPosition.XY());
                    var rot = direction2.ToRotation();
                    var perp = (rot + MathHelper.PiOver2).ToRotationVector2();
                    Vector3 perpendicularNormalized = new Vector3(perp, 0);

                    ref var topVertex = ref VertexBuffer[length++];
                    topVertex.Position = position - perpendicularNormalized * width;
                    topVertex.Color = color;

                    ref var bottomVertex = ref VertexBuffer[length++];
                    bottomVertex.Position = position + perpendicularNormalized * width;
                    bottomVertex.Color = color;

                    //don't increase quad on the last vertices, as there is no enxt point
                    quadCount++;
                }
            }

            //capp off 

        }
        if (quadCount <= 0)
            return;



        var pass = AssetReferences.Effects.Weather.TornadoStreak.CreatePrimitivesPass();
        pass.Parameters.transformMatrix = TrailDrawer.WorldViewPointCullFarAway;
        pass.Apply();
        gDevice.RasterizerState = RasterizerState.CullNone;
        gDevice.BlendState = BlendState.AlphaBlend;
        gDevice.SamplerStates[0] = SamplerState.AnisotropicWrap;
        gDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList,
            VertexBuffer, 0, length, 
            IndexBuffer, 0, length / 2);
    }
    public override void Draw(SpriteBatch spriteBatch, Vector2 screenPos)
    {
        //base.Draw(spriteBatch, screenPos);
    }
    public override void Draw(SpriteBatch spriteBatch, ref Data particle)
    {
        //throw new NotImplementedException();
    }
}
