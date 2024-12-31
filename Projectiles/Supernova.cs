using Microsoft.Xna.Framework;
using Stellamod.UI.Systems;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles
{
    public class Supernova : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("FrostShotIN");
			Main.projFrames[Projectile.type] = 25;
		}
		
		public override void SetDefaults()
		{
			Projectile.friendly = true;
			Projectile.width = 280;
			Projectile.height = 280;
			Projectile.penetrate = -1;
			Projectile.timeLeft = 50;
			Projectile.scale = 0.9f;
			
		}

		public float Timer2 = 0;
		public float Timer
		{
			get => Projectile.ai[0];
			set => Projectile.ai[0] = value;
		}
        public override void AI()
        {
			Projectile.rotation += 0.03f;
			Vector3 RGB = new(0.89f, 2.53f, 2.55f);
			// The multiplication here wasn't doing anything
			Lighting.AddLight(Projectile.position, RGB.X, RGB.Y, RGB.Z);

			Timer2++;

			if (Timer2 <= 2)
            {
				ShakeModSystem.Shake = 5;
			}
			
		}
		
		public override bool PreAI()
		{
			Projectile.tileCollide = false;
			if (++Projectile.frameCounter >= 2)
			{
				Projectile.frameCounter = 0;
				if (++Projectile.frame >= 25)
				{
					Projectile.frame = 0;
				}
			}
			return true;

			
		}
		public override Color? GetAlpha(Color lightColor)
		{
			return new Color(200, 200, 200, 0) * (1f - Projectile.alpha / 50f);
		}

		public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
		{
			behindNPCs.Add(index);

		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			

			SoundEngine.PlaySound(SoundID.Item110, Projectile.position);
		}
	}

}