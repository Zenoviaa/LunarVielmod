using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stellamod.Core.Utilities;


public struct FabrikSegment
{
    public Vector2 position;
    public float radiansOfFreedom;
    public float length;
}

public static class FabrikSolver
{
    public static Vector2 SafeNormalize(Vector2 input)
    {
        if (input == Vector2.Zero)
            return Vector2.Zero;
        input.Normalize();
        return input;
    }
    public static Vector2 ClampTarget(ref FabrikSegment[] segments, Vector2 root, Vector2 destination)
    {
        //Sum the distance between all segments and and the distance between
        //root and destination to see if the target is reachable

        var distanceStartToEnd = Vector2.Distance(root, destination);
        var distanceOfRig = 0F;
        for (var i = 0; i < segments.Length; i++)
        {
            ref var inputSegment = ref segments[i];
            distanceOfRig += inputSegment.length;
        }

        //forward iteration
        var isReachable = distanceOfRig >= distanceStartToEnd;
        if (!isReachable)
        {
            destination = root + (destination - root).Resize(distanceOfRig);
        }

        return destination;
    }
    public static void Solve(ref FabrikSegment[] segments, Vector2 root, Vector2 destination, int iterations)
    {
        //Sum the distance between all segments and and the distance between
        //root and destination to see if the target is reachable

        var distanceStartToEnd = Vector2.Distance(root, destination);
        var distanceOfRig = 0F;
        for (var i = 0; i < segments.Length; i++)
        {
            ref var inputSegment = ref segments[i];
            distanceOfRig += inputSegment.length;
        }

        //forward iteration
        var isReachable = distanceOfRig >= distanceStartToEnd;
        if (!isReachable)
        {
            destination = root + (destination - root).Resize(distanceOfRig);
        }

        for (var j = 0; j < iterations; j++)
        {
            //forawrd iteration
            for (var i = segments.Length - 1; i > 0; i--)
            {
                ref var prevSegment = ref segments[i - 1];
                ref var segment = ref segments[i];
                if (i == segments.Length - 1)
                {
                    prevSegment.position = SafeNormalize(prevSegment.position - destination) * segment.length + destination;
                }
                else
                {
                    prevSegment.position = SafeNormalize(prevSegment.position - segment.position) * segment.length + segment.position;
                }
            }

            //backward iteration
            for (var i = 0; i < segments.Length; i++)
            {
                ref var segment = ref segments[i];
                if (i == 0)
                {
                    segment.position = (SafeNormalize(segment.position - root) * segment.length) + root;
                }
                else if (i == segments.Length - 1)
                {
                    ref var prevSegment = ref segments[i - 1];
                    segment.position = (SafeNormalize(prevSegment.position - destination) * segment.length * -1) + prevSegment.position;
                }
                else
                {
                    ref var prevSegment = ref segments[i - 1];
                    segment.position = (SafeNormalize(segment.position - prevSegment.position) * segment.length) + prevSegment.position;
                }
            }
        }
    }
}