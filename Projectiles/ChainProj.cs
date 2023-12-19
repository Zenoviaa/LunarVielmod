using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Buffs;
using Stellamod.Helpers;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles
{
    public class ChainProj : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Halhurish");
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
		}

		public override void SetDefaults()
		{
			Projectile.friendly = false;
			Projectile.Size = new Vector2(16, 48);
			Projectile.tileCollide = false;
			Projectile.ignoreWater = true;
		}


		public bool Flip;
		public NPC Target;
		public Player PlayerToChainTo => Main.player[Projectile.owner];

		public override void AI()
		{
			if(Target == null || !Target.active || !Target.HasBuff<Chained>())
            {
				Projectile.Kill();
				return;
			}

			Projectile.timeLeft = 2;
			Projectile.rotation = Projectile.AngleFrom(PlayerToChainTo.Center);
			Projectile.Center = PlayerToChainTo.MountedCenter;
			Projectile.direction = Projectile.spriteDirection = -PlayerToChainTo.direction * (Flip ? -1 : 1);
		}

		public override bool PreDraw(ref Color lightColor)
		{
			DrawChainCurve(Main.spriteBatch, Target.Center, out Vector2[] chainPositions);
			return false;
		}

		//Control points for drawing chain bezier, update slowly when hooked in
		private void DrawChainCurve(SpriteBatch spriteBatch, Vector2 projBottom, out Vector2[] chainPositions)
		{
			if (PlayerToChainTo == null)
			{
				chainPositions = new Vector2[] { };
				return;
			}

			Texture2D chainTex = ModContent.Request<Texture2D>(Texture + "_Chain").Value;
			Curvature curve = new Curvature(new Vector2[] { PlayerToChainTo.MountedCenter, projBottom });
			int numPoints = 30;
			chainPositions = curve.GetPoints(numPoints).ToArray();
			
			//Draw each chain segment, skipping the very first one, as it draws partially behind the player
			for (int i = 1; i < numPoints; i++)
			{
				Vector2 position = chainPositions[i];

				float rotation = (chainPositions[i] - chainPositions[i - 1]).ToRotation() - MathHelper.PiOver2; //Calculate rotation based on direction from last point
				float yScale = Vector2.Distance(chainPositions[i], chainPositions[i - 1]) / chainTex.Height; //Calculate how much to squash/stretch for smooth chain based on distance between points

				Vector2 scale = new Vector2(1, yScale); // Stretch/Squash chain segment
				Color chainLightColor = Lighting.GetColor((int)position.X / 16, (int)position.Y / 16); //Lighting of the position of the chain segment
				Vector2 origin = new Vector2(chainTex.Width / 2, chainTex.Height); //Draw from center bottom of texture
				spriteBatch.Draw(chainTex, position - Main.screenPosition, null, chainLightColor, rotation, origin, scale, SpriteEffects.None, 0);
			}
		}

		public override void SendExtraAI(BinaryWriter writer)
		{
			writer.Write(Flip);
		}

		public override void ReceiveExtraAI(BinaryReader reader)
		{
			Flip = reader.ReadBoolean();
		}
	}
}
