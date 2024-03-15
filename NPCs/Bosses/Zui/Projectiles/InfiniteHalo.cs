using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.Zui.Projectiles
{
	public class InfiniteHalo : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Verlia's Swords Dance");
			Main.projFrames[Projectile.type] = 1;
		}

		public override void SetDefaults()
		{
			Projectile.friendly = false;
			Projectile.hostile = true;
			Projectile.width = 624 / 2;
			Projectile.height = 574 / 2;
			Projectile.penetrate = -1;
			Projectile.timeLeft = 500;
			Projectile.scale = 0.8f;
			DrawOffsetX = -175;
			DrawOriginOffsetY = -175;
		}
		public float Timer
		{
			get => Projectile.ai[0];
			set => Projectile.ai[0] = value;
		}
		int gamer = 0;
		int plant = 0;
		public override void AI()
		{

			Vector3 RGB = new(0.89f, 2.53f, 2.55f);
			// The multiplication here wasn't doing anything
			Lighting.AddLight(Projectile.position, RGB.X, RGB.Y, RGB.Z);

			NPC npc = Main.npc[(int)Projectile.ai[1]];
			Projectile.Center = npc.Center;


			gamer++;
			plant++;

			if (gamer < 10)
            {
				Projectile.alpha++;
			}

			if (gamer > 10)
            {
				if (gamer < 20)
                {

					Projectile.alpha--;




				}

			}

			if (gamer == 21)
            {
				gamer = 0;
            }
			if (plant > 245)
            {

				gamer = 50;
				Projectile.alpha++;
            }
		}

		public override bool PreAI()
		{
			Projectile.tileCollide = false;




			return true;


		}
		public override Color? GetAlpha(Color lightColor)
		{
			return new Color(200, 200, 200, 0) * (1f - Projectile.alpha / 50f);
		}
		public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
		{
			behindProjectiles.Add(index);
			behindNPCs.Add(index);
			behindNPCsAndTiles.Add(index);

		}

	}

}