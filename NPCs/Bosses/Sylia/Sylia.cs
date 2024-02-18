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
using Stellamod.Trails;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.Sylia
{
    public class SyliaSkyPlayer : ModPlayer
    {
        public override void PostUpdateMiscEffects()
        {
            if (NPC.AnyNPCs(ModContent.NPCType<Sylia>()))
            {
				ActivateSyliaSky();

			}
            else
            {
				DeActivateSyliaSky();
            }
        }

		private void ActivateSyliaSky()
		{
			if (!SkyManager.Instance["Stellamod:SyliaSky"].IsActive())
			{
				Vector2 targetCenter = Player.Center;
				SkyManager.Instance.Activate("Stellamod:SyliaSky", targetCenter);
			}
		}

		private void DeActivateSyliaSky()
		{
			if (SkyManager.Instance["Stellamod:SyliaSky"].IsActive())
			{
				Vector2 targetCenter = Player.Center;
				SkyManager.Instance.Deactivate("Stellamod:SyliaSky", targetCenter);
			}
		}
	}

	[AutoloadBossHead]
	public class Sylia : ModNPC
	{
		private bool _spawned;
		private bool _doAuraVisuals;
		private bool _resetAI;
        private bool _telegraphXScissor;
        private float _telegraphQuickSlash = -1;
		private float _attackQuickSlash;
        private float _teleportX;
		private float _teleportY;


		private AttackState _lastAttack;
		private AttackState _attack = AttackState.Idle;
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
			Summon_Void_Wall,
			Dash_Slash,
			Horizontal_Slash
		}

		private enum Phase
		{
			Phase_1,
			Phase_2,
			Phase_3
		}

		private Vector2 _slashCenter;
		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[Type] = 30;
			NPCID.Sets.MPAllowedEnemies[NPC.type] = true;
			NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers();
			drawModifiers.CustomTexturePath = "Stellamod/NPCs/Bosses/Sylia/SyliaPreview";
			drawModifiers.PortraitScale = 0.8f; // Portrait refers to the full picture when clicking on the icon in the bestiary
			drawModifiers.PortraitPositionYOverride = 0f;
			NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
			NPCID.Sets.TrailCacheLength[Type] = 60;
			NPCID.Sets.TrailingMode[Type] = 2;
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			// Sets the description of this NPC that is listed in the bestiary
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> {
				new MoonLordPortraitBackgroundProviderBestiaryInfoElement(), // Plain black background
				new FlavorTextBestiaryInfoElement("Sylia, ")
			});
		}

		//AI Values
		public override void SetDefaults()
		{
			NPC.Size = new Vector2(24, 48);

			NPC.damage = 1;
			NPC.defense = 26;
			NPC.lifeMax = 20000;
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
				Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/Sylia");
			}
		}

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * balance);
        }

        public override void SendExtraAI(BinaryWriter writer)
		{
			writer.Write(_telegraphQuickSlash);
			writer.Write(_attackQuickSlash);

			writer.Write((float)_lastAttack);
            writer.Write((float)_attack);
            writer.Write((float)_attackPhase);

            writer.WriteVector2(_slashCenter);
			writer.Write(_resetAI);

			writer.Write(_teleportX);
			writer.Write(_teleportY);
			writer.Write(_telegraphXScissor);
		}

		public override void ReceiveExtraAI(BinaryReader reader)
		{
            _telegraphQuickSlash = reader.ReadSingle();
			_attackQuickSlash = reader.ReadSingle();

            _lastAttack = (AttackState)reader.ReadSingle();
            _attack = (AttackState)reader.ReadSingle();
            _attackPhase = (Phase)reader.ReadSingle();

            _slashCenter = reader.ReadVector2();
			_resetAI = reader.ReadBoolean();

            _teleportX = reader.ReadSingle();
			_teleportY = reader.ReadSingle();
			_telegraphXScissor = reader.ReadBoolean();
        }

		private void FinishQuickSlashTelegraph()
		{
			if(_telegraphQuickSlash != -1)
			{
                Particle telegraphPart1 = ParticleManager.NewParticle(_slashCenter, Vector2.Zero,
					ParticleManager.NewInstance<RipperSlashTelegraphParticle>(), default(Color), 1f);

                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/RipperSlashTelegraph"));
                if (StellaMultiplayer.IsHost)
                {
                    var scissorTelegraphPart1 = Projectile.NewProjectileDirect(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero,
                        ModContent.ProjectileType<SyliaScissorSmall>(), 0, 0, owner: Main.myPlayer);

                    var syliaScissor1 = scissorTelegraphPart1.ModProjectile as SyliaScissorSmall;
                    syliaScissor1.startCenter = _slashCenter + new Vector2(256, 0).RotatedBy(_telegraphQuickSlash);
                    syliaScissor1.targetCenter = _slashCenter;
                    syliaScissor1.delay = Phase1_Quick_Slash_Telegraph_Time - 8;
                }

                telegraphPart1.rotation = _telegraphQuickSlash;
                _telegraphQuickSlash = -1;
			}
		}

		private void FinishXScissorTelegraph()
		{
			if (_telegraphXScissor)
			{
				AI_XScissorTelegraph(_slashCenter);
				_telegraphXScissor = false;
			}
		}

		private void FinishResetAI()
		{
			if (_resetAI)
            {
                ai_Counter = 0;
                ai_Telegraph_Counter = 0;
                _resetAI = false;
			}
		}

        private void FinishTeleport()
        {
			if(_teleportX != 0 || _teleportY != 0)
			{
                NPC.position.X = _teleportX;
				NPC.position.Y = _teleportY;
                _teleportX = 0;
                _teleportY = 0;

                //Visuals on the teleport
                Dust.QuickDustLine(NPC.position, NPC.oldPosition, 100f, Color.Violet);
                for (int i = 0; i < 64; i++)
                {
                    Vector2 speed = Main.rand.NextVector2CircularEdge(4f, 4f);
                    Particle p = ParticleManager.NewParticle(NPC.Center, speed, ParticleManager.NewInstance<VoidParticle>(),
                        default(Color), 1 / 3f);
                    p.layer = Particle.Layer.BeforeProjectiles;
                }

                SoundEngine.PlaySound(SoundID.Item165);
            }
        }

        private void ResetAI()
		{
			if (StellaMultiplayer.IsHost)
			{
				_resetAI = true;
				NPC.netUpdate = true;
			}
		}



		private void Teleport(float x, float y)
		{
			if (StellaMultiplayer.IsHost)
			{
				_teleportX = x;
				_teleportY = y;
				NPC.netUpdate = true;
			}
		}

		private float _spawnAlpha;
		private void AI_Spawn()
		{
			NPC npc = NPC;
			npc.TargetClosest();

			if (ai_Counter == 0)
			{
				SoundEngine.PlaySound(SoundID.Item119);
			}
			if (ai_Counter > 30 && npc.HasValidTarget)
			{
				//Hover above player
				Player target = Main.player[npc.target];
				Vector2 targetCenter = target.Center;
				Vector2 targetHoverCenter = targetCenter + new Vector2(0, -256);
				npc.Center = Vector2.Lerp(npc.Center, targetHoverCenter, 0.25f);
				_spawnAlpha = MathHelper.Lerp(0, 1, ((ai_Counter - 30) / 270));
			}
			if (ai_Counter > 120 && ai_Counter < 240)
			{
				for (int i = 0; i < 4; i++)
				{
					Vector2 position = NPC.Center + Main.rand.NextVector2CircularEdge(256, 256);
					Vector2 speed = (NPC.Center - position).SafeNormalize(Vector2.Zero) * 16;
					Particle p = ParticleManager.NewParticle(position, speed, ParticleManager.NewInstance<VoidParticle>(),
						default(Color), 1 / 3f);
					p.layer = Particle.Layer.BeforeProjectiles;
				}
			}
			if (ai_Counter == 300)
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

		public float Random360Degrees => MathHelper.ToRadians(Main.rand.Next(0, 360));


		public override void AI()
		{
			//Spawning Animation
			NPC.damage = 0;
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
				for (int i = 0; i < Main.maxProjectiles; i++)
				{
					var proj = Main.projectile[i];
					if (proj.hostile && proj.type == ModContent.ProjectileType<VoidHostileRift>())
					{
						proj.timeLeft = 20;
					}
				}

				SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/SyliaTransition"));
				_attackPhase = Phase.Phase_2;
				_attack = AttackState.Transition;
			}

            //No Contact Damage SmH
            

			//We do everything the frame after so that it smoothly spawns on the client
			FinishResetAI();
			FinishTeleport();
			FinishXScissorTelegraph();
			FinishQuickSlashTelegraph();

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
				Vector2 rotatedPos = drawPos + new Vector2(0f, 16f * rotationOffset).RotatedBy(radians) * time;
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

		//Trailing

		public PrimDrawer TrailDrawer { get; private set; } = null;

		public float WidthFunction(float completionRatio)
		{
			float baseWidth = NPC.scale * NPC.width;
			return MathHelper.SmoothStep(baseWidth, 3.5f, completionRatio);
		}

		public Color ColorFunction(float completionRatio)
		{
			return Color.Lerp(new Color(60, 0, 118), Color.Transparent, completionRatio);
		}

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

			if (_attackPhase == Phase.Phase_2)
			{
				TrailDrawer ??= new PrimDrawer(WidthFunction, ColorFunction, GameShaders.Misc["VampKnives:BasicTrail"]);
				GameShaders.Misc["VampKnives:BasicTrail"].SetShaderTexture(TrailRegistry.VortexTrail);
				TrailDrawer.DrawPrims(NPC.oldPos, NPC.Size * 0.5f - Main.screenPosition, 155);
			}

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

        #region Attack Cycle
        private void AI_SwitchState(AttackState attackState)
        {
			ResetAI();
			_attack = attackState;
			if(_attack != AttackState.Idle)
				_lastAttack = _attack;
			NPC.netUpdate = true;
        }

        private void AI_DetermineAttack()
		{
			//Just on a pattern for now
			//Might change it to have some randomized parts.
			if (StellaMultiplayer.IsHost)
			{
				switch (_attackPhase)
				{
					case Phase.Phase_1:
						switch (_lastAttack)
						{
							default:
                                AI_SwitchState(AttackState.X_Slash);
								break;
							case AttackState.X_Slash:
								switch (Main.rand.Next(2))
								{
									case 0:
										if (Main.rand.NextBool(2))
										{
                                            AI_SwitchState(AttackState.Void_Bolts);
										}
										else
										{
                                            AI_SwitchState(AttackState.Void_Bomb);
										}

										break;
									case 1:
                                        AI_SwitchState(AttackState.Quick_Slash);
										break;
								}
								break;
							case AttackState.Quick_Slash:
								switch (Main.rand.Next(2))
								{
									case 0:
                                        AI_SwitchState(AttackState.Void_Bolts);
										break;
									case 1:
                                        AI_SwitchState(AttackState.X_Slash);
										break;
								}
								break;
							case AttackState.Quick_Slash_V2:
                                AI_SwitchState(AttackState.X_Slash);
								break;
							case AttackState.Void_Bolts:
                                AI_SwitchState(AttackState.Quick_Slash_V2);
								break;
						}
						break;

					case Phase.Phase_2:
						switch (_lastAttack)
						{
							default:
                                AI_SwitchState(AttackState.X_Slash);
								break;
							case AttackState.X_Slash:
								switch (Main.rand.Next(2))
								{
									case 0:
                                        AI_SwitchState(AttackState.Void_Bomb);
										break;
									case 1:
                                        AI_SwitchState(AttackState.Quick_Slash);
										break;
								}
								break;
							case AttackState.Void_Bomb:
							case AttackState.Quick_Slash:
								switch (Main.rand.Next(2))
								{
									case 0:
                                        AI_SwitchState(AttackState.Horizontal_Slash);
										break;
									case 1:
                                        AI_SwitchState(AttackState.Dash_Slash);
										break;
								}

								break;
						}
						break;
				}
			}
		}
		#endregion

		#region Phase 1

		private const int Phase1_Idle_Time = 60;
		private const int Phase1_X_Scissor_Telegraph_Time = RipperSlashTelegraphParticle.Animation_Length;
		private const int Phase1_Void_Bolts_Telegraph_Time = RipperSlashTelegraphParticle.Animation_Length;
		private const int Phase1_Quick_Slash_Telegraph_Time = RipperSlashTelegraphParticle.Animation_Length / 3;

		//Attack Values
		private const float Attack_Void_Bolt_Circle_Radius = 384;
		private const int Attack_Void_Bolt_Circle_Count = 10;


		private ref float ai_Telegraph_Counter => ref NPC.ai[0];
		private ref float ai_Counter => ref NPC.ai[1];

		private void AI_Movement(Vector2 targetCenter, float moveSpeed, float accel =1f)
		{
			//This code should give quite interesting movement

			//Accelerate to being on top of the player
			float distX = targetCenter.X - NPC.Center.X;
			if(NPC.Center.X < targetCenter.X && NPC.velocity.X < moveSpeed && distX > 0)
			{
                NPC.velocity.X += accel;
                if (NPC.velocity.X > distX)
                    NPC.velocity.X = distX;
            } 
			else if (NPC.Center.X > targetCenter.X && NPC.velocity.X > -moveSpeed && distX < 0)
            {
                NPC.velocity.X -= accel;
                if (NPC.velocity.X < distX)
                    NPC.velocity.X = distX;
            }

			//Accelerate to being above the player.
			Vector2 targetFloatCenter = targetCenter + new Vector2(0, -200);
            float distY = targetFloatCenter.Y - NPC.Center.Y;
            if (NPC.Center.Y < targetFloatCenter.Y && NPC.velocity.Y < moveSpeed)
            {
                NPC.velocity.Y += accel;
                if (NPC.velocity.Y > distY)
                    NPC.velocity.Y = distY;
            }
            else if (NPC.Center.Y > targetFloatCenter.Y && NPC.velocity.Y > -moveSpeed)
            {
                NPC.velocity.Y -= accel;
                if (NPC.velocity.Y < distY)
                    NPC.velocity.Y = distY;
            }

            NPC.rotation = NPC.velocity.X * 0.1f;
		}

        private void AI_VoidBoltCircleTelegraph(Vector2 targetCenter)
		{
            for (int i = 0; i < Attack_Void_Bolt_Circle_Count; i++)
            {
                float radians = (MathHelper.TwoPi / Attack_Void_Bolt_Circle_Count) * i;
				Vector2 edge = targetCenter + Vector2.One.RotatedBy(radians) * Attack_Void_Bolt_Circle_Radius;
                for (int j = 0; j < 8; j++)
                {
                    Vector2 speed = Main.rand.NextVector2CircularEdge(4f, 4f);
                    var dust = Dust.NewDustPerfect(edge, DustID.GemAmethyst, speed, Scale: 1f);
                    dust.noGravity = true;
                }
            }
        }

		private void AI_VoidBoltCircle(Vector2 targetCenter)
		{
            for (int i = 0; i < Attack_Void_Bolt_Circle_Count; i++)
            {
                float radians = (MathHelper.TwoPi / Attack_Void_Bolt_Circle_Count) * i;
                Vector2 edge = targetCenter + Vector2.One.RotatedBy(radians) * Attack_Void_Bolt_Circle_Radius;
                Vector2 velocity = edge.DirectionTo(targetCenter) * 2;
                if (StellaMultiplayer.IsHost)
                {
                    int p = Projectile.NewProjectile(NPC.GetSource_FromThis(), edge, velocity,
                        ModContent.ProjectileType<VoidBolt>(), 30, 1, Owner: Main.myPlayer);
                    Main.projectile[p].timeLeft = Main.rand.Next(200, 300);
   
                }

                for (int j = 0; j < 8; j++)
                {
                    Vector2 speed = Main.rand.NextVector2CircularEdge(4f, 4f);
                    var dust = Dust.NewDustPerfect(edge, DustID.GemAmethyst, speed, Scale: 1f);
                    dust.noGravity = true;
                }
            }
        }

		private void AI_XScissorTelegraph(Vector2 targetCenter)
		{
			float scissorOffset = 256;
			int moveOffset = 8;
            if (StellaMultiplayer.IsHost)
            {
                //Spawn Scissor 1
                var scissorTelegraphPart1 = Projectile.NewProjectileDirect(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero,
                 ModContent.ProjectileType<SyliaScissor>(), 0, 0, owner: Main.myPlayer);

                SyliaScissor syliaScissor1 = scissorTelegraphPart1.ModProjectile as SyliaScissor;
                syliaScissor1.startCenter = targetCenter + new Vector2(-scissorOffset, -scissorOffset);
                syliaScissor1.targetCenter = targetCenter;
                syliaScissor1.delay = Phase1_X_Scissor_Telegraph_Time - moveOffset;

                //Spawn Scissor 2
                var scissorTelegraphPart2 = Projectile.NewProjectileDirect(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero,
                    ModContent.ProjectileType<SyliaScissor>(), 0, 0, owner: Main.myPlayer);

                SyliaScissor syliaScissor2 = scissorTelegraphPart2.ModProjectile as SyliaScissor;
                syliaScissor2.startCenter = targetCenter + new Vector2(scissorOffset, -scissorOffset);
                syliaScissor2.targetCenter = targetCenter;
                syliaScissor2.delay = Phase1_X_Scissor_Telegraph_Time - moveOffset;
            }

            //Spawn Ripper Particles
            Particle telegraphPart1 = ParticleManager.NewParticle(targetCenter, Vector2.Zero,
                ParticleManager.NewInstance<RipperSlashTelegraphParticle>(), Color.White, 1f);

            Particle telegraphPart2 = ParticleManager.NewParticle(targetCenter, Vector2.Zero,
                ParticleManager.NewInstance<RipperSlashTelegraphParticle>(), Color.White, 1f);

            telegraphPart1.rotation = MathHelper.ToRadians(-45);
            telegraphPart2.rotation = MathHelper.ToRadians(45);
            SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/RipperSlashTelegraph"));
        }

		private void AI_XScissorAttack(Vector2 targetCenter)
		{
            if (StellaMultiplayer.IsHost)
            {
                float riftRotation = MathHelper.ToRadians(-45);
                //X Slash Visuals
                var xSlashPart1 = Projectile.NewProjectileDirect(NPC.GetSource_FromThis(), targetCenter, Vector2.Zero,
                    ModContent.ProjectileType<RipperSlashProjBig>(), 0, 0f, owner: Main.myPlayer, 
					ai1: 0);
                var xSlashPart2 = Projectile.NewProjectileDirect(NPC.GetSource_FromThis(), targetCenter, Vector2.Zero,
                    ModContent.ProjectileType<RipperSlashProjBig>(), 0, 0f, owner: Main.myPlayer, 
					ai1: MathHelper.ToRadians(90));

                xSlashPart1.timeLeft = 1500;
                xSlashPart2.timeLeft = 1500;

                //Actual Attack Here
                var voidRiftProjectile1 = Projectile.NewProjectileDirect(NPC.GetSource_FromThis(), targetCenter, Vector2.Zero,
                    ModContent.ProjectileType<VoidHostileRift>(), 30, 1, owner: Main.myPlayer, 
					ai0: riftRotation);

                voidRiftProjectile1.timeLeft = 600;

				riftRotation = MathHelper.ToRadians(45);
                var voidRiftProjectile2 = Projectile.NewProjectileDirect(NPC.GetSource_FromThis(), targetCenter, Vector2.Zero,
                    ModContent.ProjectileType<VoidHostileRift>(), 30, 1, owner: Main.myPlayer, 
					ai0: riftRotation);

                voidRiftProjectile2.timeLeft = 600;
                int p = Projectile.NewProjectile(NPC.GetSource_FromThis(), targetCenter, Vector2.Zero,
                    ModContent.ProjectileType<VoidBolt>(), 30, 1, Owner: Main.myPlayer);
                Main.projectile[p].timeLeft = 300;
            }
        }

		private void AI_QuickSlashV2Attack(Vector2 targetCenter)
		{
            if (StellaMultiplayer.IsHost)
            {
                var xSlashPart1 = Projectile.NewProjectileDirect(NPC.GetSource_FromThis(), targetCenter, Vector2.Zero,
                    ModContent.ProjectileType<RipperSlashProjBig>(), 0, 0f, owner: Main.myPlayer, ai1: _attackQuickSlash + MathHelper.ToRadians(45));
                xSlashPart1.timeLeft = 900;

                var proj = Projectile.NewProjectileDirect(NPC.GetSource_FromThis(), targetCenter, Vector2.Zero,
                    ModContent.ProjectileType<VoidSlash>(), 60, 1, owner: Main.myPlayer);
                proj.rotation = _attackQuickSlash;
            }
        }

        private void AI_Phase1()
		{
			NPC npc = NPC;
			npc.TargetClosest();
			if (!NPC.HasValidTarget)
			{
				//Despawn in 2 seconds
				npc.velocity = Vector2.Lerp(npc.velocity, new Vector2(0, -8), 0.025f);
				npc.EncourageDespawn(60);
				return;
			}

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
                    AI_Movement(targetCenter, 21);
					ai_Counter++;
					if(ai_Counter > Phase1_Idle_Time)
                    {
						//Determine Attack
						AI_DetermineAttack();
					}

					break;

				case AttackState.X_Slash:		
					//Telegraph
					if (ai_Telegraph_Counter == 0 && StellaMultiplayer.IsHost)
                    {
						Vector2 targetVelocity = target.velocity;
						Vector2 targetOffset = targetVelocity.SafeNormalize(Vector2.Zero) * 16*24;
						_slashCenter = targetCenter + targetOffset;
						_telegraphXScissor = true;
						NPC.netUpdate = true;
                    }

                    AI_Movement(_slashCenter, 12);
                    ai_Telegraph_Counter++;
					if(ai_Telegraph_Counter > Phase1_X_Scissor_Telegraph_Time)
					{
						ai_Telegraph_Counter = 0;
						AI_XScissorAttack(_slashCenter);
						SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/SyliaRiftOpen"));

						//Return to idle state after x slashing 3 times.
						//We could randomize the number of x slashes actually.
						ai_Counter++;
						if (ai_Counter > 3)
                        {
                            //Switch State automatically resets the AI
                            AI_SwitchState(AttackState.Idle);
						}
                    }

					break;

				case AttackState.Quick_Slash:
					float quickSpeedMultiplier = ai_Telegraph_Counter / Phase1_Quick_Slash_Telegraph_Time;
					npc.velocity = VectorHelper.VelocityDirectTo(npc.Center, targetCenter, 8 * quickSpeedMultiplier);

					if (ai_Telegraph_Counter == 0)
					{
						if (StellaMultiplayer.IsHost)
                        {
                            _slashCenter = targetCenter;
                            _attackQuickSlash = _telegraphQuickSlash = MathHelper.ToRadians(Main.rand.Next(0, 360));
							NPC.netUpdate = true;
                        }
                    }

					ai_Telegraph_Counter++;
					if (ai_Telegraph_Counter > Phase1_Quick_Slash_Telegraph_Time)
					{
						ai_Telegraph_Counter = 0;
						if (StellaMultiplayer.IsHost)
						{
                            var xSlashPart1 = Projectile.NewProjectileDirect(npc.GetSource_FromThis(), _slashCenter, Vector2.Zero,
                                    ModContent.ProjectileType<RipperSlashProjBig>(), 0, 0f, owner: Main.myPlayer, 
									ai1: _attackQuickSlash + MathHelper.ToRadians(45));
  
                            var proj = Projectile.NewProjectileDirect(npc.GetSource_FromThis(), _slashCenter, Vector2.Zero,
                                ModContent.ProjectileType<VoidSlash>(), 60, 1, owner: Main.myPlayer);

                            xSlashPart1.timeLeft = 900;
                            proj.rotation = _attackQuickSlash;
                        }

						ai_Counter++;
						if (ai_Counter > 9)
						{
                            //Switch State automatically resets the AI
                            AI_SwitchState(AttackState.Idle);
                        }
					}

					break;

                case AttackState.Quick_Slash_V2:
					Vector2 targetCircleCenter = targetCenter + new Vector2(256, 0).RotatedBy(MathHelper.ToRadians((ai_Counter * (360/15))));
                    npc.velocity = VectorHelper.VelocitySlowdownTo(npc.Center, targetCircleCenter, 6);

                    if (ai_Telegraph_Counter == 0)
					{						
						_slashCenter = targetCenter;
                        _attackQuickSlash = _telegraphQuickSlash = MathHelper.ToRadians(Main.rand.Next(0, 360));
                    }

					ai_Telegraph_Counter++;
					if (ai_Telegraph_Counter > Phase1_Quick_Slash_Telegraph_Time / 2)
					{
						ai_Telegraph_Counter = 0;
						AI_QuickSlashV2Attack(_slashCenter);
						ai_Counter++;
						//Every third slash, teleport, we ned to keep this boss moving around.
						if(ai_Counter % 3 == 0)
						{
							if (StellaMultiplayer.IsHost)
                            {
                                float teleportRadius = 256;
                                Vector2 teleportPosition = new Vector2(
									targetCenter.X + Main.rand.NextFloat(-teleportRadius, teleportRadius),
									targetCenter.Y + Main.rand.NextFloat(-teleportRadius, teleportRadius));
								Teleport(teleportPosition.X, teleportPosition.Y);
                            }
						}

						if (ai_Counter > 15)
						{
                            //Switch State automatically resets the AI
                            AI_SwitchState(AttackState.Vertical_Slash);
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
								float distance = 128;
								float particleSpeed = 8;
								Vector2 position = NPC.Center + Main.rand.NextVector2CircularEdge(distance, distance);
								Vector2 speed = (NPC.Center - position).SafeNormalize(Vector2.Zero) * particleSpeed;
								Particle p = ParticleManager.NewParticle(position, speed, ParticleManager.NewInstance<VoidParticle>(), default(Color), 0.5f);
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
						if (StellaMultiplayer.IsHost)
						{
                            Projectile.NewProjectile(npc.GetSource_FromThis(), position, velocity,
                              ModContent.ProjectileType<VoidBall>(), 20, 1, Owner: Main.myPlayer);
                        }
					
						ai_Counter = 0;
						ai_Telegraph_Counter = 0;
						Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(position, 512f, 32f);
						SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/RipperSlash1"), position);
                        AI_SwitchState(AttackState.Idle);
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
						npc.velocity = VectorHelper.VelocitySlowdownTo(npc.Center, verticalHoverTarget, 12);

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
							if (StellaMultiplayer.IsHost)
							{
								float randomRotation = MathHelper.ToRadians(Main.rand.Next(0, 360));
                                var longRift = Projectile.NewProjectileDirect(npc.GetSource_FromThis(), npc.Center, Vector2.Zero,
                                    ModContent.ProjectileType<VoidHostileRift>(), 45, 1, owner: Main.myPlayer, 
									ai0: randomRotation);
                                longRift.timeLeft = 1200;

                                var xSlashPart1 = Projectile.NewProjectileDirect(npc.GetSource_FromThis(), npc.Center, Vector2.Zero,
                                    ModContent.ProjectileType<RipperSlashProjBig>(), 0, 0f, owner: Main.myPlayer,  
									ai1: randomRotation + MathHelper.ToRadians(45));
                                xSlashPart1.timeLeft = 900;
                            }	
						}

						if(ai_Telegraph_Counter > 60)
                        {
							ai_Counter++;
							ai_Telegraph_Counter = 0;
						}
                    } 
					else if (ai_Counter == 2)
					{
						float xVelocity = npc.DirectionTo(targetCenter).X;
						float lerpAmount = 0.5f * Math.Clamp(ai_Telegraph_Counter / 30, 0, 1);
						Vector2 dashVelocity = new Vector2(xVelocity, -0.75f) * 16;
						npc.velocity = Vector2.Lerp(npc.velocity, dashVelocity, lerpAmount);
						if (ai_Telegraph_Counter > 60)
						{
							//Switch State automatically resets the AI
                            AI_SwitchState(AttackState.Idle);
                        }
					}

					break;

                case AttackState.Void_Bolts:
                    float voidHoverSpeed = 2;
					float voidYVelocity = VectorHelper.Osc(1, -1, voidHoverSpeed);
					Vector2 hoverTarget = targetCenter + new Vector2(0, -96);
					npc.velocity = VectorHelper.VelocitySlowdownTo(npc.Center, hoverTarget, 5);
	
					if (ai_Telegraph_Counter == 0)
					{
						AI_VoidBoltCircleTelegraph(targetCenter);
						SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/RipperSlashTelegraph"));
					}
					
					ai_Telegraph_Counter++;
					if (ai_Telegraph_Counter > Phase1_Void_Bolts_Telegraph_Time)
					{
                        //Switch State automatically resets the AI
                        AI_VoidBoltCircle(targetCenter);
                        AI_SwitchState(AttackState.Idle);
                    }

					break;
            }
        }

        #endregion

        #region Phase 2

        private float _p2TransitionAlpha=1f;

		private const int Phase2_Idle_Time = 240;
		private const int Phase2_X_Scissor_Telegraph_Time = RipperSlashTelegraphParticle.Animation_Length;
		private const int Phase2_Transition_Duration = 180;
		private const int Phase2_Quick_Slash_Telegraph_Time = RipperSlashTelegraphParticle.Animation_Length / 4;

		private void AI_Phase2MoveRightOfPlayer(float offset =0)
		{
			NPC npc = NPC;
			ref float ai_Telegraph_Counter = ref npc.ai[0];
			ref float ai_Counter = ref npc.ai[1];
			ref float ai_Cycle = ref npc.ai[2];

			Player target = Main.player[npc.target];
			Vector2 targetCenter = target.Center;

			float playerRightCenterOffsetX = 256+offset;
			Vector2 playerRightCenterOffset = new Vector2(playerRightCenterOffsetX, 0);
			Vector2 playerRightCenter = targetCenter + playerRightCenterOffset;

			float distanceToTarget = Vector2.Distance(npc.Center, playerRightCenter);
			float slowdownDistance = 8;
			float distanceCorrector = 1;
			if (distanceToTarget < slowdownDistance)
			{
				distanceCorrector = distanceToTarget / slowdownDistance;
			}

			Vector2 playerRightVelocity = npc.Center.DirectionTo(playerRightCenter) * 16 * distanceCorrector;
			npc.velocity = Vector2.Lerp(npc.velocity, playerRightVelocity, 0.1f);
		}

		private void AuraVisuals()        
		{
			if (!_doAuraVisuals)
				return;
			for (int m = 0; m < 2; m++)
			{
				Vector2 position = NPC.RandomPositionWithinEntity();
				Particle p = ParticleManager.NewParticle(position, new Vector2(0, -2f), ParticleManager.NewInstance<VoidParticle>(),
					default(Color), Main.rand.NextFloat(0.2f, 0.8f));
				p.layer = Particle.Layer.BeforePlayersBehindNPCs;
				if (Main.rand.NextBool(6))
                {
					Dust.NewDustPerfect(NPC.RandomPositionWithinEntity(), DustID.GemAmethyst, Scale: Main.rand.NextFloat(0.2f, 0.8f));
				}
			}
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
				return;
			}

			ref float ai_Telegraph_Counter = ref npc.ai[0];
			ref float ai_Counter = ref npc.ai[1];
			ref float ai_Cycle = ref npc.ai[2];

			Player target = Main.player[npc.target];
			Vector2 targetCenter = target.Center;
			Vector2 targetPosition = target.position;
			AuraVisuals();

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
							SoundEngine.PlaySound(SoundID.DD2_BetsySummon);
							_doAuraVisuals = true;
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
                            //Switch State automatically resets the AI
                            AI_SwitchState(AttackState.Summon_Void_Wall);
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
							var entitySource = NPC.GetSource_FromThis();
                            SoundEngine.PlaySound(SoundID.NPCDeath62);
                            Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(targetCenter, 2048f, 128f);

							if (StellaMultiplayer.IsHost)
							{
                                NPC.NewNPC(entitySource, (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<VoidWall>());
                                Projectile.NewProjectile(npc.GetSource_FromThis(), NPC.Center, Vector2.Zero,
                                    ModContent.ProjectileType<RipperSlashProjBig>(), 0, 0f, Owner: Main.myPlayer, ai1: Random360Degrees);
                                Projectile.NewProjectile(npc.GetSource_FromThis(), NPC.Center, Vector2.Zero,
                                    ModContent.ProjectileType<RipperSlashProjBig>(), 0, 0f, Owner: Main.myPlayer, ai1: Random360Degrees);
                            }
						}

						ai_Telegraph_Counter++;
						if(ai_Telegraph_Counter > 60)
                        {
							//Switch State automatically resets the AI
                            AI_SwitchState(AttackState.Idle);
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
                        Vector2 targetVelocity = target.velocity;
                        Vector2 targetOffset = targetVelocity.SafeNormalize(Vector2.Zero) * 16 * 24;
                        _slashCenter = targetCenter + targetOffset;
                        AI_XScissorTelegraph(_slashCenter);

                    }

					ai_Telegraph_Counter++;
					if (ai_Telegraph_Counter > Phase2_X_Scissor_Telegraph_Time)
					{
						ai_Telegraph_Counter = 0;
                        AI_XScissorAttack(_slashCenter);
                        SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/SyliaRiftOpen"));

                        //Return to idle state after x slashing 3 times.
                        //We could randomize the number of x slashes actually.
                        ai_Counter++;
						if (ai_Counter > 6)
						{
                            AI_SwitchState(AttackState.Idle);
                        }
					}

					break;

                case AttackState.Quick_Slash:
					float quickSpeedMultiplier = ai_Telegraph_Counter / Phase2_Quick_Slash_Telegraph_Time;
					npc.velocity = VectorHelper.VelocityDirectTo(npc.Center, targetCenter, 8 * quickSpeedMultiplier);

					if (ai_Telegraph_Counter == 0)
					{ 			
						if (StellaMultiplayer.IsHost)
                        {
                            _slashCenter = targetCenter;
                            _attackQuickSlash = _telegraphQuickSlash = MathHelper.ToRadians(Main.rand.Next(0, 360));
							npc.netUpdate = true;
                        }
                    }

					ai_Telegraph_Counter++;
					if (ai_Telegraph_Counter > Phase2_Quick_Slash_Telegraph_Time)
					{
						ai_Telegraph_Counter = 0;
						if (StellaMultiplayer.IsHost)
						{
                            var xSlashPart1 = Projectile.NewProjectileDirect(npc.GetSource_FromThis(), _slashCenter, Vector2.Zero,
                                ModContent.ProjectileType<RipperSlashProjBig>(), 0, 0f, owner: Main.myPlayer, 
								ai1: _attackQuickSlash + MathHelper.ToRadians(45));
    
                            var proj = Projectile.NewProjectileDirect(npc.GetSource_FromThis(), _slashCenter, Vector2.Zero,
                                ModContent.ProjectileType<VoidSlash>(), 60, 1, owner: Main.myPlayer);

                            xSlashPart1.timeLeft = 900;
                            proj.rotation = _attackQuickSlash;
                        }

						ai_Counter++;
						if (ai_Counter > 11)
						{
                            AI_SwitchState(AttackState.Idle);
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
                        ai_Counter = 0;
                        ai_Telegraph_Counter = 0;

                        Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(position, 512f, 32f);
                        SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/RipperSlash1"), position);

						if (StellaMultiplayer.IsHost)
						{
                            Projectile.NewProjectile(npc.GetSource_FromThis(), position, velocity,
                                ModContent.ProjectileType<VoidBall>(), 20, 1, Owner: Main.myPlayer);
                        }

                        AI_SwitchState(AttackState.Idle);
                    }
					break;
				case AttackState.Dash_Slash:
					ai_Telegraph_Counter++;
					if (ai_Counter == 0)
					{
						AI_Phase2MoveRightOfPlayer(128);
						//Delay before she slashes
						if (ai_Telegraph_Counter > 120)
						{
							ai_Counter++;
							ai_Telegraph_Counter = 0;
						}
					}
					else if (ai_Counter == 1)
					{
						Vector2 dashVelocity = new Vector2(-1, 0) * 16;
						npc.velocity = Vector2.Lerp(npc.velocity, dashVelocity, 0.5f);
						if (ai_Telegraph_Counter % 5 == 0)
						{
							if (StellaMultiplayer.IsHost)
							{
								float longRiftRotation = MathHelper.ToRadians(Main.rand.Next(0, 360));
								var longRift = Projectile.NewProjectileDirect(npc.GetSource_FromThis(), npc.Center, Vector2.Zero,
									ModContent.ProjectileType<VoidHostileRift>(), 45, 1, owner: Main.myPlayer, 
									ai0: longRiftRotation);
                                longRift.timeLeft = 60 * 20;

                                var xSlashPart1 = Projectile.NewProjectileDirect(npc.GetSource_FromThis(), npc.Center, Vector2.Zero,
                                    ModContent.ProjectileType<RipperSlashProjBig>(), 0, 0f, owner: Main.myPlayer, 
									ai1: longRiftRotation + MathHelper.ToRadians(45));
                                xSlashPart1.timeLeft = 900;
                            }
						}

						if (ai_Telegraph_Counter > 30)
						{
                            AI_SwitchState(AttackState.Idle);
                        }
					}

					break;

                case AttackState.Horizontal_Slash:
					ai_Telegraph_Counter++;
					if (ai_Counter == 0)
					{
						_slashCenter = targetCenter;

						float xOffset = 768;
						float yOffset = -512;

						Vector2 verticalHoverTarget = _slashCenter + new Vector2(xOffset, yOffset);
						npc.velocity = VectorHelper.VelocitySlowdownTo(npc.position, verticalHoverTarget, 10 * (ai_Telegraph_Counter / 60));

						//Delay before she slashes
						if (ai_Telegraph_Counter > 60)
						{
							ai_Counter++;
							ai_Telegraph_Counter = 0;
						}
					}
					else if (ai_Counter == 1)
					{
						Vector2 dashVelocity = new Vector2(1, 0) * 16;
						npc.velocity = Vector2.Lerp(npc.velocity, dashVelocity, 0.5f);
						if (ai_Telegraph_Counter % 5 == 0)
						{
							if (StellaMultiplayer.IsHost)
							{
								float riftRotation = MathHelper.ToRadians(Main.rand.Next(0, 360));
                                var longRift = Projectile.NewProjectileDirect(npc.GetSource_FromThis(), npc.Center, Vector2.Zero,
									ModContent.ProjectileType<VoidHorizontalRift>(), 45, 1, owner: Main.myPlayer, 
									ai0: riftRotation);

                                longRift.timeLeft = 60 * 20;
     

                                var xSlashPart1 = Projectile.NewProjectileDirect(npc.GetSource_FromThis(), npc.Center, Vector2.Zero,
                                    ModContent.ProjectileType<RipperSlashProjBig>(), 0, 0f, owner: Main.myPlayer, 
									ai1: riftRotation + MathHelper.ToRadians(45));
                                xSlashPart1.timeLeft = 900;
                            }
						}

						if (ai_Telegraph_Counter > 75)
						{
                            AI_SwitchState(AttackState.Idle);
                        }
					}

					break;
            }
        }

        #endregion

		public override void ModifyNPCLoot(NPCLoot npcLoot)
		{
			npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<SyliaBag>()));
			npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<Items.Placeable.SyliaBossRel>()));

			LeadingConditionRule notExpertRule = new LeadingConditionRule(new Conditions.NotExpert());
			notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<MiracleThread>(), minimumDropped: 30, maximumDropped: 40));
			notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<MiracleWings>(), chanceDenominator: 4));
			npcLoot.Add(notExpertRule);
		}

        public override void OnKill()
        {   
			NPC.SetEventFlagCleared(ref DownedBossSystem.downedSyliaBoss, -1);
		}
    }
}
