using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Utilis
{
    public static class BossUtils
    {
        /// <summary>
        /// Spawn boss with MP compatiblity.
        /// Currently, with this method the boss will spawn on player who Right clicked the tile.
        /// <see cref="Player.LookForTileInteractions"/> Method ensures that.
        /// </summary>
        /// <param name="type">Type of boss need spawning</param>
        public static void SpawnBossFromTileRightClick(int i, int j ,int type)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
                NPC.SpawnOnPlayer(Main.myPlayer, type);
            else
                NetMessage.SendData(MessageID.SpawnBossUseLicenseStartEvent, -1, -1, null, Main.myPlayer, type);
        }
        /// <summary>
        /// Set some of static defaults for bosses.
        /// Currently set <see cref="NPCID.Sets.MPAllowedEnemies"/>[type] as true,
        /// and add the boss to <see cref="NPCID.Sets.BossBestiaryPriority"/>.
        /// </summary>
        /// <param name="npc"></param>
        public static void SetBossStaticDefault(this NPC npc)
        {
            int Type = npc.type;
            NPCID.Sets.MPAllowedEnemies[Type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(Type);
        }
    }
}
