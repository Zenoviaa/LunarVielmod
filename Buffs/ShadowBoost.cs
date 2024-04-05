using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Buffs
{

    public class ShadowBoost : ModBuff
    {
        //This buff doesn't do anything, it just shows you how much time you got left.
        public override void SetStaticDefaults()
        {
            Main.pvpBuff[Type] = true;
            Main.buffNoTimeDisplay[Type] = false;
        }
    }
}
