using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.NPCs.Harvesting.Morrow;
using Stellamod.Projectiles.IgniterExplosions;
using Stellamod.UI.Systems;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using ReLogic.Content;
using Stellamod.Items.Harvesting;
using Terraria.GameContent.ItemDropRules;
using Terraria.ModLoader.Utilities;
using static Terraria.ModLoader.ModContent;
using Stellamod.Buffs;

namespace Stellamod.Projectiles.Crossbows.Eckasect
{
	public class GenesisBolt2 : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Flask of KABOOM");
		}
		int timerz = 0;
		int tima = 0;
		public override void SetDefaults()
		{
			Projectile.width = 40;
			Projectile.height = 40;

			Projectile.aiStyle = 2;

			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.tileCollide = true;
		}
		public override void AI()
		{
			Vector3 RGB = new(1.00f, 0.37f, 0.30f);
			// The multiplication here wasn't doing anything
			Lighting.AddLight(Projectile.position, RGB.X, RGB.Y, RGB.Z);

			int dust = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.BlueFairy, 0f, 0f);
			Main.dust[dust].scale = 0.6f;
			tima++;

			if (tima < 10)
			{
				Projectile.tileCollide = false;
			}

			if (tima > 10)
			{
				Projectile.tileCollide = true;
			}
			timerz++;
			if (timerz >= 40)
			{
				if (Main.rand.NextBool(9))
				{
					var entitySource = Projectile.GetSource_FromThis();
					NPC.NewNPC(entitySource, (int)Projectile.Center.X / 2, (int)Projectile.Center.Y / 2, ModContent.NPCType<BlueLightBig>(), ai1: Projectile.whoAmI);
				}
			}

		}


		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			ShakeModSystem.Shake = 4;
			float speedXa = -Projectile.velocity.X * Main.rand.NextFloat(.4f, .7f) + Main.rand.NextFloat(-8f, 8f);
			float speedYa = -Projectile.velocity.Y * Main.rand.Next(0, 0) * 0.01f + Main.rand.Next(-20, 21) * 0.0f;
			Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X + speedXa, Projectile.position.Y + speedYa, speedXa * 0, speedYa * 0, ModContent.ProjectileType<GenesisBoom1>(), (int)(Projectile.damage * 2), 0f, Projectile.owner, 0f, 0f);
			SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Starexplosion"), Projectile.position);
			
			NPC npc = target;
			if (npc.active && !npc.HasBuff<Sected>())
			{
				target.AddBuff(ModContent.BuffType<Sected>(), 700);


				switch (Main.rand.Next(3))
				{
					case 0:
						target.AddBuff(ModContent.BuffType<Genesis>(), 640);

						Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X + speedXa, Projectile.position.Y + speedYa, speedXa * 0, speedYa * 0, ModContent.ProjectileType<GenesisDebuff>(), (int)(Projectile.damage * 0), 0f, Projectile.owner, 0f, 0f);


						break;


					case 1:


						Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X + speedXa, Projectile.position.Y + speedYa, speedXa * 0, speedYa * 0, ModContent.ProjectileType<ExecutorDebuff>(), (int)(Projectile.damage * 0), 0f, Projectile.owner, 0f, 0f);
						target.AddBuff(ModContent.BuffType<Executor>(), 640);
						break;


					case 2:

						Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X + speedXa, Projectile.position.Y + speedYa, speedXa * 0, speedYa * 0, ModContent.ProjectileType<LiberatorDebuff>(), (int)(Projectile.damage * 0), 0f, Projectile.owner, 0f, 0f);
						target.AddBuff(ModContent.BuffType<Liberator>(), 640);
						break;
				}



			}


			if (npc.active && npc.HasBuff<Genesis>())
			{
				npc.SimpleStrikeNPC(Projectile.damage * 4, 1, crit: false, Projectile.knockBack);

			}

		}

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			ShakeModSystem.Shake = 4;
			float speedX = Projectile.velocity.X * Main.rand.NextFloat(.3f, .3f) + Main.rand.NextFloat(4f, 4f);
			float speedY = Projectile.velocity.Y * Main.rand.Next(-1, -1) * 0.0f + Main.rand.Next(-4, -4) * 0f;
			SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Starexplosion"), Projectile.position);
			Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X + speedX, Projectile.position.Y + speedY, speedX * 0, speedY * 0, ModContent.ProjectileType<GenesisBoom1>(), (int)(Projectile.damage * 2), 0f, Projectile.owner, 0f, 0f);
			Projectile.Kill();
			return false;
		}
		Rectangle frame = new Rectangle(0, 0, 40, 40);
		Vector2 Drawoffset => new Vector2(0, Projectile.gfxOffY) + Vector2.UnitX * Projectile.spriteDirection * 0;
		public virtual string GlowTexturePath => Texture + "_Glow";
		private Asset<Texture2D> _glowTexture;
		public Texture2D GlowTexture => (_glowTexture ??= (RequestIfExists<Texture2D>(GlowTexturePath, out var asset) ? asset : null))?.Value;
		public override void PostDraw(Color lightColor)
		{
			float num108 = 4;
			float num107 = (float)Math.Cos((double)(Main.GlobalTimeWrappedHourly % 1.4f / 1.4f * 6.28318548f)) / 2f + 0.5f;
			float num106 = 0f;
			Color color1 = Color.AliceBlue * num107 * .8f;
			var effects = Projectile.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
			Main.spriteBatch.Draw(
				GlowTexture,
				Projectile.Center - Main.screenPosition + Drawoffset,
				frame,
				color1,
				Projectile.rotation,
				new Vector2(40, 40) / 2,
				Projectile.scale,
				effects,
				0
			);
			SpriteEffects spriteEffects3 = Projectile.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
			Vector2 vector33 = new Vector2(Projectile.Center.X, Projectile.Center.Y) - Main.screenPosition + Drawoffset - Projectile.velocity;
			Color color29 = new Color(127 - Projectile.alpha, 127 - Projectile.alpha, 127 - Projectile.alpha, 0).MultiplyRGBA(Color.AliceBlue);
			for (int num103 = 0; num103 < 4; num103++)
			{
				Color color28 = color29;
				color28 = Projectile.GetAlpha(color28);
				color28 *= 1f - num107;
				Vector2 vector29 = Projectile.Center + (num103 / (float)num108 * 6.28318548f + Projectile.rotation + num106).ToRotationVector2() * (4f * num107 + 2f) - Main.screenPosition + Drawoffset - Projectile.velocity * num103;
				Main.spriteBatch.Draw(GlowTexture, vector29, frame, color28, Projectile.rotation, new Vector2(40, 40) / 2f, Projectile.scale, spriteEffects3, 0f);
			}
		}

		public override bool PreDraw(ref Color lightColor)
		{
			Lighting.AddLight(Projectile.Center, Color.AliceBlue.ToVector3() * 2.25f * Main.essScale);
			return true;
		}


	}
}