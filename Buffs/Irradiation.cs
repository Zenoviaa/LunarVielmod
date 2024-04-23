using Microsoft.Xna.Framework;
using Stellamod.Helpers;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Buffs
{
    public class Irradiation : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoTimeDisplay[Type] = false;
            // DisplayName.SetDefault("Irradiation");
            // Description.SetDefault("'your Insides are Irradiating'");
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.lifeRegen -= 8;
            if (Main.rand.NextBool(3))
            {
                for (int i = 0; i < 1; i++)
                {
                    int d = Dust.NewDust(npc.position, npc.width, npc.height, ModContent.DustType<Dusts.GlowDust>(), newColor: ColorFunctions.AcidFlame, Scale: 0.33f);
                    Main.dust[d].rotation = (Main.dust[d].position - npc.position).ToRotation() - MathHelper.PiOver4;
                    Main.dust[d].velocity *= 0.5f;
                }
            }
        }

        public override void Update(Player player, ref int buffIndex)
        {
            if (player.wet)
            {
                player.lifeRegen -= 76;
            }
            else
            {
                player.lifeRegen -= 36;
                if (Main.rand.NextBool(2))
                {
                    int dust = Dust.NewDust(player.position, player.width, player.height, DustID.CursedTorch);
                    Main.dust[dust].scale = 1.5f;
                    Main.dust[dust].noGravity = true;
                }
            }
        }
    }
}