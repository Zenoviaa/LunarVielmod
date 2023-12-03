using Microsoft.Xna.Framework;
using Terraria;

namespace Stellamod.Helpers
{
    internal static class EntityExtension
    {
        /// <summary>
        /// Returns a random position that is within the entity's width and height, this is calculated from the width/height values.
        /// <param name="entity"></param>
        /// <returns></returns>
        public static Vector2 RandomPositionWithinEntity(this Entity entity)
        {
            Vector2 center = entity.Center;
            float halfWidth = entity.width / 2;
            float halfHeight = entity.height / 2;
            float x = Main.rand.NextFloat(-halfWidth, halfWidth);
            float y = Main.rand.NextFloat(-halfHeight, halfHeight);
            return center + new Vector2(x, y);
        }

        /// <summary>
        /// Returns an origin point that is the center of the entity, this is calculated from the width/height values.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static Vector2 OriginCenter(this Entity entity)
        {
            return new Vector2(entity.width / 2, entity.height / 2);
        }
    }
}
