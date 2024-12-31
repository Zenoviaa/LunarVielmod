using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Stellamod.Projectiles.Pulser
{

    public class PulseShoter : ModProjectile
	{
		private static Asset<Texture2D> PulseShot;

		

		public override void SetDefaults()
		{
			Projectile.width = 28;
			Projectile.height = 16;

			Projectile.aiStyle = ProjAIStyleID.Arrow;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.ignoreWater = true;
			Projectile.light = 0.6f;
			Projectile.tileCollide = true;
			Projectile.timeLeft = 400;
			Projectile.penetrate = 2;
			AIType = ProjectileID.PulseBolt;

		}
		public override void AI()
		{
			Projectile.aiStyle = ProjAIStyleID.Arrow;
			Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.UnusedWhiteBluePurple, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f, Scale: 0.6f);
			Lighting.AddLight(Projectile.position, 0.2f, 0.2f, 0.2f);
			Lighting.Brightness(1, 1);

		}

		public override void Load()
		{ // This is called once on mod (re)load when this piece of content is being loaded.
		  // This is the path to the texture that we'll use for the hook's chain. Make sure to update it.
			PulseShot = Request<Texture2D>("Stellamod/Assets/Effects/PulseShot");
		}

		public override void Unload()
		{ // This is called once on mod reload when this piece of content is being unloaded.
		  // It's currently pretty important to unload your static fields like this, to avoid having parts of your mod remain in memory when it's been unloaded.
			PulseShot = null;
		}
		public override bool PreDraw(ref Color lightColor)
		{
			Color drawColor = Lighting.GetColor((int)Projectile.Center.X / 16, (int)(Projectile.Center.Y / 16));

			Main.EntitySpriteDraw(PulseShot.Value, Projectile.Center - Main.screenPosition,
						  PulseShot.Value.Bounds, Color.DeepSkyBlue, Projectile.rotation,
						  PulseShot.Size() * 0.5f, 1f, SpriteEffects.None, 0);
			return true;
		}
	}
}
