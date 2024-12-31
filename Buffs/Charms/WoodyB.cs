using Microsoft.Xna.Framework;
using Stellamod.Dusts;
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
                float num = 16;
                for (int i = 0; i < num; i++)
                {
                    float progress = (float)i / num;
                    Vector2 velocity = Vector2.Zero;
                    Vector2 offset = -Vector2.UnitY;
                    offset = offset.RotatedBy(progress * MathHelper.TwoPi);
                    offset *= 80;
                    Vector2 position = player.Center + offset;
                    Dust.NewDustPerfect(position, ModContent.DustType<Sparkle>(), velocity, newColor: Color.ForestGreen, Scale: 0.7f);
                }
                player.GetDamage(DamageClass.Generic) += 0.3f;
            }
            else
            {
                player.GetDamage(DamageClass.Generic) -= 0.3f;
            }  
        }
    }
}
