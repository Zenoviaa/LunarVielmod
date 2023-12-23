using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.Fenix.Projectiles
{
	public class SpawnFen : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("FrostShotIN");
			Main.projFrames[Projectile.type] = 13;
		}

		public override void SetDefaults()
		{
			Projectile.friendly = false;
			Projectile.width = 348;
			Projectile.height = 368;
			Projectile.penetrate = -1;
			Projectile.timeLeft = 65;
			Projectile.scale = 1f;

		}
		public float Timer
		{
			get => Projectile.ai[0];
			set => Projectile.ai[0] = value;
		}
		public override void AI()
		{

			Vector3 RGB = new(0.89f, 2.53f, 2.55f);
			// The multiplication here wasn't doing anything
			Lighting.AddLight(Projectile.position, RGB.X, RGB.Y, RGB.Z);

		}

		public override bool PreAI()
		{
			Projectile.tileCollide = false;
			if (++Projectile.frameCounter >= 5)
			{
				Projectile.frameCounter = 0;
				if (++Projectile.frame >= 13)
				{
					Projectile.frame = 0;
				}
			}
			return true;


		}
		public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
		{
			behindNPCsAndTiles.Add(index);
			behindNPCs.Add(index);
			overWiresUI.Add(index);
		}
		public override Color? GetAlpha(Color lightColor)
		{
			return new Color(155, 100, 155, 0) * (1f - Projectile.alpha / 50f);
		}


	}

}