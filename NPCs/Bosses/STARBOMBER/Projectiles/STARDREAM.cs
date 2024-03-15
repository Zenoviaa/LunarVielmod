using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using ParticleLibrary;
using Stellamod.Dusts;
using Stellamod.Particles;
using Terraria.DataStructures;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Trails;
using Stellamod.Utilis;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;


namespace Stellamod.NPCs.Bosses.STARBOMBER.Projectiles
{
	public class STARDREAM : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Cactius2");
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 35;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
		}
		public override void SetDefaults()
		{
			Projectile.damage = 12;
			Projectile.width = 100;
			Projectile.height = 100;
			Projectile.light = 1.5f;
			Projectile.friendly = false;
			Projectile.hostile = true;
			Projectile.ignoreWater = true;
			Projectile.tileCollide = false;
			Projectile.timeLeft = 130;
			Projectile.tileCollide = false;
			Projectile.penetrate = -1;
		}
		public float Timer
		{
			get => Projectile.ai[0];
			set => Projectile.ai[0] = value;
		}
		public float Timer2;

		public override void AI()
		{
			Timer2++;
			Timer++;
			Projectile.velocity *= 0.98f;
			if (Timer == 2)
			{
				for (int i = 0; i < 20; i++) 
				{ 	
					Vector2 speed2 = Main.rand.NextVector2CircularEdge(2f, 2f);
					var d = Dust.NewDustPerfect(Projectile.Center, DustID.BoneTorch, speed2 * 2, Scale: 2f);
					d.noGravity = true;
				}
				Timer = 0;

			}
		}


		public override void OnKill(int timeLeft)
		{
			SoundEngine.PlaySound(SoundID.DD2_EtherianPortalOpen, Projectile.position);
			Projectile.ownerHitCheck = true;

			int radius = 250;

			// Damage enemies within the splash radius
			for (int i = 0; i < Main.npc.Length; i++)
			{
				NPC target = Main.npc[i];
				if (target.active && !target.friendly && Vector2.Distance(Projectile.Center, target.Center) < radius)
				{
					int damage = Projectile.damage * 2;
					target.SimpleStrikeNPC(damage: 12, 0);
				}
			}

			for (int i = 0; i < 150; i++)
			{
				Vector2 speed = Main.rand.NextVector2CircularEdge(4f, 4f);
				var d = Dust.NewDustPerfect(Projectile.Center, DustID.BoneTorch, speed * 3, Scale: 3f);
				;
				d.noGravity = true;
			}
		}

		public PrimDrawer TrailDrawer { get; private set; } = null;
		public float WidthFunction(float completionRatio)
		{
			float baseWidth = Projectile.scale * Projectile.width * 1.0f;
			return MathHelper.SmoothStep(baseWidth, 3.5f, completionRatio);
		}
		public Color ColorFunction(float completionRatio)
		{
			return Color.Lerp(Color.BlueViolet, Color.Transparent, completionRatio) * 0.7f;
		}

		public override bool PreDraw(ref Color lightColor)
		{
			if (Main.rand.NextBool(5))
			{
				int dustnumber = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.BoneTorch, 0f, 0f, 150, Color.White, 1f);
				Main.dust[dustnumber].velocity *= 0.3f;
				Main.dust[dustnumber].noGravity = true;
			}

			Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
			Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, new Vector2(texture.Width / 2, texture.Height / 2), 1f, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
			TrailDrawer ??= new PrimDrawer(WidthFunction, ColorFunction, GameShaders.Misc["VampKnives:BasicTrail"]);
			GameShaders.Misc["VampKnives:BasicTrail"].SetShaderTexture(TrailRegistry.SmallWhispyTrail);
			TrailDrawer.DrawPrims(Projectile.oldPos, Projectile.Size * 0.5f - Main.screenPosition, 155);
			return false;
		}

	}
}
