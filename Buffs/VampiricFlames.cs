using Microsoft.Xna.Framework;
using Stellamod.Dusts;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Buffs
{
    public class VampiricFlames : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoTimeDisplay[Type] = false;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.lifeRegen -= 120;
            if (Main.rand.NextBool(4))
            {
                Vector2 offset = new Vector2(Main.rand.Next(0, npc.width), Main.rand.Next(0, npc.height));
                Dust.NewDustPerfect(npc.position + offset, ModContent.DustType<GlyphDust>(),
                    Velocity: -Vector2.UnitY * Main.rand.NextFloat(1f, 5f),
                    newColor: Color.Red,
                    Scale: Main.rand.NextFloat(1f, 2f));
            }
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.lifeRegen -= 32;
            player.manaRegen -= 8;
            if (Main.rand.NextBool(4))
            {
                Vector2 offset = new Vector2(Main.rand.Next(0, player.width), Main.rand.Next(0, player.height));
                Dust.NewDustPerfect(player.position + offset, ModContent.DustType<GlyphDust>(),
                    Velocity: -Vector2.UnitY * Main.rand.NextFloat(1f, 5f),
                    newColor: Color.Red,
                    Scale: Main.rand.NextFloat(1f, 2f));
            }
        }
    }
}
