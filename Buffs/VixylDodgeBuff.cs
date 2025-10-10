using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Buffs
{
    public class VixylDodgeBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {

        }
    }
}
