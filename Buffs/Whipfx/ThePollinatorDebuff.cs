using Microsoft.Xna.Framework;
using Stellamod.Dusts;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Buffs.Whipfx
{
    public class ThePollinatorDebuff : ModBuff
	{
		public static readonly int TagDamage = 10;

		public override void SetStaticDefaults()
		{
			// This allows the debuff to be inflicted on NPCs that would otherwise be immune to all debuffs.
			// Other mods may check it for different purposes.
			BuffID.Sets.IsATagBuff[Type] = true;
		}

        public override void Update(NPC npc, ref int buffIndex)
        {
            if (Main.rand.NextBool(8))
            {
                for (int i = 0; i < 2; i++)
                {
                    int d = Dust.NewDust(npc.position, npc.width, npc.height,
                        ModContent.DustType<GlowDust>(), newColor: Color.DarkGoldenrod * 0.33f);
                    Main.dust[d].noGravity = true;
                }
            }

            //SLOW EM DOWN MWAHAHAH
            npc.velocity *= 0.75f;
        }
    }
}
