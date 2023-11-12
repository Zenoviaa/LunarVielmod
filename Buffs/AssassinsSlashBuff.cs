using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Buffs
{
    public class AssassinsSlashBuff : ModBuff
    {
        public bool GO;
        public float Time;
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoTimeDisplay[Type] = false;
            // DisplayName.SetDefault("Assassins Slash");
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            if (Main.rand.NextBool(2))
            {
                int dust = Dust.NewDust(npc.position, npc.width, npc.height, DustID.RedTorch);
                Main.dust[dust].scale = 1.5f;
                Main.dust[dust].noGravity = true;
                int dust1 = Dust.NewDust(npc.position, npc.width, npc.height, DustID.RedTorch);
                Main.dust[dust1].scale = 1.5f;
                Main.dust[dust1].noGravity = true;
            }
        }
    }
}