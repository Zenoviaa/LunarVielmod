using Microsoft.Xna.Framework;
using Stellamod.Core.Bases;
using Terraria;

namespace Stellamod.Common
{
    /// <summary>
    /// A collection of utility functions for common mechanics that are typically on projectiles or npcs
    /// </summary>
    public static class CombatMechanicsHelper
    {
        public static void CreateEnemySuckingEffect(Vector2 center, float strength, float radius)
        {
            foreach (var npc in Main.ActiveNPCs)
            {
                if (npc.friendly)
                    continue;

                float distance = Vector2.Distance(center, npc.Center);
                if (distance > radius)
                    continue;
                var sucker = npc.GetGlobalNPC<GlobalNPCSucker>();

                Vector2 diff = center - npc.Center;
                diff = diff.SafeNormalize(Vector2.Zero);
                Vector2 velocity = diff * strength * npc.knockBackResist;
                sucker.AdditiveSuckVelocity += velocity;
            }
        }
    }
}
