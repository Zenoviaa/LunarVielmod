using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Buffs.Whipfx
{
	public class LaceratorBuffImmunities : GlobalNPC
    {
        public override void SetDefaults(NPC npc)
        {
			//Nothing is immune.
			npc.buffImmune[ModContent.BuffType<Lacerated>()] = false;
		}
    }

    public class LaceratorDebuff : ModBuff
	{
		public static readonly int TagDamage = 10;

		public override void SetStaticDefaults()
		{
			// This allows the debuff to be inflicted on NPCs that would otherwise be immune to all debuffs.
			// Other mods may check it for different purposes.
			BuffID.Sets.IsATagBuff[Type] = true;
		}
	}
}
