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
using Stellamod.NPCs.Bosses.DreadMire;
using Stellamod.NPCs.Bosses.STARBOMBER.Projectiles;

namespace Stellamod.Projectiles.Steins
{
	public class VileFist : ModProjectile
	{
		public static bool swung = false;
		public int SwingTime = 60;
		public float holdOffset = 0f;
		public bool bounced = false;
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Slasher");
			Main.projFrames[Projectile.type] = 1;
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 20; // The length of old position to be recorded
			ProjectileID.Sets.TrailingMode[Projectile.type] = 2; // The recording mode
		}
		public override void SetDefaults()
		{
			Projectile.damage = 10;
			Projectile.timeLeft = SwingTime;
			Projectile.penetrate = -1;
			Projectile.ignoreWater = true;
			Projectile.tileCollide = false;
			Projectile.DamageType = DamageClass.Melee;
			Projectile.height = 100;
			Projectile.width = 100;
			Projectile.friendly = true;
			Projectile.scale = 1f;
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
			Vector3 RGB = new Vector3(1.45f, 2.55f, 0.94f);
			float multiplier = 1;
			float max = 2.25f;
			float min = 1.0f;
			RGB *= multiplier;
			if (RGB.X > max)
			{
				multiplier = 0.5f;
			}
			if (RGB.X < min)
			{
				multiplier = 1.5f;
			}
			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = 10000;
			AttachToPlayer();
		}
		bool Beans = false;

		public void AttachToPlayer()
		{
			Player player = Main.player[Projectile.owner];
			if (!player.active || player.dead || player.CCed || player.noItems)
				return;
			Vector2 teleportPosition = Main.MouseWorld;
			timer++;
			if (timer == 5 && Main.myPlayer == Projectile.owner)
			{
				if (Collision.CanHitLine(player.Center, 1, 1, teleportPosition, 1, 1))
				{
					player.Teleport(teleportPosition, 6);
					NetMessage.SendData(MessageID.TeleportEntity, -1, -1, null, 0, player.whoAmI, teleportPosition.X, teleportPosition.Y, 1);
					float speed = 5;
					Projectile.velocity = Projectile.DirectionTo(Main.MouseWorld) * speed;
					Projectile.netUpdate = true;

					player.immune = true;
					player.immuneTime = 3;
					Projectile.Center = player.Center;
				}
			}

			Projectile.velocity *= 0.97f;



			Vector2 oldMouseWorld = Main.MouseWorld;


			if (timer > 8)
			{
				Beans = true;

				if (timer < 10 && Main.myPlayer == Projectile.owner)
				{

					player.velocity = Projectile.DirectionTo(oldMouseWorld) * 13f;
				}


			}


			if (timer > 25)
			{
				if (!bounced)
				{
					player.itemTime = 240;
					player.itemAnimation = 240;
				}
				if (bounced)
				{
					player.itemTime = 20;
					player.itemAnimation = 20;
				}
				Projectile.Kill();
			}



			//Projectile.netUpdate = true;
		}

