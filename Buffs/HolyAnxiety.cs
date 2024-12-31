using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Buffs
{
    public class HolyAnxietyGlobalNPC : GlobalNPC
    {
        public override void SetDefaults(NPC entity)
        {
            base.SetDefaults(entity);
            int buffType = ModContent.BuffType<HolyAnxiety>();
            switch (entity.type)
            {
                case NPCID.TheDestroyer:
                case NPCID.TheDestroyerBody:
                case NPCID.TheDestroyerTail:
                    entity.buffImmune[buffType] = true;
                break;
            }
            if (entity.boss)
            {
                entity.buffImmune[buffType] = true;
            }
        }
    }

    public class HolyAnxiety : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoTimeDisplay[Type] = false;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {

        }

        public override void Update(Player player, ref int buffIndex)
        {

        }
    }
}