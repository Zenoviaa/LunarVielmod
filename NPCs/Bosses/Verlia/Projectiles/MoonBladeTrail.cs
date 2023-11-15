using Microsoft.Xna.Framework;
using ParticleLibrary;
using Stellamod.Particles;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.Verlia.Projectiles
{
    public class MoonBladeTrail : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Moon Blade Trail");
			Main.projFrames[Projectile.type] = 1;
		}
		
		public override void SetDefaults()
		{
			Projectile.friendly = false;
			Projectile.width = 45;
			Projectile.height = 45;
			Projectile.penetrate = -1;
			Projectile.timeLeft = 40;
			Projectile.scale = 0.8f;
			
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
			Timer++;
			if (Timer == 3)
            {
				Vector2 speed2 = Main.rand.NextVector2CircularEdge(0.1f, 0.1f);
				ParticleManager.NewParticle(Projectile.Center, speed2 * 0, ParticleManager.NewInstance<MoonTrailParticle>(), Color.RoyalBlue, Main.rand.NextFloat(0.2f, 0.8f));
				Timer = 0;
			}
		
		}
		
		
		public override Color? GetAlpha(Color lightColor)
		{
			return new Color(200, 200, 200, 50) * (1f - Projectile.alpha / 50f);
		}

		
		public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
		{
			behindNPCs.Add(index);
		}
	}

}