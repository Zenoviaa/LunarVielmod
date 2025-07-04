﻿using Microsoft.Xna.Framework;

namespace Stellamod.Core.SwingSystem
{
    internal interface ISwing
    {
        void SetDirection(int direction);
        float GetDuration();

        /// <summary>
        /// Update the 
        /// </summary>
        /// <param name="time"></param>
        void UpdateSwing(float time, Vector2 position, Vector2 velocity, out Vector2 offset);
        void CalculateTrailingPoints(float time, Vector2 velocity, ref Vector2[] trailCache);
    }
}
