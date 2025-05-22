using Microsoft.Xna.Framework;

using Stellamod.Helpers;
using Stellamod.Particles;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Buffs
{
    internal class ShadeWrath : ModBuff
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Charm Buff!");
            // Description.SetDefault("A true warrior such as yourself knows no bounds");
            Main.debuff[Type] = false;
            Main.pvpBuff[Type] = true;
            Main.buffNoTimeDisplay[Type] = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetDamage(DamageClass.Generic) += 0.5f;
            player.moveSpeed += 0.5f;

        }
    }
}
