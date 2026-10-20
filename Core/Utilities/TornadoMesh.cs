using Stellamod.Common.Particles;
using Stellamod.Common.Shaders;
using Stellamod.Core.Pixelation;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Core.Utilities;


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
        public bool IsActive => timeLeft > 0;
    }
    public int TrailCacheLength => 32;
    public Vector3[] OldPos;
    public Vector3[] wrappedOffsets;
    public Vector3 tornadoCenter;
    public float timer;
    public TornadoStreakLinesParticles() : base()
    {
        OldPos = new Vector3[TrailCacheLength * GetPoolSize()];
    }

    public override int GetPoolSize()
    {
        return 128;
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
                var targetCirclePos = tornadoCenter + new Vector3(offset.X * 3 , offset.Y, offset.Z);
                var targetVelocity = (targetCirclePos - particle.position) * 0.25f;
                var sqrDistToPoint = MathF.Abs(targetCirclePos.Y - particle.position.Y);
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
                particle.timeLeft-=2;
                particle.velocity *= 0.98f;
            }

            particle.position += particle.velocity;
            particle.timeLeft--;
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
        List<VertexPositionColor> vertices = new();
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
                color *= 0.7F;
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


                    VertexPositionColor topVertex = new VertexPositionColor(position - perpendicularNormalized * width, color);
                    VertexPositionColor bottomVertex = new VertexPositionColor(position + perpendicularNormalized * width, color);
                    vertices.Add(topVertex);
                    vertices.Add(bottomVertex);

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
                    VertexPositionColor topVertex = new VertexPositionColor(position - perpendicularNormalized * width, color);
                    VertexPositionColor bottomVertex = new VertexPositionColor(position + perpendicularNormalized * width, color);
                    vertices.Add(topVertex);
                    vertices.Add(bottomVertex);
                    //don't increase quad on the last vertices, as there is no enxt point
                    if(j < endTrailIndex - 1)
                        quadCount++;
                }
            }

            //capp off 

        }
        if (quadCount <= 0)
            return;


        var vertArray = vertices.ToArray();
        var indices = DrawUtilities.TexturedQuadIndices2(quadCount);
        var pass = AssetReferences.Effects.Weather.TornadoStreak.CreatePrimitivesPass();
        pass.Parameters.transformMatrix = TrailDrawer.WorldViewPointBigZPlanes;
        pass.Apply();
        gDevice.RasterizerState = RasterizerState.CullNone;
        gDevice.BlendState = BlendState.Additive;
        gDevice.SamplerStates[0] = SamplerState.LinearWrap;
        gDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, vertArray, 0, vertArray.Length, indices, 0, 
            (indices.Length / 6) );
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
public class TornadoDustParticles : ParticleUpdater<TornadoDustParticles.Data>
{
    public struct Data : IParticleData
    {
        public Vector2 oldTrackingPosition;
        public Vector2 position;
        public Vector2 velocity;
        public int pointToTrack;
        public float timeLeft;
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
            drawer.color = Color.Lerp(Color.Transparent, Color.White * 0.5f, EasingFunction.QuadraticBump(particle.timeLeft / 120f));
            drawer.scale = Vector2.One * 0.5f;
            spriteBatch.Draw(drawer);
        }
        //base.Draw(spriteBatch, screenPos);
    }

    public override void Draw(SpriteBatch spriteBatch, ref Data particle)
    {

    }
}

public class TornadoSmokeParticles : ParticleUpdater<TornadoSmokeParticles.Data>
{
    public struct Data : IParticleData
    {
        public Vector2 oldTrackingPosition;
        public Vector2 position;
        public Vector2 velocity;
        public int pointToTrack;
        public float timeLeft;
        public bool stopMoving;
        public bool IsActive => timeLeft > 0;
    }

