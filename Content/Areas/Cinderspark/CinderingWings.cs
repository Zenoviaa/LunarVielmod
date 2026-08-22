using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Cinderspark;

public class CinderingWings : ModBuff
{
    public override void SetStaticDefaults()
    {
        Main.pvpBuff[Type] = true;
        Main.buffNoTimeDisplay[Type] = true;
    }


    public override void Update(Player player, ref int buffIndex)
    {
        base.Update(player, ref buffIndex);
        if (player.wingTime > player.wingTimeMax / 2)
            player.wingTime = player.wingTimeMax / 2;
        if (player.mount.Active && player.mount.CanFly())
        {
            player.mount.Dismount(player);
            CombatText.NewText(player.getRect(), Color.White, LangText.Common("NoMount"));
        }
    }
}
