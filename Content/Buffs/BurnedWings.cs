using Stellamod.Helpers;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Buffs;

public class BurnedWings : ModBuff
{
    public override void SetStaticDefaults()
    {
        Main.pvpBuff[Type] = true;
        Main.buffNoTimeDisplay[Type] = true;
    }


    public override void Update(Player player, ref int buffIndex)
    {
        base.Update(player, ref buffIndex);
        player.wingTimeMax = 0;
        player.wingTime = 0;
        if (player.mount.Active && player.mount.CanFly())
        {
            player.mount.Dismount(player);
            CombatText.NewText(player.getRect(), Color.White, LangText.Common("NoMount"));
        }

        if (DownedBossTracker.IsDowned(DownedBossFlag.StarBomber))
        {
            player.ClearBuff(Type);
        }
    }
}
