using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Items.Consumables;
using Stellamod.Items.Materials;
using Stellamod.Items.Placeable;
using Stellamod.Items.Weapons.Mage;
using Stellamod.Items.Weapons.Ranged.GunSwapping;
using Stellamod.NPCs.Bosses.STARBOMBER.Projectiles;
using Stellamod.Particles;
using Stellamod.Projectiles.IgniterExplosions;
using Stellamod.Tiles.Furniture;
using Stellamod.Trails;
using Stellamod.UI.Systems;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.STARBOMBER
{
    [AutoloadBossHead] // This attribute looks for a texture called "ClassName_Head_Boss" and automatically registers it as the NPC boss head ic
	public class STARBOMBER : ModNPC
	{
		public enum ActionState
		{

			StartStar,
			IdleStar,
			GunStar,
			BomberStar,
			BreakdownStar,
			SpinStar,
			SpinVerticleStar,
			SpinGroundStar,
			DisappearStar,
			TeleportStar,
			DropdownSpinStar,
			RageSpinStar,
			PullInStar,
			LaserdrillStar,
			WaitStar,
			FallStar,

		}
		// Current state
		private bool _resetTimers;
		private float _teleportX;
		private float _teleportY;
		private ActionState _state = ActionState.StartStar;
		public ActionState State
		{
			get
			{
				return _state;
			}
			set
			{
				_state = value;
                if (StellaMultiplayer.IsHost)
                    NPC.netUpdate = true;
			}
		}

        float ChargeTrailOpacity;
        bool DrawChargeTrail;
		bool DesperationPhase;
		float DesperationTimer;
		private int StarFieldFrameCounter;
		private int StarFieldFrameTick;
		private float StarFieldTimer;
		private bool StarFieldOffset;
		private bool DrawStarField;

        // Current frame
        public int frameCounter;
		// Current frame's progress
		public int frameTick;
		// Current state's timer
		public float timer;

		// AI counter
		public int counter;

		public int rippleCount = 20;
		public int rippleSize = 5;
		public int rippleSpeed = 15;
		public float distortStrength = 300f;


		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Verlia of The Moon");

			Main.npcFrameCount[Type] = 62;

			NPCID.Sets.TrailCacheLength[NPC.type] = 24;
			NPCID.Sets.TrailingMode[NPC.type] = 3;

			// Add this in for bosses that have a summon item, requires corresponding code in the item (See MinionBossSummonItem.cs)
			// Automatically group with other bosses
			NPCID.Sets.BossBestiaryPriority.Add(Type);
			NPCDebuffImmunityData debuffData = new NPCDebuffImmunityData
			{
				SpecificallyImmuneTo = new int[] {
					BuffID.Poisoned,

					BuffID.Confused // Most NPCs have this
				}
			};
			NPCID.Sets.MPAllowedEnemies[NPC.type] = true;
			NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
			NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.OnFire] = true;
			NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Frostburn2] = true;
			NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Frostburn] = true;
			NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.OnFire3] = true;

			// Influences how the NPC looks in the Bestiary
			NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers();
			drawModifiers.CustomTexturePath = "Stellamod/NPCs/Bosses/STARBOMBER/STARBOMBERPreview";
			drawModifiers.PortraitScale = 0.8f; // Portrait refers to the full picture when clicking on the icon in the bestiary
			drawModifiers.PortraitPositionYOverride = 0f;
			NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
		}
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            // We can use AddRange instead of calling Add multiple times in order to add multiple items at once
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				// Sets the preferred biomes of this town NPC listed in the bestiary.
				// With Town NPCs, you usually set this to what biome it likes the most in regards to NPC happiness.
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.UndergroundJungle,

				// Sets your NPC's flavor text in the bestiary.
				new FlavorTextBestiaryInfoElement(LangText.Bestiary(this, "Destruction is what these bring, Fenix may of purchased some things off the black market, destroy these immediately...")),

				// You can add multiple elements if you really wanted to
				// You can also use localization keys (see Localization/en-US.lang)
				new FlavorTextBestiaryInfoElement((this.FullName))
            });
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<STARBOMBERBossRel>()));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Gambit>(), 1, 1, 6));	
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<AuroreanStarI>(), 1, 20, 100));
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<STARCORE>(), 1, 1, 2));
			npcLoot.Add(ItemDropRule.AlwaysAtleastOneSuccess(
					ItemDropRule.Common(ModContent.ItemType<FurihaMKIII>(), 4, 1),
					ItemDropRule.Common(ModContent.ItemType<StarSilk>(), 4, 1, 40),
					ItemDropRule.Common(ModContent.ItemType<AlcaricMush>(), 4, 2, 5),
					ItemDropRule.Common(ModContent.ItemType<STARBUST>(), 4, 1, 1) 
			));
		}

        public override void HitEffect(NPC.HitInfo hit)
        {

			if (NPC.life <= 0)
			{
				if (!DesperationPhase)
				{
                    DesperationPhase = true;
                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/STARDEATH"));
                }

                NPC.life = 1;
            }
		
        }

        public override void SetDefaults()
		{
			NPC.Size = new Vector2(96, 65);
			NPC.damage = 1;
			NPC.defense = 45;
			NPC.lifeMax = 11000;
            NPC.HitSound = new SoundStyle("Stellamod/Assets/Sounds/Gintze_Hit") with { PitchVariance = 0.1f };
            NPC.DeathSound = new SoundStyle("Stellamod/Assets/Sounds/Gintze_Death") with { PitchVariance = 0.1f };
            NPC.knockBackResist = 0f;
			NPC.noGravity = false;
			NPC.noTileCollide = false;
			NPC.value = Item.buyPrice(gold: 10);
			NPC.SpawnWithHigherTime(30);
			NPC.boss = true;
			NPC.npcSlots = 10f;
			NPC.scale = 2f;
			NPC.alpha = 255;
			NPC.aiStyle = 0;
            NPC.takenDamageMultiplier = 0.75f;





            // Take up open spawn slots, preventing random NPCs from spawning during the fight

            // Don't set immunities like this as of 1.4:
            // NPC.buffImmune[BuffID.Confused] = true;
            // immunities are handled via dictionaries through NPCID.Sets.DebuffImmunitySets

            // Custom AI, 0 is "bound town NPC" AI which slows the NPC down and changes sprite orientation towards the target
            NPC.aiStyle = -1;

			// Custom boss bar
			NPC.BossBar = ModContent.GetInstance<AlcaBossBar>();

			// The following code assigns a music track to the boss in a simple way.
			if (!Main.dedServ)
			{
				Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/Boss6");
			}
		}
        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * balance);
        }

       

		public override void SendExtraAI(BinaryWriter writer)
		{
			writer.Write((float)State);
			writer.Write(_resetTimers);
            writer.Write(_teleportX);
            writer.Write(_teleportY);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
		{
			_state = (ActionState)reader.ReadSingle();
			_resetTimers = reader.ReadBoolean();
            _teleportX = reader.ReadSingle();
            _teleportY = reader.ReadSingle();
        }

		public float squish = 0f;
        public float WidthFunctionCharge(float completionRatio)
        {
            return (NPC.width * NPC.scale / 0.75f * (1f - completionRatio)) * 0.4f;
        }

        public Color ColorFunctionCharge(float completionRatio)
        {
            if (!DrawChargeTrail)
            {
                ChargeTrailOpacity -= 0.05f;
                if (ChargeTrailOpacity <= 0)
                    ChargeTrailOpacity = 0;
            }
            else
            {
                ChargeTrailOpacity += 0.05f;
                if (ChargeTrailOpacity >= 1)
                    ChargeTrailOpacity = 1;
            }

			Color color = Color.Pink;
            return color * ChargeTrailOpacity * (1f - completionRatio);
        }

        public PrimDrawer TrailDrawer { get; private set; } = null;
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
			SpriteEffects effects = SpriteEffects.None;

			Rectangle rect;
			originalHitbox = new Vector2(0, 60);

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


            if (TrailDrawer == null)
            {
                TrailDrawer = new PrimDrawer(WidthFunctionCharge, ColorFunctionCharge, GameShaders.Misc["VampKnives:BasicTrail"]);
            }

            GameShaders.Misc["VampKnives:BasicTrail"].SetShaderTexture(TrailRegistry.StarTrail);
            Vector2 size = new Vector2(206, 129);
            TrailDrawer.DrawPrims(NPC.oldPos, size * 0.5f - screenPos, 155);


            switch (State)
			{
				//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~


				case ActionState.StartStar:
					rect = new(0, 1 * 129, 206, 1 * 129);
					spriteBatch.Draw(texture, NPC.Center - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 800, 1, rect), drawColor, 0f, texture.AnimationFrame(ref frameCounter, ref frameTick, 800, 1, rect).Size() / 2, NPC.scale, effects, 0f);
					break;

				case ActionState.IdleStar:
					rect = new(0, 1 * 129, 206, 28 * 129);
					spriteBatch.Draw(texture, NPC.Center - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 4, 28, rect), drawColor, 0f, texture.AnimationFrame(ref frameCounter, ref frameTick, 4, 28, rect).Size() / 2, NPC.scale, effects, 0f);
					break;

				case ActionState.GunStar:
					rect = new(0, 1 * 129, 206, 28 * 129);
					spriteBatch.Draw(texture, NPC.Center - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 4, 28, rect), drawColor, 0f, texture.AnimationFrame(ref frameCounter, ref frameTick, 4, 28, rect).Size() / 2, NPC.scale, effects, 0f);
					break;

				case ActionState.BomberStar:
					rect = new(0, 1 * 129, 206, 28 * 129);
					spriteBatch.Draw(texture, NPC.Center - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 4, 28, rect), drawColor, 0f, texture.AnimationFrame(ref frameCounter, ref frameTick, 4, 28, rect).Size() / 2, NPC.scale, effects, 0f);
					break;

				case ActionState.BreakdownStar:
					rect = new(0, 29 * 129, 206, 26 * 129);
					spriteBatch.Draw(texture, NPC.Center - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 1, 26, rect), drawColor, 0f, texture.AnimationFrame(ref frameCounter, ref frameTick, 1, 26, rect).Size() / 2, NPC.scale, effects, 0f);
					break;

				case ActionState.SpinStar:
					rect = new(0, 55 * 129, 206, 7 * 129);
					spriteBatch.Draw(texture, NPC.Center - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 1, 7, rect), drawColor, 0f, texture.AnimationFrame(ref frameCounter, ref frameTick, 1, 7, rect).Size() / 2, NPC.scale, effects, 0f);
					
					break;

				case ActionState.DropdownSpinStar:
					rect = new(0, 55 * 129, 206, 7 * 129);
					spriteBatch.Draw(texture, NPC.Center - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 1, 7, rect), drawColor, 0f, texture.AnimationFrame(ref frameCounter, ref frameTick, 1, 7, rect).Size() / 2, NPC.scale, effects, 0f);
					break;

				case ActionState.SpinGroundStar:
					rect = new(0, 55 * 129, 206, 7 * 129);
					spriteBatch.Draw(texture, NPC.Center - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 1, 7, rect), drawColor, 0f, texture.AnimationFrame(ref frameCounter, ref frameTick, 1, 7, rect).Size() / 2, NPC.scale, effects, 0f);
					break;

				case ActionState.RageSpinStar:
					rect = new(0, 55 * 129, 206, 7 * 129);
					spriteBatch.Draw(texture, NPC.Center - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 1, 7, rect), drawColor, 0f, texture.AnimationFrame(ref frameCounter, ref frameTick, 1, 7, rect).Size() / 2, NPC.scale, effects, 0f);
					break;

				case ActionState.PullInStar:
					rect = new(0, 1 * 129, 206, 28 * 129);
					spriteBatch.Draw(texture, NPC.Center - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 4, 28, rect), drawColor, 0f, texture.AnimationFrame(ref frameCounter, ref frameTick, 4, 28, rect).Size() / 2, NPC.scale, effects, 0f);
					break;

				case ActionState.LaserdrillStar:
					rect = new(0, 1 * 129, 206, 28 * 129);
					spriteBatch.Draw(texture, NPC.Center - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 4, 28, rect), drawColor, 0f, texture.AnimationFrame(ref frameCounter, ref frameTick, 4, 28, rect).Size() / 2, NPC.scale, effects, 0f);
					break;

				case ActionState.SpinVerticleStar:
					rect = new(0, 55 * 129, 206, 7 * 129);
					spriteBatch.Draw(texture, NPC.Center - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 1, 7, rect), drawColor, 0f, texture.AnimationFrame(ref frameCounter, ref frameTick, 1, 7, rect).Size() / 2, NPC.scale, effects, 0f);
					break;

				case ActionState.WaitStar:
					rect = new(0, 55 * 129, 206, 1 * 129);
					spriteBatch.Draw(texture, NPC.Center - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 800, 1, rect), drawColor, 0f, texture.AnimationFrame(ref frameCounter, ref frameTick, 800, 1, rect).Size() / 2, NPC.scale, effects, 0f);
					break;

				case ActionState.FallStar:
					rect = new(0, 1 * 129, 206, 1 * 129);
					spriteBatch.Draw(texture, NPC.Center - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 800, 1, rect), drawColor, 0f, texture.AnimationFrame(ref frameCounter, ref frameTick, 800, 1, rect).Size() / 2, NPC.scale, effects, 0f);
					break;
			}


			return false;
		}

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {


			float starFieldColorProgress = StarFieldTimer / 60f;
			string texturePath = "Stellamod/NPCs/Bosses/STARBOMBER/Projectiles/STARFIELD";
			Texture2D texture = ModContent.Request<Texture2D>(texturePath).Value;
			Rectangle animationFrame = texture.AnimationFrame(ref StarFieldFrameCounter, ref StarFieldFrameTick, 1, 30, true);
			Color starFieldDrawColor = new Color(Color.White.R, Color.White.G, Color.White.B, 0) * starFieldColorProgress;
			float starFieldScale = 1;
			float starFieldRotation = 0;

			Vector2 starFieldDrawPos = NPC.Center - screenPos;
			if (StarFieldOffset)
			{
				starFieldDrawPos += new Vector2(0, -80);
			}

            spriteBatch.Draw(texture, starFieldDrawPos, animationFrame, starFieldDrawColor, starFieldRotation, 
				animationFrame.Size() / 2, starFieldScale, SpriteEffects.None, 0);
        }

        //Custom function so that I don't have to copy and paste the same thing in FindFrame


        int bee = 220;
		private Vector2 originalHitbox;
        private void FinishTeleport()
        {


            if (_teleportX != 0 || _teleportY != 0)
            {

                NPC.position.X = _teleportX;
                NPC.position.Y = _teleportY;
                _teleportX = 0;
                _teleportY = 0;
                if (StellaMultiplayer.IsHost)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero,
                        ModContent.ProjectileType<STARTELEPORTBOOM>(), 0, 0, Main.myPlayer);
                }
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


        public override void AI()
        {
            if (DrawStarField)
            {
                StarFieldTimer++;
                if (StarFieldTimer >= 60)
                {
                    StarFieldTimer = 60;
                }
            }
            else
            {
                StarFieldTimer--;
                if (StarFieldTimer <= 0)
                {
                    StarFieldTimer = 0;
                }
            }

            if (DesperationPhase)
			{
				DesperationTimer++;

                Dust.NewDust(NPC.position, 206, 129, ModContent.DustType<TSmokeDust>(), newColor: Color.Gray);
                Vector2 velocity = Main.rand.NextVector2Circular(64, 64);
                if (Main.rand.NextBool(2))
                {
                    Dust.NewDust(NPC.position, 206, 129, DustID.BoneTorch, velocity.X, velocity.Y, newColor: Color.Gray, Scale: 2f);
                }

                if (Main.rand.NextBool(2))
                {
                    velocity = Main.rand.NextVector2Circular(64, 64);
                    Dust.NewDust(NPC.position, 206, 129, DustID.Torch, velocity.X, velocity.Y, newColor: Color.Gray, Scale: 3f);
                }

                if (DesperationTimer >= 462)
                {
                    MyPlayer myPlayer = Main.LocalPlayer.GetModPlayer<MyPlayer>();
                    myPlayer.ShakeAtPosition(NPC.position, 6000, 128);

                    ScreenShaderSystem screenShaderSystem = ModContent.GetInstance<ScreenShaderSystem>();
                    screenShaderSystem.TintScreen(Color.White, 0.3f, timer: 5);

					if (StellaMultiplayer.IsHost)
					{
						Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero,
							ModContent.ProjectileType<STARBOMBERBOOM>(), 50, 2, Main.myPlayer);
					}

                    for (int i = 0; i < 80; i++)
                    {
                        Dust.NewDustPerfect(NPC.Center, ModContent.DustType<TSmokeDust>(),
                            (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.DarkGray, 1f).noGravity = true;
                    }

                    //Death Effect here
                    NPC.Kill();
				}
			}

			NPC.velocity *= 0.97f;
			bee--;
			if (bee == 0)
			{
				bee = 220;
			}

			Vector3 RGB = new(2.30f, 0.21f, 0.72f);
			// The multiplication here wasn't doing anything
			Lighting.AddLight(NPC.Center, RGB.X, RGB.Y, RGB.Z);
			
			NPC.TargetClosest();
			if (!NPC.HasValidTarget)
			{
				// If the targeted player is dead, flee
				NPC.velocity.Y += 0.5f;
				NPC.noTileCollide = true;
				NPC.noGravity = false;
				NPC.alpha++;
				// This method makes it so when the boss is in "despawn range" (outside of the screen), it despawns in 10 ticks
				NPC.EncourageDespawn(2);

				if (Main.netMode != NetmodeID.Server && Terraria.Graphics.Effects.Filters.Scene["Shockwave"].IsActive())
				{
					Terraria.Graphics.Effects.Filters.Scene["Shockwave"].Deactivate();
				}
			}

			FinishResetTimers();
			FinishTeleport();

			//Defaults
            DrawStarField = true;
            StarFieldOffset = false;

            switch (State)
			{
				case ActionState.BomberStar:
					NPC.damage = 0;
					counter++;

					if (Main.netMode != NetmodeID.Server && !Terraria.Graphics.Effects.Filters.Scene["Shockwave"].IsActive())
					{
						Terraria.Graphics.Effects.Filters.Scene.Activate("Shockwave", NPC.Center).GetShader().UseColor(rippleCount, rippleSize, rippleSpeed).UseTargetPosition(NPC.Center);

					}

					if (Main.netMode != NetmodeID.Server && Terraria.Graphics.Effects.Filters.Scene["Shockwave"].IsActive())
					{
						float progress = (180f - bee) / 60f; // Will range from -3 to 3, 0 being the point where the bomb explodes.
						Terraria.Graphics.Effects.Filters.Scene["Shockwave"].GetShader().UseProgress(progress).UseOpacity(distortStrength * (1 - progress / 3f));
					}

                    StarFieldOffset = true;
                    BombStar();
					break;



				case ActionState.StartStar:
					StarFieldOffset = true;
					NPC.damage = 0;
					counter++;
					NPC.noTileCollide = false;
					NPC.noGravity = false;
					StartStar();
					break;

				case ActionState.IdleStar:
                    StarFieldOffset = true;
                    NPC.damage = 0;
					counter++;
					NPC.noTileCollide = false;
					NPC.noGravity = false;
					IdleStar();
					break;

				case ActionState.BreakdownStar:
                    DrawStarField = false;
                    NPC.damage = 0;
					counter++;
					NPC.noTileCollide = false;
					NPC.noGravity = false;
					BreakdownStar();
					break;

				case ActionState.GunStar:
					StarFieldOffset = true;
					NPC.damage = 0;
					counter++;
					NPC.noTileCollide = false;
					NPC.noGravity = false;
					GUNSTAR();
					break;

				case ActionState.FallStar:
					NPC.damage = 0;
					counter++;

					DrawStarField = false;
					NPC.noTileCollide = false;
					NPC.noGravity = false;
					FallStar();
					break;

				case ActionState.LaserdrillStar:
					StarFieldOffset = true;
					NPC.damage = 0;
					counter++;
					NPC.noTileCollide = false;
					NPC.noGravity = false;
					LaserStar();
					break;

				case ActionState.SpinStar:
					NPC.damage = 100;
					counter++;
					NPC.noTileCollide = true;
					NPC.noGravity = true;
					SpinStar();
					break;

				case ActionState.SpinGroundStar:
					NPC.damage = 0;
					counter++;
					NPC.velocity.X *= 0f;
					NPC.noTileCollide = true;
					NPC.noGravity = true;
					SpinGroundStar();
					break;

				case ActionState.SpinVerticleStar:
					NPC.damage = 100;
					counter++;
					NPC.noTileCollide = true;
					NPC.noGravity = true;
					SpinVerticleStar();
					break;

				case ActionState.RageSpinStar:
					NPC.damage = 100;
					counter++;
					NPC.noTileCollide = true;
					NPC.noGravity = true;				
					TeleportRageStar();
					break;


				case ActionState.TeleportStar:
                    DrawStarField = false;
                    NPC.damage = 0;
					counter++;	
					TeleportStar();
					break;

				case ActionState.WaitStar:
					NPC.damage = 0;
					counter++;
					NPC.noTileCollide = true;
					NPC.noGravity = true;
					WaitStar();
					break;

				default:
					break;
			}
		}

		private void StartStar()
		{
			timer++;
			Player player = Main.player[NPC.target];
			if (timer == 0)
			{
				NPC.scale = 0f;
				NPC.alpha = 255;
			}
			if (timer == 1)
            {
				NPC.scale = 0f;
				NPC.alpha = 255;
            }
			if (timer == 2)
			{
				int distanceY = Main.rand.Next(-150, -150);
                Teleport(player.Center.X, player.Center.Y + distanceY);
                if (Main.netMode != NetmodeID.Server && Terraria.Graphics.Effects.Filters.Scene["Shockwave"].IsActive())
				{
					Terraria.Graphics.Effects.Filters.Scene["Shockwave"].Deactivate();
				}
			}

			if (timer < 101)
            {
				NPC.scale += 0.02f;
				NPC.alpha--;
            }

			if (timer == 50)
			{
				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/STARBOMBERWAKE"));	
			}

			if (timer > 130)
			{
                ResetTimers();
                State = ActionState.IdleStar;
            }
		}


		private void IdleStar()
		{
			timer++;


			if (timer > 112)
			{
                ResetTimers();
				if (StellaMultiplayer.IsHost)
				{
                    switch (Main.rand.Next(4))
                    {
                        case 0:
                            State = ActionState.BomberStar;
                            break;

                        case 1:
                            State = ActionState.BreakdownStar;
                            break;

                        case 2:
                            State = ActionState.GunStar;
                            break;

                        case 3:
                            State = ActionState.LaserdrillStar;
                            break;
                    }
                }
			}

		}

		private void GUNSTAR()
		{
			timer++;

			if (timer == 2)
			{
				if (StellaMultiplayer.IsHost)
				{
                    float speedYa = NPC.velocity.Y * Main.rand.Next(-1, -1) * 0.0f + Main.rand.Next(-4, -4) * 0f;
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y + speedYa + 110, 0, speedYa - 1 * 3,
                        ModContent.ProjectileType<STARBOMBERGUN2>(), 5, 0f, Owner: Main.myPlayer);
                }
			}
				
			
			if (timer > 560)
			{
                ResetTimers();
				if (StellaMultiplayer.IsHost)
				{
                    switch (Main.rand.Next(2))
                    {
                        case 0:
                            State = ActionState.BomberStar;
                            break;
                        case 1:
                            State = ActionState.BreakdownStar;
                            break;
                    }
                }
            }

		}
		 
		private void LaserStar()
		{
			timer++;

			if (timer == 2)
			{

				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/AssassinsKnifeHit"));
				if (StellaMultiplayer.IsHost)
				{
                    var entitySource = NPC.GetSource_FromThis();
                    NPC.NewNPC(entitySource, (int)NPC.Center.X - 10, (int)NPC.Center.Y - 200, ModContent.NPCType<STARBOMBERLASERWARNBIG>());
                }
            }

			if (timer < 180)
            {
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
							npc.velocity -= direction * 0.5f;
						}
					}
				}


			}
			if (timer > 360)
			{
				// We apply an initial velocity the first tick we are in the Jump frame. Remember that -Y is up.
				if (Main.netMode != NetmodeID.Server && Terraria.Graphics.Effects.Filters.Scene["Shockwave"].IsActive())
				{
					Terraria.Graphics.Effects.Filters.Scene["Shockwave"].Deactivate();
				}

                ResetTimers();
				if (StellaMultiplayer.IsHost)
				{
                    switch (Main.rand.Next(3))
                    {
                        case 0:
                            State = ActionState.BreakdownStar;
                            break;
                        case 1:
                            State = ActionState.GunStar;
                            break;
                        case 2:
                            State = ActionState.BomberStar;
                            break;
                    }
                }
			}
		}

        private void ChargeVisuals(float timer, float maxTimer)
        {
            float progress = timer / maxTimer;
            float minParticleSpawnSpeed = 8;
            float maxParticleSpawnSpeed = 2;
            int particleSpawnSpeed = (int)MathHelper.Lerp(minParticleSpawnSpeed, maxParticleSpawnSpeed, progress);
            if (timer % particleSpawnSpeed == 0)
            {
                for (int i = 0; i < 4; i++)
                {
                    Vector2 pos = NPC.Center + Main.rand.NextVector2CircularEdge(168, 168);
                    Vector2 vel = (NPC.Center - pos).SafeNormalize(Vector2.Zero) * 4;
                    ParticleManager.NewParticle<AVoidParticle>(pos, vel, Color.RoyalBlue, 0.2f);
                }
            }
        }

        private void BombStar()
		{
			timer++;
			ChargeVisuals(timer, 120);
			if (timer == 120)
			{
				if (StellaMultiplayer.IsHost)
                {
                    NPC.NewNPC(NPC.GetSource_FromThis(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<STARLINGBIG>());
                }

				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/STARIGNITE"));
			}

			if (timer > 540)
			{
				// We apply an initial velocity the first tick we are in the Jump frame. Remember that -Y is up.

				if (Main.netMode != NetmodeID.Server && Terraria.Graphics.Effects.Filters.Scene["Shockwave"].IsActive())
				{
					Terraria.Graphics.Effects.Filters.Scene["Shockwave"].Deactivate();
				}

                ResetTimers();
                State = ActionState.BreakdownStar;
			}
		}


		private void BreakdownStar()
		{
			timer++;


			if (timer == 26)
			{
                ResetTimers();
                if (StellaMultiplayer.IsHost)
				{
                    switch (Main.rand.Next(2))
                    {
                        case 0:
                            State = ActionState.SpinGroundStar;
                            break;

                        case 1:
                            State = ActionState.SpinStar;
                            break;
                    }
                }
            }
		}


		private void WaitStar()
		{
			timer++;

			if (timer == 5)
            {
				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/STARLAUGH"));

			}

			if (timer == 90)
            {
				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/STARWAVE"));
			}

			if (timer > 120)
			{
                ResetTimers();
                State = ActionState.SpinStar;
            }
		}


		private void FallStar()
		{
			timer++;

			if (timer == 5)
			{
				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/STARLAUGH"));

			}
			NPC.noTileCollide = false;
			NPC.noGravity = false;
			NPC.damage = 0;

			Player player = Main.player[NPC.target];



			if (timer == 2)
			{
				int distanceY = Main.rand.Next(-150, -150);
                Teleport(player.Center.X, player.Center.Y + distanceY);
            }


			if (timer < 50)
            {
				NPC.velocity.Y += 0.4f;
            }
			if (timer > 120)
			{
				NPC.scale += 0.02f;

			}

			if (timer > 220)
			{
				for (int j = 0; j < 50; j++)
				{
					Vector2 speedg = Main.rand.NextVector2CircularEdge(1f, 1f);
					ParticleManager.NewParticle(NPC.Center, speedg * 7, ParticleManager.NewInstance<BurnParticle>(), Color.RoyalBlue, Main.rand.NextFloat(0.2f, 0.8f));


				}
				// We apply an initial velocity the first tick we are in the Jump frame. Remember that -Y is up.
				NPC.scale = 2f;
                ResetTimers();
                State = ActionState.IdleStar;
			}
		}

		private void TeleportStar()
		{
			timer++;

			NPC.scale -= 0.01f;
			NPC.velocity *= 0.96f;
			if (timer == 5)
			{
				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/STARLAUGH"));

			}



			if (timer > 201)
			{
				// We apply an initial velocity the first tick we are in the Jump frame. Remember that -Y is up.
				NPC.scale = 0f;
                if (StellaMultiplayer.IsHost)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero,
                        ModContent.ProjectileType<STARTELEPORTBOOM>(), 0, 0, Main.myPlayer);
                }

                for (int j = 0; j < 25; j++)
                {
                    Vector2 speedg = Main.rand.NextVector2CircularEdge(1f, 1f);
                    Dust.NewDustPerfect(NPC.Center, ModContent.DustType<GlowDust>(), speedg * 7, newColor: Color.Pink);
                }

                ResetTimers();
				if (StellaMultiplayer.IsHost)
                {
                    switch (Main.rand.Next(2))
                    {
                        case 0:
                            State = ActionState.FallStar;
                            break;

                        case 1:
                            State = ActionState.RageSpinStar;
                            break;
                    }
                }


				// Finally, iterate through itemsToAdd and actually create the Item instances and add to the chest.item array


			}

		}

		public float Voiden = 0;
		public float missue = 0;
		private void TeleportRageStar()
		{
			timer++;
			Voiden++;
			missue++;
			Player player = Main.player[NPC.target];
			Vector2 directionToPlayer = NPC.Center.DirectionTo(player.Center);
			Vector2 velocityToPlayer = directionToPlayer * 4;
			float movementSpeed = 3;
			if(NPC.velocity.X < velocityToPlayer.X )
			{
				NPC.velocity.X += 0.2f;
				if(NPC.velocity.X > movementSpeed)
				{
					NPC.velocity.X = movementSpeed;
                }
			}
			else
			{
                NPC.velocity.X -= 0.2f;
                if (NPC.velocity.X < -movementSpeed)
                {
                    NPC.velocity.X = -movementSpeed;
                }
            }

            if (NPC.velocity.Y < velocityToPlayer.Y)
            {
                NPC.velocity.Y += 0.2f;
                if (NPC.velocity.Y > movementSpeed)
                {
                    NPC.velocity.Y = movementSpeed;
                }
            }
            else
            {
                NPC.velocity.Y -= 0.2f;
                if (NPC.velocity.Y < -movementSpeed)
                {
                    NPC.velocity.Y = -movementSpeed;
                }
            }

			if (timer == 5)
			{
				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/STARLAUGH"));
				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/STARWAVE"));
			}

			if (timer < 201)
            {
				NPC.scale += 0.02f;
			
            }

			if (timer == 2)
			{
				int distanceY = Main.rand.Next(-600, -600);
				Teleport(player.Center.X, player.Center.Y + distanceY);
			}

			if (Voiden == 5)
            {	
				for (int j = 0; j < 50; j++)
				{
					Vector2 speedg = Main.rand.NextVector2Circular(1f, 1f);
					ParticleManager.NewParticle(NPC.Center, speedg * 7, ParticleManager.NewInstance<ShadeParticle>(), Color.RoyalBlue, Main.rand.NextFloat(0.2f, 0.8f));
				}
				Voiden = 0;
			}


			if (missue == 50)
            {
				SoundEngine.PlaySound(SoundID.Item92, NPC.position);
				if (StellaMultiplayer.IsHost)
				{
					Vector2 randPosOffset = Main.rand.NextVector2Circular(64, 64);
					Vector2 randVelocity = Main.rand.NextVector2CircularEdge(15, 15);
                    Projectile.NewProjectile(NPC.GetSource_FromThis(),NPC.Center + randPosOffset, randVelocity,
                        ModContent.ProjectileType<STARROCKET>(), 25, 0f, Owner: Main.myPlayer);

                }

                for (int j = 0; j < 30; j++)
				{
					Vector2 speedg = Main.rand.NextVector2CircularEdge(1f, 1f);
					ParticleManager.NewParticle(NPC.Center, speedg * 7, ParticleManager.NewInstance<BurnParticle>(), Color.RoyalBlue, Main.rand.NextFloat(0.2f, 0.8f));
				}

				missue = 0;
            }


		

			float speed = 8f;
			NPC.noTileCollide = true;
			NPC.noGravity = true;

			if (timer < 75)
			{

				int distance = Main.rand.Next(2, 2);
				NPC.ai[3] = Main.rand.Next(1);
				double anglex = Math.Sin(NPC.ai[3] * (Math.PI / 180));
				double angley = Math.Abs(Math.Cos(NPC.ai[3] * (Math.PI / 180))) * 0;
				Vector2 angle = new Vector2((float)anglex, (float)angley);
				Vector2 dashDirection = (player.Center - (angle * distance)) - NPC.Center;
				float dashDistance = dashDirection.Length();
				dashDirection.Normalize();
				dashDirection *= speed;
				NPC.velocity = dashDirection;
				ShakeModSystem.Shake = 3;
			}

			if (timer < 302 && timer > 201)
            {
				NPC.scale -= 0.02f;	
			}

			if (timer > 303)
			{
                if (StellaMultiplayer.IsHost)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero,
                        ModContent.ProjectileType<STARTELEPORTBOOM>(), 0, 0, Main.myPlayer);
                }

                for (int j = 0; j < 48; j++)
				{
					Vector2 speedg = Main.rand.NextVector2CircularEdge(1f, 1f);
					Dust.NewDustPerfect(NPC.Center, ModContent.DustType<GlowDust>(), speedg * 7, newColor: Color.Pink);
				}

				// We apply an initial velocity the first tick we are in the Jump frame. Remember that -Y is up.
				NPC.scale = 0f;
                ResetTimers();
                State = ActionState.FallStar;
            }
		}

		private void SpinGroundStar()
		{
			timer++;
			Player player = Main.player[NPC.target];
			float speed = 14f;
			NPC.noTileCollide = true;
			NPC.noGravity = true;

			if (timer < 75)
			{
				int distance = Main.rand.Next(2, 2);
				NPC.ai[3] = Main.rand.Next(1);
				double anglex = Math.Sin(NPC.ai[3] * (Math.PI / 180));
				double angley = 0;
				Vector2 angle = new Vector2((float)anglex, (float)angley);
                Vector2 dashDirection = (player.Center - (angle * distance)) - NPC.Center;
				float dashDistance = dashDirection.Length();
				dashDirection.Normalize();
				dashDirection *= speed;
				dashDirection.Y = 0;
				NPC.velocity = dashDirection;
				ShakeModSystem.Shake = 3;
			}

			if (timer == 5)
            {
				if (StellaMultiplayer.IsHost)
				{
                    var entitySource = NPC.GetSource_FromThis();
                    NPC.NewNPC(entitySource, (int)NPC.Center.X - 10, (int)NPC.Center.Y - 200, ModContent.NPCType<STARBOMBERLASERWARN>());
                }

                SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/AssassinsKnifeHit"));
			}


			if (timer == 25)
			{
				if (StellaMultiplayer.IsHost)
				{
					var entitySource = NPC.GetSource_FromThis();
					NPC.NewNPC(entitySource, (int)NPC.Center.X - 10, (int)NPC.Center.Y - 200, ModContent.NPCType<STARBOMBERLASERWARN>());
				}
				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/AssassinsKnifeHit"));
			}

			if (timer == 45)
			{
				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/AssassinsKnifeHit"));
				if (StellaMultiplayer.IsHost)
				{
					var entitySource = NPC.GetSource_FromThis();
					NPC.NewNPC(entitySource, (int)NPC.Center.X - 10, (int)NPC.Center.Y - 200, ModContent.NPCType<STARBOMBERLASERWARN>());
				}
			}

			if (timer == 55)
			{
				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/AssassinsKnifeHit"));
				if (StellaMultiplayer.IsHost)
				{
					var entitySource = NPC.GetSource_FromThis();
					NPC.NewNPC(entitySource, (int)NPC.Center.X - 10, (int)NPC.Center.Y - 200, ModContent.NPCType<STARBOMBERLASERWARN>());
				}
			}

			if (timer == 75)
			{
				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/AssassinsKnifeHit"));
				if (StellaMultiplayer.IsHost)
				{
					var entitySource = NPC.GetSource_FromThis();
					NPC.NewNPC(entitySource, (int)NPC.Center.X - 10, (int)NPC.Center.Y - 200, ModContent.NPCType<STARBOMBERLASERWARN>());
				}
			}

			if (timer == 95)
			{
				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/AssassinsKnifeHit"));
				if (StellaMultiplayer.IsHost)
				{
					var entitySource = NPC.GetSource_FromThis();
					NPC.NewNPC(entitySource, (int)NPC.Center.X - 10, (int)NPC.Center.Y - 200, ModContent.NPCType<STARBOMBERLASERWARN>());
				}
			}

			if (timer > 110)
			{
                // We apply an initial velocity the first tick we are in the Jump frame. Remember that -Y is up.
                ResetTimers();
                if (StellaMultiplayer.IsHost)
				{
                    switch (Main.rand.Next(3))
                    {
                        case 0:
                            State = ActionState.SpinGroundStar;
                            break;

                        case 1:
                            State = ActionState.SpinStar;
                            break;

                        case 2:
                            State = ActionState.SpinVerticleStar;
                            break;
                    }
                } 
			}
		}


		private void SpinVerticleStar()
		{
			timer++;
			Player player = Main.player[NPC.target];
			float speed = 14f;
			NPC.noTileCollide = true;
			NPC.noGravity = true;

			if (timer < 75)
			{
				int distance = Main.rand.Next(2, 2);
				NPC.ai[3] = Main.rand.Next(1);
				double anglex = Math.Sin(NPC.ai[3] * (Math.PI / 180));
				double angley = Math.Abs(Math.Cos(NPC.ai[3] * (Math.PI / 180))) ;
				Vector2 angle = new Vector2(0, (float)angley);
				Vector2 dashDirection = (player.Center - (angle * distance)) - NPC.Center;
				float dashDistance = dashDirection.Length();
				dashDirection.Normalize();
				dashDirection *= speed;
				dashDirection.X = 0;
				NPC.velocity = dashDirection;
				ShakeModSystem.Shake = 3;
			}

			if (timer == 5)
			{
				if (StellaMultiplayer.IsHost)
				{
                    float speedXb = NPC.velocity.X * Main.rand.NextFloat(0f, 0f) + Main.rand.NextFloat(-4f, -4f);
                    float speedXa = NPC.velocity.X * Main.rand.NextFloat(0f, 0f) + Main.rand.NextFloat(4f, 4f);
                    float speedYa = NPC.velocity.Y * Main.rand.Next(0, 0) * 0.0f + Main.rand.Next(0, 0) * 0f;
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa, NPC.position.Y + speedYa, speedXa * 1, speedYa - 1 * 0,
                        ModContent.ProjectileType<SINESTAR>(), 50, 0f, Owner: Main.myPlayer);
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa, NPC.position.Y + speedYa, speedXb * 1, speedYa - 1 * 0,
                        ModContent.ProjectileType<SINESTAR>(), 50, 0f, Owner: Main.myPlayer);
                }

				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/gun1"));
			}

			if (timer == 45)
			{
				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/gun1"));
				if (StellaMultiplayer.IsHost)
				{
                    float speedXb = NPC.velocity.X * Main.rand.NextFloat(0f, 0f) + Main.rand.NextFloat(-4f, -4f);
                    float speedXa = NPC.velocity.X * Main.rand.NextFloat(0f, 0f) + Main.rand.NextFloat(4f, 4f);
                    float speedYa = NPC.velocity.Y * Main.rand.Next(0, 0) * 0.0f + Main.rand.Next(0, 0) * 0f;
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa, NPC.position.Y + speedYa, speedXa * 1, speedYa - 1 * 0,
                        ModContent.ProjectileType<SINESTAR>(), 50, 0f, Owner: Main.myPlayer);
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa, NPC.position.Y + speedYa, speedXb * 1, speedYa - 1 * 0,
                        ModContent.ProjectileType<SINESTAR>(), 50, 0f, Owner: Main.myPlayer);
                }
			}

			

			if (timer == 75)
			{

				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/gun1"));
				if (StellaMultiplayer.IsHost)
				{
                    float speedXb = NPC.velocity.X * Main.rand.NextFloat(0f, 0f) + Main.rand.NextFloat(-4f, -4f);
                    float speedXa = NPC.velocity.X * Main.rand.NextFloat(0f, 0f) + Main.rand.NextFloat(4f, 4f);
                    float speedYa = NPC.velocity.Y * Main.rand.Next(0, 0) * 0.0f + Main.rand.Next(0, 0) * 0f;
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa, NPC.position.Y + speedYa, speedXa * 1, speedYa - 1 * 0,
                        ModContent.ProjectileType<SINESTAR>(), 50, 0f, Owner: Main.myPlayer);
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa, NPC.position.Y + speedYa, speedXb * 1, speedYa - 1 * 0,
                        ModContent.ProjectileType<SINESTAR>(), 50, 0f, Owner: Main.myPlayer);
                }
			}

			if (timer == 105)
			{

				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/gun1"));
				if (StellaMultiplayer.IsHost)
				{
                    float speedXb = NPC.velocity.X * Main.rand.NextFloat(0f, 0f) + Main.rand.NextFloat(-4f, -4f);
                    float speedXa = NPC.velocity.X * Main.rand.NextFloat(0f, 0f) + Main.rand.NextFloat(4f, 4f);
                    float speedYa = NPC.velocity.Y * Main.rand.Next(0, 0) * 0.0f + Main.rand.Next(0, 0) * 0f;
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa, NPC.position.Y + speedYa, speedXa * 1, speedYa - 1 * 0,
                        ModContent.ProjectileType<SINESTAR>(), 50, 0f, Owner: Main.myPlayer);
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa, NPC.position.Y + speedYa, speedXb * 1, speedYa - 1 * 0,
                        ModContent.ProjectileType<SINESTAR>(), 50, 0f, Owner: Main.myPlayer);
                }
			}

			if (timer > 180)
			{
                ResetTimers();
                State = ActionState.WaitStar;
			}
		}

		public float spinst = 0;
		public float constshoot = 0;
		private void SpinStar()
		{
			timer++;

			constshoot++;

		
			Player player = Main.player[NPC.target];
	
			if(timer == 1 || timer == 241 || timer == 482)
			{
				SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/HavocCharge"), NPC.position);
			}


			if(timer == 60 || timer == 300 || timer == 540)
			{
				Vector2 directionToPlayer = NPC.Center.DirectionTo(player.Center);
				Vector2 targetVelocity = directionToPlayer * 64;
				NPC.velocity = targetVelocity;
			}

			if(timer >= 60)
			{
				NPC.damage = 100;
                NPC.noTileCollide = true;
                NPC.noGravity = true;
            }

			if(timer > 60 && timer < 240 || (timer > 300 && timer < 480) || (timer > 540))
			{
				DrawChargeTrail = true;
				NPC.velocity *= 0.99f;
				if (spinst == 0)
					spinst = 1;

				if(player.Center.Y < NPC.Center.Y)
				{
					spinst = -1;
				}else if (player.Center.Y > NPC.Center.Y)
				{
					spinst = 1;
				}

				NPC.velocity.Y += 0.4f * spinst;
			}
			else
            {
                DrawChargeTrail = false;
                NPC.noTileCollide = false;
            }
			NPC.rotation = NPC.velocity.X * 0.05f;

            if (constshoot == 70)
            {
				if (StellaMultiplayer.IsHost)
				{
                    float speedXa = NPC.velocity.X * Main.rand.NextFloat(0f, 0f) + Main.rand.NextFloat(4f, 4f);
                    float speedYa = NPC.velocity.Y * Main.rand.Next(0, 0) * 0.0f + Main.rand.Next(0, 0) * 0f;
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa + 100, NPC.position.Y + speedYa, speedXa * 5, speedYa - 1 * 0,
                        ModContent.ProjectileType<STRIKEBULLET2>(), 40, 0f, Owner: Main.myPlayer);
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa, NPC.position.Y + 70, speedXa * -5, speedYa - 1 * 0,
                        ModContent.ProjectileType<STRIKEBULLET>(), 40, 0f, Owner: Main.myPlayer);
                }

				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/SunStalker_Bomb_2"));

				constshoot = 0;

				if (StellaMultiplayer.IsHost)
				{
                    float speedXaz = NPC.velocity.Y * Main.rand.Next(-1, -1) * 0.0f + Main.rand.Next(-10, 10);
                    float speedYaz = NPC.velocity.Y * Main.rand.Next(-1, -1) * 0.0f + Main.rand.Next(-10, 10);
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X + speedXaz, NPC.position.Y + speedYaz + 110, speedXaz * 0, speedYaz - 1 * 1,
                        ProjectileID.ShadowBeamHostile, 25, 0f, Owner: Main.myPlayer);
                }
			}

			if (timer > 720)
			{
				NPC.damage = 0;
                if (StellaMultiplayer.IsHost)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero,
                        ModContent.ProjectileType<STARTELEPORTBOOM>(), 0, 0, Main.myPlayer);
                }

                for (int j = 0; j < 48; j++)
                {
                    Vector2 speedg = Main.rand.NextVector2CircularEdge(1f, 1f);
                    Dust.NewDustPerfect(NPC.Center, ModContent.DustType<GlowDust>(), speedg * 7, newColor: Color.Pink);
                }

                ResetTimers();
                State = ActionState.TeleportStar;
			}
		}

        private void FinishResetTimers()
        {
            if (_resetTimers)
            {
                timer = 0;
                frameCounter = 0;
                frameTick = 0;
                _resetTimers = false;
            }
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
			NPC.SetEventFlagCleared(ref DownedBossSystem.downedSTARBoss, -1);
			if (Main.netMode != NetmodeID.Server && Terraria.Graphics.Effects.Filters.Scene["Shockwave"].IsActive())
			{
				Terraria.Graphics.Effects.Filters.Scene["Shockwave"].Deactivate();
			}
		}
	}
}
