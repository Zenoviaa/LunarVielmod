using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.IO;
using Stellamod.Dusts;
using Stellamod.Trails;
using Stellamod.Utilis;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Stellamod.Items.Accessories.Players;
using ParticleLibrary;
using Stellamod.Particles;
using Stellamod.Projectiles.IgniterExplosions.Stein;
using Stellamod.Items.Weapons.Mage.Stein;
using Terraria.DataStructures;

namespace Stellamod.Projectiles.Slashers.IshNYire
{
	public class IshThrow : ModProjectile
	{
		public static bool swung = false;
		public int SwingTime = 60;
		public float holdOffset = 0f;
		public bool bounced = false;
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Slasher");
			Main.projFrames[Projectile.type] = 1;
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10; // The length of old position to be recorded
			ProjectileID.Sets.TrailingMode[Projectile.type] = 2; // The recording mode
		}
		public override void SetDefaults()
		{
			Projectile.damage = 15;
			Projectile.penetrate = -1;
			Projectile.ignoreWater = true;
			Projectile.tileCollide = false;
			Projectile.DamageType = DamageClass.Melee;
			Projectile.height = 100;
			Projectile.width = 100;
			Projectile.friendly = true;
			Projectile.scale = 1f;
			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = 5;
		}
		int timer = 0;
		public float Timer
		{
			get => Projectile.ai[0];
			set => Projectile.ai[0] = value;
		}
		public virtual float Lerp(float val)
		{
			return val == 1f ? 1f : (val == 1f ? 1f : (float)Math.Pow(2, val * 6.5f - 5f) / 2f);
		}
	

		public override void AI()
		{
			Player player = Main.player[Projectile.owner];
			float rotation = Projectile.rotation;
			player.RotatedRelativePoint(Projectile.Center);
			Projectile.rotation -= 0.5f;

			if (Main.mouseLeft)
			{
				Projectile.velocity = Projectile.DirectionTo(Main.MouseWorld) * Projectile.Distance(Main.MouseWorld) / 12;
			}
			else
			{
				Projectile.velocity = Projectile.DirectionTo(player.Center) * 20;
				if (Projectile.Hitbox.Intersects(player.Hitbox))
				{
					Projectile.Kill();
				}
			}

			Vector3 RGB = new(2.55f, 2.55f, 0.94f);
			// The multiplication here wasn't doing anything
			Lighting.AddLight(Projectile.Center, RGB.X, RGB.Y, RGB.Z);

			player.heldProj = Projectile.whoAmI;
			player.ChangeDir(Projectile.velocity.X < 0 ? -1 : 1);
			player.itemTime = 2;
			player.itemAnimation = 2;
			player.itemRotation = rotation * player.direction;
			//Projectile.netUpdate = true;
		}
		bool Beans = false;

		

		
		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			Player player = Main.player[Projectile.owner];
			Vector2 oldMouseWorld = Main.MouseWorld;