    public Vector3[] wrappedOffsets;
    public Vector2 tornadoCenter;
    public float timer;
    public float alpha;
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
                var targetVelocity = (targetCirclePos - particle.position) * 0.3f;
                var sqrDistToPoint = Vector2.DistanceSquared(particle.position, targetCirclePos);
                if (particle.oldTrackingPosition != Vector2.Zero)
                {
                    if (sqrDistToPoint > 128 * 128)
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
                particle.velocity *= 0.98f;
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
            drawer.color = Color.Lerp(Color.Transparent, Color.White * 0.15f, EasingFunction.QuadraticBump(particle.timeLeft / 120f));
            drawer.color *= alpha;
            drawer.scale = Vector2.One * 1.5f;
            spriteBatch.Draw(drawer);
        }
        //base.Draw(spriteBatch, screenPos);
    }

    public override void Draw(SpriteBatch spriteBatch, ref Data particle)
    {

    }
}
public class StormingTornadoV2 : ModProjectile
{
    TornadoMesh CylinderMesh
    {
        get
        {
            if(field == null)
            {
                field = new(height: 1024, radius: 127, heightPerBand: 256, sidesPerBand: 16, Color.White);
            }
            return field;
        }
    }
    TornadoCircles TornadoCircles
    {
        get
        {
            if (field == null)
            {
                field = new(8, 64, 128, 16);
            }
            return field;
        }
    }
    TornadoStreakLinesParticles StreakParticles
    {
        get
        {
            if (field == null)
                field = new();
            return field;
        }
    }
    TornadoSmokeParticles SmokeParticles
    {
        get
        {
            if (field == null)
                field = new();
            return field;
        }
    }
    TornadoDustParticles DustParticles
    {
        get
        {
            if (field == null)
                field = new();
            return field;
        }
    }
    Matrix BandRotationMatrix
    {
        get
        {
            return Matrix.CreateFromAxisAngle(new Vector3(0, 1, 0), Main.GlobalTimeWrappedHourly);
        }
    }
    Matrix PerspectiveMatrix
    {
        get
        {
            return Matrix.CreateFromAxisAngle(new Vector3(1, 0, 0), 0.3f);

        }
    }
    Matrix TornadoMatrixBase
    {
        get
        {
            return BandRotationMatrix * PerspectiveMatrix * Matrix.CreateTranslation(Projectile.Center.X, Projectile.Center.Y, 0);
        }
    }

