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


    public class LaceratorDebuffNPC : GlobalNPC
    {
        // This is required to store information on entities that isn't shared between them.
        public override bool InstancePerEntity => true;

        public bool markedByWhip;

        public override void ResetEffects(NPC npc)
        {
            markedByWhip = false;
        }

        // TODO: Inconsistent with vanilla, increasing damage AFTER it is randomised, not before. Change to a different hook in the future.
        public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            // Only player attacks should benefit from this buff, hence the NPC and trap checks.
            if (markedByWhip && !projectile.npcProj && !projectile.trap && (projectile.minion || ProjectileID.Sets.MinionShot[projectile.type]))
            {
                projectile.damage += LaceratorDebuff.TagDamage;
            }
        }
    }
}
