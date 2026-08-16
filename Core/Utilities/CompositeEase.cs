using System;
using System.Collections.Generic;

namespace Stellamod.Core.Utilities;

public delegate T Ease<T>(T start, T end, float progress);


/// <summary>
/// Allows for chaining together interpolation between multiple values, by linearly interpolation between several ease functions
/// </summary>
/// <typeparam name="EasingValue"></typeparam>
public class CompositeEase<EasingValue>
{
    public record struct EasingData(EasingValue start, EasingValue end, float ticks, Func<float, float> easingFunction);
    public CompositeEase(Ease<EasingValue> interpolationFunction, float totalTime)
    {
        InterpolationFunction = interpolationFunction;
        Interpolations = new List<EasingData>();
        TotalTime = totalTime;
    }
    public readonly Ease<EasingValue> InterpolationFunction;
    public readonly float TotalTime;
    public List<EasingData> Interpolations;
    public CompositeEase<EasingValue> Add(EasingValue start, EasingValue end, float time, Func<float, float> easingFunction)
    {
        Interpolations.Add(new(start, end, time, easingFunction));
        return this;
    }

    public EasingValue GetValue(float elapsedTicks)
    {
        EasingValue current = default!;
        Queue<EasingValue> easingQueue = new Queue<EasingValue>();
        for (int i = 0; i < Interpolations.Count; i++)
        {
            EasingData easingData = Interpolations[i];
            float p = elapsedTicks / easingData.ticks;
            EasingValue value = InterpolationFunction(easingData.start, easingData.end, easingData.easingFunction(p));
            easingQueue.Enqueue(value);
        }

        float totalP = elapsedTicks / TotalTime;
        //While there's atleast two elements
        while (easingQueue.Count > 1)
        {
            EasingValue start = easingQueue.Dequeue();
            EasingValue end = easingQueue.Dequeue();
            EasingValue newValue = InterpolationFunction(start, end, totalP);
            easingQueue.Enqueue(newValue);
            current = newValue;
        }
        return current!;
    }
}
