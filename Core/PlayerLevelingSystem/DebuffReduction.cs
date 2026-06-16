using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Core.PlayerLevelingSystem;

public class DebuffReduction : ModSystem
{
    public override void Load()
    {
        base.Load();
        On_Player.AddBuff_DetermineBuffTimeToAdd += ReduceDebuffTime;
    }

    private int ReduceDebuffTime(On_Player.orig_AddBuff_DetermineBuffTimeToAdd orig, Player self, int type, int time1)
    {
        /*
        if (type == BuffID.PotionSickness || type == BuffID.ManaSickness)
            return orig(self, type, time1);
        */
        if (Main.debuff[type])
        {
            float baseTime = time1;
            float strength = self.GetModPlayer<LevelingPlayer>().FinalResourcefulness * 0.015f;
            float amountToRemove = baseTime * strength;
            float newTime = baseTime - amountToRemove;
            time1 = (int)newTime;
        }
        else
        {
            //Positive Effect :)
            float baseTime = time1;
            float strength = self.GetModPlayer<LevelingPlayer>().FinalResourcefulness * 0.05f;
            float amountToAdd = baseTime * strength;
            float newTime = baseTime + amountToAdd;
            time1 = (int)newTime;
        }
        return orig(self, type, time1);
    }
}
