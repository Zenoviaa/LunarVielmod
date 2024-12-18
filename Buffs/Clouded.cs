using Microsoft.Xna.Framework;
using Stellamod.Dusts;
using Terraria;
using Terraria.ModLoader;


namespace Stellamod.Buffs
{
    internal class Clouded : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoTimeDisplay[Type] = false;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            base.Update(npc, ref buffIndex);
            if (Main.rand.NextBool(8))
            {
                Dust.NewDust(npc.position, npc.width, npc.height, ModContent.DustType<GlyphDust>(), newColor: Color.White, Scale: Main.rand.NextFloat(0.5f, 1.5f));
            }

            float risingSpeed = -2f;

            npc.defDefense -= 15;
            npc.lifeRegen -= 4;
            if (npc.velocity.Y > risingSpeed)
                npc.velocity.Y = MathHelper.Lerp(npc.velocity.Y, risingSpeed, 0.5f);
        }

        public override void Update(Player player, ref int buffIndex)
        {
            if (Main.rand.NextBool(8))
            {
                Dust.NewDust(player.position, player.width, player.height, ModContent.DustType<GlyphDust>(), newColor: Color.White, Scale: Main.rand.NextFloat(0.5f, 1.5f));
            }

            float risingSpeed = -2f;
            if (player.velocity.Y > risingSpeed)
                player.velocity.Y = MathHelper.Lerp(player.velocity.Y, risingSpeed, 0.5f);
        }
    }
}
