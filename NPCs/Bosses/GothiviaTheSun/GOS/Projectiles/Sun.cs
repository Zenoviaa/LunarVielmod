using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using Stellamod.NPCs.Bosses.IrradiaNHavoc.Havoc.Projectiles;
using Stellamod.Projectiles.Summons;
using Stellamod.Projectiles.Visual;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.GothiviaTheSun.GOS.Projectiles
{
    public class Sun : ModNPC
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Morrowed Swampster");
			Main.npcFrameCount[NPC.type] = 18;
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
			NPC.width = 300;
			NPC.height = 300;
			NPC.damage = 910;
			NPC.defense = 30;
			NPC.lifeMax = 500000;
			NPC.value = 0f;
			NPC.timeLeft = 450;
			NPC.knockBackResist = .0f;
			NPC.aiStyle = -1;
			NPC.noTileCollide = true;
			NPC.noGravity = true;
			NPC.dontTakeDamage = true;
		}

		int invisibilityTimer;
		public override void HitEffect(NPC.HitInfo hit)
		{
			for (int k = 0; k < 11; k++)
			{
				Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Torch, 1, -1f, 1, default, .61f);
			}
		}

		public override Color? GetAlpha(Color lightColor)
		{
			return new Color(255, 255, 255, 0) * (1f - NPC.alpha / 80f);
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			// Since the NPC sprite naturally faces left, we want to flip it when its X velocity is positive

			SpriteEffects effects = NPC.direction == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

			Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;

			spriteBatch.Draw(texture, new Vector2(NPC.Center.X, NPC.Center.Y) - Main.screenPosition + new Vector2(0, NPC.gfxOffY), NPC.frame, new Color(255, 255, 255, 0) * (1f - NPC.alpha / 1f), NPC.rotation, NPC.frame.Size() / 2, NPC.scale * 3, effects, 0);


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
			NPC.frame.Width = 300;
			NPC.frame.X = ((int)trueFrame % 16) * NPC.frame.Width;
			NPC.frame.Y = (((int)trueFrame - ((int)trueFrame % 16)) / 16) * NPC.frame.Height;
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

		int bee = 1100;
		int bee2 = 560;
		public int rippleCount = 20;
		public int rippleSize = 5;
		public int rippleSpeed = 15;
		public float distortStrength = 300f;
		int gr = 58;
		public bool HHH = false;


    private void AI_Movement(Vector2 targetCenter, float moveSpeed, float accel = 1f)
    {
        //This code should give quite interesting movement
        //Accelerate to being on top of the player

        float distX = targetCenter.X - NPC.Center.X;
        if (NPC.Center.X < targetCenter.X && NPC.velocity.X < moveSpeed)
        {
            NPC.velocity.X += accel;
        }
        else if (NPC.Center.X > targetCenter.X && NPC.velocity.X > -moveSpeed)
        {
                NPC.velocity.X -= accel;
        }

        //Accelerate to being above the player.
        float distY = targetCenter.Y - NPC.Center.Y;
        if (NPC.Center.Y < targetCenter.Y && NPC.velocity.Y < moveSpeed)
        {
                NPC.velocity.Y += accel;
        }
        else if (NPC.Center.Y > targetCenter.Y && NPC.velocity.Y > -moveSpeed)
        {
                NPC.velocity.Y -= accel;
        }
    }

        public override bool CheckActive()
        {
			return false;
        }

        public override void AI()
	{
			var entitySource = NPC.GetSource_FromAI();
			timer++;
			NPC.TargetClosest();
			NPC.spriteDirection = NPC.direction;
			Player target = Main.player[NPC.target];
			float maxMoveSpeed = 5;
			float accel = 0.3f;
			AI_Movement(target.Center, maxMoveSpeed, accel);

			Shooting++;
			if (Shooting == 1)
            {
                



                ScreenShaderSystem shaderSystem = ModContent.GetInstance<ScreenShaderSystem>();
                shaderSystem.TintScreen(Color.DarkOrange, 0.2f, timer: 560);
                shaderSystem.DistortScreen(TextureRegistry.NormalNoise1, new Vector2(0.001f, 0.001f), blend: 0.05f, timer: 560);
                shaderSystem.VignetteScreen(-1f, timer: 560);

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
						ModContent.ProjectileType<Starbombing>(), 0, 0f, Owner: Main.myPlayer);
                }
				invisibilityTimer = 0;
			}
			NPC.rotation -= 0.02f;
			UpdateFrame(0.4f, 1, 288);
			bee2--;
			gr++;



			if (gr == 60 && !HHH)
			{

                if (StellaMultiplayer.IsHost)
                {
                    int type = ModContent.ProjectileType<SunLaserProj>();
                    int damage = 800;
                    int knockback = 1;
                    Vector2 pos = NPC.Center;
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), pos, NPC.rotation.ToRotationVector2(),
                        type, damage, knockback, Main.myPlayer, 0, ai1: NPC.whoAmI);
                }

				gr = 0;
            }

			if (bee2 <= 280)
			{
				HHH = true;
			}

			for (int i = 0; i < Main.maxPlayers; i++)
			{
				Player npc = Main.player[i];

				if (npc.active)
				{
					float distance = Vector2.Distance(NPC.Center, npc.Center);
					if (distance <= 4000)
					{
						Vector2 direction = npc.Center - NPC.Center;
						direction.Normalize();
						npc.velocity -= direction * 0.2f;
					}
				}
			}

			if (bee2 == 0)
			{
				NPC.Kill();

                ScreenShaderSystem shaderSystem = ModContent.GetInstance<ScreenShaderSystem>();
                shaderSystem.UnTintScreen();
                shaderSystem.UnDistortScreen();
                shaderSystem.UnVignetteScreen();


                for (int i = 0; i < 150; i++)
				{
					Vector2 speed = Main.rand.NextVector2CircularEdge(4f, 4f);
					var d = Dust.NewDustPerfect(NPC.Center, DustID.Torch, speed * 17, Scale: 5f);
					d.noGravity = true;

					Vector2 speeda = Main.rand.NextVector2CircularEdge(4f, 4f);
					var da = Dust.NewDustPerfect(NPC.Center, DustID.OrangeTorch, speeda * 11, Scale: 5f);
					da.noGravity = false;

					Vector2 speedab = Main.rand.NextVector2CircularEdge(5f, 5f);
					var dab = Dust.NewDustPerfect(NPC.Center, DustID.Torch, speeda * 30, Scale: 5f);
					dab.noGravity = false;
				}

				SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/STARGROP"));
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