		public override bool? CanDamage()
		{

			if (Beans)
			{
				return false;
			}

			return base.CanDamage();
		}
		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			Player player = Main.player[Projectile.owner];
			Vector2 oldMouseWorld = Main.MouseWorld;
			player.GetModPlayer<SteinPlayer>().HasHitDance = true;
			if (!bounced)
			{
				player.velocity = Projectile.DirectionTo(oldMouseWorld) * -17f;
				bounced = true;




				



				switch (Main.rand.Next(3))
				{
					case 0:
						SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Dreadmire_BoneSpawn1") { Pitch = Main.rand.NextFloat(-0.5f, 0.5f) });
						SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Dreadmire__Skulls"), Projectile.Center);
						break;
					case 1:
						SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Dreadmire_Fire1") { Pitch = Main.rand.NextFloat(-0.5f, 0.5f) });
						SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Dreadmire__Skulls"), Projectile.Center);
						break;
					case 2:
						SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Dreadmire__LightingRain2") { Pitch = Main.rand.NextFloat(-0.5f, 0.5f) });
						SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Dreadmire__Skulls"), Projectile.Center);
						break;

				}

				//Wow, Amazing, So Hot, SEXY, Great
				
				float rot = player.velocity.ToRotation();
				float spread = 0.6f;

				Vector2 offset = new Vector2(1.5f, -0.1f * player.direction).RotatedBy(rot);
				for (int k = 0; k < 7; k++)
				{
					Vector2 direction = offset.RotatedByRandom(spread);
					Dust.NewDustPerfect(Projectile.position + offset * 43, ModContent.DustType<Dusts.GlowDust>(), new Vector2(0, 0), 125, new Color(250, 10, 40), 1);
					Dust.NewDustPerfect(player.Center + offset * 43, ModContent.DustType<Dusts.TSmokeDust>(), Vector2.UnitY * -2 + offset.RotatedByRandom(spread), 150, Color.Red * 0.5f, Main.rand.NextFloat(0.5f, 1));

				}




				switch (Main.rand.Next(3))
				{
					case 0:
						target.SimpleStrikeNPC(Projectile.damage * 7, 1, crit: false, Projectile.knockBack);
						
                        if (StellaMultiplayer.IsHost)
                        {
                            var entitySource = Projectile.GetSource_FromThis();
                            NPC.NewNPC(entitySource, (int)player.Center.X, (int)player.Center.Y, ModContent.NPCType<DreadMirePentagramV2>());
                        }


                        float num = 64;
                        for (float i = 0; i < num; i++)
                        {
                            float progress = i / num;
                            float rot2 = MathHelper.TwoPi * progress;
                            Vector2 direction2 = Vector2.UnitY.RotatedBy(rot2);
                            Vector2 velocity = direction2 * 10;
                            int projectile = Projectile.NewProjectile(Projectile.GetSource_FromThis(), player.Center.X, player.Center.Y, velocity.X, velocity.Y, ModContent.ProjectileType<DreadSineSkull>(), Projectile.damage / 2, 0f, Owner: Main.myPlayer);
                            Main.projectile[projectile].timeLeft = 300;
                            Projectile ichor = Main.projectile[projectile];
                            ichor.hostile = false;
                            ichor.friendly = true;
                        }




                        for (int i = 0; i < 26; i++)
						{
							Dust.NewDustPerfect(target.Center, ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.Next(1, 9)).RotatedByRandom(MathHelper.TwoPi), 0, Color.Red, 1f).noGravity = true;
						}
						for (int i = 0; i < 20; i++)
						{
							Dust.NewDustPerfect(target.Center, ModContent.DustType<TSmokeDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(MathHelper.TwoPi), 0, Color.DarkRed, 1f).noGravity = true;
						}



						break;



                    case 1:
                        target.SimpleStrikeNPC(Projectile.damage * 3, 1, crit: false, Projectile.knockBack);

                        if (StellaMultiplayer.IsHost)
                        {
                            var entitySource = Projectile.GetSource_FromThis();
                            NPC.NewNPC(entitySource, (int)player.Center.X, (int)player.Center.Y, ModContent.NPCType<DreadMirePentagramV2>());
                        }


                        float num2 = 16;
                        for (float i = 0; i < num2; i++)
                        {
                            float progress = i / num2;
                            float rot2 = MathHelper.TwoPi * progress;
                            Vector2 direction2 = Vector2.UnitY.RotatedBy(rot2);
                            Vector2 velocity = direction2 * 10;
                            int projectile = Projectile.NewProjectile(Projectile.GetSource_FromThis(), player.Center.X, player.Center.Y, velocity.X, velocity.Y, ModContent.ProjectileType<DreadSineSkull>(), Projectile.damage / 2, 0f, Owner: Main.myPlayer);
                            Main.projectile[projectile].timeLeft = 300;
                            Projectile ichor = Main.projectile[projectile];
                            ichor.hostile = false;
                            ichor.friendly = true;
                        }




                        for (int i = 0; i < 26; i++)
                        {
                            Dust.NewDustPerfect(target.Center, ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.Next(1, 9)).RotatedByRandom(MathHelper.TwoPi), 0, Color.Red, 1f).noGravity = true;
                        }
                        for (int i = 0; i < 20; i++)
                        {
                            Dust.NewDustPerfect(target.Center, ModContent.DustType<TSmokeDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(MathHelper.TwoPi), 0, Color.DarkRed, 1f).noGravity = true;
                        }



