using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Buffs;
using Stellamod.Helpers;
using Stellamod.Items.Accessories;
using Stellamod.Items.Armors.Vanity.Verlia;
using Stellamod.Items.Consumables;
using Stellamod.Items.Ores;
using Stellamod.Items.Quest.Zui;
using Stellamod.NPCs.Bosses.Zui.Projectiles;
using Stellamod.NPCs.Town;
using Stellamod.UI.Systems;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.Zui
{
    [AutoloadBossHead] // This attribute looks for a texture called "ClassName_Head_Boss" and automatically registers it as the NPC boss head ic
	public class ZuiTheTraveller : ModNPC
	{
		private float _teleportX;
		private float _teleportY;
		private bool _resetTimers;
		public enum ActionState
		{
			StartZui,
			IdleFloatZui,
			ElectricityZui,
			BeamsZui,
			LightrayZui,
			HomingGoldZui,
			SonicOutSpin,
			SpinAroundPlayerZui,
			SonicDashZui,
			SonicDashZui2,
			SonicGroundpound,
			RunZui,
			RunZuiLeft,
			AnticipateBigZui,
			AnticipateDashZui,
			HomeRunZui,
			DashZui,
		}
		// Current state

		private ActionState _state = ActionState.StartZui;
		public int ZuiLongSlash = 0;
		public int ZuiSonic = 0;
		public ActionState State
		{
			get
			{
				return _state;
			}
			set
			{
				_state = value;
				if(StellaMultiplayer.IsHost)
					NPC.netUpdate = true;
			}
		}

		// Current frame
		public int frameCounter;
		// Current frame's progress
		public int frameTick;
		// Current state's timer
		public float timer;

		public int rippleCount = 20;
		public int rippleSize = 5;
		public int rippleSpeed = 15;
		public float distortStrength = 300f;
		public bool Running = false;

		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Verlia of The Moon");

			Main.npcFrameCount[Type] = 53;

			NPCID.Sets.TrailCacheLength[NPC.type] = 15;
			NPCID.Sets.TrailingMode[NPC.type] = 0;
            NPCID.Sets.MPAllowedEnemies[NPC.type] = true;

            // Add this in for bosses that have a summon item, requires corresponding code in the item (See MinionBossSummonItem.cs)
            // Automatically group with other bosses
            NPCID.Sets.BossBestiaryPriority.Add(Type);
			NPCDebuffImmunityData debuffData = new NPCDebuffImmunityData
			{
				SpecificallyImmuneTo = new int[] {
					BuffID.Poisoned,
					BuffID.Burning,
					BuffID.Ichor,
					BuffID.Frostburn,
					BuffID.Confused // Most NPCs have this
				}
			};
			NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
			NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.OnFire] = true;
			NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.OnFire3] = true;
			NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Frostburn] = true;
			NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Frostburn2] = true;
			NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Ichor] = true;
			// Influences how the NPC looks in the Bestiary
			NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers();
			drawModifiers.CustomTexturePath = "Stellamod/NPCs/Bosses/Zui/ZuiBestiary";
			drawModifiers.PortraitScale = 1f; // Portrait refers to the full picture when clicking on the icon in the bestiary
			drawModifiers.PortraitPositionYOverride = 0f;
			NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
		}

		public override void SetDefaults()
		{
			NPC.Size = new Vector2(27, 42);
			NPC.damage = 1;
			NPC.defense = 45;
			NPC.lifeMax = 51250;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
			NPC.knockBackResist = 0f;
			NPC.noGravity = true;
			NPC.noTileCollide = true;
			NPC.value = Item.buyPrice(gold: 20);
			NPC.SpawnWithHigherTime(30);
			NPC.boss = true;
			NPC.npcSlots = 10f;
			NPC.scale = 2f;

			NPC.aiStyle = 0;






			// Take up open spawn slots, preventing random NPCs from spawning during the fight

			// Don't set immunities like this as of 1.4:
			// NPC.buffImmune[BuffID.Confused] = true;
			// immunities are handled via dictionaries through NPCID.Sets.DebuffImmunitySets

			// Custom AI, 0 is "bound town NPC" AI which slows the NPC down and changes sprite orientation towards the target
			NPC.aiStyle = -1;

			// Custom boss bar
			NPC.BossBar = ModContent.GetInstance<ZuiBossBar>();

			// The following code assigns a music track to the boss in a simple way.
			if (!Main.dedServ)
			{
				Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/ZuiTheTraveller");
			}
		}

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * balance);
        }

        
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            // We can use AddRange instead of calling Add multiple times in order to add multiple items at once
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				// Sets the preferred biomes of this town NPC listed in the bestiary.
				// With Town NPCs, you usually set this to what biome it likes the most in regards to NPC happiness.
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.UndergroundJungle,

				// Sets your NPC's flavor text in the bestiary.
				new FlavorTextBestiaryInfoElement(LangText.Bestiary(this, "Zui, not exactly someone that can be killed but loves to play around I guess? Sirestias is closely accompanied with her")),

				// You can add multiple elements if you really wanted to
				// You can also use localization keys (see Localization/en-US.lang)
				new FlavorTextBestiaryInfoElement(LangText.Bestiary(this, "Zui the Radiance", "2"))
            });
        }

        public override void SendExtraAI(BinaryWriter writer)
		{
			writer.Write((float)_state);
			writer.WriteVector2(dashDirection);
			writer.Write(dashDistance);
			//writer.Write(frameCounter);
			//writer.Write(frameTick);
			//writer.Write(yud);
			//writer.Write(yum);
			//writer.Write(gruber1);
			//writer.Write(gruber2);
			//writer.Write(rayer);
			writer.Write(downtoclown);
			writer.Write(ZuiLongSlash);
			writer.Write(ZuiSonic);
			writer.Write(_teleportX);
			writer.Write(_teleportY);
			writer.Write(_resetTimers);
		}

		public override void ReceiveExtraAI(BinaryReader reader)
		{
			_state = (ActionState)reader.ReadSingle();
			dashDirection = reader.ReadVector2();
			dashDistance = reader.ReadSingle();
			//frameCounter = reader.ReadInt32();
			//frameTick = reader.ReadInt32();
			//yud = reader.ReadInt32();
			//yum = reader.ReadInt32();
			//gruber1 = reader.ReadInt32();
			//gruber2 = reader.ReadInt32();
			//rayer = reader.ReadInt32();
			downtoclown = reader.ReadBoolean();
			ZuiLongSlash = reader.ReadInt32();
			ZuiSonic = reader.ReadInt32();
			_teleportX = reader.ReadSingle();
			_teleportY = reader.ReadSingle();
			_resetTimers = reader.ReadBoolean();
        }

		private int _wingFrameCounter;
		private int _wingFrameTick;
		Vector2 dashDirection = Vector2.Zero;
		float dashDistance = 0f;
		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			Player player = Main.player[NPC.target];

			Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
			SpriteEffects effects = SpriteEffects.None;
			SpriteEffects effects2 = NPC.direction == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
			if (player.Center.X > NPC.Center.X)
			{
				effects = SpriteEffects.FlipHorizontally;
			}

			

			Rectangle rect;
			originalHitbox = new Vector2(NPC.width / 100, NPC.height / 2) + new Vector2(-16, -32);
			
			
				Vector2 drawPosition = NPC.Center - screenPos;
				Vector2 origin = new Vector2(45, 40);
				Texture2D syliaWingsTexture = ModContent.Request<Texture2D>("Stellamod/NPCs/Bosses/Zui/ZuiElement").Value;
				int wingFrameSpeed = 1;
				int wingFrameCount = 60;
				spriteBatch.Draw(syliaWingsTexture, drawPosition,
				syliaWingsTexture.AnimationFrame(ref _wingFrameCounter, ref _wingFrameTick, wingFrameSpeed, wingFrameCount, true),
				drawColor, 0f, origin, 2f, effects, 0f);


			///Animation Stuff for Verlia
			/// 1 - 2 Summon Start
			/// 3 - 7 Summon Idle / Idle
			/// 8 - 11 Summon down
			/// 12 - 19 Hold UP
			/// 20 - 30 Sword UP
			/// 31 - 35 Sword Slash Simple
			/// 36 - 45 Hold Sword
			/// 46 - 67 Barrage 
			/// 68 - 75 Explode
			/// 76 - 80 Appear
			/// 133 width
			/// 92 height


			///Animation Stuff for Veribloom
			/// 1 = Idle
			/// 2 = Blank
			/// 2 - 8 Appear Pulse
			/// 9 - 19 Pulse Buff Att
			/// 20 - 26 Disappear Pulse
			/// 27 - 33 Appear Winding
			/// 34 - 38 Wind Up
			/// 39 - 45 Dash
			/// 46 - 52 Slam Appear
			/// 53 - 58 Slam
			/// 59 - 64 Spin
			/// 80 width
			/// 89 height
			/// 

			if (Running)
            {

				spriteBatch.End();
				spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
				for (int k = 0; k < NPC.oldPos.Length; k++)
				{
					Vector2 drawPos = NPC.oldPos[k] - Main.screenPosition + NPC.Size / 2 + new Vector2(0f, NPC.gfxOffY);
					Color color = NPC.GetAlpha(Color.Lerp(new Color(228, 200, 17), new Color(209, 166, 15), 1f / NPC.oldPos.Length * k) * (1f - 1f / NPC.oldPos.Length * k));
					spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, drawPos, new Microsoft.Xna.Framework.Rectangle?(NPC.frame), color, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects2, 0f);
				}

				spriteBatch.End();
				spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

			}



			switch (State)
			{



				case ActionState.StartZui:
					rect = new(0, 14 * 70, 112, 6 * 70);
					spriteBatch.Draw(texture, NPC.Center - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 12, 6, rect), drawColor, 0f, texture.AnimationFrame(ref frameCounter, ref frameTick, 12, 6, rect).Size() / 2, NPC.scale, effects, 0f);
					break;

				case ActionState.IdleFloatZui:
					rect = new(0, 14 * 70, 112, 6 * 70);
					spriteBatch.Draw(texture, NPC.Center - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 12, 6, rect), drawColor, 0f, texture.AnimationFrame(ref frameCounter, ref frameTick, 12, 6, rect).Size() / 2, NPC.scale, effects, 0f);
					break;

				case ActionState.BeamsZui:
					rect = new(0, 14 * 70, 112, 6 * 70);
					spriteBatch.Draw(texture, NPC.Center - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 12, 6, rect), drawColor, 0f, texture.AnimationFrame(ref frameCounter, ref frameTick, 12, 6, rect).Size() / 2, NPC.scale, effects, 0f);
					break;

				case ActionState.HomingGoldZui:
					rect = new(0, 14 * 70, 112, 6 * 70);
					spriteBatch.Draw(texture, NPC.Center - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 12, 6, rect), drawColor, 0f, texture.AnimationFrame(ref frameCounter, ref frameTick, 12, 6, rect).Size() / 2, NPC.scale, effects, 0f);
					break;

				case ActionState.LightrayZui:
					rect = new(0, 14 * 70, 112, 6 * 70);
					spriteBatch.Draw(texture, NPC.Center - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 12, 6, rect), drawColor, 0f, texture.AnimationFrame(ref frameCounter, ref frameTick, 12, 6, rect).Size() / 2, NPC.scale, effects, 0f);
					break;

				case ActionState.SonicOutSpin:
					rect = new(0, 1 * 70, 112, 11 * 70);
					spriteBatch.Draw(texture, NPC.Center - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 3, 11, rect), drawColor, 0f, texture.AnimationFrame(ref frameCounter, ref frameTick, 3, 11, rect).Size() / 2, NPC.scale, effects, 0f);
					break;

				case ActionState.SpinAroundPlayerZui:
					rect = new(0, 1 * 70, 112, 11 * 70);
					spriteBatch.Draw(texture, NPC.Center - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 3, 11, rect), drawColor, 0f, texture.AnimationFrame(ref frameCounter, ref frameTick, 3, 11, rect).Size() / 2, NPC.scale, effects, 0f);
					break;

				case ActionState.ElectricityZui:
					rect = new(0, 14 * 70, 112, 6 * 70);
					spriteBatch.Draw(texture, NPC.Center - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 12, 6, rect), drawColor, 0f, texture.AnimationFrame(ref frameCounter, ref frameTick, 12, 6, rect).Size() / 2, NPC.scale, effects, 0f);
					break;

				case ActionState.SonicDashZui:
					rect = new(0, 50 * 70, 112, 3 * 70);
					spriteBatch.Draw(texture, NPC.Center - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick,4, 3, rect), drawColor, 0f, texture.AnimationFrame(ref frameCounter, ref frameTick,4, 3, rect).Size() / 2, NPC.scale, effects, 0f);
					break;

				case ActionState.SonicDashZui2:
					rect = new(0, 50 * 70, 112, 3 * 70);
					spriteBatch.Draw(texture, NPC.Center - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick,4, 3, rect), drawColor, 0f, texture.AnimationFrame(ref frameCounter, ref frameTick,4, 3, rect).Size() / 2, NPC.scale, effects, 0f);
					break;

				case ActionState.SonicGroundpound:
					rect = new(0, 50 * 70, 112, 3 * 70);
					spriteBatch.Draw(texture, NPC.Center - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick,4, 3, rect), drawColor, 0f, texture.AnimationFrame(ref frameCounter, ref frameTick,4, 3, rect).Size() / 2, NPC.scale, effects, 0f);
					break;

				case ActionState.RunZui:
					rect = new(0, 21 * 70, 112, 8 * 70);
					spriteBatch.Draw(texture, NPC.Center - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 4, 8, rect), drawColor, 0f, texture.AnimationFrame(ref frameCounter, ref frameTick, 4, 8, rect).Size() / 2, NPC.scale, effects, 0f);
					break;

				case ActionState.RunZuiLeft:
					rect = new(0, 21 * 70, 112, 8 * 70);
					spriteBatch.Draw(texture, NPC.Center - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 4, 8, rect), drawColor, 0f, texture.AnimationFrame(ref frameCounter, ref frameTick, 4, 8, rect).Size() / 2, NPC.scale, effects, 0f);
					break;

				case ActionState.HomeRunZui:
					rect = new(0, 30 * 70, 112, 19 * 70);
					spriteBatch.Draw(texture, NPC.Center - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 4, 19, rect), drawColor, 0f, texture.AnimationFrame(ref frameCounter, ref frameTick, 4, 19, rect).Size() / 2, NPC.scale, effects, 0f);
					break;

				case ActionState.DashZui:
					rect = new(0, 38 * 70, 112, 11 * 70);
					spriteBatch.Draw(texture, NPC.Center - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 4, 11, rect), drawColor, 0f, texture.AnimationFrame(ref frameCounter, ref frameTick, 4, 11, rect).Size() / 2, NPC.scale, effects, 0f);
					break;

				case ActionState.AnticipateBigZui:
					rect = new(0, 30 * 70, 112, 7 * 70);
					spriteBatch.Draw(texture, NPC.Center - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 5, 8, rect), drawColor, 0f, texture.AnimationFrame(ref frameCounter, ref frameTick, 5, 8, rect).Size() / 2, NPC.scale, effects, 0f);
					break;

				case ActionState.AnticipateDashZui:
					rect = new(0, 30 * 70, 112, 19 * 70);
					spriteBatch.Draw(texture, NPC.Center - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 4, 19, rect), drawColor, 0f, texture.AnimationFrame(ref frameCounter, ref frameTick, 4, 19, rect).Size() / 2, NPC.scale, effects, 0f);
					break;
			}
			
			return false;
		}

		//Custom function so that I don't have to copy and paste the same thing in FindFrame
        int bee = 220;
		private Vector2 originalHitbox;
		public override void AI()
		{
			NPC.velocity *= 0.96f;
			bee--;
			if (bee == 0)
			{
				bee = 220;
			}

			Vector3 RGB = new(2.30f, 0.21f, 0.72f);
			// The multiplication here wasn't doing anything
			Lighting.AddLight(NPC.Center, RGB.X, RGB.Y, RGB.Z);
			
			Player player = Main.player[NPC.target];
			player.AddBuff(ModContent.BuffType<Zuid>(), 30);

			if (!NPC.HasValidTarget)
			{
				NPC.TargetClosest();
			}

			if (!NPC.HasValidTarget)
			{
				// If the targeted player is dead, flee
				NPC.velocity.Y -= 0.8f;
				NPC.noTileCollide = true;
				NPC.noGravity = false;
				// This method makes it so when the boss is in "despawn range" (outside of the screen), it despawns in 10 ticks
				NPC.EncourageDespawn(4);
			}

			FinishResetTimers();
			switch (State)
			{
				case ActionState.StartZui:
					NPC.damage = 0;
					StartZui();
					break;

				case ActionState.BeamsZui:
					NPC.damage = 0;
					BeamsZui();
					break;

				case ActionState.HomingGoldZui:
					NPC.damage = 0;
					GoldenBoltsZui();
					break;

				case ActionState.SpinAroundPlayerZui:
					NPC.damage = 0;
					CirclesZui();
					break;

				case ActionState.RunZuiLeft:
					NPC.damage = 0;
					RunningZuiLeft();
					break;

				case ActionState.RunZui:
					NPC.damage = 0;
					RunningZuiRight();
					break;

				case ActionState.HomeRunZui:
					NPC.damage = 200;
					HomeRunZui();
					break;

				case ActionState.AnticipateDashZui:
					NPC.damage = 260;
					SlasherZui();
					break;

				case ActionState.LightrayZui:
					NPC.damage = 150;
					RaysZui();
					break;

				case ActionState.SonicOutSpin:
					NPC.damage = 0;
					SonicOutZui();
					break;

				case ActionState.SonicDashZui:
					NPC.damage = 310;
					SonicZui();
					break;


				case ActionState.SonicGroundpound:
					NPC.damage = 0;
					if (Main.netMode != NetmodeID.Server && !Terraria.Graphics.Effects.Filters.Scene["Shockwave"].IsActive())
					{
						Terraria.Graphics.Effects.Filters.Scene.Activate("Shockwave", NPC.Center).GetShader().UseColor(rippleCount, rippleSize, rippleSpeed).UseTargetPosition(NPC.Center);
					}

					if (Main.netMode != NetmodeID.Server && Terraria.Graphics.Effects.Filters.Scene["Shockwave"].IsActive())
					{
						float progress = (180f - bee) / 60f; // Will range from -3 to 3, 0 being the point where the bomb explodes.
						Terraria.Graphics.Effects.Filters.Scene["Shockwave"].GetShader().UseProgress(progress).UseOpacity(distortStrength * (1 - progress / 3f));
					}
					SonicGroundpound();
					break;
			}
		}




		private void StartZui()
		{
			
			float ai1 = NPC.whoAmI;
			timer++;
			Player player = Main.player[NPC.target];
			var entitySource = NPC.GetSource_FromThis();
			NPC.TargetClosest();


			if (timer == 1)
			{
				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/GoldenAura2"), NPC.position);
				if (StellaMultiplayer.IsHost)
				{
					Projectile.NewProjectile(entitySource, NPC.Center + new Vector2(150, 150), Vector2.Zero,
						ModContent.ProjectileType<ZuiSpawnEffect>(), 0, 0f, Owner: Main.myPlayer);
				}
				
				for (int i = 0; i < 150; i++)
				{
					Vector2 speed = Main.rand.NextVector2CircularEdge(4f, 4f);
					var d = Dust.NewDustPerfect(NPC.Center, DustID.GoldFlame, speed * 11, Scale: 3f);
					d.noGravity = true;
				}

				if (StellaMultiplayer.IsHost)
					{
						float speedXb = NPC.velocity.X * Main.rand.NextFloat(0f, 0f) + Main.rand.NextFloat(0f, 0f);
						float speedYb = NPC.velocity.Y * Main.rand.Next(0, 0) * 0.0f + Main.rand.Next(0, 0) * 0f;
						Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb, NPC.position.Y + speedYb, speedXb - 2 * 2, speedYb - 2 * 2,
							ModContent.ProjectileType<InfiniteHalo>(), 0, 0f, Main.myPlayer, 0f, ai1);
					}
			}


			if (timer < 50) 
			{
				NPC.velocity.Y -= 0.05f;
			}
			
			if (timer == 55)
			{
				// We apply an initial velocity the first tick we are in the Jump frame. Remember that -Y is up.
				ResetTimers();
				if (StellaMultiplayer.IsHost)
				{
					if (NPC.life > (NPC.lifeMax / 2))
					{
						switch (Main.rand.Next(4))
						{
							case 0:
								State = ActionState.BeamsZui;// BeamsZui;
								break;
							case 1:
								State = ActionState.HomingGoldZui;// ElectricityZui;
								break;
							case 2:
								State = ActionState.BeamsZui;//HomingGoldZui;
								break;
							case 3:
								State = ActionState.HomingGoldZui;//LightrayZui;
								break;
						}
					}

					if (NPC.life < (NPC.lifeMax / 2))
					{
						switch (Main.rand.Next(4))
						{
							case 0:
								State = ActionState.BeamsZui;// BeamsZui;
								break;
							case 1:
								State = ActionState.HomingGoldZui;// ElectricityZui;
								break;
							case 2:
								State = ActionState.LightrayZui;//HomingGoldZui;
								break;
							case 3:
								State = ActionState.LightrayZui;//LightrayZui;
								break;
						}
					}
				}


			}
		}

		private float _circleDegrees;
		private void BeamsZui()
		{

			float ai1 = NPC.whoAmI;
			timer++;
			NPC.TargetClosest();
			Player target = Main.player[NPC.target];
             Vector2 velocity = NPC.Center.DirectionTo(target.Center) * 10;
                if (timer == 1)
                {
                    if (StellaMultiplayer.IsHost)
                    {
                       // Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, velocity,
                         //   ModContent.ProjectileType<ZuiRay>(), 70, 10, Main.myPlayer, ai0: NPC.whoAmI);

					int index2 = NPC.NewNPC(NPC.GetSource_FromThis(), (int)NPC.Center.X, (int)NPC.Center.Y,
					ModContent.NPCType<ZuiLASERWARN>());


					}

					switch (Main.rand.Next(2))
				{
					case 0:
						SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Laserlock"), NPC.position);

						break;
					case 1:
						SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Laserlock2"), NPC.position);
						break;


				}

				



			}


                if (timer == 20)
                {
                    if (StellaMultiplayer.IsHost)
                    {
					int index2 = NPC.NewNPC(NPC.GetSource_FromThis(), (int)NPC.Center.X, (int)NPC.Center.Y,
					ModContent.NPCType<ZuiLASERWARN>());
					}
				switch (Main.rand.Next(2))
				{
					case 0:
						SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Laserlock"), NPC.position);

						break;
					case 1:
						SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Laserlock2"), NPC.position);
						break;


				}

			}


				if (timer == 40)
				{
					if (StellaMultiplayer.IsHost)
					{
					int index2 = NPC.NewNPC(NPC.GetSource_FromThis(), (int)NPC.Center.X, (int)NPC.Center.Y,
				ModContent.NPCType<ZuiLASERWARN>());


					switch (Main.rand.Next(2))
					{
						case 0:
							SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Laserlock"), NPC.position);

							break;
						case 1:
							SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Laserlock2"), NPC.position);
							break;


					}
				}

				}


				if (timer == 60)
				{
					if (StellaMultiplayer.IsHost)
					{
					int index2 = NPC.NewNPC(NPC.GetSource_FromThis(), (int)NPC.Center.X, (int)NPC.Center.Y,
				ModContent.NPCType<ZuiLASERWARN>());


					switch (Main.rand.Next(2))
					{
						case 0:
							SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Laserlock"), NPC.position);

							break;
						case 1:
							SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Laserlock2"), NPC.position);
							break;


					}
				}

				}


			if (timer == 80)
			{
				if (StellaMultiplayer.IsHost)
				{
					int index2 = NPC.NewNPC(NPC.GetSource_FromThis(), (int)NPC.Center.X, (int)NPC.Center.Y,
					ModContent.NPCType<ZuiLASERWARN>());


					switch (Main.rand.Next(2))
					{
						case 0:
							SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Laserlock"), NPC.position);

							break;
						case 1:
							SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Laserlock2"), NPC.position);
							break;


					}
				}

			}

			if (timer == 100)
			{
				if (StellaMultiplayer.IsHost)
				{
					int index2 = NPC.NewNPC(NPC.GetSource_FromThis(), (int)NPC.Center.X, (int)NPC.Center.Y,
					ModContent.NPCType<ZuiLASERWARN>());


					switch (Main.rand.Next(2))
					{
						case 0:
							SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Laserlock"), NPC.position);

							break;
						case 1:
							SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Laserlock2"), NPC.position);
							break;


					}
				}

			}

			if (timer == 120)
			{
				if (StellaMultiplayer.IsHost)
				{
					int index2 = NPC.NewNPC(NPC.GetSource_FromThis(), (int)NPC.Center.X, (int)NPC.Center.Y,
					ModContent.NPCType<ZuiLASERWARN>());

					switch (Main.rand.Next(2))
					{
						case 0:
							SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Laserlock"), NPC.position);

							break;
						case 1:
							SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Laserlock2"), NPC.position);
							break;
					}
				}
			}

			if (timer == 300)
			{
				// We apply an initial velocity the first tick we are in the Jump frame. Remember that -Y is up.
				ResetTimers();
				if (StellaMultiplayer.IsHost)
				{
					if (!downtoclown)
                    {
						switch (Main.rand.Next(2))
						{
							case 0:
								State = ActionState.SpinAroundPlayerZui;
								break;
							case 1:
								State = ActionState.SpinAroundPlayerZui;//RunZui;
								break;

						}
					}

					if (downtoclown)
					{
						downtoclown = false;
						switch (Main.rand.Next(1))
						{
							case 0:
								State = ActionState.AnticipateDashZui;
								break;
						}
					}
				}
			}
		}


		public int gruber1 = 0;
		public int gruber2 = 0;
		public int rayer = 0;

		private void RaysZui()
		{
			rayer++;			
			float ai1 = NPC.whoAmI;
			timer++;
			NPC.TargetClosest();
			Player target = Main.player[NPC.target];
			Vector2 velocity = NPC.Center.DirectionTo(target.Center) * 10;
			if (timer == 1)
			{
				if (StellaMultiplayer.IsHost)
				{
					NPC.NewNPC(NPC.GetSource_FromThis(), (int)NPC.Center.X, (int)NPC.Center.Y,
						ModContent.NPCType<ZuiLASERWARN>());
				}

				if (StellaMultiplayer.IsHost)
				{
					var entitySource = NPC.GetSource_FromThis();
					NPC.NewNPC(entitySource, (int)NPC.Center.X + Main.rand.Next(-40, 40), (int)target.Center.Y,
						ModContent.NPCType<GoldBeamWarn>());
				}
			}

			if (timer < 150)
            {
				if (rayer == 9)
				{
					if (StellaMultiplayer.IsHost)
					{
						var entitySource = NPC.GetSource_FromThis();
						NPC.NewNPC(entitySource, (int)NPC.Center.X + gruber1, (int)target.Center.Y,
							ModContent.NPCType<GoldBeamWarn>());
					}

					if (StellaMultiplayer.IsHost)
					{
						var entitySource = NPC.GetSource_FromThis();
						NPC.NewNPC(entitySource, (int)NPC.Center.X + gruber2, (int)target.Center.Y,
							ModContent.NPCType<GoldBeamWarn>());
					}

					gruber1 += 20;
					gruber2 -= 20;
					switch (Main.rand.Next(3))
					{
						case 0:
							SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Gold1"), NPC.position);

							break;
						case 1:
							SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Gold2"), NPC.position);
							break;
						case 2:
							SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Gold3"), NPC.position);
							break;

					}
					rayer = 0;
				}
			}


			if (timer == 50)
			{
				if (StellaMultiplayer.IsHost)
				{
					NPC.NewNPC(NPC.GetSource_FromThis(), (int)NPC.Center.X, (int)NPC.Center.Y,
						ModContent.NPCType<ZuiLASERWARN>());
				}

			}


			if (timer == 100)
			{
				if (StellaMultiplayer.IsHost)
				{
					NPC.NewNPC(NPC.GetSource_FromThis(), (int)NPC.Center.X, (int)NPC.Center.Y,
						ModContent.NPCType<ZuiLASERWARN>());
				}

			}


			if (timer == 300)
			{

				rayer = 0;
				// We apply an initial velocity the first tick we are in the Jump frame. Remember that -Y is up.
				ResetTimers();
				if (StellaMultiplayer.IsHost)
				{

					if (!downtoclown)
					{
						switch (Main.rand.Next(2))
						{
							case 0:
								State = ActionState.SonicDashZui;
								break;
							case 1:
								State = ActionState.SonicDashZui;//RunZui;
								break;

						}

					}

					if (downtoclown)
					{
						downtoclown = false;
						switch (Main.rand.Next(1))
						{
							case 0:
								State = ActionState.AnticipateDashZui;
								break;
						}
					}
				}
			}
		}

		int yum = 0;
		private void GoldenBoltsZui()
		{
			float ai1 = NPC.whoAmI;
			timer++;
			yum++;

			NPC.TargetClosest();
			Player target = Main.player[NPC.target];
			Vector2 velocity = NPC.Center.DirectionTo(target.Center) * 10;
			if (timer == 1)
			{
				if (StellaMultiplayer.IsHost)
				{
					NPC.NewNPC(NPC.GetSource_FromThis(), (int)NPC.Center.X, (int)NPC.Center.Y,
						ModContent.NPCType<ZuiLASERWARN>());
				}

				switch (Main.rand.Next(3))
				{
					case 0:
						SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Laserlock"), NPC.position);

						break;
					case 1:
						SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Laserlock2"), NPC.position);
						break;
					case 2:
						SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Laserlock"), NPC.position);
						break;

				}
			}

			if (timer < 200)
            {
				if (yum == 8)
				{
					Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, velocity * Main.rand.Next(-2, 2),
					ModContent.ProjectileType<GoldenHoe>(), 38, 10, Main.myPlayer, ai0: NPC.whoAmI);

					Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center + new Vector2(150,150), velocity * 0,
					ModContent.ProjectileType<ZuiSpawnEffect>(), 0, 10, Main.myPlayer, ai0: NPC.whoAmI);
					yum = 0;
				}
			}
			


			if (timer == 100)
			{
				if (StellaMultiplayer.IsHost)
				{
					NPC.NewNPC(NPC.GetSource_FromThis(), (int)NPC.Center.X, (int)NPC.Center.Y,
						ModContent.NPCType<ZuiLASERWARN>());
				}
				switch (Main.rand.Next(3))
				{
					case 0:
						SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Laserlock"), NPC.position);

						break;
					case 1:
						SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Laserlock2"), NPC.position);
						break;
					case 2:
						SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Laserlock2"), NPC.position);
						break;

				}
			}


			

			//SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Moaning"), NPC.position);
			//SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/SwordHoldVerlia"), NPC.position);



			if (timer < 50)
			{
				NPC.velocity.Y -= 0.01f;



			}

			if (timer == 300)
			{
				// We apply an initial velocity the first tick we are in the Jump frame. Remember that -Y is up.
				ResetTimers();
				if (StellaMultiplayer.IsHost)
				{
					switch (Main.rand.Next(2))
					{
						case 0:
							State = ActionState.SpinAroundPlayerZui;
							break;
						case 1:
							State = ActionState.SpinAroundPlayerZui; //RunZui;
							break;
							
					}
				}
			}
		}

		float movementSpeed = 5;
		float circleSpeed = 2;
		float circleDistance = 170;
		private void CirclesZui()
		{
			timer++;
			yum++;
			Player target = Main.player[NPC.target];
			Vector2 velocity = NPC.Center.DirectionTo(target.Center) * 10;
			
			if (timer < 30)
            {
				NPC.TargetClosest();
			}

			if (timer == 90)
			{
				movementSpeed = 15;
				circleSpeed = 3;
			}


			if (timer == 150)
			{
				movementSpeed = 25;
				circleDistance = 240;
			}

			if (timer == 210)
			{
				movementSpeed = 16;
			}


			if (timer == 250)
			{
				movementSpeed = 12;
				circleSpeed = 2;
				circleDistance = 128;
			}


			if (timer < 300)
			{


				if (timer > 30)
				{

					if (yum >= 8)
					{
						Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, velocity * Main.rand.Next(-2, 2),
						ModContent.ProjectileType<GoldenHoe>(), 40, 10, Main.myPlayer, ai0: NPC.whoAmI);



						Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center + new Vector2(150, 150), velocity * 0,
						ModContent.ProjectileType<ZuiSpawnEffect>(), 0, 10, Main.myPlayer, ai0: NPC.whoAmI);
						yum = 0;
					}

					//Circling Code


					//How far from the player the NPC will be


					_circleDegrees += circleSpeed;
					float circleRadians = MathHelper.ToRadians(_circleDegrees);
					Vector2 offsetFromPlayer = new Vector2(circleDistance, 0).RotatedBy(circleRadians);
					Vector2 circlePosition = target.Center + offsetFromPlayer;

					//This is just how quickly the NPC will move to the circle position
					//This number should be higher than the circle speed

					NPC.velocity = VectorHelper.VelocitySlowdownTo(NPC.Center, circlePosition, movementSpeed);

				}
			}
		


            if (timer > 250)
			{
				NPC.velocity *= 0.95f;
			}

			if (timer == 360)
			{
				// We apply an initial velocity the first tick we are in the Jump frame. Remember that -Y is up.
				ResetTimers();
				if (StellaMultiplayer.IsHost)
				{
					switch (Main.rand.Next(2))
					{
						case 0:
							State = ActionState.RunZuiLeft;
							break;
						case 1:
							State = ActionState.RunZui;
							break;

					}
				}
			}
		}

		private void RunningZuiLeft()
		{
			Running = true;
			timer++;
			yum++;

			NPC.TargetClosest();
			Player target = Main.player[NPC.target];
			Vector2 velocity = NPC.Center.DirectionTo(target.Center) * 10;
			if (timer == 1)
			{
				if (StellaMultiplayer.IsHost)
				{
					int distanceY = Main.rand.Next(-175, -175);
					int distanceYa = Main.rand.Next(-175, -175);
					NPC.position.X = target.Center.X + distanceY;
					NPC.position.Y = target.Center.Y + distanceYa;
					NPC.netUpdate = true;

					Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center + new Vector2(150, 150), velocity * 0,
					ModContent.ProjectileType<ZuiSpawnEffect>(), 0, 10, Main.myPlayer, ai0: NPC.whoAmI);					

					Holdr Holdr = Projectile.NewProjectileDirect(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero,
					 ModContent.ProjectileType<Holdr>(), 1, 1, target.whoAmI).ModProjectile as Holdr;
					Holdr.Target = NPC;
				}
			}

			if (timer < 150)
			{
				NPC.velocity.X -= 0.55f;
				NPC.velocity.Y -= 0.35f;

				if (target.active)
				{
					float distance = Vector2.Distance(NPC.Center - new Vector2(104, 0), target.Center);
					if (distance <= 4000)
					{
						Vector2 direction = target.Center - new Vector2(104, 0) - NPC.Center;
						direction.SafeNormalize(Vector2.Zero);
						target.velocity -= direction * 0.005f;
						target.position -= direction * 0.4f;
					}
				}
			}

			

			if (timer > 150)
			{
				float distance = Vector2.Distance(NPC.Center - new Vector2(104, 0), target.Center);
				if (distance <= 4000)
				{
					Vector2 direction = target.Center - new Vector2(104, 0) - NPC.Center;
					direction.SafeNormalize(Vector2.Zero);
					target.velocity -= direction * 0.005f;

				}
				target.velocity.Y -= 0.5f;
			}


			if (timer == 155)
			{
				ResetTimers();
				if (StellaMultiplayer.IsHost)
				{

					Running = false;
					switch (Main.rand.Next(2))
					{
						case 0:
							State = ActionState.HomeRunZui;
							break;
						case 1:
							State = ActionState.HomeRunZui; //RunZui;
							break;

					}
				}

			}
		}

		private void RunningZuiRight()
		{
			Running = true;
			float ai1 = NPC.whoAmI;
			timer++;
			yum++;

			NPC.TargetClosest();
			Player target = Main.player[NPC.target];
			Vector2 velocity = NPC.Center.DirectionTo(target.Center) * 10;
			if (timer == 1)
			{
				if (StellaMultiplayer.IsHost)
				{
					int distanceY = Main.rand.Next(-175, -175);
					int distanceYa = Main.rand.Next(-175, -175);
					NPC.position.X = target.Center.X + distanceY;
					NPC.position.Y = target.Center.Y + distanceYa;
					NPC.netUpdate = true;

					Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center + new Vector2(150, 150), velocity * 0,
					ModContent.ProjectileType<ZuiSpawnEffect>(), 0, 10, Main.myPlayer, ai0: NPC.whoAmI);

				
					Holdr Holdr = Projectile.NewProjectileDirect(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero,
					 ModContent.ProjectileType<Holdr>(), 1, 1, target.whoAmI).ModProjectile as Holdr;
					Holdr.Target = NPC;
				}
			}

			if (timer < 150)
			{
				NPC.velocity.X += 0.55f;
				NPC.velocity.Y -= 0.35f;

				if (target.active)
				{
					float distance = Vector2.Distance(NPC.Center + new Vector2(104, 0), target.Center);
					if (distance <= 4000)
					{
						Vector2 direction = target.Center + new Vector2(104, 0) - NPC.Center;
						direction.SafeNormalize(Vector2.Zero);
						target.velocity -= direction * 0.005f;
						target.position -= direction * 0.4f;
					}
				}
			}

			if (timer > 150)
			{
				float distance = Vector2.Distance(NPC.Center + new Vector2(104, 0), target.Center);
				if (distance <= 4000)
				{
					Vector2 direction = target.Center + new Vector2(104, 0) - NPC.Center;
					direction.SafeNormalize(Vector2.Zero);
					target.velocity -= direction * 0.005f;
				}
				target.velocity.Y -= 0.5f;
			}


			if (timer == 155)
			{
				ResetTimers();
				if (StellaMultiplayer.IsHost)
				{

					Running = false;
					switch (Main.rand.Next(2))
					{
						case 0:
							State = ActionState.HomeRunZui;
							break;
						case 1:
							State = ActionState.HomeRunZui; //RunZui;
							break;

					}
				}

			}
			// We apply an initial velocity the first tick we are in the Jump frame. Remember that -Y is up.
			


			
		}



		public bool downtoclown = false;

		private void HomeRunZui()
		{

			float ai1 = NPC.whoAmI;
			timer++;

			var entitySource = NPC.GetSource_FromThis();
			NPC.TargetClosest();
			Player target = Main.player[NPC.target];
			Vector2 velocity = NPC.Center.DirectionTo(target.Center) * 10;

			if (timer == 1)
			{
				
				target.velocity *= 0;
				if (StellaMultiplayer.IsHost)
				{
					Projectile.NewProjectile(entitySource, NPC.Center + new Vector2(150, 150), Vector2.Zero,
						ModContent.ProjectileType<ZuiSpawnEffect>(), 0, 0f, Owner: Main.myPlayer);
				}
				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Zoee"));




				//SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Moaning"), NPC.position);
				//SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/SwordHoldVerlia"), NPC.position);
			}

			if (timer < 10)
            {
				target.velocity.Y -= 0.7f;
			}
			if (timer == 32)
			{

				if (StellaMultiplayer.IsHost)
				{
					Projectile.NewProjectile(entitySource, NPC.Center + new Vector2(150, 150), Vector2.Zero,
						ModContent.ProjectileType<ZuiSpawnEffect>(), 0, 0f, Owner: Main.myPlayer);
				}
				
					
						SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/AbsoluteSwing"));
						SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Hyuh"));




				for (int i = 0; i < 150; i++)
				{
					Vector2 speed = Main.rand.NextVector2CircularEdge(4f, 4f);
					var d = Dust.NewDustPerfect(NPC.Center, DustID.GoldFlame, speed * 11, Scale: 3f);
					;
					d.noGravity = true;
				}


			
				target.velocity.X = NPC.direction * 22f;
				target.velocity.Y = -13f;

				for (int i = 0; i < 50; i++)
				{
					int num = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.GoldCoin, 0f, -2f, 0, default(Color), 1.5f);
					Main.dust[num].noGravity = true;
					Main.dust[num].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
					Main.dust[num].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
					{
						Main.dust[num].velocity = NPC.DirectionTo(Main.dust[num].position) * 6f;
					}
				}
				for (int i = 0; i < 14; i++)
				{
					Dust.NewDustPerfect(base.NPC.Center, DustID.GoldCoin, (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(19.0), 0, default(Color), 4f).noGravity = true;
				}
				for (int i = 0; i < 40; i++)
				{
					Dust.NewDustPerfect(base.NPC.Center, DustID.GoldCoin, (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(10.0), 0, default(Color), 1f).noGravity = false;
				}
				for (int i = 0; i < 40; i++)
				{
					Dust.NewDustPerfect(base.NPC.Center, DustID.CopperCoin, (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(25.0), 0, default(Color), 6f).noGravity = true;
				}
				for (int i = 0; i < 20; i++)
				{
					Dust.NewDustPerfect(base.NPC.Center, DustID.CopperCoin, (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(25.0), 0, default(Color), 2f).noGravity = false;
				}

				//SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Moaning"), NPC.position);
				//SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/SwordHoldVerlia"), NPC.position);
			}
			if (timer == 55)
			{
                switch (Main.rand.Next(3))
                {
                    case 0:
                        SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/GoldPrice4"), NPC.position);

                        break;
                    case 1:
                        SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/GoldenPrice5"), NPC.position);
                        break;
                    case 2:
                        SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/GoldenPrice6"), NPC.position);
                        break;

                }
                SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/SunStalker_PreSpawn"));
                Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(NPC.Center, 1024f, 32f);

                if (StellaMultiplayer.IsHost)
				{
					Projectile.NewProjectile(entitySource, NPC.Center + new Vector2(150, 150), Vector2.Zero,
					ModContent.ProjectileType<ZuiSpawnEffect>(), 0, 0f, Owner: Main.myPlayer);
					

					int numProjectiles = Main.rand.Next(12, 24);
					for (int p = 0; p < numProjectiles; p++)
					{
						// Rotate the velocity randomly by 30 degrees at max.
						Vector2 newVelocity = velocity.RotatedByRandom(MathHelper.ToRadians(15));
						newVelocity *= 1f - Main.rand.NextFloat(0.3f);
						Projectile.NewProjectile(entitySource, NPC.Center, newVelocity, ModContent.ProjectileType<GoldenChildren>(), 50, 0, Owner: Main.myPlayer);
					}

				}
				
			}

				
			if (timer == 76)
			{
				// We apply an initial velocity the first tick we are in the Jump frame. Remember that -Y is up.
				ResetTimers();
				if (StellaMultiplayer.IsHost)
				{
					downtoclown = true;
					switch (Main.rand.Next(2))
					{
						case 0:
							State = ActionState.BeamsZui;// BeamsZui;
							break;
						case 1:
							State = ActionState.AnticipateDashZui;// ElectricityZui;
							break;
					}
				}
			}
		}

		


		private void SlasherZui()
		{
			float ai1 = NPC.whoAmI;
			timer++;

			var entitySource = NPC.GetSource_FromThis();
			NPC.TargetClosest();
			Player target = Main.player[NPC.target];
			Vector2 velocity = NPC.Center.DirectionTo(target.Center) * 10;

            //Teleporting Code
            if (_teleportX != 0 || _teleportY != 0)
            {
                NPC.position.X = _teleportX;
                NPC.position.Y = _teleportY;
                NPC.velocity.X = 0f;
                NPC.velocity.Y = 0f;
                _teleportX = 0f;
                _teleportY = 0f;
            }

            if (timer == 1)
			{
				int distanceY;
				int distanceYa;
				Vector2 teleportPos = NPC.Center;
				if (StellaMultiplayer.IsHost)
				{
                    switch (Main.rand.Next(2))
                    {
						default:
                        case 0:
                            distanceY = Main.rand.Next(-50, -50);
                            distanceYa = Main.rand.Next(-125, -125);
                            break;


                        case 1:
                            distanceY = Main.rand.Next(-50, -50);
                            distanceYa = Main.rand.Next(125, 125);
                            break;
                    }

					_teleportX = target.Center.X + distanceYa;
					_teleportY = target.Center.Y + distanceY;
                    teleportPos = new Vector2(_teleportX, _teleportY);
                    NPC.netUpdate = true;
                }

                if (StellaMultiplayer.IsHost)
				{
					Projectile.NewProjectile(NPC.GetSource_FromThis(), teleportPos + new Vector2(150, 150), velocity * 0,
					ModContent.ProjectileType<ZuiSpawnEffect>(), 0, 10, Main.myPlayer, ai0: NPC.whoAmI);
				}

				switch (Main.rand.Next(3))
				{
					case 0:
						SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/GoldPrice4"), NPC.position);

						break;
					case 1:
						SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/GoldenPrice5"), NPC.position);
						break;
					case 2:
						SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/GoldenPrice6"), NPC.position);
						break;

				}

				//SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Moaning"), NPC.position);
				//SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/SwordHoldVerlia"), NPC.position);
			}



            NPC.velocity *= 0.96f;
			float speed = 20f;
			NPC.noTileCollide = true;
			NPC.noGravity = true;


			if (timer > 32)
			{

				if (timer < 40)
				{
					int distance = Main.rand.Next(2, 2);
					NPC.ai[3] = Main.rand.Next(1);
					double anglex = Math.Sin(NPC.ai[3] * (Math.PI / 180));
					double angley = 0;
					Vector2 angle = new Vector2((float)anglex, (float)angley);
					Vector2 dashDirection = (target.Center - (angle * distance)) - NPC.Center;
					float dashDistance = dashDirection.Length();
					dashDirection.Normalize();
					dashDirection *= speed;
					dashDirection.Y = 0;
					NPC.velocity = dashDirection;
					ShakeModSystem.Shake = 3;
				}
			}


			if (timer == 32)
			{
				ZuiLongSlash += 1;
                if (StellaMultiplayer.IsHost)
				{
					Projectile.NewProjectile(entitySource, NPC.Center + new Vector2(150, 150), Vector2.Zero,
						ModContent.ProjectileType<ZuiSpawnEffect>(), 0, 0f, Owner: Main.myPlayer);
				}

				for (int i = 0; i < 150; i++)
				{
					Vector2 speeda = Main.rand.NextVector2CircularEdge(4f, 4f);
					var d = Dust.NewDustPerfect(NPC.Center, DustID.GoldFlame, speeda * 11, Scale: 3f);
					;
					d.noGravity = true;
				}
				switch (Main.rand.Next(3))
				{
					case 0:
						SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/GoldenSlice1"), NPC.position);

						break;
					case 1:
						SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/GoldenSlice2"), NPC.position);
						break;
					case 2:
						SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/GoldenSlice3"), NPC.position);
						break;

				}




				for (int i = 0; i < 50; i++)
				{
					int num = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.GoldCoin, 0f, -2f, 0, default(Color), 1.5f);
					Main.dust[num].noGravity = true;
					Main.dust[num].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
					Main.dust[num].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
					{
						Main.dust[num].velocity = NPC.DirectionTo(Main.dust[num].position) * 6f;
					}
				}
				for (int i = 0; i < 14; i++)
				{
					Dust.NewDustPerfect(base.NPC.Center, DustID.GoldCoin, (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(19.0), 0, default(Color), 4f).noGravity = true;
				}
				for (int i = 0; i < 40; i++)
				{
					Dust.NewDustPerfect(base.NPC.Center, DustID.GoldCoin, (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(10.0), 0, default(Color), 1f).noGravity = false;
				}
				for (int i = 0; i < 40; i++)
				{
					Dust.NewDustPerfect(base.NPC.Center, DustID.CopperCoin, (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(25.0), 0, default(Color), 6f).noGravity = true;
				}
				for (int i = 0; i < 20; i++)
				{
					Dust.NewDustPerfect(base.NPC.Center, DustID.CopperCoin, (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(25.0), 0, default(Color), 2f).noGravity = false;
				}

				//SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Moaning"), NPC.position);
				//SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/SwordHoldVerlia"), NPC.position);
			}
			if (timer == 35)
			{
                Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(NPC.Center, 1024f, 32f);
                if (StellaMultiplayer.IsHost)
				{
					Projectile.NewProjectile(entitySource, NPC.Center + new Vector2(150, 150), Vector2.Zero,
					ModContent.ProjectileType<ZuiSpawnEffect>(), 0, 0f, Owner: Main.myPlayer);

					int numProjectiles = Main.rand.Next(12, 24);
					for (int p = 0; p < numProjectiles; p++)
					{
						// Rotate the velocity randomly by 30 degrees at max.
						Vector2 newVelocity = velocity.RotatedByRandom(MathHelper.ToRadians(15));
						newVelocity *= 1f - Main.rand.NextFloat(0.3f);
						Projectile.NewProjectile(entitySource, NPC.Center, -newVelocity, ModContent.ProjectileType<GoldenChildren>(), 50, 0, Owner: Main.myPlayer);
					}

				}

				switch (Main.rand.Next(3))
				{
					case 0:
						SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Gold1"), NPC.position);

						break;
					case 1:
						SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Gold2"), NPC.position);
						break;
					case 2:
						SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Gold3"), NPC.position);
						break;

				}
			}


			if (timer == 76 && ZuiLongSlash < 8)
            {
				ResetTimers();
				State = ActionState.AnticipateDashZui;
				timer = 0;
            }

			if (timer == 76 && ZuiLongSlash >= 8)
			{
				// We apply an initial velocity the first tick we are in the Jump frame. Remember that -Y is up.
				
				if (StellaMultiplayer.IsHost)
				{
					ResetTimers();
					downtoclown = false;
					ZuiLongSlash = 0;
					if (NPC.life >= (NPC.lifeMax / 2))
					{
						switch (Main.rand.Next(2))
						{
							case 0:
								// BeamsZui;	
								// 		

								Projectile.NewProjectile(entitySource, NPC.Center + new Vector2(150, 150), Vector2.Zero,
								ModContent.ProjectileType<ZuiSpawnEffect>(), 0, 0f, Owner: Main.myPlayer);
								int distanceY = Main.rand.Next(0, 0);
								int distanceYa = Main.rand.Next(-425, -425);
								NPC.position.X = target.Center.X + distanceY;
								NPC.position.Y = target.Center.Y + distanceYa;
								NPC.netUpdate = true;
								NPC.noTileCollide = true;
								State = ActionState.SonicGroundpound;

								break;
							case 1:
								State = ActionState.StartZui;// ElectricityZui;
								break;


						}

					}


					if (NPC.life < (NPC.lifeMax / 2))
					{
						switch (Main.rand.Next(2))
						{
							case 0:
								// BeamsZui;	
								// 		

								Projectile.NewProjectile(entitySource, NPC.Center + new Vector2(150, 150), Vector2.Zero,
								ModContent.ProjectileType<ZuiSpawnEffect>(), 0, 0f, Owner: Main.myPlayer);
								int distanceY = Main.rand.Next(0, 0);
								int distanceYa = Main.rand.Next(-425, -425);
								NPC.position.X = target.Center.X + distanceY;
								NPC.position.Y = target.Center.Y + distanceYa;
								NPC.netUpdate = true;
								NPC.noTileCollide = true;
								State = ActionState.SonicGroundpound;

								break;
							case 1:
								State = ActionState.SonicDashZui; ;// ElectricityZui;
								break;
						}
					}
				}
			}
		}

		public int yud = 0;


		private void SonicGroundpound()
		{
			float ai1 = NPC.whoAmI;
			timer++;
			var entitySource = NPC.GetSource_FromThis();
			NPC.TargetClosest();
			Player target = Main.player[NPC.target];
			Vector2 velocity = NPC.Center.DirectionTo(target.Center) * 10;
			NPC.velocity.X *= 0;

			if (timer == 1)
			{
				int distanceY = 0;
				int distanceYa = -425; 
                NPC.position.X = target.Center.X + distanceY;
                NPC.position.Y = target.Center.Y + distanceYa;

                SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/CorsageRune2"), NPC.position);
				switch (Main.rand.Next(3))
				{
					case 0:
						SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/SunStalker_Charge"), NPC.position);

						break;
					case 1:
						SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/SunStalker_Charge"), NPC.position);
						break;
					case 2:
						SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/SunStalker_Charge"), NPC.position);
						break;

				}

				if (StellaMultiplayer.IsHost)
				{

					Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center + new Vector2(150, 150), velocity * 0,
					ModContent.ProjectileType<ZuiSpawnEffect>(), 0, 10, Main.myPlayer, ai0: NPC.whoAmI);			
				}
			}
			
			if (timer < 15)
            {
				NPC.noTileCollide = true;
				NPC.velocity.Y += 0.1f;
				NPC.collideY = false;
			}

			if (timer > 15)
			{
				NPC.noTileCollide = false;

			}

			if (timer == 15)
            {

				if (StellaMultiplayer.IsHost)
				{
					Projectile.NewProjectile(entitySource, NPC.Center + new Vector2(150, 150), Vector2.Zero,
					ModContent.ProjectileType<ZuiSpawnEffect>(), 0, 0f, Owner: Main.myPlayer);


					Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(NPC.Center, 1024f, 32f);
					int numProjectiles = Main.rand.Next(12, 24);
					for (int p = 0; p < numProjectiles; p++)
					{
						Projectile.NewProjectile(entitySource, NPC.Center, -NPC.velocity, ModContent.ProjectileType<GoldenChildren>(), 50, 0, Owner: Main.myPlayer);
					}

				}

			}
			NPC.noGravity = true;
		


			if (timer >= 15)
			{	NPC.velocity.Y += 0.6f;

				if (NPC.collideY || timer >= 120)
				{

					yud++;
					NPC.velocity *= 0;

					if (StellaMultiplayer.IsHost)
					{
						Projectile.NewProjectile(entitySource, NPC.Center + new Vector2(150, 150), Vector2.Zero,
							ModContent.ProjectileType<ZuiSpawnEffect>(), 0, 0f, Owner: Main.myPlayer);
					}

					for (int i = 0; i < 150; i++)
					{
						Vector2 speeda = Main.rand.NextVector2CircularEdge(4f, 4f);
						var d = Dust.NewDustPerfect(NPC.Center, DustID.GoldFlame, speeda * 11, Scale: 3f);
						;
						d.noGravity = true;
					}

					if (StellaMultiplayer.IsHost)
					{
						float speedXB = NPC.velocity.X * Main.rand.NextFloat(-.3f, -.3f) + Main.rand.NextFloat(-4f, -4f);
						float speedX = NPC.velocity.X * Main.rand.NextFloat(.3f, .3f) + Main.rand.NextFloat(4f, 4f);
						float speedY = NPC.velocity.Y * Main.rand.Next(0, 0) * 0.0f + Main.rand.Next(0, 0) * 0f;
						Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y - 20, speedX + 2 * 6, speedY,
							ModContent.ProjectileType<GroundSpikeBullet>(), 80, 0f, Owner: Main.myPlayer);
						Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y - 20, speedXB - 2 * 6, speedY,
							ModContent.ProjectileType<GroundSpikeBullet>(), 80, 0f, Owner: Main.myPlayer);
					}



					for (int i = 0; i < 50; i++)
					{
						int num = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.GoldCoin, 0f, -2f, 0, default(Color), 1.5f);
						Main.dust[num].noGravity = true;
						Main.dust[num].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
						Main.dust[num].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
						{
							Main.dust[num].velocity = NPC.DirectionTo(Main.dust[num].position) * 6f;
						}
					}
					for (int i = 0; i < 14; i++)
					{
						Dust.NewDustPerfect(base.NPC.Center, DustID.GoldCoin, (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(19.0), 0, default(Color), 4f).noGravity = true;
					}
					for (int i = 0; i < 40; i++)
					{
						Dust.NewDustPerfect(base.NPC.Center, DustID.GoldCoin, (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(10.0), 0, default(Color), 1f).noGravity = false;
					}
					for (int i = 0; i < 40; i++)
					{
						Dust.NewDustPerfect(base.NPC.Center, DustID.CopperCoin, (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(25.0), 0, default(Color), 6f).noGravity = true;
					}
					for (int i = 0; i < 20; i++)
					{
						Dust.NewDustPerfect(base.NPC.Center, DustID.CopperCoin, (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(25.0), 0, default(Color), 2f).noGravity = false;
					}

					NPC.noTileCollide = true;
					if (yud >= 10)
					{
						SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/GoldenFall"), NPC.position);
						SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Verifall"));

						if (Main.netMode != NetmodeID.Server && Terraria.Graphics.Effects.Filters.Scene["Shockwave"].IsActive())
						{
							Terraria.Graphics.Effects.Filters.Scene["Shockwave"].Deactivate();
						}

						ResetTimers();
						if (StellaMultiplayer.IsHost)
						{
							downtoclown = false;
							ZuiLongSlash = 0;
							switch (Main.rand.Next(2))
							{
								case 0:
									State = ActionState.StartZui;// BeamsZui;
									break;
								case 1:
									State = ActionState.StartZui;// ElectricityZui;
									break;
							}
						}
					}
				}
			}
		}


		private void SonicZui()
		{
			timer++;
			var entitySource = NPC.GetSource_FromThis();
			NPC.TargetClosest();
			Player target = Main.player[NPC.target];
			Vector2 velocity = NPC.Center.DirectionTo(target.Center) * 10;

			if (timer == 1)
			{
				int distanceY = -50;
				int distanceYa = 425;
                NPC.position.X = target.Center.X + distanceYa;
                NPC.position.Y = target.Center.Y + distanceY;

                if (StellaMultiplayer.IsHost)
				{
					Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center + new Vector2(150, 150), velocity * 0,
					ModContent.ProjectileType<ZuiSpawnEffect>(), 0, 10, Main.myPlayer, ai0: NPC.whoAmI);
				}


				ZuiSonic += 1;
				for (int i = 0; i < 150; i++)
				{
					Vector2 speeda = Main.rand.NextVector2CircularEdge(4f, 4f);
					var d = Dust.NewDustPerfect(NPC.Center, DustID.GoldFlame, speeda * 11, Scale: 3f);
					d.noGravity = true;
				}

				NPC.velocity.X -= 17f;
				switch (Main.rand.Next(3))
				{
					case 0:
						SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/GW1"));

						break;
					case 1:
						SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/GW2"));
						break;
					case 2:
						SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/GW3"));
						break;

				}
			}

			if (timer == 2)
            {
				if (StellaMultiplayer.IsHost)
				{
					NPC.NewNPC(entitySource, (int)NPC.Center.X + Main.rand.Next(-1, 1), (int)target.Center.Y,
						ModContent.NPCType<GoldBeamWarn>());
				}
			}

			rayer++;
			if (rayer == 9)
			{
				if (StellaMultiplayer.IsHost)
				{
					NPC.NewNPC(entitySource, (int)NPC.Center.X + gruber1, (int)target.Center.Y,
						ModContent.NPCType<GoldBeamWarn>());
				}
				
				gruber1 += 20;
				rayer = 0;
			}

			float speed = 7f;
			NPC.noTileCollide = true;
			NPC.noGravity = true;

			if (timer < 32)
			{
				int distance = Main.rand.Next(2, 2);
				double angley = Math.Abs(Math.Cos(NPC.ai[3] * (Math.PI / 180)));
				Vector2 angle = new Vector2(0, (float)angley);

				Vector2 dashDirection = (target.Center - (angle * distance)) - NPC.Center;
				dashDirection.Normalize();
				dashDirection *= speed;
				dashDirection.X = NPC.velocity.X;
				NPC.velocity = dashDirection;
				ShakeModSystem.Shake = 3;
			}


			NPC.velocity.X *= 1.02f;
			NPC.noTileCollide = true;
			NPC.noGravity = true;
				
			if (timer == 35)
			{
                Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(NPC.Center, 1024f, 32f);
                if (StellaMultiplayer.IsHost)
				{
					Projectile.NewProjectile(entitySource, NPC.Center + new Vector2(150, 150), Vector2.Zero,
						ModContent.ProjectileType<ZuiSpawnEffect>(), 0, 0f, Owner: Main.myPlayer);

					int numProjectiles = Main.rand.Next(12, 24);
					for (int p = 0; p < numProjectiles; p++)
					{
						// Rotate the velocity randomly by 30 degrees at max.
						Vector2 newVelocity = velocity.RotatedByRandom(MathHelper.ToRadians(15));
						newVelocity *= 1f - Main.rand.NextFloat(0.3f);
						Projectile.NewProjectile(entitySource, NPC.Center, -newVelocity, ModContent.ProjectileType<GoldenChildren>(), 50, 0, Owner: Main.myPlayer);
					}

					Projectile.NewProjectile(entitySource, NPC.Center, velocity, ModContent.ProjectileType<BlightShot>(), 30, 0, Owner: Main.myPlayer);
				}
			}


			if (timer == 64 && ZuiSonic < 8)
			{
				ResetTimers();
				State = ActionState.SonicDashZui;
				timer = 0;
			}

			if (timer == 64 && ZuiSonic >= 8)
			{
				// We apply an initial velocity the first tick we are in the Jump frame. Remember that -Y is up.
				if (StellaMultiplayer.IsHost)
				{
					ResetTimers();
					downtoclown = false;
					ZuiLongSlash = 0;
					ZuiSonic = 0;
					switch (Main.rand.Next(2))
					{
						case 0:
							Projectile.NewProjectile(entitySource, NPC.Center + new Vector2(150, 150), Vector2.Zero,
							ModContent.ProjectileType<ZuiSpawnEffect>(), 0, 0f, Owner: Main.myPlayer);
							int distanceY = 0;
							int distanceYa = -425;
							NPC.position.X = target.Center.X + distanceY;
							NPC.position.Y = target.Center.Y + distanceYa;
							NPC.netUpdate = true;
							NPC.noTileCollide = true;
							State = ActionState.SonicGroundpound;
							break;
						case 1:
							State = ActionState.SonicOutSpin;// ElectricityZui;
							break;
					}
				}
			}
		}




		private void SonicOutZui()
		{
			timer++;
			NPC.TargetClosest();
			Player target = Main.player[NPC.target];
			Vector2 velocity = NPC.Center.DirectionTo(target.Center) * 10;

			if (timer == 1)
			{
                int distanceY = Main.rand.Next(-50, -50);
                int distanceYa = Main.rand.Next(-425, -425);
                NPC.position.X = target.Center.X + distanceYa;
                NPC.position.Y = target.Center.Y + distanceY;

                if (StellaMultiplayer.IsHost)
				{
					Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center + new Vector2(150, 150), velocity * 0,
						ModContent.ProjectileType<ZuiSpawnEffect>(), 0, 10, Main.myPlayer, ai0: NPC.whoAmI);
				}

				ZuiSonic += 1;
				for (int i = 0; i < 150; i++)
				{
					Vector2 speeda = Main.rand.NextVector2CircularEdge(4f, 4f);
					var d = Dust.NewDustPerfect(NPC.Center, DustID.GoldFlame, speeda * 11, Scale: 3f);
					d.noGravity = true;
				}

				NPC.velocity.X -= 0.3f;
			}
			

			NPC.velocity.X *= 0.98f;
			NPC.noTileCollide = true;
			NPC.noGravity = true;

			if (timer == 64)
			{
				if (StellaMultiplayer.IsHost)
				{
					ResetTimers();
					var source = NPC.GetSource_FromThis();
					downtoclown = false;
					ZuiLongSlash = 0;
					ZuiSonic = 0;
					switch (Main.rand.Next(2))
					{
						case 0:
							Projectile.NewProjectile(source, NPC.Center + new Vector2(150, 150), Vector2.Zero,
								ModContent.ProjectileType<ZuiSpawnEffect>(), 0, 0f, Owner: Main.myPlayer);
							int distanceY = Main.rand.Next(0, 0);
							int distanceYa = Main.rand.Next(-425, -425);
							NPC.position.X = target.Center.X + distanceY;
							NPC.position.Y = target.Center.Y + distanceYa;
							NPC.netUpdate = true;
							NPC.noTileCollide = true;
							State = ActionState.SonicGroundpound;

							break;
						case 1:
							Projectile.NewProjectile(source, NPC.Center + new Vector2(150, 150), Vector2.Zero,
								ModContent.ProjectileType<ZuiSpawnEffect>(), 0, 0f, Owner: Main.myPlayer);
							int distanceYb = Main.rand.Next(0, 0);
							int distanceYab = Main.rand.Next(-425, -425);
							NPC.position.X = target.Center.X + distanceYb;
							NPC.position.Y = target.Center.Y + distanceYab;
							NPC.netUpdate = true;
							NPC.noTileCollide = true;
							State = ActionState.SonicGroundpound;// ElectricityZui;
							break;
					}
				}
			}
		}

		public override void ModifyNPCLoot(NPCLoot npcLoot)
		{
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.Consumables.Gambit>(), 1, 5, 10));
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<MagiciansCodeHat>(), 1, 1, 1));
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<RadianuiBar>(), 1, 10, 40));
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ShopNote>(), 1, 1, 1));
			npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<Items.Placeable.ZuiBossRel>()));
			npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<ZuiBomb>()));
			LeadingConditionRule notExpertRule = new LeadingConditionRule(new Conditions.NotExpert());
			notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<CompletedFlowerBag>(), minimumDropped: 1, maximumDropped: 3));
			npcLoot.Add(notExpertRule);
		}

		private void FinishResetTimers()
		{
			if (_resetTimers)
            {
                timer = 0;
                frameCounter = 0;
                frameTick = 0;
                yud = 0;
                yum = 0;
                gruber1 = 0;
                gruber2 = 0;
                rayer = 0;
				_resetTimers = false;
            }
        }

        public override bool? CanFallThroughPlatforms()
        {
            if (State == ActionState.SonicGroundpound && NPC.HasValidTarget && Main.player[NPC.target].Top.Y > NPC.Bottom.Y)
            {
                // If Flutter Slime is currently falling, we want it to keep falling through platforms as long as it's above the player
                return true;
            }

            return false;
            // You could also return null here to apply vanilla behavior (which is the same as false for custom AI)
        }

        public void ResetTimers()
		{
			if (StellaMultiplayer.IsHost)
			{
                _resetTimers = true;
                NPC.netUpdate = true;
            }
		}
	
		public override void OnKill()
		{
			SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Binding_Abyss_Spawn"), NPC.position);
			Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(base.NPC.Center, 2048f, 128f);
			var entitySource = NPC.GetSource_FromThis();
			if (StellaMultiplayer.IsHost)
			{
                NPC.NewNPC(entitySource, (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<ZuiDeath>());
            }

			NPC.SetEventFlagCleared(ref DownedBossSystem.downedZuiBoss, -1);
			ZuiQuestSystem.QuestsCompleted += 30;
			ZuiQuestSystem.SendCompleteQuestPacket();
		}
	}
}
