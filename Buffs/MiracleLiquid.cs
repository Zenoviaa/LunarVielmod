using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Buffs
{
    public class MiracleLiquid : ModBuff
    {
        //This buff doesn't do anything, it just shows you how much time you got left.
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoTimeDisplay[Type] = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.lifeRegen -= 120;
            player.GetDamage(DamageClass.Generic) *= 2.2f;
        }
    }
}
