using Stellamod.Content.Areas.Tundra.Abyss.AccAB;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Tundra.Abyss.BossesAB.RoyalStar;

public class Grounded : ModBuff
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Main.debuff[Type] = true;
    }
    public override void Update(Player player, ref int buffIndex)
    {
        base.Update(player, ref buffIndex);
        player.GetModPlayer<FastFlightPlayer>().weakWinged = true;
    }
}
