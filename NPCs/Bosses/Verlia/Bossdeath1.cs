using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Projectiles.Summons;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.Verlia
{
	public class Bossdeath1 : ModNPC
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Morrowed Swampster");
			Main.npcFrameCount[NPC.type] = 8;
		}

		public enum ActionState
		{

			Speed,
			Wait
		}
		// Current state
		public int frameTick;
		// Current state's timer
		public float timer;
		public int PrevAtack;
		float DaedusDrug = 4;
		// AI counter
		public int counter;

		public ActionState State = ActionState.Wait;
		public override void SetDefaults()
		{
			NPC.width = 400;
			NPC.height = 711;
			NPC.damage = 1;
			NPC.defense = 30;
			NPC.lifeMax = 500000;
			NPC.value = 0f;
			NPC.timeLeft = 160;
			NPC.knockBackResist = .0f;
			NPC.noTileCollide = true;
			NPC.noGravity = true;
			NPC.dontTakeDamage = true;
		}

		int invisibilityTimer;
		

		public override Color? GetAlpha(Color lightColor)
		{
			return new Color(255, 255, 255, 0) * (1f - NPC.alpha / 80f);
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			// Since the NPC sprite naturally faces left, we want to flip it when its X velocity is positive

			SpriteEffects effects = NPC.direction == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

			Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;

			spriteBatch.Draw(texture, new Vector2(NPC.Center.X, NPC.Center.Y) - Main.screenPosition + new Vector2(0, NPC.gfxOffY), NPC.frame, new Color(255, 255, 255, 0) * (1f - NPC.alpha / 80f), NPC.rotation, NPC.frame.Size() / 2, NPC.scale * 2, effects, 0);


			// Draw the periodic glow effect behind the item when dropped in the world (hence PreDrawInWorld)


			// Using a rectangle to crop a texture can be imagined like this:
			// Every rectangle has an X, a Y, a Width, and a Height
			// Our X and Y values are the position on our texture where we start to sample from, using the top left corner as our origin
			// Our Width and Height values specify how big of an area we want to sample starting from X and Y
			return false;
		}

		float trueFrame = 0;
		public override void FindFrame(int frameHeight)
		{
			NPC.frame.Width = 400;
			NPC.frame.X = ((int)trueFrame % 10) * NPC.frame.Width;
			NPC.frame.Y = (((int)trueFrame - ((int)trueFrame % 10)) / 10) * NPC.frame.Height;
		}

		public void UpdateFrame(float speed, int minFrame, int maxFrame)
		{
			trueFrame += speed;
			if (trueFrame < minFrame)
			{
				trueFrame = minFrame;
			}
			if (trueFrame > maxFrame)
			{
				trueFrame = minFrame;
			}
		}
		public float Shooting = 0f;

		int bee = 220;
		int bee2 = 300;
		public int rippleCount = 20;
		public int rippleSize = 5;
		public int rippleSpeed = 15;
		public float distortStrength = 300f;
		public override void AI()
		{
			var entitySource = NPC.GetSource_FromAI();
			timer++;
			NPC.spriteDirection = NPC.direction;

			Shooting++;
			if (Shooting == 1)
			{
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/BiggerCharge"));
                //NPC.NewNPC(entitySource, (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<ALCADSWIRL>());
            }
			NPC.noTileCollide = true;
			invisibilityTimer++;
			if (invisibilityTimer == 5)
			{
				if (StellaMultiplayer.IsHost)
				{
					float speedYa = NPC.velocity.Y * Main.rand.Next(0, 0) * 0.0f + Main.rand.Next(-20, 20);
					Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, speedYa * 0, speedYa * 0,
						ModContent.ProjectileType<Starbombing>(), 0, 0f, 0, 0f, 0f);
				}
				invisibilityTimer = 0;
			}
			UpdateFrame(0.3f, 1, 80);
			bee2--;

			

			if (bee2 == 0)
			{
				NPC.Kill();
				for (int i = 0; i < 150; i++)
				{

					
						
					
					Vector2 speed = Main.rand.NextVector2CircularEdge(4f, 4f);
					var d = Dust.NewDustPerfect(NPC.Center, ModContent.DustType<GlowDust>(), speed * 5, 0, Color.DeepSkyBlue, Scale: 3f);
					d.noGravity = true;

					Vector2 speeda = Main.rand.NextVector2CircularEdge(4f, 4f);
					var da = Dust.NewDustPerfect(NPC.Center, ModContent.DustType<GunFlash>(), speed * 10, 0, Color.DeepSkyBlue, Scale: 3f);
					da.noGravity = false;

					Vector2 speedab = Main.rand.NextVector2CircularEdge(5f, 5f);
					var dab = Dust.NewDustPerfect(NPC.Center, ModContent.DustType<GlowDust>(), speed * 20, 0, Color.White, Scale: 3f);
					dab.noGravity = false;
				}
                ActiveSound sound = SoundEngine.FindActiveSound(new SoundStyle("Stellamod/Assets/Sounds/BiggerCharge"));
                if (sound != null)
                {
                    sound.Stop();
                }

                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/NStarblast"));
				if (Main.netMode != NetmodeID.Server && Terraria.Graphics.Effects.Filters.Scene["Shockwave"].IsActive())
				{
					Terraria.Graphics.Effects.Filters.Scene["Shockwave"].Deactivate();
				}
			}


			switch (State)
			{

				case ActionState.Wait:
					counter++;
					Wait();

					if (Main.netMode != NetmodeID.Server && !Terraria.Graphics.Effects.Filters.Scene["Shockwave"].IsActive())
					{
						Terraria.Graphics.Effects.Filters.Scene.Activate("Shockwave", NPC.Center).GetShader().UseColor(rippleCount, rippleSize, rippleSpeed).UseTargetPosition(NPC.Center);

					}

					if (Main.netMode != NetmodeID.Server && Terraria.Graphics.Effects.Filters.Scene["Shockwave"].IsActive())
					{
						float progress = (180f - bee) / 60f; // Will range from -3 to 3, 0 being the point where the bomb explodes.
						Terraria.Graphics.Effects.Filters.Scene["Shockwave"].GetShader().UseProgress(progress).UseOpacity(distortStrength * (1 - progress / 3f));
					}


					break;

				case ActionState.Speed:
					counter++;
					Speed();

					if (Main.netMode != NetmodeID.Server && !Terraria.Graphics.Effects.Filters.Scene["Shockwave"].IsActive())
					{
						Terraria.Graphics.Effects.Filters.Scene.Activate("Shockwave", NPC.Center).GetShader().UseColor(rippleCount, rippleSize, rippleSpeed).UseTargetPosition(NPC.Center);

					}

					if (Main.netMode != NetmodeID.Server && Terraria.Graphics.Effects.Filters.Scene["Shockwave"].IsActive())
					{
						float progress = (180f - bee) / 60f; // Will range from -3 to 3, 0 being the point where the bomb explodes.
						Terraria.Graphics.Effects.Filters.Scene["Shockwave"].GetShader().UseProgress(progress).UseOpacity(distortStrength * (1 - progress / 3f));
					}
					break;


				default:
					counter++;
					break;
			}
		}


		public void Wait()
		{
			timer++;
			if (timer > 50)
			{




			}
			else if (timer == 60)
			{
				State = ActionState.Speed;
				timer = 0;


				if (Main.netMode != NetmodeID.Server && Terraria.Graphics.Effects.Filters.Scene["Shockwave"].IsActive())
				{
					Terraria.Graphics.Effects.Filters.Scene["Shockwave"].Deactivate();
				}
			}
		}


		public void Speed()
		{
			timer++;
			if (timer == 60)
			{
				State = ActionState.Wait;
				timer = 0;


				if (Main.netMode != NetmodeID.Server && Terraria.Graphics.Effects.Filters.Scene["Shockwave"].IsActive())
				{
					Terraria.Graphics.Effects.Filters.Scene["Shockwave"].Deactivate();
				}
			}

		}
	}
}