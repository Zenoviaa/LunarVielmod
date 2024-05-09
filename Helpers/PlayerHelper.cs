using Microsoft.Xna.Framework;
using Terraria;

namespace Stellamod.Helpers
{
    internal static class PlayerHelper
    {
        public static bool RemoveItem(this Player player, int reqItem, int count = 1)
        {
            foreach (Item item in player.inventory)
            {
                if (item.type == reqItem)
                {
                    item.stack-= count;
                    return true;
                }
            }

            return false;
        }
        public static bool HasItemEquipped(this Player player, Item reqItem)
        {
            foreach (Item item in player.armor)
            {
                if (item == reqItem)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool HasItemEquipped(this Player player, int reqItem)
        {
            foreach (Item item in player.armor)
            {
                if (item.type == reqItem)
                {
                    return true;
                }
            }

            return false;
        }
        public static Player FindClosestPlayer(Vector2 position, float maxDetectDistance)
        {
            Player closestPlayer = null;

            // Using squared values in distance checks will let us skip square root calculations, drastically improving this method's speed.
            float sqrMaxDetectDistance = maxDetectDistance * maxDetectDistance;

            // Loop through all NPCs(max always 200)
            for (int k = 0; k < Main.maxPlayers; k++)
            {
                var target = Main.player[k];
                if (!target.active)
                    continue;

                // Check if NPC able to be targeted. It means that NPC is
                // 1. active (alive)
                // 2. chaseable (e.g. not a cultist archer)
                // 3. max life bigger than 5 (e.g. not a critter)
                // 4. can take damage (e.g. moonlord core after all it's parts are downed)
                // 5. hostile (!friendly)
                // 6. not immortal (e.g. not a target dummy)
                // The DistanceSquared function returns a squared distance between 2 points, skipping relatively expensive square root calculations
                float sqrDistanceToTarget = Vector2.DistanceSquared(target.Center, position);

                // Check if it is within the radius
                if (sqrDistanceToTarget < sqrMaxDetectDistance)
                {
                    sqrMaxDetectDistance = sqrDistanceToTarget;
                    closestPlayer = target;
                }
            }

            return closestPlayer;
        }
    }
}