    Matrix TornadoCullCloseMatrix
    {
        get
        {
            return TornadoMatrixBase * TrailDrawer.WorldViewPointCullClose;
        }
    }
    Matrix TornadoCullFarMatrix
    {
        get
        {
            return TornadoMatrixBase * TrailDrawer.WorldViewPointCullFarAway;
        }
    }
    Color LightGoldenColor => Color.Lerp(Color.Gold, Color.Black, 0.5f);
    Color DarkGoldenColor => Color.Lerp(Color.DarkGoldenrod, Color.Black, 0.5f);
    ref float Timer => ref Projectile.ai[0];
    public override string Texture => TextureRegistry.EmptyTexture;
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.tileCollide = false;

    }
    public override bool ShouldUpdatePosition()
    {
        return false;
    }


    public override void AI()
    {
        base.AI();
        Timer++;
        StreakParticles.wrappedOffsets = TornadoCircles.WrappedOffsets;
        DustParticles.wrappedOffsets = TornadoCircles.WrappedOffsets;
        SmokeParticles.wrappedOffsets = TornadoCircles.WrappedOffsets;
        TornadoCircles.upwardVelocity = -7;
        TornadoCircles.Update(Timer);
        //TornadoCircles.Update(Timer);
        if(Timer % 1 == 0)
        {
            var pointToTrack = TornadoCircles.GetRandPointToTrack();
            ref var offset = ref TornadoCircles.WrappedOffsets[pointToTrack];
            var targetCirclePos = SmokeParticles.tornadoCenter + new Vector2(offset.X, offset.Y);

            SmokeParticles.Spawn(new()
            {
                position = targetCirclePos +Main.rand.NextVector2Circular(128, 32),
                pointToTrack = pointToTrack,
                timeLeft = 120,
            });
        }
        if (Timer % 1 == 0)
        {
            var pointToTrack = Main.rand.Next(0, TornadoCircles.WrappedOffsets.Length);
            ref var offset = ref TornadoCircles.WrappedOffsets[pointToTrack];
            var targetCirclePos = DustParticles.tornadoCenter + new Vector2(offset.X, offset.Y) + Main.rand.NextVector2Circular(48, 48);

            DustParticles.Spawn(new()
            {
                position = targetCirclePos,
                pointToTrack = pointToTrack,
                timeLeft = 120,
            });
        }
        if (Timer % 2 == 0)
        {
            var pointToTrack = TornadoCircles.GetRandPointToTrack();
            ref var offset = ref TornadoCircles.WrappedOffsets[pointToTrack];
            var targetCirclePos = DustParticles.tornadoCenter + new Vector2(offset.X, offset.Y) + Main.rand.NextVector2Circular(48, 48);
            var targetCirclePos3 = new Vector3(targetCirclePos + Main.rand.NextVector2CircularEdge(374, 32), 0);
            StreakParticles.Spawn(new()
            {
                position = targetCirclePos3,
                pointToTrack = pointToTrack,
                timeLeft = 120,
            });
        }
        if (Main.rand.NextBool(15))
        {
            var pos = Projectile.Center + Main.rand.NextVector2Circular(275, 512);
            pos.Y -= 512;
            var vel = -Vector2.UnitY;
            vel *= 12 * Main.rand.NextFloat(1f, 1.6f);
            vel = vel.RotatedBy(Main.rand.NextFloat(0, 6.28f));

            var color = Color.Lerp(LightGoldenColor, DarkGoldenColor, Main.rand.NextFloat(0f, 1f));
            color = Color.Lerp(color, Color.White, 0.6f);


            Common.Particles.Particles.LightningSpark.Spawn(new()
            {
                position = pos,
                velocity = vel,
                color = color,
                scale = Main.rand.NextFloat(0.7f, 1.5f) * 0.3f,
                timeLeft = Main.rand.Next(30, 60)
            });

        }

        if (Main.rand.NextBool(6))
        {
            var pos = Projectile.Center + Main.rand.NextVector2Circular(275, 512);
            pos.Y -= 512;
            var vel = -Vector2.UnitY;
            vel *= 6;
            vel = vel.RotatedBy(Main.rand.NextFloat(0, 6.28f));

            var color = Color.Lerp(LightGoldenColor, DarkGoldenColor, Main.rand.NextFloat(0f, 1f));
            color = Color.Lerp(color, Color.White, 0.6f);


            Common.Particles.Particles.LightningSpikeySpark.Spawn(new()
            {
                position = pos,
                velocity = vel,
                scale = Main.rand.NextFloat(0.4f, 0.8f),
                color = color,
                timeLeft = 50
            });
        }
        if (Main.rand.NextBool(6))
        {
            var pos = Projectile.Center + Main.rand.NextVector2Circular(275, 512);
            pos.Y -= 512;
            var vel = -Vector2.UnitY;
            vel *= 6;
            vel = vel.RotatedBy(Main.rand.NextFloat(0, 6.28f));

            var color = Color.Lerp(LightGoldenColor, DarkGoldenColor, Main.rand.NextFloat(0f, 1f));
            color = Color.Lerp(color, Color.White, 0.6f);


            Common.Particles.Particles.LightningSpikeySpark.Spawn(new()
            {
                position = pos,
                velocity = vel,
                scale = Main.rand.NextFloat(0.4f, 0.8f),
                color = color,
                timeLeft = 50
            });
        }
 
        SmokeParticles.tornadoCenter = Projectile.Center;
        SmokeParticles.Update();

        DustParticles.tornadoCenter = Projectile.Center;
        DustParticles.Update();

        StreakParticles.tornadoCenter = new Vector3(Projectile.Center.X, Projectile.Center.Y, 0);
        StreakParticles.Simulate();
    }

    void DrawTornado()
    {
        var spriteBatch = Main.spriteBatch;
        spriteBatch.EndOut(out var oldParameters);

        var gDevice = spriteBatch.GraphicsDevice;
        gDevice.RasterizerState = RasterizerState.CullNone;
        gDevice.BlendState = BlendState.AlphaBlend;
        gDevice.SamplerStates[0] = SamplerState.LinearWrap;

        var pass = AssetReferences.Effects.Weather.ThunderingTornado.CreatePrimitivesPass();
       
        pass.Parameters.time = Main.GlobalTimeWrappedHourly;
        pass.Parameters.transformMatrix = TornadoCullCloseMatrix;
        pass.Parameters.rotationMatrix = BandRotationMatrix;
        pass.Parameters.tintColor = Color.Lerp(Color.White, Color.Black, 0.75f).ToVector4();
        pass.Parameters.dir = 1f;
        pass.Apply();
        Main.graphics.GraphicsDevice.Textures[0] = AssetReferences.Assets.Noise.TornadoNoise.Asset.Value;
        CylinderMesh.Draw(gDevice);

        pass.Parameters.time = Main.GlobalTimeWrappedHourly;
        pass.Parameters.transformMatrix = TornadoCullFarMatrix;
        pass.Parameters.rotationMatrix = BandRotationMatrix;
        pass.Parameters.tintColor = Color.White.ToVector4();
        pass.Parameters.dir = -0.65F;
        pass.Apply();
        CylinderMesh.Draw(gDevice);
        spriteBatch.Begin(oldParameters);
    }

    void DrawPixelated(SpriteBatch sb, Vector2 sp)
    {
        SmokeParticles.alpha = 0.45f;
        SmokeParticles.Draw(Main.spriteBatch, Main.screenPosition);
        DrawTornado();
        DustParticles.Draw(Main.spriteBatch, Main.screenPosition);
        SmokeParticles.alpha = 0.3f;
        SmokeParticles.Draw(Main.spriteBatch, Main.screenPosition);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        PixelationManager.QueueSpritebatchDrawAction(DrawPixelated, DrawLayer.OverNPCs);
        PixelationManager.QueuePrimitivesDrawAction(StreakParticles.DrawStreaks, DrawLayer.OverPlayers);
        return false;
    }
}

