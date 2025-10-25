using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Core.SummonerSystem
{
    public class BellSummoning : ModBuff
    {
        public override void Update(Player player, ref int buffIndex)
        {
            base.Update(player, ref buffIndex);
            player.moveSpeed *= 0.5f;
        }
    }
}
