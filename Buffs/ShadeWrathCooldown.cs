using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Buffs
{
    public class ShadeWrathCooldown : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoTimeDisplay[Type] = false;
        }
    }
}
