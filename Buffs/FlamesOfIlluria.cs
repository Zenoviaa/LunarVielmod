using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Buffs
{
    internal class FlamesOfIlluria : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoTimeDisplay[Type] = false;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.lifeRegen -= 100;
            float speed = 2;
            Vector2 velocity = -Vector2.UnitY * speed;
            velocity = velocity.RotatedByRandom(MathHelper.PiOver4 / 4);
            Vector2 offset = Main.rand.NextVector2Circular(8, 8);
            if (Main.rand.NextBool(4))
            {
                //Snowflake particle
            }
            else
            {
                //Star particle
            }
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.lifeRegen -= 100;
            float speed = 2;
            Vector2 velocity = -Vector2.UnitY * speed;
            velocity = velocity.RotatedByRandom(MathHelper.PiOver4 / 4);
            Vector2 offset = Main.rand.NextVector2Circular(8, 8);
            if (Main.rand.NextBool(4))
            {
                //Snowflake particle
            }
            else
            {
                //Star particle
            }
        }
    }
}
