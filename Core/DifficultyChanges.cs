using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Core
{
    public class DifficultyPlayer : ModPlayer
    {
        public bool AnyPlayersAlive()
        {
            foreach(var player in Main.ActivePlayers)
            {
                if (!player.dead)
                    return true;
            }
            return false;
        }
        public override void UpdateDead()
        {
            base.UpdateDead();
            if (!Main.expertMode && Main.masterMode)
                return;
            if (NPC.AnyDanger() && Main.netMode != NetmodeID.SinglePlayer && AnyPlayersAlive())
            {
                Player.respawnTimer = 60 * 5;
            }
            
        }
    }

    public class DifficultyChanges : ModSystem
    {
        public static void ApplyDifficultyAndScaling(NPC npc, float numPlayers)
        {
            float balance = 1.0f + (0.6f * (numPlayers - 1));
            npc.lifeMax = (int)(npc.lifeMax * balance);
        }
    }
}
