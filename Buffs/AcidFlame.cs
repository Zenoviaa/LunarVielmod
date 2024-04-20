using Microsoft.Xna.Framework;
using ParticleLibrary;
using Stellamod.Helpers;
using Stellamod.Items.Armors.AcidArmour;
using Stellamod.Particles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;


namespace Stellamod.Buffs
{
    public class AcidFlame : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoTimeDisplay[Type] = false;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.lifeRegen -= 30;
        }

        public override void Update(Player player, ref int buffIndex)
        {

            player.lifeRegen -= 16;
            player.manaRegen -= 8;
            player.blind = true;
            player.blackout = true;
            player.yoraiz0rDarkness = true;
        }
    }
}