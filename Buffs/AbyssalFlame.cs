using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Buffs
{
    public class AbyssalFlame : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoTimeDisplay[Type] = false;
            // DisplayName.SetDefault("Abyssal Flame");
            // Description.SetDefault("'A Dark force saps your vitality'");
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.lifeRegen -= 30;
            if (Main.rand.NextBool(2))
            {
                int dust = Dust.NewDust(npc.position, npc.width, npc.height, DustID.UnusedWhiteBluePurple);
                Main.dust[dust].scale = 3.5f;
                Main.dust[dust].noGravity = true;
            }
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.lifeRegen -= 8;
            if (Main.rand.NextBool(2))
            {
                int dust = Dust.NewDust(player.position, player.width, player.height, DustID.UnusedWhiteBluePurple);
                Main.dust[dust].scale = 3.5f;
                Main.dust[dust].noGravity = true;
            }
        }
    }
}