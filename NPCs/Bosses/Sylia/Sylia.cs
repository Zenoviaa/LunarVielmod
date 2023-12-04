using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using Stellamod.Helpers;
using Stellamod.Items.Accessories.Wings;
using Stellamod.Items.Consumables;
using Stellamod.Items.Materials;
using Stellamod.NPCs.Bosses.Sylia.Projectiles;
using Stellamod.Particles;
using Stellamod.Projectiles.Summons.VoidMonsters;
using Stellamod.Projectiles.Swords;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.Sylia
{
    [AutoloadBossHead]
    public class Sylia : ModNPC
	{
		private bool _spawned;
		private float _quickSlashRotation;
		private AttackState _lastAttack;
		private AttackState _attack=AttackState.Idle;
		private Phase _attackPhase;
		private enum AttackState
        {
			Idle,
			X_Slash,
			Quick_Slash,
			Quick_Slash_V2,
			Void_Bomb,
			Vertical_Slash,
			Void_Bolts,
			Transition,
			Summon_Void_Wall
        }

		private enum Phase
        {
			Phase_1,
			Phase_2,
			Phase_3
        }

		private Vector2 _slashCenter;
		private Vector2[] _voidBoltPositions;
        public override void SetStaticDefaults()
		{
			Main.npcFrameCount[Type] = 30;
			NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers();
			drawModifiers.CustomTexturePath = "Stellamod/NPCs/Bosses/Sylia/SyliaPreview";
			drawModifiers.PortraitScale = 0.8f; // Portrait refers to the full picture when clicking on the icon in the bestiary
			drawModifiers.PortraitPositionYOverride = 0f;
			NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			// Sets the description of this NPC that is listed in the bestiary
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> {
				new MoonLordPortraitBackgroundProviderBestiaryInfoElement(), // Plain black background
				new FlavorTextBestiaryInfoElement("Sylia, The Empress of the Stars and moon, Vixyl's sister and a master magic swordswoman.")
			});
		}

		//AI Values
		public override void SetDefaults()
		{
			NPC.Size = new Vector2(24, 48);

			NPC.damage = 1;
			NPC.defense = 26;
			NPC.lifeMax = 28000;  
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
			NPC.knockBackResist = 0f;
			NPC.noGravity = true;
			NPC.noTileCollide = true;
			NPC.value = Item.buyPrice(gold: 40);
			NPC.SpawnWithHigherTime(30);
			NPC.boss = true;
			NPC.scale = 1f;

			// Take up open spawn slots, preventing random NPCs from spawning during the fight
			NPC.npcSlots = 10f;

			// Custom AI, 0 is "bound town NPC" AI which slows the NPC down and changes sprite orientation towards the target
			NPC.aiStyle = -1;

			// Custom boss bar
			NPC.BossBar = ModContent.GetInstance<SyliaBossBar>();

			// The following code assigns a music track to the boss in a simple way.
			if (!Main.dedServ)
			{
				Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/VerliaOfTheMoon");
			}
		}

		private void ResetAI()
        {
			NPC npc = NPC;
			ref float ai_Telegraph_Counter = ref npc.ai[0];
			ref float ai_Counter = ref npc.ai[1];
			ref float ai_Cycle = ref npc.ai[2];
			ai_Counter = 0;
			ai_Telegraph_Counter = 0;
			ai_Cycle = 0;
		}

		private float _spawnAlpha;
		private void AI_Spawn()
		{
			NPC npc = NPC;
			npc.TargetClosest();

			ref float ai_Telegraph_Counter = ref npc.ai[0];
			ref float ai_Counter = ref npc.ai[1];
			ref float ai_Cycle = ref npc.ai[2];
			if (ai_Counter == 0)
            {
				ActivateSyliaSky();
				SoundEngine.PlaySound(SoundID.Item119);
			} 
			if (ai_Counter > 30 && npc.HasValidTarget)
            {
				//Hover above player
				Player target = Main.player[npc.target];
				Vector2 targetCenter = target.Center;
				Vector2 targetHoverCenter = targetCenter + new Vector2(0, -256);
				npc.Center = Vector2.Lerp(npc.Center, targetHoverCenter, 0.25f);
				_spawnAlpha = MathHelper.Lerp(0, 1, ((ai_Counter-30) / 270));
			}
			if(ai_Counter > 120 && ai_Counter < 240)
            {
				for (int i = 0; i < 4; i++)
				{
					Vector2 position = NPC.Center+Main.rand.NextVector2CircularEdge(256, 256);
					Vector2 speed = (NPC.Center - position).SafeNormalize(Vector2.Zero) * 16;
					Particle p = ParticleManager.NewParticle(position, speed, ParticleManager.NewInstance<VoidParticle>(),
						default(Color), 1 / 3f);
					p.layer = Particle.Layer.BeforeProjectiles;
				}
			}
			if(ai_Counter == 300)
            {
				//Charged Sound thingy
				for (int i = 0; i < 32; i++)
				{
					Vector2 position = NPC.Center;
					Vector2 speed = Main.rand.NextVector2CircularEdge(8f, 8f);
					Particle p = ParticleManager.NewParticle(position, speed, ParticleManager.NewInstance<VoidParticle>(),
						default(Color), 1);
					p.layer = Particle.Layer.BeforeProjectiles;
				}

				SoundEngine.PlaySound(SoundID.Item100);
				ai_Counter = 0;
				_spawned = true;
			}

			ai_Counter++;
		}

		public override void AI()
		{
			//Spawning Animation
            if (!_spawned)
			{
				AI_Spawn();
				return;
			}

			//Determine Attack Phase
			float lifeMax = NPC.lifeMax;
			float partOfLifeMax = lifeMax / 3;
			if (NPC.life < lifeMax - partOfLifeMax && _attackPhase < Phase.Phase_2)
            {
				ResetAI();
				SoundEngine.PlaySound(SoundID.Item161);
				_attackPhase = Phase.Phase_2;
				_attack = AttackState.Transition;
            }

			//No Contact Damage SmH
			NPC.damage = 0;
            switch (_attackPhase)
            {
				case Phase.Phase_1:
					AI_Phase1();
					break;

				case Phase.Phase_2:
					AI_Phase2();
                    break;

				//Maaaaybe?
                case Phase.Phase_3:
					AI_Phase3();
					break;
            }
        }

        #region Draw Code
        private void PreDrawAfterImage(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
			Vector2 drawPos = NPC.Center - screenPos;

			float time = Main.GlobalTimeWrappedHourly;
			float timer = Main.GlobalTimeWrappedHourly / 2f + time * 0.04f;
			float rotationOffset = VectorHelper.Osc(1f, 2f, 5);
			time %= 4f;
			time /= 2f;

			if (time >= 1f)
			{
				time = 2f - time;
			}

			time = time * 0.5f + 0.5f;
			SpriteEffects effects = SpriteEffects.None;
			Player player = Main.player[NPC.target];
			if (player.Center.X < NPC.Center.X)
			{
				effects = SpriteEffects.FlipHorizontally;
			}

			Color rotatedColor = new Color(60, 0, 118, 75);
			if (_attackPhase == Phase.Phase_2
			&& _attack == AttackState.Transition)
			{
				rotatedColor = rotatedColor.MultiplyAlpha(_p2TransitionAlpha);
			}

			for (float i = 0f; i < 1f; i += 0.25f)
			{
				float radians = (i + timer) * MathHelper.TwoPi;
				Vector2 rotatedPos = drawPos + new Vector2(0f, 8f * rotationOffset).RotatedBy(radians) * time;
				spriteBatch.Draw(texture, rotatedPos,
					texture.AnimationFrame(ref _frameCounter, ref _frameTick, 2, 30, false),
				rotatedColor, 0f, new Vector2(48, 48), 1f, effects, 0f);	
			}

			for (float i = 0f; i < 1f; i += 0.34f)
			{
				float radians = (i + timer) * MathHelper.TwoPi;
				Vector2 rotatedPos = drawPos +new Vector2(0f, 16f * rotationOffset).RotatedBy(radians) * time;
				spriteBatch.Draw(texture, rotatedPos,
					texture.AnimationFrame(ref _frameCounter, ref _frameTick, 2, 30, false),
					rotatedColor, 0f, new Vector2(48, 48), 1f, effects, 0f);
			}
		}

		//Animation Stuffs
		private int _frameCounter;
		private int _frameTick;
		private int _wingFrameCounter;
		private int _wingFrameTick;

		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
			Player player = Main.player[NPC.target];
			SpriteEffects effects = SpriteEffects.None;
			if (player.Center.X < NPC.Center.X)
			{
				effects = SpriteEffects.FlipHorizontally;
			}

			//Draw Faint Glow
			Vector3 huntrianColorXyz = DrawHelper.HuntrianColorOscillate(
				new Vector3(60, 0, 118),
				new Vector3(117, 1, 187),
				new Vector3(3, 3, 3), 0);



            //I moved this into a separate function just so it doesn't clutter everything
            if (_spawned)
			{           
				//Draw the glow
				DrawHelper.DrawDimLight(NPC, huntrianColorXyz.X, huntrianColorXyz.Y, huntrianColorXyz.Z, new Color(60, 0, 118), drawColor, 2);
				PreDrawAfterImage(spriteBatch, screenPos, drawColor);
			} else
            {
				drawColor = drawColor.MultiplyAlpha(_spawnAlpha);
			}

			//Go invisible during the phase 2 transition
			if (_attackPhase == Phase.Phase_2
				&& _attack == AttackState.Transition)
			{
				drawColor = drawColor.MultiplyAlpha(_p2TransitionAlpha);
			}


			//Draw the Wings
			Vector2 drawPosition = NPC.Center - screenPos;
			Vector2 origin = new Vector2(48, 48);
			Texture2D syliaWingsTexture = ModContent.Request<Texture2D>("Stellamod/NPCs/Bosses/Sylia/SyliaWings").Value;
			int wingFrameSpeed = 2;
			int wingFrameCount = 10;
			spriteBatch.Draw(syliaWingsTexture, drawPosition,
				syliaWingsTexture.AnimationFrame(ref _wingFrameCounter, ref _wingFrameTick, wingFrameSpeed, wingFrameCount, true),
				drawColor, 0f, origin, 1f, effects, 0f);

			Texture2D syliaIdleTexture = ModContent.Request<Texture2D>(Texture).Value;
			int syliaIdleSpeed = 2;
			int syliaIdleFrameCount = 30;
			spriteBatch.Draw(syliaIdleTexture, drawPosition, 
				syliaIdleTexture.AnimationFrame(ref _frameCounter, ref _frameTick, syliaIdleSpeed, syliaIdleFrameCount, true), 
				drawColor, 0f, origin, 1f, effects, 0f);

			return false;
        }
        #endregion

        #region Sky
		private void ActivateSyliaSky()
		{
			NPC npc = NPC;
			npc.TargetClosest();
			if (NPC.HasValidTarget && !SkyManager.Instance["Stellamod:SyliaSky"].IsActive())
			{
				Player target = Main.player[NPC.target];
				Vector2 targetCenter = target.Center;
				SkyManager.Instance.Activate("Stellamod:SyliaSky", targetCenter);
			}
		}

		private void DeActivateSyliaSky()
		{
			NPC npc = NPC;
			npc.TargetClosest();
			if (SkyManager.Instance["Stellamod:SyliaSky"].IsActive())
			{
				Player target = Main.player[NPC.target];
				Vector2 targetCenter = target.Center;
				SkyManager.Instance.Deactivate("Stellamod:SyliaSky", targetCenter);
			}
		}

		#endregion

		#region Attack Cycle

		private void AI_DetermineAttack()
		{
			//Just on a pattern for now
			//Might change it to have some randomized parts.
			switch (_attackPhase)
			{
				case Phase.Phase_1:
					switch (_lastAttack)
					{
						default:
							_attack = AttackState.X_Slash;
							break;
						case AttackState.X_Slash:
							switch (Main.rand.Next(2))
							{
								case 0:
									if (Main.rand.NextBool(2))
									{
										_attack = AttackState.Void_Bolts;
									}
									else
									{
										_attack = AttackState.Void_Bomb;
									}

									break;
								case 1:
									_attack = AttackState.Quick_Slash;
									break;
							}
							break;
						case AttackState.Quick_Slash:
							switch (Main.rand.Next(2))
							{
								case 0:
									_attack = AttackState.Void_Bolts;
									break;
								case 1:
									_attack = AttackState.X_Slash;
									break;
							}
							break;
						case AttackState.Quick_Slash_V2:
							_attack = AttackState.X_Slash;
							break;
						case AttackState.Void_Bolts:
							_attack = AttackState.Quick_Slash_V2;
							break;
					}
					break;

				case Phase.Phase_2:
					switch (_lastAttack)
					{
						default:
							_attack = AttackState.X_Slash;
							break;
						case AttackState.X_Slash:
							switch (Main.rand.Next(2))
							{
								case 0:
									_attack = AttackState.Void_Bomb;
									break;
								case 1:
									_attack = AttackState.Quick_Slash;
									break;
							}
							break;
						case AttackState.Quick_Slash:
							_attack = AttackState.X_Slash;
							break;
					}
					break;
			}

			_lastAttack = _attack;
		}
		#endregion

		#region Phase 1

		private const int Phase1_Idle_Time = 60;
		private const int Phase1_X_Scissor_Telegraph_Time = RipperSlashTelegraphParticle.Animation_Length;
		private const int Phase1_Void_Bolts_Telegraph_Time = RipperSlashTelegraphParticle.Animation_Length;
		private const int Phase1_Quick_Slash_Telegraph_Time = RipperSlashTelegraphParticle.Animation_Length / 3;

        private void AI_Phase1()
		{
			NPC npc = NPC;
			npc.TargetClosest();
			if (!NPC.HasValidTarget)
			{
				//Despawn in 2 seconds
				npc.velocity = Vector2.Lerp(npc.velocity, new Vector2(0, -8), 0.025f);
				npc.EncourageDespawn(120);
				DeActivateSyliaSky();
				return;
			}

			ref float ai_Telegraph_Counter = ref npc.ai[0];
			ref float ai_Counter = ref npc.ai[1];
			ref float ai_Cycle = ref npc.ai[2];

			Player target = Main.player[npc.target];
			Vector2 targetCenter = target.Center;
			Vector2 targetPosition = target.position;

			//So how does this bosses attack cycle works?
			//I do want it to use a little bit of RNG, but also have some patterns that you can follow
			//But the core concept is basically that she tears rifts in reality that summon horrors to attack you
			//You need to manage these portals while attacking sylia at the same time.
			//Let's start with her basic attacks first

			//1. X-Slash, Sylia telegraphs the attack with the dotted line telegraph before slashing through and opening up a rift.
			//The rift releases 1-2 monsters before closing

			//2. Quick Slash, Sylia telegraphs the attack with the dotted line telegraph before doing a quick slash, this does not open a rift.
			//This attack will be used multiple times in quick succession, forcing the player to move around a lot to dodge it.

			//3. Quick Slash V2, Sylia moves away for a bit and then slowly moves towards the player, then she does several seemingly random slashes in a horizontal motion
			//These slashes leave behind lingering rifts that damage you, but won't release monsters.

			//4 Monster Slash, Sylia cuts 2-3 spots around her and releases homing void eaters at you, this attack will be used shortly after some attacks sometimes

			//5.Vertical Slash, Sylia cuts down the middle of the arena, leaving behind a large gaping rift that blocks movement to the other side. You'll need to dodge in a cramped space for a bit.

			//6.Pot of Greed, Sylia summons the pot of greed, and it cooks up a  plethora of homing monsters to assault you

			//7.Void Bolts, Sylia releases several homing void projectiles.

			//General AI Pattern:
			//Sylia always starts the fight with a Quick Slash
			//Sylia will have a bit of delay between each of her attacks as she decides what to do.
			//Sylia will have a degree of randomness, the same attack will never be used multiple times in a row.
			//For example, she may do Quick Slash -> Monster Slash -> X Slash, but she'll never do X-Slash -> X-Slash -> X-Slash, this will keep the fight varied
			//Quick Slash V1 and V2 cannot be used consecutively.

			//Phase 2 Transition:
			//Sylia creates a gaping cross rift, before throwing a voodoo doll into it
			//This causes the screen to shake for a bit and world to start going dark
			//Eventually, the rift explodes, filling the arena with void monsters

			switch (_attack)
			{
				case AttackState.Idle:
					float hoverSpeed = 5;
					float yVelocity = VectorHelper.Osc(1, -1, hoverSpeed);
					npc.velocity = Vector2.Lerp(npc.velocity, new Vector2(0, yVelocity), 0.2f);

					float rotation = VectorHelper.Osc(-5, 5, hoverSpeed / 2);
					npc.rotation = MathHelper.ToRadians(rotation);

					ai_Counter++;
					if(ai_Counter > Phase1_Idle_Time)
                    {
						//Determine Attack
						ai_Counter = 0;
						ai_Telegraph_Counter = 0;

						//For now let's just always go into x slash
						//Ok we need to determine what attacks to do
						//Determien next attack
						//ermm
						//Let's do cycle for now?
						AI_DetermineAttack();
					}

					break;

				case AttackState.X_Slash:
					//float speedMultiplier = ai_Telegraph_Counter / Phase1_X_Scissor_Telegraph_Time;
					//npc.velocity = VectorHelper.VelocityDirectTo(npc.Center, targetCenter, 5 * speedMultiplier);
					Vector2 targetXCircleCenter = targetCenter + new Vector2(256, 0).RotatedBy(MathHelper.ToRadians((ai_Telegraph_Counter * (360 / Phase1_X_Scissor_Telegraph_Time))));
					npc.Center = Vector2.Lerp(npc.Center, targetXCircleCenter, 0.005f);

					//Telegraph
					if (ai_Telegraph_Counter == 0)
                    {
						Vector2 targetVelocity = target.velocity;
						Vector2 targetOffset = targetVelocity.SafeNormalize(Vector2.Zero) * 16*24;
						_slashCenter = targetCenter + targetOffset;

						//Spawn Scissor 1
						var scissorTelegraphPart1 = Projectile.NewProjectileDirect(npc.GetSource_FromThis(), npc.Center, Vector2.Zero,
							ModContent.ProjectileType<SyliaScissor>(), 0, 0);

						SyliaScissor syliaScissor1 = scissorTelegraphPart1.ModProjectile as SyliaScissor;
						syliaScissor1.startCenter = _slashCenter + new Vector2(-256, -256);
						syliaScissor1.targetCenter = _slashCenter;
						syliaScissor1.delay = Phase1_X_Scissor_Telegraph_Time - 8;

						//Spawn Scissor 2
						var scissorTelegraphPart2 = Projectile.NewProjectileDirect(npc.GetSource_FromThis(), npc.Center, Vector2.Zero,
							ModContent.ProjectileType<SyliaScissor>(), 0, 0);

						SyliaScissor syliaScissor2 = scissorTelegraphPart2.ModProjectile as SyliaScissor;
						syliaScissor2.startCenter = _slashCenter + new Vector2(256, -256);
						syliaScissor2.targetCenter = _slashCenter;
						syliaScissor2.delay = Phase1_X_Scissor_Telegraph_Time - 8;

						//Spawn Ripper Particles
						Particle telegraphPart1 = ParticleManager.NewParticle(_slashCenter, Vector2.Zero,
							ParticleManager.NewInstance<RipperSlashTelegraphParticle>(), default(Color), 1f);

						Particle telegraphPart2 = ParticleManager.NewParticle(_slashCenter, Vector2.Zero,
							ParticleManager.NewInstance<RipperSlashTelegraphParticle>(), default(Color), 1f);

						telegraphPart1.rotation = MathHelper.ToRadians(-45);
						telegraphPart2.rotation = MathHelper.ToRadians(45);
						SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/RipperSlashTelegraph"));
					}

					ai_Telegraph_Counter++;
					if(ai_Telegraph_Counter > Phase1_X_Scissor_Telegraph_Time)
					{
						ai_Telegraph_Counter = 0;

						//X Slash Visuals
						var xSlashPart1 = Projectile.NewProjectileDirect(npc.GetSource_FromThis(), _slashCenter, Vector2.Zero,
							ModContent.ProjectileType<RipperSlashProjBig>(), 0, 0f);
						var xSlashPart2 = Projectile.NewProjectileDirect(npc.GetSource_FromThis(), _slashCenter, Vector2.Zero,
							ModContent.ProjectileType<RipperSlashProjBig>(), 0, 0f);

						xSlashPart1.timeLeft = 1500;
						xSlashPart2.timeLeft = 1500;
						(xSlashPart1.ModProjectile as RipperSlashProjBig).randomRotation = false;
						(xSlashPart2.ModProjectile as RipperSlashProjBig).randomRotation = false;
						xSlashPart2.rotation = MathHelper.ToRadians(90);

						//Actual Attack Here
						var voidRiftProjectile1 = Projectile.NewProjectileDirect(npc.GetSource_FromThis(), _slashCenter, Vector2.Zero,
							ModContent.ProjectileType<VoidRift>(), 30, 1);

						voidRiftProjectile1.timeLeft = 600;
						voidRiftProjectile1.hostile = true;
						voidRiftProjectile1.friendly = false;
						voidRiftProjectile1.rotation = MathHelper.ToRadians(-45);

						var voidRiftProjectile2 = Projectile.NewProjectileDirect(npc.GetSource_FromThis(), _slashCenter, Vector2.Zero,
							ModContent.ProjectileType<VoidRift>(), 30, 1);

						voidRiftProjectile2.timeLeft = 600;
						voidRiftProjectile2.hostile = true;
						voidRiftProjectile2.friendly = false;

						voidRiftProjectile2.rotation = MathHelper.ToRadians(45);
						int p = Projectile.NewProjectile(npc.GetSource_FromThis(), _slashCenter, Vector2.Zero,
							ModContent.ProjectileType<VoidBolt>(), 30, 1);
						Main.projectile[p].timeLeft = 300;

						//Return to idle state after x slashing 3 times.
						//We could randomize the number of x slashes actually.
						ai_Counter++;
						if (ai_Counter > 3)
                        {
							ai_Counter = 0;
							_attack = AttackState.Idle;
						}
                    }

					break;

				case AttackState.Quick_Slash:
					float quickSpeedMultiplier = ai_Telegraph_Counter / Phase1_Quick_Slash_Telegraph_Time;
					npc.velocity = VectorHelper.VelocityDirectTo(npc.Center, targetCenter, 8 * quickSpeedMultiplier);

					if (ai_Telegraph_Counter == 0)
					{
						_slashCenter = targetCenter;

						Particle telegraphPart1 = ParticleManager.NewParticle(_slashCenter, Vector2.Zero,
							ParticleManager.NewInstance<RipperSlashTelegraphParticle>(), default(Color), 1f);

						_quickSlashRotation = MathHelper.ToRadians(Main.rand.Next(0, 360));
						telegraphPart1.rotation = _quickSlashRotation;
						SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/RipperSlashTelegraph"));

						var scissorTelegraphPart1 = Projectile.NewProjectileDirect(npc.GetSource_FromThis(), npc.Center, Vector2.Zero,
							ModContent.ProjectileType<SyliaScissorSmall>(), 0, 0);

						var syliaScissor1 = scissorTelegraphPart1.ModProjectile as SyliaScissorSmall;
						syliaScissor1.startCenter = _slashCenter + new Vector2(256, 0).RotatedBy(_quickSlashRotation);
						syliaScissor1.targetCenter = _slashCenter;
						syliaScissor1.delay = Phase1_Quick_Slash_Telegraph_Time - 8;
					}

					ai_Telegraph_Counter++;
					if (ai_Telegraph_Counter > Phase1_Quick_Slash_Telegraph_Time)
					{
						ai_Telegraph_Counter = 0;
						var xSlashPart1 = Projectile.NewProjectileDirect(npc.GetSource_FromThis(), _slashCenter, Vector2.Zero,
							ModContent.ProjectileType<RipperSlashProjBig>(), 0, 0f);
						(xSlashPart1.ModProjectile as RipperSlashProjBig).randomRotation = false;

						var proj = Projectile.NewProjectileDirect(npc.GetSource_FromThis(), _slashCenter, Vector2.Zero, 
							ModContent.ProjectileType<VoidSlash>(), 60, 1);

						xSlashPart1.timeLeft = 900;
						xSlashPart1.rotation = _quickSlashRotation + MathHelper.ToRadians(45);
						proj.rotation = _quickSlashRotation;
	
						ai_Counter++;
						if (ai_Counter > 9)
						{
							ai_Counter = 0;
							_attack = AttackState.Idle;
						}
					}

					break;

                case AttackState.Quick_Slash_V2:
					Vector2 targetCircleCenter = targetCenter + new Vector2(256, 0).RotatedBy(MathHelper.ToRadians((ai_Counter * (360/15))));
					npc.Center = Vector2.Lerp(npc.Center, targetCircleCenter, 0.025f);
					//float quickSpeedMultiplier2 = ai_Telegraph_Counter / Phase1_Quick_Slash_Telegraph_Time;
					//npc.velocity = VectorHelper.VelocityDirectTo(targetCircleCenter, npc.Center, 15 * quickSpeedMultiplier2);

					if (ai_Telegraph_Counter == 0)
					{
						_slashCenter = targetCenter;

						Particle telegraphPart1 = ParticleManager.NewParticle(_slashCenter, Vector2.Zero,
							ParticleManager.NewInstance<RipperSlashTelegraphParticle>(), default(Color), 1f);

						var scissorTelegraphPart1 = Projectile.NewProjectileDirect(npc.GetSource_FromThis(), npc.Center, Vector2.Zero,
							ModContent.ProjectileType<SyliaScissorSmall2>(), 0, 0);

						var syliaScissor1 = scissorTelegraphPart1.ModProjectile as SyliaScissorSmall2;
						syliaScissor1.startCenter = _slashCenter + new Vector2(256, 0).RotatedBy(_quickSlashRotation);
						syliaScissor1.targetCenter = _slashCenter;
						syliaScissor1.delay = (Phase1_Quick_Slash_Telegraph_Time / 2) - 8;

						_quickSlashRotation = MathHelper.ToRadians(Main.rand.Next(0, 360));
						telegraphPart1.rotation = _quickSlashRotation;
						SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/RipperSlashTelegraph"));
					}

					ai_Telegraph_Counter++;
					if (ai_Telegraph_Counter > Phase1_Quick_Slash_Telegraph_Time / 2)
					{
						ai_Telegraph_Counter = 0;
						Vector2 ripperSlashOffset = new Vector2(16, 16);
						var xSlashPart1 = Projectile.NewProjectileDirect(npc.GetSource_FromThis(), _slashCenter, Vector2.Zero,
							ModContent.ProjectileType<RipperSlashProjBig>(), 0, 0f);
						xSlashPart1.timeLeft = 900;
						(xSlashPart1.ModProjectile as RipperSlashProjBig).randomRotation = false;
						xSlashPart1.rotation = _quickSlashRotation + MathHelper.ToRadians(45); ;
						
						var proj = Projectile.NewProjectileDirect(npc.GetSource_FromThis(), _slashCenter, Vector2.Zero,
							ModContent.ProjectileType<VoidSlash>(), 60, 1);
						proj.rotation = _quickSlashRotation;

						ai_Counter++;

						//Every third slash, teleport, we ned to keep this boss moving around.
						if(ai_Counter % 3 == 0)
						{
							float teleportRadius = 256;
							Vector2 teleportPosition = new Vector2(
								targetCenter.X + Main.rand.NextFloat(-teleportRadius, teleportRadius),
								targetCenter.Y + Main.rand.NextFloat(-teleportRadius, teleportRadius));

							//Visuals on the teleport
							Dust.QuickDustLine(npc.Center, teleportPosition, 100f, Color.Violet);
							for (int i = 0; i < 64; i++)
							{
								Vector2 speed = Main.rand.NextVector2CircularEdge(4f, 4f);
								Particle p = ParticleManager.NewParticle(teleportPosition, speed, ParticleManager.NewInstance<VoidParticle>(),
									default(Color), 1 / 3f);
								p.layer = Particle.Layer.BeforeProjectiles;
							}

							npc.Center = teleportPosition;
					
							SoundEngine.PlaySound(SoundID.Item165);
						}

						if (ai_Counter > 15)
						{
							ai_Counter = 0;
							ai_Telegraph_Counter = 0;
							_attack = AttackState.Vertical_Slash;
						}
					}

					//Visuals
					if (Main.rand.NextBool(6))
					{
						Vector2 vel = new Vector2(Main.rand.NextFloat(-1f, 1f), -1f);
						var dust = Dust.NewDustPerfect(npc.Center, DustID.GemAmethyst, vel, Scale: 1.5f);
						dust.noGravity = true;
					}

					break;

                case AttackState.Void_Bomb:	
					if(ai_Counter == 0)
					{
						int telegraphLength = 50;
						ai_Telegraph_Counter++;
						if (ai_Telegraph_Counter < telegraphLength)
						{
							for (int i = 0; i < 1; i++)
							{
								Vector2 position = NPC.Center + Main.rand.NextVector2CircularEdge(128, 128);
								Vector2 speed = (NPC.Center - position).SafeNormalize(Vector2.Zero) * 8;
								Particle p = ParticleManager.NewParticle(position, speed, ParticleManager.NewInstance<VoidParticle>(),
									default(Color), 0.5f);
								p.layer = Particle.Layer.BeforeProjectiles;
								Particle tearParticle = ParticleManager.NewParticle(position, Vector2.Zero, ParticleManager.NewInstance<VoidTearParticle>(),
								default(Color), 0.5f + 0.025f);

								tearParticle.layer = Particle.Layer.BeforePlayersBehindNPCs;
							}
							
						}
                        if (ai_Telegraph_Counter == telegraphLength)
                        {
							ai_Counter++;
						}
					} 
					else if (ai_Counter == 1)
                    {
						Vector2 position = NPC.Center + new Vector2(0, -64);
						Vector2 velocity = (targetCenter - position).SafeNormalize(Vector2.Zero) * 12;
						Projectile.NewProjectile(npc.GetSource_FromThis(), position, velocity,
							ModContent.ProjectileType<VoidBall>(), 20, 1);
						ai_Counter = 0;
						ai_Telegraph_Counter = 0;
						Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(position, 512f, 32f);
						SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/RipperSlash1"), position);
						_attack = AttackState.Idle;
					}
	
					break;

				case AttackState.Vertical_Slash:
					//Move to either the left or right of the player depending on which way you're moving
					//If you're not moving at all she'll cut straight down you lmao
					ai_Telegraph_Counter++;
					if (ai_Counter == 0)
					{
						Vector2 targetVelocity = target.velocity;
						Vector2 targetOffset = targetVelocity.SafeNormalize(Vector2.Zero) * 16 * 24;
						_slashCenter = targetCenter + targetOffset;
						float verticalHoverSpeed = 2;
						float verticalHoverYVelocity = VectorHelper.Osc(1, -1, verticalHoverSpeed);
						float verticalSlashStartOffsetDistance = 512;

						Vector2 verticalHoverTarget = _slashCenter + new Vector2(0, -verticalSlashStartOffsetDistance + verticalHoverYVelocity);
						npc.Center = Vector2.Lerp(npc.Center, verticalHoverTarget, 0.023f);

						//Delay before she slashes
						if (ai_Telegraph_Counter > 60)
						{
							ai_Counter++;
							ai_Telegraph_Counter = 0;
						}
					} 
					else if (ai_Counter == 1)
                    {
						Vector2 dashVelocity = new Vector2(0, 1) * 16;
						npc.velocity = Vector2.Lerp(npc.velocity, dashVelocity, 0.5f);
						if(ai_Telegraph_Counter % 5 == 0)
                        {
							var longRift = Projectile.NewProjectileDirect(npc.GetSource_FromThis(), npc.Center, Vector2.Zero,
								ModContent.ProjectileType<VoidRift>(), 45, 1);

							longRift.timeLeft = 60*20;
							longRift.hostile = true;
							longRift.friendly = false;
							longRift.rotation = MathHelper.ToRadians(Main.rand.Next(0, 360));

							var xSlashPart1 = Projectile.NewProjectileDirect(npc.GetSource_FromThis(), npc.Center, Vector2.Zero,
								ModContent.ProjectileType<RipperSlashProjBig>(), 0, 0f);
							xSlashPart1.timeLeft = 900;

							(xSlashPart1.ModProjectile as RipperSlashProjBig).randomRotation = false;
							xSlashPart1.rotation = longRift.rotation + MathHelper.ToRadians(45);
						}

						if(ai_Telegraph_Counter > 60)
                        {
							ai_Counter = 0;
							ai_Telegraph_Counter = 0;
							_attack = AttackState.Idle;
						}
                    }

					break;

                case AttackState.Void_Bolts:
					float voidHoverSpeed = 2;
					float voidYVelocity = VectorHelper.Osc(1, -1, voidHoverSpeed);
					Vector2 hoverTarget = targetCenter + new Vector2(0, -96);
					Vector2 velocityToTarget = VectorHelper.VelocityDirectTo(npc.Center, hoverTarget, 5);
					npc.Center = Vector2.Lerp(npc.Center, hoverTarget, 0.075f);
					//npc.velocity = Vector2.Lerp(npc.velocity, velocityToTarget, 0.95f);

					if (ai_Telegraph_Counter == 0)
					{
						_voidBoltPositions = new Vector2[12];
						_quickSlashRotation = MathHelper.ToRadians(Main.rand.Next(-360, 360));

						float voidBoltRadius = 384;
						for(int i = 0; i < _voidBoltPositions.Length; i++)
                        {
							Vector2 edge = Main.rand.NextVector2CircularEdge(voidBoltRadius, voidBoltRadius);
							Vector2 voidBoltPosition = new Vector2(
								targetCenter.X + edge.X,
								targetCenter.Y + edge.Y);

							Particle telegraphPart1 = ParticleManager.NewParticle(voidBoltPosition, Vector2.Zero,
								ParticleManager.NewInstance<RipperSlashTelegraphParticle>(), default(Color), 1f);
					
							telegraphPart1.rotation = _quickSlashRotation;
							_voidBoltPositions[i] = voidBoltPosition;
                        }

						SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/RipperSlashTelegraph"));
					}
					

					ai_Telegraph_Counter++;
					if (ai_Telegraph_Counter > Phase1_Void_Bolts_Telegraph_Time)
					{
						ai_Telegraph_Counter = 0;
						ai_Counter = 0;
						for (int i = 0; i < _voidBoltPositions.Length; i++)
						{
							//Spawn teh void bolts
							Vector2 voidBoltPosition = _voidBoltPositions[i];
							Vector2 velocity = VectorHelper.VelocityDirectTo(voidBoltPosition, targetCenter, 2);
							int p =Projectile.NewProjectile(npc.GetSource_FromThis(), voidBoltPosition, velocity,
								ModContent.ProjectileType<VoidBolt>(), 30, 1);
							Main.projectile[p].timeLeft = Main.rand.Next(200, 300);

							//Slash effect
			
							var xSlashPart1 = Projectile.NewProjectileDirect(npc.GetSource_FromThis(), voidBoltPosition, Vector2.Zero,
								ModContent.ProjectileType<RipperSlashProjSmall>(), 0, 0f);
							xSlashPart1.timeLeft = 900;

							(xSlashPart1.ModProjectile as RipperSlashProjSmall).randomRotation = false;
							xSlashPart1.rotation = _quickSlashRotation + MathHelper.ToRadians(45);
						}

						_attack = AttackState.Idle;
					}

					break;
            }
        }

        #endregion

        #region Phase 2

        private float _p2TransitionAlpha=1f;

		private const int Phase2_Idle_Time = 300;
		private const int Phase2_X_Scissor_Telegraph_Time = RipperSlashTelegraphParticle.Animation_Length;
		private const int Phase2_Transition_Duration = 180;
		private const int Phase2_Quick_Slash_Telegraph_Time = RipperSlashTelegraphParticle.Animation_Length / 4;

		private void AI_Phase2MoveRightOfPlayer()
		{
			NPC npc = NPC;
			ref float ai_Telegraph_Counter = ref npc.ai[0];
			ref float ai_Counter = ref npc.ai[1];
			ref float ai_Cycle = ref npc.ai[2];

			Player target = Main.player[npc.target];
			Vector2 targetCenter = target.Center;

			float playerRightCenterOffsetX = 256;
			Vector2 playerRightCenterOffset = new Vector2(playerRightCenterOffsetX, 0);
			Vector2 playerRightCenter = targetCenter + playerRightCenterOffset;

			float distanceToTarget = Vector2.Distance(npc.Center, playerRightCenter);
			float slowdownDistance = 8;
			float distanceCorrector = 1;
			if (distanceToTarget < slowdownDistance)
			{
				distanceCorrector = distanceToTarget / slowdownDistance;
			}


			Vector2 playerRightVelocity = (playerRightCenter - npc.Center).SafeNormalize(Vector2.Zero) * 16 * distanceCorrector;
			npc.velocity = Vector2.Lerp(npc.velocity, playerRightVelocity, 0.1f);
		}

		private void AI_Phase2()
		{
			NPC npc = NPC;
			npc.TargetClosest();
			if (!NPC.HasValidTarget)
			{
				//Despawn in 2 seconds
				npc.velocity = Vector2.Lerp(npc.velocity, new Vector2(0, -8), 0.025f);
				npc.EncourageDespawn(120);
				DeActivateSyliaSky();
				return;
			}

			ref float ai_Telegraph_Counter = ref npc.ai[0];
			ref float ai_Counter = ref npc.ai[1];
			ref float ai_Cycle = ref npc.ai[2];

			Player target = Main.player[npc.target];
			Vector2 targetCenter = target.Center;
			Vector2 targetPosition = target.position;

			//Aight so what does Sylia do here?
			//Sylia continues using her phase 1 attacks, but they're faster and have slightly adjusted patterns making them harder to dodge.
			//She'll do more quick slashes, and the monster slashes will release more threatening monsters.
			//The edges of the arena are crawling with void monsters and cannot be touched, occassionally, Sylia will call forth monsters from the edge to assault you
			//This includes void eaters, big void eaters, void worms, and void tentacles (if I can figure out how to do these lol
			switch (_attack)
            {
                case AttackState.Transition:
					npc.velocity = Vector2.Lerp(npc.velocity, Vector2.Zero, 0.2f);
					ai_Telegraph_Counter++;
					float halfTransitionDuration = (float)Phase2_Transition_Duration / 2;
					float halfTransition = ai_Telegraph_Counter / halfTransitionDuration;

					//Move above the player after completely disappeared
					if(ai_Counter == 0)
					{
						//Need to invert it so it starts from 1 and goes to 0
						_p2TransitionAlpha = 1 - halfTransition;		
						if (halfTransition >= 1f)
						{
							//Actually move the npc
							float teleportCenterOffset = -128;
							Vector2 transitionTeleportCenter = targetCenter + new Vector2(0, teleportCenterOffset);
							npc.Center = transitionTeleportCenter;

							ai_Counter++;
							ai_Telegraph_Counter = 0;
						}
					}
					else if (ai_Counter == 1)
					{
						float teleportCenterOffset = -128;
						Vector2 transitionTeleportCenter = targetCenter + new Vector2(0, teleportCenterOffset);
						npc.Center = transitionTeleportCenter;

						//0 to 1, she re-appears and enters her idle state
						_p2TransitionAlpha = halfTransition;
						if (halfTransition >= 1f)
						{
							ai_Counter = 0;
							ai_Telegraph_Counter = 0;
							_attack = AttackState.Summon_Void_Wall;
						}
					}

					break;

				case AttackState.Summon_Void_Wall:
					if(ai_Counter == 0)
					{
						float xDistance = -512;
						float yDistance = 0;
						Vector2 summonVoidWallCenterOffset = new Vector2(xDistance, yDistance);
						Vector2 summonVoidWallCenter = targetCenter + summonVoidWallCenterOffset;
						npc.Center = Vector2.Lerp(npc.Center, summonVoidWallCenter, 0.025f);

						//This is just some delay before doing that huge slash
						ai_Telegraph_Counter++;
						if (ai_Telegraph_Counter > 60)
						{
							ai_Counter++;
							ai_Telegraph_Counter = 0;
						}
					}
					else if(ai_Counter == 1)
                    {
						//Vertical Dash
						Vector2 dashVelocity = new Vector2(0, 1) * 4;
						npc.velocity = Vector2.Lerp(npc.velocity, dashVelocity, 0.5f);
						if(ai_Telegraph_Counter == 30)
                        {
							Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, Vector2.Zero, ModContent.ProjectileType<VoidWall>(), 30, 2);
                        }

						ai_Telegraph_Counter++;
						if(ai_Telegraph_Counter > 60)
                        {
							ai_Counter = 0;
							ai_Telegraph_Counter = 0;
							_attack = AttackState.Idle;
                        }
					}

					break;

                case AttackState.Idle:
					AI_Phase2MoveRightOfPlayer();
					ai_Counter++;
					if (ai_Counter > Phase2_Idle_Time)
					{
						//Determine Attack
						ai_Counter = 0;
						ai_Telegraph_Counter = 0;

						//For now let's just always go into x slash
						//Ok we need to determine what attacks to do
						//Determien next attack
						//ermm
						//Let's do cycle for now?
						AI_DetermineAttack();
					}

					break;

				case AttackState.X_Slash:
					AI_Phase2MoveRightOfPlayer();
					//Telegraph
					if (ai_Telegraph_Counter == 0)
					{
						float yOffset = Main.rand.NextFloat(-256, 256);
						Vector2 xSlashOffset = new Vector2(0, yOffset);
						_slashCenter = targetCenter + xSlashOffset;

						//Spawn Scissor 1
						var scissorTelegraphPart1 = Projectile.NewProjectileDirect(npc.GetSource_FromThis(), npc.Center, Vector2.Zero,
							ModContent.ProjectileType<SyliaScissor>(), 0, 0);

						SyliaScissor syliaScissor1 = scissorTelegraphPart1.ModProjectile as SyliaScissor;
						syliaScissor1.startCenter = _slashCenter + new Vector2(-256, -256);
						syliaScissor1.targetCenter = _slashCenter;
						syliaScissor1.delay = Phase2_X_Scissor_Telegraph_Time - 8;

						//Spawn Scissor 2
						var scissorTelegraphPart2 = Projectile.NewProjectileDirect(npc.GetSource_FromThis(), npc.Center, Vector2.Zero,
							ModContent.ProjectileType<SyliaScissor>(), 0, 0);

						SyliaScissor syliaScissor2 = scissorTelegraphPart2.ModProjectile as SyliaScissor;
						syliaScissor2.startCenter = _slashCenter + new Vector2(256, -256);
						syliaScissor2.targetCenter = _slashCenter;
						syliaScissor2.delay = Phase2_X_Scissor_Telegraph_Time - 8;

						Particle telegraphPart1 = ParticleManager.NewParticle(_slashCenter, Vector2.Zero,
							ParticleManager.NewInstance<RipperSlashTelegraphParticle>(), default(Color), 1f);

						Particle telegraphPart2 = ParticleManager.NewParticle(_slashCenter, Vector2.Zero,
							ParticleManager.NewInstance<RipperSlashTelegraphParticle>(), default(Color), 1f);

						telegraphPart1.rotation = MathHelper.ToRadians(-45);
						telegraphPart2.rotation = MathHelper.ToRadians(45);
						SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/RipperSlashTelegraph"));
					}

					ai_Telegraph_Counter++;
					if (ai_Telegraph_Counter > Phase2_X_Scissor_Telegraph_Time)
					{
						ai_Telegraph_Counter = 0;

						//X Slash Visuals
						var xSlashPart1 = Projectile.NewProjectileDirect(npc.GetSource_FromThis(), _slashCenter, Vector2.Zero,
							ModContent.ProjectileType<RipperSlashProjBig>(), 0, 0f);
						var xSlashPart2 = Projectile.NewProjectileDirect(npc.GetSource_FromThis(), _slashCenter, Vector2.Zero,
							ModContent.ProjectileType<RipperSlashProjBig>(), 0, 0f);

						xSlashPart1.timeLeft = 1500;
						xSlashPart2.timeLeft = 1500;
						(xSlashPart1.ModProjectile as RipperSlashProjBig).randomRotation = false;
						(xSlashPart2.ModProjectile as RipperSlashProjBig).randomRotation = false;
						xSlashPart2.rotation = MathHelper.ToRadians(90);

						//Actual Attack Here
						var voidRiftProjectile1 = Projectile.NewProjectileDirect(npc.GetSource_FromThis(), _slashCenter, Vector2.Zero,
							ModContent.ProjectileType<VoidRift>(), 30, 1);

						voidRiftProjectile1.timeLeft = 600;
						voidRiftProjectile1.hostile = true;
						voidRiftProjectile1.friendly = false;
						voidRiftProjectile1.rotation = MathHelper.ToRadians(-45);

						var voidRiftProjectile2 = Projectile.NewProjectileDirect(npc.GetSource_FromThis(), _slashCenter, Vector2.Zero,
							ModContent.ProjectileType<VoidRift>(), 30, 1);

						voidRiftProjectile2.timeLeft = 600;
						voidRiftProjectile2.hostile = true;
						voidRiftProjectile2.friendly = false;
						voidRiftProjectile2.rotation = MathHelper.ToRadians(45);

						//Return to idle state after x slashing 3 times.
						//We could randomize the number of x slashes actually.
						ai_Counter++;
						if (ai_Counter > Main.rand.Next(5, 7))
						{
							ai_Counter = 0;
							_attack = AttackState.Idle;
						}
					}

					break;

                case AttackState.Quick_Slash:
					float quickSpeedMultiplier = ai_Telegraph_Counter / Phase2_Quick_Slash_Telegraph_Time;
					npc.velocity = VectorHelper.VelocityDirectTo(npc.Center, targetCenter, 8 * quickSpeedMultiplier);

					if (ai_Telegraph_Counter == 0)
					{
						_slashCenter = targetCenter;

						Particle telegraphPart1 = ParticleManager.NewParticle(_slashCenter, Vector2.Zero,
							ParticleManager.NewInstance<RipperSlashTelegraphParticle>(), default(Color), 1f);

						_quickSlashRotation = MathHelper.ToRadians(Main.rand.Next(0, 360));
						telegraphPart1.rotation = _quickSlashRotation;
						SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/RipperSlashTelegraph"));

						var scissorTelegraphPart1 = Projectile.NewProjectileDirect(npc.GetSource_FromThis(), npc.Center, Vector2.Zero,
							ModContent.ProjectileType<SyliaScissorSmall>(), 0, 0);

						var syliaScissor1 = scissorTelegraphPart1.ModProjectile as SyliaScissorSmall;
						syliaScissor1.startCenter = _slashCenter + new Vector2(256, 0).RotatedBy(_quickSlashRotation);
						syliaScissor1.targetCenter = _slashCenter;
						syliaScissor1.delay = Phase2_Quick_Slash_Telegraph_Time - 8;
					}

					ai_Telegraph_Counter++;
					if (ai_Telegraph_Counter > Phase2_Quick_Slash_Telegraph_Time)
					{
						ai_Telegraph_Counter = 0;
						var xSlashPart1 = Projectile.NewProjectileDirect(npc.GetSource_FromThis(), _slashCenter, Vector2.Zero,
							ModContent.ProjectileType<RipperSlashProjBig>(), 0, 0f);
						(xSlashPart1.ModProjectile as RipperSlashProjBig).randomRotation = false;

						var proj = Projectile.NewProjectileDirect(npc.GetSource_FromThis(), _slashCenter, Vector2.Zero,
							ModContent.ProjectileType<VoidSlash>(), 60, 1);

						xSlashPart1.timeLeft = 900;
						xSlashPart1.rotation = _quickSlashRotation + MathHelper.ToRadians(45);
						proj.rotation = _quickSlashRotation;

						ai_Counter++;
						if (ai_Counter > 11)
						{
							ai_Counter = 0;
							_attack = AttackState.Idle;
						}
					}
					break;
				case AttackState.Void_Bomb:
					AI_Phase2MoveRightOfPlayer();
					if (ai_Counter == 0)
					{
						int telegraphLength = 50;
						ai_Telegraph_Counter++;
						if (ai_Telegraph_Counter < telegraphLength)
						{
							for (int i = 0; i < 1; i++)
							{
								Vector2 position = NPC.Center + Main.rand.NextVector2CircularEdge(128, 128);
								Vector2 speed = (NPC.Center - position).SafeNormalize(Vector2.Zero) * 8;
								Particle p = ParticleManager.NewParticle(position, speed, ParticleManager.NewInstance<VoidParticle>(),
									default(Color), 0.5f);
								p.layer = Particle.Layer.BeforeProjectiles;
								Particle tearParticle = ParticleManager.NewParticle(position, Vector2.Zero, ParticleManager.NewInstance<VoidTearParticle>(),
								default(Color), 0.5f + 0.025f);

								tearParticle.layer = Particle.Layer.BeforePlayersBehindNPCs;
							}

						}
						if (ai_Telegraph_Counter == telegraphLength)
						{
							ai_Counter++;
						}
					}
					else if (ai_Counter == 1)
					{
						Vector2 position = NPC.Center + new Vector2(0, -64);
						Vector2 velocity = (targetCenter - position).SafeNormalize(Vector2.Zero) * 12;
						Projectile.NewProjectile(npc.GetSource_FromThis(), position, velocity,
							ModContent.ProjectileType<VoidBall>(), 20, 1);
						ai_Counter = 0;
						ai_Telegraph_Counter = 0;
						Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(position, 512f, 32f);
						SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/RipperSlash1"), position);
						_attack = AttackState.Idle;
					}
					break;
			}
        }

        #endregion

        private void AI_Phase3()
        {
			//Sylia gains an intense aura around her, she stops summoning void mosnters (though some will still leak in from the edges) and purely focuses on cutting you down
			//This would probably trigger at 20%, her attacks become fast and fierce.
			//The border of the arena gets smaller
        }

		public override void ModifyNPCLoot(NPCLoot npcLoot)
		{
			npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<SyliaBag>()));
			npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<Items.Placeable.SyliaBossRel>()));

			LeadingConditionRule notExpertRule = new LeadingConditionRule(new Conditions.NotExpert());
			//notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<LittleScissor>(), 1));
			notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<MiracleThread>(), minimumDropped: 30, maximumDropped: 40));
			notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<MiracleWings>(), chanceDenominator: 4));
			npcLoot.Add(notExpertRule);
		}

        public override void OnKill()
        {   
			DeActivateSyliaSky();
			NPC.SetEventFlagCleared(ref DownedBossSystem.downedSyliaBoss, -1);
		}
    }
}