public class TornadoCircles
{
    public TornadoCircles(int numBands, float spaceBetweenBands, float radius, int numPointsPerBand = 8)
    {
        InitialRadians = new float[numPointsPerBand];
        for(var i = 0; i < InitialRadians.Length; i++)
        {
            var ratio = (float)i / (float)InitialRadians.Length;
            InitialRadians[i] = ratio * MathHelper.TwoPi;
        }

        RootOffsets = new Vector3[numBands * numPointsPerBand];
        for(var i =0; i < RootOffsets.Length; i+=numPointsPerBand)
        {
            for(var j = 0; j < numPointsPerBand; j++)
            {
                var index = i + j;
                ref var rootOffset = ref RootOffsets[index];
                rootOffset.Y = i * spaceBetweenBands;
            }
        }
        NumBands = numBands;
        NumPointsPerBand = numPointsPerBand;
        WrappedOffsets = new Vector3[RootOffsets.Length];
        upwardVelocity = -6;
        angularVelocity = 0.05f;
        bandRadius = radius;
        bandSpacing = spaceBetweenBands;
        height = spaceBetweenBands * numBands;
    }

    public readonly Vector3[] RootOffsets;
    public readonly Vector3[] WrappedOffsets;
    public readonly float[] InitialRadians;
    public readonly int NumPointsPerBand;
    public float NumBands;
    public float bandSpacing;
    public float upwardVelocity;
    public float angularVelocity;
    public float bandRadius;
    public float minBandRadius;
    public float height;
    public int GetRandPointToTrack()
    {
        var pool = new List<int>();
        for (var i = 0; i < WrappedOffsets.Length; i += NumPointsPerBand)
        {
            ref var wrappedOffset = ref WrappedOffsets[i];
            if(wrappedOffset.Y < -height * 0.8f)
            {
                continue;
            }
            else
            {
                for(var j = 0; j < NumPointsPerBand; j++)
                {
                    pool.Add(i + j);
                }
            }
        }
        if (pool.Count <= 0)
            return 0;
        return pool[Main.rand.Next(0, pool.Count)];

    }
    public void Update(float timeStep)
    {
        var rotationRadians = timeStep * angularVelocity;
        var perpMatrix = Matrix.CreateFromAxisAngle(new Vector3(1, 0, 0), 0.5f);
        for(var i = 0; i < WrappedOffsets.Length; i+= NumPointsPerBand)
        {
            var bandIndex = i / NumPointsPerBand;

            for (var j = 0; j < NumPointsPerBand; j++)
            {
                var radiansIndex = j;
                var offsetIndex = i + j;
    
                ref var wrappedOffset = ref WrappedOffsets[offsetIndex];

                wrappedOffset.Y = bandIndex * bandSpacing + timeStep * upwardVelocity;
                wrappedOffset.Y %= -height;
                var myBandRadius = MathHelper.Lerp(minBandRadius, bandRadius, (float)wrappedOffset.Y / height);
                //wrappedOffset.Y %= height;

                var initialRadians = InitialRadians[radiansIndex];
                wrappedOffset.X = MathF.Sin(initialRadians + rotationRadians) * myBandRadius;
                wrappedOffset.Z = MathF.Cos(initialRadians + rotationRadians) * myBandRadius;
                wrappedOffset = Vector3.Transform(wrappedOffset, perpMatrix);
            }
 
        }
    }
}
/// <summary>
/// Represents a cylindrical mesh, there is no top or bottom. For use with tornados!
/// </summary>
public class TornadoMesh
{ 
    public TornadoMesh(float height, float radius, float heightPerBand, int sidesPerBand, Color tornadoColor)
    {
        var vertices = new List<VertexPositionColorTexture>();
        //Calculate the number of bands
        var numberOfBands = height / heightPerBand;
        var yHeight = 0F;
        var halfQuadSize = new Vector2(radius / sidesPerBand, heightPerBand / 2f);

        var matricies = new Matrix[sidesPerBand];

        for (var v = 0; v < sidesPerBand; v++)
        {
            var ratio = v / (float)(sidesPerBand - 1);
            var radians = ratio * MathHelper.TwoPi;
            matricies[v] = Matrix.CreateFromAxisAngle(new Vector3(0, 1f, 0), radians);
        }

        //So what we need to do 
        //Is create bands of points, not quads
        //and then create quads between those bands of points so they connect correctly
        //    var bandPoints = new Vector3[(int)numberOfBands * (int)sidesPerBand];
        var bandPoints = new Vector3[(int)numberOfBands][];


        var xTexScale = 1f / (sidesPerBand - 1);
        var yTexScale = 3f;
        var xScale = 0F;
        var yScale = 0F;
        var texCoords = new Vector2[(int)numberOfBands][];
        for (var n = 0; n < numberOfBands; n++)
        {

            bandPoints[n] = new Vector3[sidesPerBand];
            texCoords[n] = new Vector2[sidesPerBand];
            for (var v = 0; v < sidesPerBand; v++)
            {
                var ratio = n / (float)numberOfBands;
                var bandRadius = radius * ratio + 8f;
                ref var bandPoint = ref bandPoints[n][v];
                bandPoint = new Vector3(0, -yHeight, -bandRadius * 2);

                ref var transformationMatrix = ref matricies[v];
                bandPoint = Vector3.Transform(bandPoint, transformationMatrix);
           
                ref var texCoord = ref texCoords[n][v];
                texCoord = new Vector2(xScale, yScale);
                xScale += xTexScale;
            }
            yHeight += heightPerBand;
            yScale += yTexScale;
        }

        //For any given point index, if we offset by the sides per band we get the point opposite of it
        //if we add 1 we get the point to the right of it
        //remember we start from the bottom so
        //hey wait this method is more performant than the old one :sob:
        var primitiveCount = 0;
        for (var n = 0; n < numberOfBands - 1; n++)
        {

            for (var v = 0; v < sidesPerBand; v++)
            {
                var leftIndex = v;
                var rightIndex = (v + 1) % sidesPerBand;

                var bottomLeft = bandPoints[n][leftIndex];
                var bottomRight = bandPoints[n][rightIndex];
                var topLeft = bandPoints[n + 1][leftIndex];
                var topRight = bandPoints[n + 1][rightIndex];

                var bottomLeftCoord = texCoords[n][leftIndex];
                var bottomRightCoord = texCoords[n][rightIndex];
                var topLeftCoord = texCoords[n + 1][leftIndex];
                var topRightCoord = texCoords[n + 1][rightIndex];


                var color = Color.Lerp(tornadoColor, Color.Transparent, n / (float)numberOfBands);
                var nextColor = Color.Lerp(tornadoColor, Color.Transparent, (n + 1) / (float)numberOfBands);
                var normal = new Vector3(0, 0, -1);


                var topLeftVertex = new VertexPositionColorTexture(topLeft, nextColor, topLeftCoord);
                var topRightVertex = new VertexPositionColorTexture(topRight, nextColor, topRightCoord);
                var bottomLeftVertex = new VertexPositionColorTexture(bottomLeft, color, bottomLeftCoord);
                var bottomRightVertex = new VertexPositionColorTexture(bottomRight, color, bottomRightCoord);

                vertices.Add(topLeftVertex);
                vertices.Add(topRightVertex);
                vertices.Add(bottomLeftVertex);
                vertices.Add(bottomRightVertex);

                primitiveCount += 1;
            }
        }

        Vertices = vertices.ToArray();
        Indices = DrawUtilities.TexturedQuadIndices(primitiveCount);
    }
  
    public readonly VertexPositionColorTexture[] Vertices;
    public readonly int[] Indices;
    public void Draw(GraphicsDevice graphicsDevice)
    {
        graphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList,
            Vertices, 0, Vertices.Length, Indices, 0, Indices.Length / 3);
    }
}