                        break;




                    case 2:
                        target.SimpleStrikeNPC(Projectile.damage * 1, 1, crit: false, Projectile.knockBack);

                        if (StellaMultiplayer.IsHost)
                        {
                            var entitySource = Projectile.GetSource_FromThis();
                            NPC.NewNPC(entitySource, (int)player.Center.X, (int)player.Center.Y, ModContent.NPCType<DreadMirePentagramV2>());
                        }


                        float num3 = 4;
                        for (float i = 0; i < num3; i++)
                        {
                            float progress = i / num3;
                            float rot2 = MathHelper.TwoPi * progress;
                            Vector2 direction2 = Vector2.UnitY.RotatedBy(rot2);
                            Vector2 velocity = direction2 * 10;
                            int projectile = Projectile.NewProjectile(Projectile.GetSource_FromThis(), player.Center.X, player.Center.Y, velocity.X, velocity.Y, ModContent.ProjectileType<DreadSineSkull>(), Projectile.damage / 2, 0f, Owner: Main.myPlayer);
                            Main.projectile[projectile].timeLeft = 300;
                            Projectile ichor = Main.projectile[projectile];
                            ichor.hostile = false;
                            ichor.friendly = true;
                        }




                        for (int i = 0; i < 26; i++)
                        {
                            Dust.NewDustPerfect(target.Center, ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.Next(1, 9)).RotatedByRandom(MathHelper.TwoPi), 0, Color.Red, 1f).noGravity = true;
                        }
                        for (int i = 0; i < 20; i++)
                        {
                            Dust.NewDustPerfect(target.Center, ModContent.DustType<TSmokeDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(MathHelper.TwoPi), 0, Color.DarkRed, 1f).noGravity = true;
                        }



                        break;


                }

				target.SimpleStrikeNPC(Projectile.damage * 5, 1, crit: false, 1);
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

			var TrailTex = ModContent.Request<Texture2D>("Stellamod/Effects/Primitives/Trails/DirmTrail").Value;
			var TrailTex2 = ModContent.Request<Texture2D>("Stellamod/Effects/Primitives/Trails/WaterTrail").Value;
			var TrailTex3 = ModContent.Request<Texture2D>("Stellamod/Effects/Primitives/Trails/CrystalTrail").Value;
			var TrailTex4 = ModContent.Request<Texture2D>("Stellamod/Effects/Primitives/Trails/WhiteTrail").Value;
			Color color = Color.Multiply(new(1.50f, 1.75f, 3.5f, 0), 200);



			if (SwordSlash == null)
			{
				SwordSlash = new TrailRenderer(TrailTex, TrailRenderer.DefaultPass, (p) => new Vector2(25f), (p) => new Color(250, 10, 0, 90) * (1f - p));
				SwordSlash.drawOffset = Projectile.Size / 1.8f;
			}
			if (SwordSlash2 == null)
			{
				SwordSlash2 = new TrailRenderer(TrailTex2, TrailRenderer.DefaultPass, (p) => new Vector2(40f), (p) => new Color(250, 25, 10, 100) * (1f - p));
				SwordSlash2.drawOffset = Projectile.Size / 1.9f;

			}
			if (SwordSlash3 == null)
			{
				SwordSlash3 = new TrailRenderer(TrailTex3, TrailRenderer.DefaultPass, (p) => new Vector2(50f), (p) => new Color(255, 20, 25, 90) * (1f - p));
				SwordSlash3.drawOffset = Projectile.Size / 2f;

			}

			if (SwordSlash4 == null)
			{
				SwordSlash4 = new TrailRenderer(TrailTex3, TrailRenderer.DefaultPass, (p) => new Vector2(40f), (p) => new Color(255, 255, 255, 90) * (1f - p));
				SwordSlash4.drawOffset = Projectile.Size / 2.2f;

			}
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);


			SwordSlash.Draw(Projectile.oldPos);
			SwordSlash2.Draw(Projectile.oldPos);
			SwordSlash3.Draw(Projectile.oldPos);
			SwordSlash4.Draw(Projectile.oldPos);



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