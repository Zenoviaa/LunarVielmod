using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using Stellamod.UI.Systems;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.STARBOMBER.Projectiles
{
    public class STARLINGBIG : ModNPC
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Morrowed Swampster");
			Main.npcFrameCount[NPC.type] = 28;
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
			NPC.width = 126;
			NPC.height = 126;
			NPC.damage = 50;
			NPC.defense = 30;
			NPC.lifeMax = 9000;
			NPC.HitSound = SoundID.NPCHit32;
			NPC.DeathSound = SoundID.NPCDeath6;
			NPC.value = 0f;
			NPC.knockBackResist = .45f;
			NPC.aiStyle = 85;
			AIType = NPCID.StardustCellBig;
			NPC.noTileCollide = true;
			NPC.scale = 0.5f;
			NPC.noGravity = true;
			NPC.dontTakeDamage = true;
		}


		int invisibilityTimer;
		public override void HitEffect(NPC.HitInfo hit)
		{
			for (int k = 0; k < 11; k++)
			{
				Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.BoneTorch, 1, -1f, 1, default, .61f);
			}
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
		{
			Color drawColor = new Color(Color.Pink.R, Color.Pink.G, Color.Pink.B, 0);
			Vector2 center = NPC.Center;// - new Vector2(50, 50);
            Vector2 lineDrawPos = center - screenPos;
            if (invisibilityTimer <= 40)
			{
				float alphaProgress = invisibilityTimer / 40f;
				float easedAlphaProgress = Easing.SpikeInOutCirc(alphaProgress);
				drawColor *= easedAlphaProgress;
				DrawHelper.DrawLineTelegraph(lineDrawPos, drawColor, new Vector2(20, 20));
                DrawHelper.DrawLineTelegraph(lineDrawPos, drawColor, new Vector2(-20, -20));
                DrawHelper.DrawLineTelegraph(lineDrawPos, drawColor, new Vector2(-20, 20));
                DrawHelper.DrawLineTelegraph(lineDrawPos, drawColor, new Vector2(20, -20));
            }

			if(invisibilityTimer > 40 && invisibilityTimer <= 140)
            {
                float alphaProgress = (invisibilityTimer - 40) / 100f;
                float easedAlphaProgress = Easing.SpikeInOutCirc(alphaProgress);
                drawColor *= easedAlphaProgress;
                DrawHelper.DrawLineTelegraph(lineDrawPos, drawColor, new Vector2(20, 20));
                DrawHelper.DrawLineTelegraph(lineDrawPos, drawColor, new Vector2(-20, -20));
                DrawHelper.DrawLineTelegraph(lineDrawPos, drawColor, new Vector2(-20, 20));
                DrawHelper.DrawLineTelegraph(lineDrawPos, drawColor, new Vector2(20, -20));

                DrawHelper.DrawLineTelegraph(lineDrawPos, drawColor, new Vector2(0, 20));
                DrawHelper.DrawLineTelegraph(lineDrawPos, drawColor, new Vector2(0, -20));
                DrawHelper.DrawLineTelegraph(lineDrawPos, drawColor, new Vector2(-20, 0));
                DrawHelper.DrawLineTelegraph(lineDrawPos, drawColor, new Vector2(20, 0));
            }


            if (invisibilityTimer > 140 && invisibilityTimer <= 220)
            {
                float alphaProgress = (invisibilityTimer - 140) / 80f;
                float easedAlphaProgress = Easing.SpikeInOutCirc(alphaProgress);
                drawColor *= easedAlphaProgress;
                DrawHelper.DrawLineTelegraph(lineDrawPos, drawColor, new Vector2(20, 20));
                DrawHelper.DrawLineTelegraph(lineDrawPos, drawColor, new Vector2(-20, -20));
                DrawHelper.DrawLineTelegraph(lineDrawPos, drawColor, new Vector2(-20, 20));
                DrawHelper.DrawLineTelegraph(lineDrawPos, drawColor, new Vector2(20, -20));

                DrawHelper.DrawLineTelegraph(lineDrawPos, drawColor, new Vector2(0, 20));
                DrawHelper.DrawLineTelegraph(lineDrawPos, drawColor, new Vector2(0, -20));
                DrawHelper.DrawLineTelegraph(lineDrawPos, drawColor, new Vector2(-20, 0));
                DrawHelper.DrawLineTelegraph(lineDrawPos, drawColor, new Vector2(20, 0));
            }

            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
			Vector2 frameOrigin = NPC.frame.Size();
			Vector2 drawPos = NPC.Center - screenPos;

			float time = Main.GlobalTimeWrappedHourly;
			float timer = Main.GlobalTimeWrappedHourly / 2f + time * 0.04f;

			time %= 4f;
			time /= 2f;

			if (time >= 1f)
			{
				time = 2f - time;
			}

			time = time * 0.5f + 0.5f;
			SpriteEffects Effects = NPC.spriteDirection != -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
			for (float i = 0f; i < 1f; i += 0.25f)
			{
				float radians = (i + timer) * MathHelper.TwoPi;

				spriteBatch.Draw(texture, drawPos + new Vector2(0f, DaedusDrug).RotatedBy(radians) * time, NPC.frame, new Color(77, 113, 255, 50), NPC.rotation, frameOrigin / 2, NPC.scale, Effects, 0);
			}

			for (float i = 0f; i < 1f; i += 0.34f)
			{
				float radians = (i + timer) * MathHelper.TwoPi;

				spriteBatch.Draw(texture, drawPos + new Vector2(0f, DaedusDrug * 2).RotatedBy(radians) * time, NPC.frame, new Color(254, 77, 77, 77), NPC.rotation, frameOrigin / 2, NPC.scale, Effects, 0);
			}

			return false;
		}

		public override void FindFrame(int frameHeight)
		{
			NPC.frameCounter += 0.45f;
			NPC.frameCounter %= Main.npcFrameCount[NPC.type];
			int frame = (int)NPC.frameCounter;
			NPC.frame.Y = frame * frameHeight;
		}
		public float Shooting = 0f;
		public override void AI()
		{
			var entitySource = NPC.GetSource_FromAI();
			timer++;
			NPC.spriteDirection = NPC.direction;
			Shooting++;

			invisibilityTimer++;
			if (invisibilityTimer == 40)
			{
				Speed();

				for (int k = 0; k < 11; k++)
					Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.BoneTorch, NPC.direction, -1f, 1, default, .61f);


				if (StellaMultiplayer.IsHost)
				{

                    float speedYa = NPC.velocity.Y * Main.rand.Next(-1, -1) * 0.0f + Main.rand.Next(-4, -4) * 0f;
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, 20, 20,
                        ModContent.ProjectileType<STARDREAM>(), 40, 0f, Owner: Main.myPlayer);
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, -20, -20,
                        ModContent.ProjectileType<STARDREAM>(), 40, 0f, Owner: Main.myPlayer);
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, -20, 20,
                        ModContent.ProjectileType<STARDREAM>(), 40, 0f, Owner: Main.myPlayer);
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, 20, -20,
                        ModContent.ProjectileType<STARDREAM>(), 40, 0f, Owner: Main.myPlayer);
                }




                SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/STARGROP"));


				ShakeModSystem.Shake = 7;


			}

			if (invisibilityTimer == 140)
			{
				Speed();

				for (int k = 0; k < 11; k++)
					Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.BoneTorch, NPC.direction, -1f, 1, default, .61f);


				if (StellaMultiplayer.IsHost)
				{
                    float speedYa = NPC.velocity.Y * Main.rand.Next(-1, -1) * 0.0f + Main.rand.Next(-4, -4) * 0f;
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, 20, 20,
                        ModContent.ProjectileType<STARDREAM>(), 40, 0f, Owner: Main.myPlayer);
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, -20, -20,
                        ModContent.ProjectileType<STARDREAM>(), 40, 0f, Owner: Main.myPlayer);
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, -20, 20,
                        ModContent.ProjectileType<STARDREAM>(), 40, 0f, Owner: Main.myPlayer);
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, 20, -20,
                        ModContent.ProjectileType<STARDREAM>(), 40, 0f, Owner: Main.myPlayer);

                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y , 0, 20,
                        ModContent.ProjectileType<STARDREAM>(), 40, 0f, Owner: Main.myPlayer);
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X , NPC.Center.Y , 0, -20,
                        ModContent.ProjectileType<STARDREAM>(), 40, 0f, Owner: Main.myPlayer);
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y , -20, 0,
                        ModContent.ProjectileType<STARDREAM>(), 40, 0f, Owner: Main.myPlayer);
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X , NPC.Center.Y, 20, 0,
                        ModContent.ProjectileType<STARDREAM>(), 40, 0f, Owner: Main.myPlayer);
                }


                SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/STARGROP"));



				ShakeModSystem.Shake = 7;

			}


			if (invisibilityTimer == 220)
			{
				Speed();

				for (int k = 0; k < 11; k++)
					Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.BoneTorch, NPC.direction, -1f, 1, default, .61f);


				if (StellaMultiplayer.IsHost)
				{
                    float speedYa = NPC.velocity.Y * Main.rand.Next(-1, -1) * 0.0f + Main.rand.Next(-4, -4) * 0f;


                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, 20, 20,
                        ModContent.ProjectileType<STARDREAM>(), 40, 0f, Owner: Main.myPlayer);
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, -20, -20,
                        ModContent.ProjectileType<STARDREAM>(), 40, 0f, Owner: Main.myPlayer);
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, -20, 20,
                        ModContent.ProjectileType<STARDREAM>(), 40, 0f, Owner: Main.myPlayer);
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, 20, -20,
                        ModContent.ProjectileType<STARDREAM>(), 40, 0f, Owner: Main.myPlayer);



                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, 0, 20,
                        ModContent.ProjectileType<STARDREAM>(), 40, 0f, Owner: Main.myPlayer);
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, 0, -20,
                        ModContent.ProjectileType<STARDREAM>(), 40, 0f, Owner: Main.myPlayer);
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, -20, 0,
                        ModContent.ProjectileType<STARDREAM>(), 40, 0f, Owner: Main.myPlayer);
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, 20, 0,
                        ModContent.ProjectileType<STARDREAM>(), 40, 0f, Owner: Main.myPlayer);
                }

                SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/STARGROP"));
				ShakeModSystem.Shake = 7;
			}


			NPC.scale += 0.01f;
			

			if (timer == 2)
			{
				SoundEngine.PlaySound(SoundID.DD2_EtherianPortalOpen, NPC.position);
			}

			if (invisibilityTimer == 250)
            {

				NPC.Kill();

				for (int i = 0; i < 150; i++)
				{
					Vector2 speed = Main.rand.NextVector2CircularEdge(4f, 4f);
					var d = Dust.NewDustPerfect(NPC.Center, DustID.BoneTorch, speed * 11, Scale: 3f);
					d.noGravity = true;

					Vector2 speeda = Main.rand.NextVector2CircularEdge(4f, 4f);
					var da = Dust.NewDustPerfect(NPC.Center, DustID.CoralTorch, speeda * 5, Scale: 3f);
					da.noGravity = false;

					Vector2 speedab = Main.rand.NextVector2CircularEdge(5f, 5f);
					var dab = Dust.NewDustPerfect(NPC.Center, DustID.BlueTorch, speeda * 20, Scale: 3f);
					dab.noGravity = false;
				}

				if (StellaMultiplayer.IsHost)
				{
					Vector2 pos = NPC.Center;
					pos.X -= 200;
					pos.Y -= 200;
					Projectile.NewProjectile(entitySource, pos, Vector2.Zero, ModContent.ProjectileType<STARLINGPRESPAWN>(), 1, 1, Main.myPlayer);
                }
			}

			switch (State)
			{
				case ActionState.Wait:
					counter++;
					Wait();
					break;

				case ActionState.Speed:
					counter++;
					Speed();
					NPC.velocity *= 0.98f;
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
			}
		}


		public void Speed()
		{
			timer++;
			if (timer == 100)
			{
				State = ActionState.Wait;
				timer = 0;
			}
		}

        public override bool? CanFallThroughPlatforms()
        {
            return true;
        }
    }
}