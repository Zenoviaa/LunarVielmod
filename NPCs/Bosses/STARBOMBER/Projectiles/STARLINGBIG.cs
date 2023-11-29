using Stellamod.Assets.Biomes;
using Stellamod.Items.Harvesting;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.DropRules;
using Stellamod.Helpers;
using Stellamod.Items.Accessories;
using Stellamod.Items.Accessories.Brooches;
using Stellamod.Items.Armors.Daeden;
using Stellamod.Items.Consumables;
using Stellamod.Items.Materials;
using Stellamod.Items.Weapons.Ranged;
using Stellamod.Items.Weapons.Thrown;
using Stellamod.NPCs.Bosses.Daedus;
using System.Collections.Generic;
using System.IO;
using Terraria.Audio;
using Stellamod.UI.Systems;

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
			NPC.width = 46;
			NPC.height = 50;
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
			Vector2 center = NPC.Center + new Vector2(0f, NPC.height * -0.1f);
			Lighting.AddLight(NPC.Center, Color.LightBlue.ToVector3() * 1.25f * Main.essScale);
			// This creates a randomly rotated vector of length 1, which gets it's components multiplied by the parameters
			Vector2 direction = Main.rand.NextVector2CircularEdge(NPC.width * 0.6f, NPC.height * 0.6f);
			float distance = 0.3f + Main.rand.NextFloat() * 0.5f;
			Vector2 velocity = new Vector2(0f, -Main.rand.NextFloat() * 0.3f - 1.5f);
			Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;



			Vector2 frameOrigin = NPC.frame.Size();
			Vector2 offset = new Vector2(NPC.width - frameOrigin.X + (NPC.scale * 4), NPC.height - NPC.frame.Height + 0);
			Vector2 drawPos = NPC.position - screenPos + frameOrigin + offset;

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

				spriteBatch.Draw(texture, drawPos + new Vector2(0f, DaedusDrug).RotatedBy(radians) * time, NPC.frame, new Color(77, 113, 255, 50), NPC.rotation, frameOrigin, NPC.scale, Effects, 0);
			}

			for (float i = 0f; i < 1f; i += 0.34f)
			{
				float radians = (i + timer) * MathHelper.TwoPi;

				spriteBatch.Draw(texture, drawPos + new Vector2(0f, DaedusDrug * 2).RotatedBy(radians) * time, NPC.frame, new Color(254, 77, 77, 77), NPC.rotation, frameOrigin, NPC.scale, Effects, 0);
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
			

				float speedYa = NPC.velocity.Y * Main.rand.Next(-1, -1) * 0.0f + Main.rand.Next(-4, -4) * 0f;
	

				Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X - 50, NPC.Center.Y - 50 , 20, 20, ModContent.ProjectileType<STARDREAM>(), 40, 0f, 0, 0f, 0f);
				Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X - 50, NPC.Center.Y - 50 , -20, -20, ModContent.ProjectileType<STARDREAM>(), 40, 0f, 0, 0f, 0f);
				Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X - 50, NPC.Center.Y - 50 , -20, 20, ModContent.ProjectileType<STARDREAM>(), 40, 0f, 0, 0f, 0f);
				Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X - 50, NPC.Center.Y - 50, 20, -20, ModContent.ProjectileType<STARDREAM>(), 40, 0f, 0, 0f, 0f);






				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/STARGROP"));


				ShakeModSystem.Shake = 7;


			}

			if (invisibilityTimer == 140)
			{
				Speed();

				for (int k = 0; k < 11; k++)
					Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.BoneTorch, NPC.direction, -1f, 1, default, .61f);


				float speedYa = NPC.velocity.Y * Main.rand.Next(-1, -1) * 0.0f + Main.rand.Next(-4, -4) * 0f;


				Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X - 200, NPC.Center.Y - 200, 20, 20, ModContent.ProjectileType<STARDREAM>(), 40, 0f, 0, 0f, 0f);
				Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X - 200, NPC.Center.Y - 200, -20, -20, ModContent.ProjectileType<STARDREAM>(), 40, 0f, 0, 0f, 0f);
				Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X - 200, NPC.Center.Y - 200, -20, 20, ModContent.ProjectileType<STARDREAM>(), 40, 0f, 0, 0f, 0f);
				Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X - 200, NPC.Center.Y - 200, 20, -20, ModContent.ProjectileType<STARDREAM>(), 40, 0f, 0, 0f, 0f);




				Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X - 200, NPC.Center.Y - 200, 0, 20, ModContent.ProjectileType<STARDREAM>(), 40, 0f, 0, 0f, 0f);
				Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X - 200, NPC.Center.Y - 200, 0, -20, ModContent.ProjectileType<STARDREAM>(), 40, 0f, 0, 0f, 0f);
				Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X - 200, NPC.Center.Y - 200, -20, 0, ModContent.ProjectileType<STARDREAM>(), 40, 0f, 0, 0f, 0f);
				Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X - 200, NPC.Center.Y - 200, 20, 0, ModContent.ProjectileType<STARDREAM>(), 40, 0f, 0, 0f, 0f);


				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/STARGROP"));



				ShakeModSystem.Shake = 7;

			}


			if (invisibilityTimer == 220)
			{
				Speed();

				for (int k = 0; k < 11; k++)
					Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.BoneTorch, NPC.direction, -1f, 1, default, .61f);


				float speedYa = NPC.velocity.Y * Main.rand.Next(-1, -1) * 0.0f + Main.rand.Next(-4, -4) * 0f;


				Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X - 200, NPC.Center.Y - 200, 20, 20, ModContent.ProjectileType<STARDREAM>(), 40, 0f, 0, 0f, 0f);
				Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X - 200, NPC.Center.Y - 200, -20, -20, ModContent.ProjectileType<STARDREAM>(), 40, 0f, 0, 0f, 0f);
				Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X - 200, NPC.Center.Y - 200, -20, 20, ModContent.ProjectileType<STARDREAM>(), 40, 0f, 0, 0f, 0f);
				Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X - 200, NPC.Center.Y - 200, 20, -20, ModContent.ProjectileType<STARDREAM>(), 40, 0f, 0, 0f, 0f);



				Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X - 200, NPC.Center.Y - 200, 0, 20, ModContent.ProjectileType<STARDREAM>(), 40, 0f, 0, 0f, 0f);
				Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X - 200, NPC.Center.Y - 200, 0, -20, ModContent.ProjectileType<STARDREAM>(), 40, 0f, 0, 0f, 0f);
				Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X - 200, NPC.Center.Y - 200, -20, 0, ModContent.ProjectileType<STARDREAM>(), 40, 0f, 0, 0f, 0f);
				Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X - 200, NPC.Center.Y - 200, 20, 0, ModContent.ProjectileType<STARDREAM>(), 40, 0f, 0, 0f, 0f);


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
					;
					d.noGravity = true;

					Vector2 speeda = Main.rand.NextVector2CircularEdge(4f, 4f);
					var da = Dust.NewDustPerfect(NPC.Center, DustID.CoralTorch, speeda * 5, Scale: 3f);
					;
					da.noGravity = false;

					Vector2 speedab = Main.rand.NextVector2CircularEdge(5f, 5f);
					var dab = Dust.NewDustPerfect(NPC.Center, DustID.BlueTorch, speeda * 20, Scale: 3f);
					;
					dab.noGravity = false;
				}






				int index = NPC.NewNPC(entitySource, (int)NPC.Center.X - 200, (int)NPC.Center.Y - 200, ModContent.NPCType<STARLING>());
				NPC minionNPC = Main.npc[index];
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


			if (timer > 50)
			{








			}

			if (timer == 100)
			{
				State = ActionState.Wait;
				timer = 0;
			}

		}
	}
}