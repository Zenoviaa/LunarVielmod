using System;
using Terraria;

namespace Stellamod.Common.TornadoRendering;

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
        BandWarps = new Vector3[numBands * numPointsPerBand];
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
        RandPointBuffer = new int[WrappedOffsets.Length];
        upwardVelocity = -6;
        angularVelocity = 0.05f;
        bandRadius = radius;
        bandSpacing = spaceBetweenBands;
        height = spaceBetweenBands * numBands;
    }

    public readonly Vector3[] RootOffsets;
    public readonly Vector3[] WrappedOffsets;
    public readonly Vector3[] BandWarps;
    public readonly float[] InitialRadians;
    public readonly int[] RandPointBuffer;
    public readonly int NumPointsPerBand;
    public float NumBands;
    public float bandSpacing;
    public float upwardVelocity;
    public float angularVelocity;
    public float bandRadius;
    public float minBandRadius;
    public float height;
    public Matrix perspectiveMatrix;
    public int GetRandPointToTrack()
    {
        var length = 0;
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
                    RandPointBuffer[length++] = i + j;
                }
            }
        }
        if (length <= 0)
            return 0;
        return RandPointBuffer[Main.rand.Next(0, length)];

    }

    public void ApplyWarp(Vector3[] warpOffsets)
    {
        for (var i = 0; i < BandWarps.Length; i += NumPointsPerBand)
        {
            var bandIndex = i / NumPointsPerBand;

            for (var j = 0; j < NumPointsPerBand; j++)
            {
                var radiansIndex = j;
                var offsetIndex = i + j;
                BandWarps[offsetIndex] = warpOffsets[bandIndex];
            }
        }
    }

    
    public void Update(float timeStep)
    {
        var rotationRadians = timeStep * angularVelocity;

        var startOffset = upwardVelocity * 160;

        for(var i = 0; i < WrappedOffsets.Length; i+= NumPointsPerBand)
        {
            var bandIndex = i / NumPointsPerBand;

            for (var j = 0; j < NumPointsPerBand; j++)
            {
                var radiansIndex = j;
                var offsetIndex = i + j;
    
                ref var wrappedOffset = ref WrappedOffsets[offsetIndex];

                wrappedOffset.Y = bandIndex * bandSpacing + timeStep * upwardVelocity + startOffset;
                wrappedOffset.Y %= -height;
                var myBandRadius = MathHelper.Lerp(minBandRadius, bandRadius, (float)wrappedOffset.Y / height);
    
                var initialRadians = InitialRadians[radiansIndex];
                wrappedOffset.X = MathF.Sin(initialRadians + rotationRadians) * myBandRadius;
                wrappedOffset.Z = MathF.Cos(initialRadians + rotationRadians) * myBandRadius;
                wrappedOffset = Vector3.Transform(wrappedOffset, perspectiveMatrix);
                wrappedOffset += BandWarps[bandIndex];
            }
        }
    }
}
