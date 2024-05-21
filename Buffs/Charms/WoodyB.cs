using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Buffs.Charms
{
    internal class WoodyB : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.pvpBuff[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            if (Collision.SolidCollision(player.position + new Vector2(0, 2), player.width, player.height, true))
            {
                Dust.NewDustPerfect(new Vector2(player.position.X + Main.rand.Next(player.width), player.position.Y + player.height - Main.rand.Next(7)), DustID.t_LivingWood, Vector2.Zero);
                player.GetDamage(DamageClass.Generic) += 0.3f;
            }
            else
            {
                player.GetDamage(DamageClass.Generic) -= 0.3f;
            }  
        }
    }
}
