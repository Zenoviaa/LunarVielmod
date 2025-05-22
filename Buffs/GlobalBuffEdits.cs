using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Buffs
{
    internal class GlobalBuffEdits : GlobalBuff
    {
        public override void Update(int type, Player player, ref int buffIndex)
        {
            base.Update(type, player, ref buffIndex);

            //NERFS
            if(type == BuffID.NebulaUpDmg1)
            {
                player.GetDamage(DamageClass.Magic) -= 0.075f;
            }
            else if (type == BuffID.NebulaUpDmg1)
            {
                player.GetDamage(DamageClass.Magic) -= 0.15f;
            }
            else if (type == BuffID.NebulaUpDmg1)
            {
                player.GetDamage(DamageClass.Magic) -= 0.225f;
            }
        }
    }
}
