using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Tundra.MoonspiralTower;

public class ForceNighttimeInMoonspiralTower : ModSystem
{
    public override void PostUpdateTime()
    {
        base.PostUpdateTime();
        bool fastForward = false;
        foreach (var player in Main.ActivePlayers)
        {
            if (player.InModBiome<MoonspiralTowerBiome>())
            {
                fastForward = true;
                break;
            }
        }
        if (Main.dayTime && fastForward)
        {
            Main.time += 128;
            return;
        }


        Main.time = (double)MathHelper.Lerp((float)Main.time, (float)Main.nightLength * 0.5f, 0.1f);
    }
}
