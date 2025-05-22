using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Buffs
{
 
    public class LaceratedNPC : GlobalNPC
    {
        public override void ModifyHitNPC(NPC npc, NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(npc, target, ref modifiers);
            if (npc.HasBuff<Lacerated>())
            {
                modifiers.Defense -= 15;
            }
        }
    }

    public class Lacerated : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoTimeDisplay[Type] = false;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {

            if (Main.rand.NextBool(2))
            {
                int dust = Dust.NewDust(npc.position, npc.width, npc.height, DustID.DarkCelestial);
                Main.dust[dust].scale = 1.5f;
                Main.dust[dust].noGravity = true;
            }
        }
    }
}
