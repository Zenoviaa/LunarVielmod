using Microsoft.Xna.Framework;
using ParticleLibrary;
using Stellamod.Helpers;
using Stellamod.Particles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;


namespace Stellamod.Buffs
{
    public class AcidFlame : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoTimeDisplay[Type] = false;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.lifeRegen -= 30;
            if (Main.rand.NextBool(6))
            {
                int x = Main.rand.Next(0, npc.width);
                int y = Main.rand.Next(0, npc.height);
                ParticleManager.NewParticle<AcidFlameParticle>(npc.position + new Vector2(x, y), Vector2.Zero, Color.White, 1f);
            }
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.lifeRegen -= 16;
            player.manaRegen -= 8;
            player.blind = true;
            player.blackout = true;
            player.yoraiz0rDarkness = true;
            if (Main.rand.NextBool(6))
            {
                int x = Main.rand.Next(0, player.width);
                int y = Main.rand.Next(0, player.height);
                ParticleManager.NewParticle<AcidFlameParticle>(player.position + new Vector2(x, y), Vector2.Zero, Color.White, 1f);
            }
        }
    }
}