			if (bounced)
            {
				switch (Main.rand.Next(9))
				{
					case 0:

						break;

					case 1:

						break;

					case 2:

						break;


					case 3:

						break;


					case 4:

						break;


					case 5:

						break;


					case 6:

						break;


					case 7:

						for (int i = 0; i < 12; i++)
						{
							Dust.NewDustPerfect(target.Center, ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.Next(1, 9)).RotatedByRandom(MathHelper.TwoPi), 0, Color.Pink, 1f).noGravity = true;
						}
						target.SimpleStrikeNPC(Projectile.damage * 2, 1, crit: false, Projectile.knockBack);
						break;

					case 8:
						bounced = false;
						break;



				}


			}

			if (!bounced)
			{


				//player.velocity = Projectile.DirectionTo(oldMouseWorld) * -10f;
				bounced = true;



				
				float rot = player.velocity.ToRotation();
				float spread = 0.6f;

				Vector2 offset = new Vector2(1.5f, -0.1f * player.direction).RotatedBy(rot);
				for (int k = 0; k < 7; k++)
				{
					Vector2 direction = offset.RotatedByRandom(spread);
					Dust.NewDustPerfect(Projectile.position + offset * 43, ModContent.DustType<Dusts.GlowDust>(), new Vector2(0, 0), 125, new Color(255, 255, 255), 1);
					Dust.NewDustPerfect(player.Center + offset * 43, ModContent.DustType<Dusts.TSmokeDust>(), Vector2.UnitY * -2 + offset.RotatedByRandom(spread), 150, Color.LightPink * 0.5f, Main.rand.NextFloat(0.5f, 1));

				}




				switch (Main.rand.Next(2))
				{
					case 0:
						target.SimpleStrikeNPC(Projectile.damage * 3, 1, crit: false, Projectile.knockBack);
					//	Projectile.NewProjectile(Projectile.GetSource_FromThis(), player.position.X, player.position.Y, 0, 0, ModContent.ProjectileType<Hulthit1>(), (int)(Projectile.damage * 1.5), 0f, Projectile.owner, 0f, 0f);


						for (int i = 0; i < 26; i++)
						{
							Dust.NewDustPerfect(target.Center, ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.Next(1, 9)).RotatedByRandom(MathHelper.TwoPi), 0, Color.Pink, 1f).noGravity = true;
						}
						for (int i = 0; i < 20; i++)
						{
							Dust.NewDustPerfect(target.Center, ModContent.DustType<TSmokeDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(MathHelper.TwoPi), 0, Color.DeepPink, 1f).noGravity = true;
						}



						break;

					case 1:
						target.SimpleStrikeNPC(Projectile.damage * 2, 1, crit: false, Projectile.knockBack);
					//	Projectile.NewProjectile(Projectile.GetSource_FromThis(), player.position.X, player.position.Y, 0, 0, ModContent.ProjectileType<Hulthit2>(), (int)(Projectile.damage * 1.5), 0f, Projectile.owner, 0f, 0f);
						for (int i = 0; i < 26; i++)
						{
							Dust.NewDustPerfect(target.Center, ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.Next(1, 9)).RotatedByRandom(MathHelper.TwoPi), 0, Color.White, 1f).noGravity = true;
						}
						for (int i = 0; i < 20; i++)
						{
							Dust.NewDustPerfect(target.Center, ModContent.DustType<TSmokeDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(MathHelper.TwoPi), 0, Color.IndianRed, 1f).noGravity = true;
						}
						break;

				}

				target.SimpleStrikeNPC(Projectile.damage * 2, 1, crit: false, 1);
				Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(base.Projectile.Center, 512f, 32f);


				if (target.lifeMax <= 2000)
				{
					if (target.life < target.lifeMax / 2)
					{
						target.SimpleStrikeNPC(99999, 1, crit: false, 1);
					}
				}
			}
		}

		public PrimDrawer TrailDrawer { get; private set; } = null;
		public float WidthFunction(float completionRatio)
		{
			float baseWidth = Projectile.scale * Projectile.width * 0.5f;
			return MathHelper.SmoothStep(baseWidth, 1.5f, completionRatio);
		}
		public Color ColorFunction(float completionRatio)
		{
			return Color.Lerp(Color.Turquoise, Color.Transparent, completionRatio) * 0.7f;
		}


		public TrailRenderer SwordSlash;
		public TrailRenderer SwordSlash2;
		public TrailRenderer SwordSlash3;
		public TrailRenderer SwordSlash4;
		public override bool PreDraw(ref Color lightColor)
		{
			Main.spriteBatch.End();

			var TrailTex = ModContent.Request<Texture2D>("Stellamod/Effects/Primitives/Trails/MetalTrail").Value;
			var TrailTex2 = ModContent.Request<Texture2D>("Stellamod/Effects/Primitives/Trails/WhispyTrail").Value;
			var TrailTex3 = ModContent.Request<Texture2D>("Stellamod/Effects/Primitives/Trails/WhispyTrail").Value;
			var TrailTex4 = ModContent.Request<Texture2D>("Stellamod/Effects/Primitives/Trails/DirnTrail").Value;
			Color color = Color.Multiply(new(1.50f, 1.75f, 3.5f, 0), 200);



			if (SwordSlash == null)
			{
				SwordSlash = new TrailRenderer(TrailTex, TrailRenderer.DefaultPass, (p) => new Vector2(50f), (p) => new Color(150, 110, 15, 90) * (1f - p));
				SwordSlash.drawOffset = Projectile.Size / 1.8f;
			}
			if (SwordSlash2 == null)
			{
				SwordSlash2 = new TrailRenderer(TrailTex2, TrailRenderer.DefaultPass, (p) => new Vector2(80f), (p) => new Color(200, 150, 25, 100) * (1f - p));
				SwordSlash2.drawOffset = Projectile.Size / 1.9f;

			}
			if (SwordSlash3 == null)
			{
				SwordSlash3 = new TrailRenderer(TrailTex3, TrailRenderer.DefaultPass, (p) => new Vector2(100f), (p) => new Color(250, 25, 25, 90) * (1f - p));
				SwordSlash3.drawOffset = Projectile.Size / 2f;

			}

			if (SwordSlash4 == null)
			{
				SwordSlash4 = new TrailRenderer(TrailTex3, TrailRenderer.DefaultPass, (p) => new Vector2(60f), (p) => new Color(255, 2, 255, 90) * (1f - p));
				SwordSlash4.drawOffset = Projectile.Size / 2.2f;

			}
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);
			
			float[] rotation = new float[Projectile.oldRot.Length];
			for (int i = 0; i < rotation.Length; i++)
			{
				rotation[i] = Projectile.oldRot[i] - MathHelper.ToRadians(45);
			}


			SwordSlash.Draw(Projectile.oldPos, rotation);
			SwordSlash2.Draw(Projectile.oldPos, rotation);
			SwordSlash3.Draw(Projectile.oldPos, rotation);
			SwordSlash4.Draw(Projectile.oldPos, rotation);



			Texture2D texture = (Texture2D)ModContent.Request<Texture2D>(Texture);

			int frameHeight = texture.Height / Main.projFrames[Projectile.type];
			int startY = frameHeight * Projectile.frame;

			Rectangle sourceRectangle = new Rectangle(0, startY, texture.Width, frameHeight);
			Vector2 origin = sourceRectangle.Size() / 2f;
			Color drawColor = Projectile.GetAlpha(lightColor);


			Main.EntitySpriteDraw(texture,
			   Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY),
			   sourceRectangle, drawColor, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0); // drawing the sword itself
			Main.instance.LoadProjectile(Projectile.type);
			Texture2D texture2 = TextureAssets.Projectile[Projectile.type].Value;

			// Redraw the projectile with the color not influenced by light
			Vector2 drawOrigin = new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f);

			Main.spriteBatch.End();

			Main.spriteBatch.Begin();


			return false;

		}

		public override void PostDraw(Color lightColor)
		{
			Player player = Main.player[Projectile.owner];
			Texture2D texture = (Texture2D)ModContent.Request<Texture2D>(Texture);

			int frameHeight = texture.Height / Main.projFrames[Projectile.type];
			int startY = frameHeight * Projectile.frame;

			float mult = Lerp(Utils.GetLerpValue(0f, SwingTime, Projectile.timeLeft));
			float alpha = (float)Math.Sin(mult * Math.PI);
			Vector2 pos = player.Center + Projectile.velocity * (mult);

			Rectangle sourceRectangle = new Rectangle(0, startY, texture.Width, frameHeight);
			Vector2 origin = sourceRectangle.Size() / 2f;
			Color drawColor = Projectile.GetAlpha(lightColor);

			Main.EntitySpriteDraw(texture,
				Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY),
				sourceRectangle, drawColor, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);

			float rotation = Projectile.rotation;


			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
			Main.instance.LoadProjectile(Projectile.type);


			// Redraw the projectile with the color not influenced by light
			Vector2 Dorigin = sourceRectangle.Size() / 2f;
			Vector2 drawOrigin = new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f);
			for (int k = 0; k < Projectile.oldPos.Length; k++)
			{
				Vector2 drawPos = (Projectile.oldPos[k] - Main.screenPosition) + Dorigin + new Vector2(0f, Projectile.gfxOffY);
				Color color = Projectile.GetAlpha(Color.Lerp(new Color(93, 203, 243), new Color(59, 72, 168), 1f / Projectile.oldPos.Length * k) * (1f - 1f / Projectile.oldPos.Length * k / 0.2f));
				Main.EntitySpriteDraw(texture, drawPos, null, color, rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
			}
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

			return;
		}
	}
}