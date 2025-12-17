using Microsoft.Xna.Framework;
using System;
using Terraria;

namespace Stellamod.Core.Utilities
{
    public static class VectorMath
    {
        public static float NormalizeVelocityToAngleBetweenZeroOne(Vector2 velocity)
        {
            Vector2 normalVelocity = velocity.SafeNormalize(Vector2.Zero);
            float angle = MathF.Atan2(velocity.Y, velocity.X);

            //Get the number between 0-1
            float normalAngle = angle / MathHelper.Pi * 0.5f + 0.5f;
            return normalAngle;
        }


    }
}
