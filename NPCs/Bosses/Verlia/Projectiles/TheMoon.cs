using Microsoft.Xna.Framework;
using Stellamod.UI.Systems;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.Verlia.Projectiles
{
    public class TheMoon : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("The Coveted Great Moon");
			Main.projFrames[Projectile.type] = 20;
		}
		private int rippleCount = 3;
		private int rippleSize = 5;
		private int rippleSpeed = 15;
		private float distortStrength = 100f;
		public override void SetDefaults()
		{
			Projectile.friendly = false;
			Projectile.hostile = false;
			Projectile.width = 300;
			Projectile.height = 300;
			Projectile.penetrate = -1;
			Projectile.timeLeft = 300;
			Projectile.scale = 1f;
			
			
			
		}
		public float Timer
		{
			get => Projectile.ai[0];
			set => Projectile.ai[0] = value;
		}

        public override void AI()
        {
			
			Projectile.scale *= .98f;

			
			Projectile.velocity *= 0.96f;
			Vector3 RGB = new(0.89f, 2.53f, 2.55f);
			// The multiplication here wasn't doing anything
			Lighting.AddLight(Projectile.position, RGB.X, RGB.Y, RGB.Z);
			Timer++;
		

			if (Projectile.timeLeft == 180)
			{
				ShakeModSystem.Shake = 6;
			
				for (int j = 0; j < 35; j++)
				{
					if(Main.myPlayer == Projectile.owner)
					{
                        Vector2 speed2 = Main.rand.NextVector2CircularEdge(1f, 1f);
						Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position + new Vector2(150, 150), speed2 * 9 + Projectile.velocity,
							ModContent.ProjectileType<MoonOut>(), Projectile.damage, 0f, Owner: Projectile.owner);
                    }
					
					SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Starexplosion"));
				}
			}
			if (Projectile.timeLeft <= 180)
			{
				if (Projectile.ai[0] == 0)
				{
					Projectile.ai[0] = 1; // Set state to exploded
					Projectile.alpha = 255; // Make the Projectile invisible.

					if (Main.netMode != NetmodeID.Server && !Terraria.Graphics.Effects.Filters.Scene["Shockwave"].IsActive())
					{
						Terraria.Graphics.Effects.Filters.Scene.Activate("Shockwave", Projectile.position + new Vector2(150, 150)).GetShader().UseColor(rippleCount, rippleSize, rippleSpeed).UseTargetPosition(Projectile.position + new Vector2(150, 150));
					}
				}

				if (Main.netMode != NetmodeID.Server && Terraria.Graphics.Effects.Filters.Scene["Shockwave"].IsActive())
				{
					float progress = (180f - Projectile.timeLeft) / 60f;
					Terraria.Graphics.Effects.Filters.Scene["Shockwave"].GetShader().UseProgress(progress).UseOpacity(distortStrength * (1 - progress / 3f));
				}
			}
		}
		
		public override bool PreAI()
		{
			Projectile.tileCollide = false;
			if (++Projectile.frameCounter >= 8)
			{
				Projectile.frameCounter = 0;
				if (++Projectile.frame >= 20)
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
		

		public override void OnKill(int timeLeft)
		{
			if (Main.netMode != NetmodeID.Server && Terraria.Graphics.Effects.Filters.Scene["Shockwave"].IsActive())
			{
				Terraria.Graphics.Effects.Filters.Scene["Shockwave"].Deactivate();
			}
		}

	}

}