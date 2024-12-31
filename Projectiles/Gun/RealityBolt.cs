using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Gun
{
    public class RealityBolt : ModProjectile
    {
        bool Moved;
        public int Color;
        public override void SetStaticDefaults()
		{
            // DisplayName.SetDefault("Star Bolt");
		}

		public override void SetDefaults()
		{
			Projectile.width = 4;       //projectile width
			Projectile.height = 4;  //projectile height
			Projectile.friendly = true;      //make that the projectile will not damage you
			Projectile.DamageType = DamageClass.Ranged;         // 
			Projectile.tileCollide = true;   //make that the projectile will be destroed if it hits the terrain
			Projectile.timeLeft = 200;   //how many time projectile projectile has before disepire // projectile light
			Projectile.ignoreWater = true;
			Projectile.alpha = 255;
			Projectile.aiStyle = -1;
			Projectile.hide = true;
		}

        public override void OnKill(int timeLeft)
        {
            if (Main.myPlayer == Projectile.owner && Main.rand.NextBool(4))
            {
                int itemIndex = Item.NewItem(Projectile.GetSource_Death(), Projectile.getRect(), ItemID.Star, Main.rand.Next(1, 2), false, 0, false, false);
                NetMessage.SendData(MessageID.SyncItem, -1, -1, null, itemIndex, 1f);
            }
        }

        public override void AI()
		{
			Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            if (!Moved)
            {
                Color = Main.rand.Next(1, 4);
                Moved = true;
            }


            if (Projectile.alpha < 170)
			{
				for (int i = 0; i < 10; i++)
				{
					float x = Projectile.position.X - 3 - Projectile.velocity.X / 10f * i;
					float y = Projectile.position.Y - 3 - Projectile.velocity.Y / 10f * i;
					int num = Dust.NewDust(new Vector2(x, y), 2, 2, DustID.SilverCoin);
					Main.dust[num].alpha = Projectile.alpha;
					Main.dust[num].velocity = Vector2.Zero;
					Main.dust[num].noGravity = true;
					int num2 = Dust.NewDust(new Vector2(x, y), 2, 2, DustID.SilverCoin);
					Main.dust[num2].alpha = Projectile.alpha;
					Main.dust[num2].velocity = Vector2.Zero;
					Main.dust[num2].noGravity = true;
				}

            }


			Projectile.alpha = Math.Max(0, Projectile.alpha - 25);

			bool flag25 = false;
			int jim = 1;
			for (int index1 = 0; index1 < 200; index1++)
			{
				if (Main.npc[index1].CanBeChasedBy(Projectile, false)
					&& Projectile.Distance(Main.npc[index1].Center) < 500
					&& Collision.CanHit(Projectile.Center, 1, 1, Main.npc[index1].Center, 1, 1))
				{
					flag25 = true;
					jim = index1;
				}
			}

			if (flag25)
			{
				float num1 = 6f;
				Vector2 vector2 = new Vector2(Projectile.position.X + Projectile.width * 0.5f, Projectile.position.Y + Projectile.height * 0.5f);
				float num2 = Main.npc[jim].Center.X - vector2.X;
				float num3 = Main.npc[jim].Center.Y - vector2.Y;
				float num4 = (float)Math.Sqrt((double)num2 * num2 + num3 * num3);
				float num5 = num1 / num4;
				float num6 = num2 * num5;
				float num7 = num3 * num5;
				int num8 = 10;
				Projectile.velocity.X = (Projectile.velocity.X * (num8 - 1) + num6) / num8;
				Projectile.velocity.Y = (Projectile.velocity.Y * (num8 - 1) + num7) / num8;
			}
		}
	}
}