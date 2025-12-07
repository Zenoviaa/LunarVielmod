using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace Stellamod.Helpers
{
    public static class DamageHelper
    {
        /// <summary>
        /// Helper function for increasing the damage of a hit by a certain percent, so we don't accidentally code it incorrectly
        /// </summary>
        /// <param name="modifiers"></param>
        /// <param name="pctIncrease"></param>
        public static void PercentIncreasedamage(ref NPC.HitModifiers modifiers, float pctIncrease)
        {
            modifiers.FinalDamage *= 1.0f + pctIncrease;
        }
    }
}
