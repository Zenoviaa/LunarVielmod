using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Buffs
{
    public class CrystalLuck : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoTimeDisplay[Type] = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            if (Main.rand.NextBool(8))
            {

            }
        }
    }
}
