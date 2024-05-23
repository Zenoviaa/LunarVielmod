using Microsoft.Xna.Framework;
using Stellamod.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Buffs
{
    internal class MoonFlame : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoTimeDisplay[Type] = false;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            if(npc.life > npc.lifeMax / 2)
            {
                npc.life = npc.lifeMax / 2;
            }

            npc.lifeRegen -= 8;
            if (Main.rand.NextBool(3))
            {
                for (int i = 0; i < 1; i++)
                {
                    int d = Dust.NewDust(npc.position, npc.width, npc.height, ModContent.DustType<Dusts.GlowDust>(), newColor: ColorFunctions.Niivin, Scale: 0.33f);
                    Main.dust[d].rotation = (Main.dust[d].position - npc.position).ToRotation() - MathHelper.PiOver4;
                    Main.dust[d].velocity *= 0.5f;
                }
            }
        }
    }
}
