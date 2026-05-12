using System;
using System.Collections.Generic;
using Terraria;

namespace Stellamod.Helpers
{
    public class PatternManager<T>
    {
        private Dictionary<T, float> _defaultWeights;
        private Dictionary<T, float> _weights;
        private Queue<T> _patternOverrideQueue;
        public PatternManager(params Tuple<T, float>[] defaultWeights)
        {
            _patternOverrideQueue = new Queue<T>();
            _defaultWeights = new Dictionary<T, float>();
            _weights = new Dictionary<T, float>();
            for (int i = 0; i < defaultWeights.Length; i++)
            {
                _defaultWeights.Add(defaultWeights[i].Item1, defaultWeights[i].Item2);
            }

            //So we have a list of attack states and weights
            //basically
            //If an attack is successfully dodged, the weight of the attack decreases by 0.5, and cannot go below the default weight
            //If an attack hits, the current weight of the attack increases by 0.25, and the default weight increases by 0.5
        }

        public void EmptyWeights()
        {
            _defaultWeights.Clear();
            _weights.Clear();
        }
        public void AddPattern(T pattern, float weight)
        {
            _defaultWeights.Add(pattern, weight);
        }

        public bool HasNothingLeft()
        {
            float totalWeight = 0f;
            foreach (var kvp in _weights)
            {
                totalWeight += kvp.Value;
            }
            return totalWeight <= 0;
        }
        public void AddWeight(T t, float weight)
        {
            _weights[t] += weight;
        }

        public void AddDefaultWeight(T t, float weight)
        {
            _defaultWeights[t] += weight;
        }

        public void ResetToDefaultWeights()
        {
            _weights.Clear();
            foreach (var kvp in _defaultWeights)
            {
                _weights.Add(kvp.Key, kvp.Value);
            }
        }
        public void ResetToDefaultWeight(T key)
        {
            _weights[key] = 0;
        }




        public void ZeroWeights()
        {
            foreach (var kvp in _defaultWeights)
            {
                _weights[kvp.Key] = 0f;
            }
        }

        public void SetWeight(T t, float weight)
        {
            _weights[t] = weight;
        }

        public void QueueSetPattern(T t)
        {
            _patternOverrideQueue.Enqueue(t);
        }
        public T NextPattern()
        {
            if (_patternOverrideQueue.Count > 0)
                return _patternOverrideQueue.Dequeue();
            int weight = 0;
            int totalWeight = 0;

            //Here we are multiplying the float weight by 1000 because next float isn't exactly reliable at decimal places
            //It's better to just use an int
            foreach (var kvp in _weights)
            {
                int addedWeight = (int)(kvp.Value * 1000);
                totalWeight += addedWeight;
            }

            if (totalWeight <= 0)
            {
                ResetToDefaultWeights();
                return NextPattern();
            }

            var rand = Main.rand;
            float randWeight = rand.Next(totalWeight);
            T result = default;
            foreach (var kvp in _weights)
            {
                int addedWeight = (int)(kvp.Value * 1000);
                weight += addedWeight;
                if (weight >= randWeight)
                {
                    result = kvp.Key;
                    break;
                }
            }

            _weights[result] -= 1.0f;

            //Not sure if this is needed, but just set zero if it goes negative
            if (_weights[result] <= 0)
                _weights[result] = 0;
            return result;
        }
    }
}
