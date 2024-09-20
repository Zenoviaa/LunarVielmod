using Microsoft.Xna.Framework;
using ParticleLibrary;
using Stellamod.Brooches;
using Stellamod.Buffs;
using Stellamod.Buffs.Minions;
using Stellamod.Dusts;
using Stellamod.Gores.Foreground;
using Stellamod.Helpers;
using Stellamod.Items.Accessories.PicturePerfect;
using Stellamod.Items.Accessories.Runes;
using Stellamod.Items.Armors.Alsis;
using Stellamod.Items.Armors.Artisan;
using Stellamod.Items.Armors.Daedia;
using Stellamod.Items.Armors.Ducanblitz;
using Stellamod.Items.Armors.Govheil;
using Stellamod.Items.Armors.Lovestruck;
using Stellamod.Items.Armors.Terric;
using Stellamod.Items.Armors.Verl;
using Stellamod.Items.Consumables;
using Stellamod.Items.Special.Sirestias;
using Stellamod.Items.Weapons.Melee;
using Stellamod.NPCs.Bosses.Caeva;
using Stellamod.NPCs.Bosses.DaedusRework;
using Stellamod.NPCs.Bosses.DreadMire;
using Stellamod.NPCs.Bosses.DreadMire.Heart;
using Stellamod.NPCs.Bosses.Fenix;
using Stellamod.NPCs.Bosses.GothiviaNRek.Reks;
using Stellamod.NPCs.Bosses.INest;
using Stellamod.NPCs.Bosses.singularityFragment;
using Stellamod.NPCs.Bosses.SupernovaFragment;
using Stellamod.NPCs.Bosses.Verlia;
using Stellamod.NPCs.Minibosses;
using Stellamod.Particles;
using Stellamod.Projectiles;
using Stellamod.Projectiles.Ambient;
using Stellamod.Projectiles.Paint;
using Stellamod.Projectiles.Summons.Minions;
using Stellamod.Projectiles.Swords;
using Stellamod.UI.Dialogue;
using Stellamod.WorldG;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Stellamod
{


    public class MyPlayer : ModPlayer
	{
		public bool Bossdeath = false;
		public bool Boots = false;
		public int extraSlots;
		public bool TAuraSpawn;
		public bool AdvancedBrooches;
		public bool HikersBSpawn;
		public bool PlantH;
		public bool Dice;
		public bool PlantHL;
		public int increasedLifeRegen;
		public int TAuraCooldown = 600;
		public int HikersBCooldown = 30;
		public int DiceCooldown = 0;
		public bool ArcaneM;
		public bool ThornedBook;
		public int ArcaneMCooldown = 0;
		public bool ZoneMorrow = false;
		public int Timer = 0;
		public bool NotiaB;
		public int NotiaBCooldown = 0;
		public int SwordCombo;
		public int SwordComboSlash;
		public int SwordComboR;
		public int lastSelectedI;
		public bool Lovestruck;
		public bool ZoneCathedral = false;
		public int LovestruckBCooldown = 0;
		public bool ADisease;
		public bool ZoneFable = false;
		public bool ReflectionS;

		private Vector2 RandomOrig;
		private Vector2 RandomOrig2;
		private Vector2 RandomOrig3;
		public int GoldenRingCooldown = 0;
		public int GoldenSparkleCooldown = 0;
		public int RayCooldown = 0;
		public int VerliaBDCooldown = 5;
		public int BurningGBDCooldown = 5;
		public bool GovheilB;
		public bool GovheilC;
		public int GovheilBCooldown = 0;
		public bool DucanB;
		public int DucanBCooldown = 0;
		public bool Daedstruck;
		public int DaedstruckBCooldown = 1;
		public bool MasteryMagic;
		public int MasteryMagicBCooldown = 0;

		public bool ThreeTwoOneSmile;
		public int ThreeTwoOneSmileBCooldown = 1440;
		public int PaintdropBCooldown = 3;



		//--------------------------------------- Picture perfect stuff
		public int PPDefense = 0;
		public int PPDMG = 0;
		public float PPPaintDMG = 0;
		public int PPPaintDMG2 = 0;
		public bool PPPaintI = false;
		public bool PPPaintII = false;
		public bool PPPaintIII = false;
		public float PPSpeed = 0;
		public int PPCrit = 0;
		public int PPPaintTime = 0;
		public int PPFrameTime = 0;
		public bool Cameraaa = false;
		public float CameraaaTime;
		//----------------------------------------- Pikmin stuff

		public int OnionDamage = 0;
		public bool Onion1 = false;
		public bool Onion2 = false;
		public bool Onion3 = false;
		public bool Onion4 = false;



		//---------------------------------------------------- Igniter effects and uh accessory stuff

		public float IgniterVelocity = 1f;
		public int IgniterDamage = 0;
		public int IgniterStrike = 0;
		public bool LuckyW= false;
		public bool FlamedTomeDusts = false;
		public bool MagicTomeDusts = false;

		//---------------------------------------------------------------------------------------------------------------
		// Brooches
		public bool BroochSpragald;
		public int SpragaldBCooldown = 1;
		public bool BroochFrile;
		public int FrileBCooldown = 1;
		public int FrileBDCooldown = 1;
		public bool BroochFlyfish;
		public int FlyfishBCooldown = 1;
		public bool BroochMorrow;
		public int MorrowBCooldown = 1;
		public bool BroochSlime;
		public int SlimeBCooldown = 1;
		public bool BroochDiari;
		public int DiariBCooldown = 1;
		public bool BroochVerlia;
		public int VerliaBCooldown = 1;
		public bool BroochAmethyst;
		public int AmethystBCooldown = 1;
		public bool BroochAmber;
		public int AmberBCooldown = 1;
		public bool BroochLonelyBones;
		public int LonelyBonesBCooldown = 1;
		public bool BroochMagesticWood;
		public int MagesticWoodBCooldown = 1;
		public bool BroochFamiliarWood;
		public int FamiliarWoodBCooldown = 1;
		public bool BroochMerchantsCoat;
		public int MerchantsCoatBCooldown = 1;
		public bool BroochMorrowedJellies;
		public int MorrowedJelliesBCooldown = 1;
		public bool BroochAllEye;
		public int AllEyeBCooldown = 1;
		public bool BroochMOS;
		public int MOSBCooldown = 1;
		public bool BroochBonedEye;
		public int BonedEyeBCooldown = 1;
		public bool BroochGint;
		public int GintBCooldown = 1;
		public bool BroochAureBlight;
		public int AureBCooldown = 1;

		public bool BroochDread;
		public int DreadBCooldown = 1;
		public bool BroochStone;
		public int StoneBCooldown = 1;
		public bool BroochMal;
		public int MalBCooldown = 1;
		public bool BroochVixed;
		public int VixedBCooldown = 1;
		public bool BroochBear;
		public int BearBCooldown = 1;
		public bool BroochGovheill;
		public int GovheillBCooldown = 1;
		public bool BroochBurningG;
		public int BurningGBCooldown = 1;
        public bool ArchariliteSC;

        //---------------------------------------------------------------------------------------------------------------








        public float screenFlash;
        //private float screenFlashSpeed = 0.05f;
        //private Vector2? screenFlashCenter;
        private float shakeDrama;
        public Vector2 startPoint;

        public Vector2 focusPoint;
        public float focusTransition;
        public float focusLength;
        public bool shouldFocus;
        public bool Leather;
        public bool HMArmor;
        public bool FCArmor;
        public float FCArmorTime;
        public float HMArmorTime;






        public bool ZoneAbyss;
		public bool ZoneAurelus;
		public bool ZoneStarbloom;
		public bool ZoneAcid;
		public bool ZoneGovheil;
		public bool ZoneNaxtrin;
		public bool ZoneAlcadzia;
		public bool ZoneVeri;
		public bool ZoneCatacombsFire;
		public bool ZoneCatacombsTrap;
		public bool ZoneCatacombsWater;
		public bool ZoneVillage;
		public bool ZoneCinder;
		public bool ZoneDrakonic;
		public bool ZoneMechanics;
		public bool ZoneLab;
		public bool ZoneIlluria;
		public bool ZoneIshtar;
		public bool ZoneVeil;
        public bool ZoneGreenSun;



        public float AssassinsSlashes;
        public float AssassinsTime;
        public bool AssassinsSlash;
        public NPC AssassinsSlashnpc;
        public bool StealthRune;
        public bool SingularityFragment;
		public bool NiiviFight;
        public float StealthTime;

        public bool CorsageRune;
        public float CorsageTime;

        public bool DetonationRune;
		public bool Towned = false;
		public bool GIBomb = false;
		public bool RadiantBomb = false;
		public int RadiantBombCooldown = 0;

		public bool ClamsPearl;

        public bool WindRuneOn;
        public bool WindRune;
        public bool ShadeRune = false;
        public bool SpiritPendent = false;

        public NPC CrysalizerNpc;

        public int CrysalizerHits;

        public int GHETime;
        public bool GHE;
        public Vector2 GHEVector;
        public Entity GHETarget;

        public bool heart = false;
        public int heartDead = 0;

        public int IrradiatedKilled;
		public int Bridget = 0;


        public bool Dead;
		public bool DreadMonOne = false;
        public bool DreadMonTwo = false;
        public bool DreadMonThree = false;


        public bool Teric = false;
		public int TericGramTime = 0;
        public int TericGramLevel = 0;
        public bool TericGram = false;

		public bool HasAlcaliteSet;
        public bool Waterwhisps;



        public NPC VoidBlasterNPC;
        public int VoidBlasterHits;
        public int VoidBlasterHitsTime;

        public void ShakeAtPosition(Vector2 position, float distance, float strength)
        {
			LunarVeilClientConfig config = ModContent.GetInstance<LunarVeilClientConfig>();
			if (!config.ShakeToggle)
				return;
            shakeDrama = strength * (1f - base.Player.Center.Distance(position) / distance) * 0.5f;
        }

        public override void CatchFish(FishingAttempt attempt, 
			ref int itemDrop, ref int npcSpawn, ref AdvancedPopupRequest sonar, ref Vector2 sonarPosition)
        {
			int chance = 35;
			string jellyfishWarning = "You feel threatened...";
			if (Player.ZoneBeach && Main.rand.NextBool(chance))
			{
                int npc = ModContent.NPCType<GoliathJellyfish>();
                if (!NPC.AnyNPCs(npc))
                {
                    // Make sure itemDrop = -1 when summoning an NPC, as otherwise terraria will only spawn the item
                    npcSpawn = npc;
                    itemDrop = -1;

                    // Also, to make it cooler, we will make a special sonar message for when it shows up
                    sonar.Text = jellyfishWarning;
                    sonar.Color = Color.LimeGreen;
                    sonar.Velocity = Vector2.Zero;
                    sonar.DurationInFrames = 300;

                    // And that text shows up on the player's head, not on the bobber location.
                    sonarPosition = new Vector2(Player.position.X, Player.position.Y - 64);

                    return; // This is important so your code after this that rolls items will not run
                }
            }
		

            if (Player.ZoneSnow && Main.rand.NextBool(chance))
            {
                int npc = ModContent.NPCType<GoliathCryogenicJellyfish>();
                if (!NPC.AnyNPCs(npc))
                {
                    // Make sure itemDrop = -1 when summoning an NPC, as otherwise terraria will only spawn the item
                    npcSpawn = npc;
                    itemDrop = -1;

                    // Also, to make it cooler, we will make a special sonar message for when it shows up
                    sonar.Text = jellyfishWarning;
                    sonar.Color = Color.LimeGreen;
                    sonar.Velocity = Vector2.Zero;
                    sonar.DurationInFrames = 300;

                    // And that text shows up on the player's head, not on the bobber location.
                    sonarPosition = new Vector2(Player.position.X, Player.position.Y - 64);

                    return; // This is important so your code after this that rolls items will not run
                }
            }
        }

        public override void ModifyScreenPosition()
        {
            if (shouldFocus)
            {
                if (focusLength > 0f)
                {
                    if (focusTransition <= 1f)
                    {
                        Main.screenPosition = Vector2.SmoothStep(startPoint, focusPoint, focusTransition += 0.05f);
                    }
                    else
                    {
                        Main.screenPosition = focusPoint;
                    }
                    focusLength -= 0.05f;
                }
                else if (focusTransition >= 0f)
                {
                    Main.screenPosition = Vector2.SmoothStep(base.Player.Center - new Vector2(Main.screenWidth / 2, Main.screenHeight / 2), focusPoint, focusTransition -= 0.05f);
                }
                else
                {
                    shouldFocus = false;
                }
            }
            if (shakeDrama > 0.5f)
            {
                shakeDrama *= 0.92f;
                Vector2 shake = new Vector2(Main.rand.NextFloat(shakeDrama), Main.rand.NextFloat(shakeDrama));
                Main.screenPosition += shake;
            }
        }
        public void FocusOn(Vector2 pos, float length)
        {
            if (base.Player.Center.Distance(pos) < 2000f)
            {
                focusPoint = pos - new Vector2(Main.screenWidth / 2, Main.screenHeight / 2);
                focusTransition = 0f;
                startPoint = Main.screenPosition;
                focusLength = length;
                shouldFocus = true;
            }
        }
        public override bool PreKill(double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
        {
			Dead = true;
            HMArmor = false;
			Cameraaa = false;
			if (damageSource.SourceOtherIndex == 8)
                CustomDeath(ref damageSource);
            return true;
        }
        private void CustomDeath(ref PlayerDeathReason reason)
        {
            if (Player.FindBuffIndex(ModContent.BuffType<AbyssalFlame>()) >= 0)
            {
                reason = PlayerDeathReason.ByCustomReason(Player.name + " was consumed by the abyss.");
            }
            if (Player.FindBuffIndex(ModContent.BuffType<Irradiation>()) >= 0)
            {
                reason = PlayerDeathReason.ByCustomReason(Player.name + " was contaminated");
            }
            if (Player.FindBuffIndex(ModContent.BuffType<SFBuff>()) >= 0)
            {
                reason = PlayerDeathReason.ByCustomReason("You touched a black hole... WHAT DID YOU THINK WOULD HAPPEN?");
            }
        }
        public override void OnHitAnything(float x, float y, Entity victim)
        {
            if (GHE)
            {
                GHETarget = victim;
            }
            if (DetonationRune)
            {
                if (Main.rand.NextBool(7))
                {
                    var EntitySource = Player.GetSource_FromThis();
                    Projectile.NewProjectile(EntitySource, victim.Center.X, victim.Center.Y, 0, 0, ModContent.ProjectileType<DetonationBomb>(), Player.HeldItem.damage * 2, 1, Player.whoAmI, 0, 0);

                }
            }

			if (RadiantBomb && RadiantBombCooldown <= 0)
			{
				for (int d = 0; d < 4; d++)
				{
					float speedXa = Main.rand.NextFloat(.4f, .7f) + Main.rand.NextFloat(-1f, 1f);
					float speedYa = Main.rand.Next(10, 15) * 0.01f + Main.rand.Next(-1, 1);

					Projectile.NewProjectile(Player.GetSource_OnHit(victim), (int)victim.Center.X, (int)victim.Center.Y, speedXa * 0, speedYa * 0, ModContent.ProjectileType<GoldsSpawnEffect>(), 490, 1f, Player.whoAmI);
					Projectile.NewProjectile(Player.GetSource_OnHit(victim), (int)victim.Center.X, (int)victim.Center.Y, speedXa * 0.7f, speedYa * 0.6f, ModContent.ProjectileType<GoldsSlashProj>(), 400, 1f, Player.whoAmI);
					Projectile.NewProjectile(Player.GetSource_OnHit(victim), (int)victim.Center.X, (int)victim.Center.Y, speedXa * 0.5f, speedYa * 0.3f, ModContent.ProjectileType<GoldsSlashProj>(), 405, 1f, Player.whoAmI);
					Projectile.NewProjectile(Player.GetSource_OnHit(victim), (int)victim.Center.X, (int)victim.Center.Y, speedXa * 1.3f, speedYa * 0.3f, ModContent.ProjectileType<GoldsSlashProj>(), 405, 1f, Player.whoAmI);
					Projectile.NewProjectile(Player.GetSource_OnHit(victim), (int)victim.Center.X, (int)victim.Center.Y, speedXa * 1f, speedYa * 1.5f, ModContent.ProjectileType<GoldsSlashProj>(), 401, 1f, Player.whoAmI);
				}

				RadiantBombCooldown = 220;
			}
		}


        public override void ModifyHurt(ref Player.HurtModifiers modifiers)/* tModPorter Override ImmuneTo, FreeDodge or ConsumableDodge instead to prevent taking damage */
		{
			if (WindRune && !Player.HasBuff(ModContent.BuffType<GintzelSheildCD>()) && !Player.HasBuff(ModContent.BuffType<GintzelSheild>()))
			{

				if (Main.rand.NextBool(3))
				{
					var EntitySource = Player.GetSource_FromThis();
					Projectile.NewProjectile(EntitySource, Player.Center.X, Player.Center.Y, 0, 0, ModContent.ProjectileType<WindeffectGintzl>(), Player.HeldItem.damage * 2, 1, Player.whoAmI, 0, 0);
					SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Verispin"), Player.position);
					Player.AddBuff(ModContent.BuffType<GintzelSheild>(), 800);
					WindRuneOn = true;

				}
			}

			if (StealthRune && StealthTime >= 1800)
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/StealthRune"), Player.position);
                for (int m = 0; m < 20; m++)
                {
                    int num1 = Dust.NewDust(Player.position, Player.width, Player.height, DustID.Firework_Red, 0f, -2f, 0, default, .8f);
                    Main.dust[num1].noGravity = true;
                    Main.dust[num1].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
                    Main.dust[num1].position.Y += Main.rand.Next(-50, 51) * .05f - 1.5f;
                    if (Main.dust[num1].position != Player.Center)
                        Main.dust[num1].velocity = Player.DirectionTo(Main.dust[num1].position) * 6f;
                    int num = Dust.NewDust(Player.position, Player.width, Player.height, DustID.Firework_Red, 0f, -2f, 0, default, .8f);
                    Main.dust[num].noGravity = true;
                    Main.dust[num].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
                    Main.dust[num].position.Y += Main.rand.Next(-50, 51) * .05f - 1.5f;
                    if (Main.dust[num].position != Player.Center)
                        Main.dust[num].velocity = Player.DirectionTo(Main.dust[num].position) * 6f;
                }
                StealthTime = 0;
            }
            if (CorsageRune && CorsageTime == 0)
            {

                if (Main.rand.NextBool(5))
                {
                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/CorsageRune1"), Player.position);
                    for (int i = 0; i < 20; i++)
                    {
                        var entitySource = Player.GetSource_FromThis();
                        int num1 = Gore.NewGore(entitySource, new Vector2(Player.Center.X + Main.rand.Next(-10, 10), Player.Center.Y + Main.rand.Next(-10, 10)), Player.velocity, 911);
                        Main.gore[num1].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
                        Main.gore[num1].position.Y += Main.rand.Next(-50, 51) * .05f - 1.5f;
                        if (Main.dust[num1].position != Player.Center)
                        {
                            Main.dust[num1].velocity = Player.DirectionTo(Main.dust[num1].position) * 6f;
                        }

                        int num = Gore.NewGore(entitySource, new Vector2(Player.Center.X + Main.rand.Next(-10, 10), Player.Center.Y + Main.rand.Next(-10, 10)), Player.velocity, 911);

                        Main.gore[num].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
                        Main.gore[num].position.Y += Main.rand.Next(-50, 51) * .05f - 1.5f;
                        if (Main.dust[num].position != Player.Center)
                        {
                            Main.dust[num].velocity = Player.DirectionTo(Main.dust[num].position) * 6f;

                        }
                    }


                    CorsageTime = 1;
                }

            }
            if (CorsageTime >= 1 && CorsageRune)
            {
                int Sound = Main.rand.Next(1, 3);
                if (Sound == 1)
                {
                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/CorsageRune2"), Player.position);
                }
                else
                {
                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/CorsageRune3"), Player.position);
                }
                for (int i = 0; i < 200; i++)
                {
                    if (Main.npc[i].active && !Main.npc[i].friendly && Main.npc[i].type != NPCID.TargetDummy)
                    {
                        int distance = (int)Main.npc[i].Distance(Player.Center);
                        if (distance < 320)
                        {
                            Main.npc[i].AddBuff(BuffID.Poisoned, 120);
                        }

                    }
                }
                for (int i = 0; i < 20; i++)
                {
                    if (Main.npc[i].active && !Main.npc[i].friendly && Main.npc[i].type != NPCID.TargetDummy)
                    {
                        var entitySource = Main.npc[i].GetSource_FromThis();
                        int num1 = Gore.NewGore(entitySource, new Vector2(Main.npc[i].Center.X + Main.rand.Next(-10, 10), Main.npc[i].Center.Y + Main.rand.Next(-10, 10)), Main.npc[i].velocity, 911);
                        Main.gore[num1].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
                        Main.gore[num1].position.Y += Main.rand.Next(-50, 51) * .05f - 1.5f;
                        if (Main.dust[num1].position != Main.npc[i].Center)
                        {
                            Main.dust[num1].velocity = Main.npc[i].DirectionTo(Main.dust[num1].position) * 6f;
                        }

                        int num = Gore.NewGore(entitySource, new Vector2(Main.npc[i].Center.X + Main.rand.Next(-10, 10), Main.npc[i].Center.Y + Main.rand.Next(-10, 10)), Main.npc[i].velocity, 911);

                        Main.gore[num].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
                        Main.gore[num].position.Y += Main.rand.Next(-50, 51) * .05f - 1.5f;
                        if (Main.dust[num].position != Main.npc[i].Center)
                        {
                            Main.dust[num].velocity = Main.npc[i].DirectionTo(Main.dust[num].position) * 6f;
                        }

                    }
                }
            }
        }


        public override void ResetEffects()
		{
			// Reset our equipped flag. If the accessory is equipped somewhere, ExampleShield.UpdateAccessory will be called and set the flag before PreUpdateMovement
			Teric = false;
			TAuraSpawn = false;
            ArchariliteSC = false;
            HikersBSpawn = false;
			Player.lifeRegen += increasedLifeRegen;
			increasedLifeRegen = 0;
			ArcaneM = false;
			PlantH = false;
			ThornedBook = false;
            Waterwhisps = false;
            Dice = false;
			NotiaB = false;
			Lovestruck = false;
			ADisease = false;
			GovheilB = false;
			DucanB = false;
			GovheilC = false;
			Daedstruck = false;
			BroochSpragald = false;
			BroochFrile = false;
			BroochFlyfish = false;
			BroochMorrow = false;
			BroochSlime = false;
			BroochDiari = false;
			BroochVerlia = false;
			BroochAureBlight = false;
			BroochGint = false;
			BroochDread = false;
			BroochMal = false;
			BroochVixed = false;
			BroochBear = false;
			BroochGovheill = false;
			BroochBurningG = false;
			BroochStone = false;
			HasAlcaliteSet = false;

            ReflectionS = false;
			SpiritPendent = false;
            GHE = false;
            ShadeRune = false;
            FCArmor = false;
            ClamsPearl = false;
            HMArmor = false;
			Cameraaa = false;
			DetonationRune = false;
            CorsageRune = false;
            StealthRune = false;
            Leather = false;
			MasteryMagic = false;
			WindRune = false;
			RadiantBomb = false;
			GIBomb = false;

			if (SwordComboR <= 0)
			{
				SwordCombo = 0;
				SwordComboR = 0;
			}
			else
			{
				SwordComboR--;
			}

			PPDefense = 0;
			PPDMG = 0;
			PPPaintDMG = 0;
			PPPaintDMG2 = 0;
			PPPaintI = false;
			PPPaintII = false;
			PPPaintIII = false;
			PPSpeed = 0;
			PPCrit = 0;
			PPPaintTime = 0;
			PPFrameTime = 0;
			Cameraaa = false;

			 Onion1 = false;
			 Onion2 = false;
			 Onion3 = false;
			 Onion4 = false;
			 OnionDamage = 0;




		IgniterVelocity = 1f;
			IgniterDamage = 0;
			IgniterStrike = 0;
			LuckyW = false;
			FlamedTomeDusts = false;
			MagicTomeDusts = false;
		}




		public override void UpdateDead()
		{
			ResetStats();
			
		}
		
		public void ResetStats()
		{
			Bossdeath = false;
			WindRune = false;



		}


        public static bool AuroreanBool;
        public static float Aurorean;
        public static float AuroreanB = 0.5f;
        public override void PostUpdateMiscEffects()
		{
	
            Player.ManageSpecialBiomeVisuals("Stellamod:VeilSky", ZoneVeil);
            base.Player.ManageSpecialBiomeVisuals("Stellamod:GovheilSky", ZoneFable);

            base.Player.ManageSpecialBiomeVisuals("Stellamod:GreenSunSky", EventWorld.GreenSun && ZoneAcid);

            base.Player.ManageSpecialBiomeVisuals("Stellamod:ChaosD", EventWorld.ChaosD && Player.ZoneBeach);
            base.Player.ManageSpecialBiomeVisuals("Stellamod:Veil", ZoneVeil);

            base.Player.ManageSpecialBiomeVisuals("Stellamod:Starbloom", EventWorld.Aurorean && (Player.ZoneOverworldHeight || Player.ZoneSkyHeight));
			base.Player.ManageSpecialBiomeVisuals("Stellamod:Aurelus", ZoneAurelus);
			base.Player.ManageSpecialBiomeVisuals("Stellamod:Illuria", ZoneIlluria);
			base.Player.ManageSpecialBiomeVisuals("Stellamod:Acid", ZoneAcid);
			base.Player.ManageSpecialBiomeVisuals("Stellamod:Lab", ZoneLab);
			base.Player.ManageSpecialBiomeVisuals("Stellamod:Ishtar", ZoneIshtar);
			base.Player.ManageSpecialBiomeVisuals("Stellamod:Veriplant", ZoneVeri);
			base.Player.ManageSpecialBiomeVisuals("Stellamod:Gintzing", EventWorld.Gintzing);
            base.Player.ManageSpecialBiomeVisuals("Stellamod:Daedussss", NPC.AnyNPCs(ModContent.NPCType<DaedusR>()));


            base.Player.ManageSpecialBiomeVisuals("Stellamod:Jellyfish1", NPC.AnyNPCs(ModContent.NPCType<GoliathJellyfish>()));
			base.Player.ManageSpecialBiomeVisuals("Stellamod:Jellyfish2", NPC.AnyNPCs(ModContent.NPCType<GoliathCryogenicJellyfish>()));
			base.Player.ManageSpecialBiomeVisuals("Stellamod:Govheil", ZoneGovheil);
            base.Player.ManageSpecialBiomeVisuals("Stellamod:Verlia", NPC.AnyNPCs(ModContent.NPCType<VerliaB>()));
			base.Player.ManageSpecialBiomeVisuals("Stellamod:Mechanics", ZoneMechanics);
		}

		public override IEnumerable<Item> AddStartingItems(bool mediumCoreDeath)
		{

			return (IEnumerable<Item>)(object)new Item[2]
			{
				new Item(ModContent.ItemType<SirestiasStarterBag>(), 1, 0),
			   
				
				new Item(ModContent.ItemType<SiresMail>(), 1, 0),
            };
		}

        public override void OnEnterWorld()
        {
            Main.NewText(LangText.Misc("EnterWorld"));
        }
        public override void PostUpdate()
        {
			if (Main.netMode != NetmodeID.Server)
			{

				if (!Sirestiastalk)
				{

					DialogueSystem dialogueSystem = ModContent.GetInstance<DialogueSystem>();

					//2. Create a new instance of your dialogue
					SirestiasBeginDialogue exampleDialogue = new SirestiasBeginDialogue();

					//3. Start it
					dialogueSystem.StartDialogue(exampleDialogue);

					Sirestiastalk = true;
				}
				if (NPC.downedPlantBoss && Sirestiastalk && !Zuitalk)
				{

					DialogueSystem dialogueSystem = ModContent.GetInstance<DialogueSystem>();

					//2. Create a new instance of your dialogue
					ZuiPlantDialogue exampleDialogue = new ZuiPlantDialogue();

					//3. Start it
					dialogueSystem.StartDialogue(exampleDialogue);

					Zuitalk = true;
				}


				if (!DreadMonOne && DownedBossSystem.downedDreadMonolith1)
				{

					DialogueSystem dialogueSystem = ModContent.GetInstance<DialogueSystem>();

					//2. Create a new instance of your dialogue
					DreadDialogue1 exampleDialogue = new DreadDialogue1();

					//3. Start it
					dialogueSystem.StartDialogue(exampleDialogue);

					DreadMonOne = true;
				}

				if (!DreadMonTwo && DownedBossSystem.downedDreadMonolith2)
				{

					DialogueSystem dialogueSystem = ModContent.GetInstance<DialogueSystem>();

					//2. Create a new instance of your dialogue
					DreadDialogue2 exampleDialogue = new DreadDialogue2();

					//3. Start it
					dialogueSystem.StartDialogue(exampleDialogue);

					DreadMonTwo = true;
				}

				if (!DreadMonThree && DownedBossSystem.downedDreadMonolith3)
				{

					DialogueSystem dialogueSystem = ModContent.GetInstance<DialogueSystem>();

					//2. Create a new instance of your dialogue
					DreadDialogue3 exampleDialogue = new DreadDialogue3();

					//3. Start it
					dialogueSystem.StartDialogue(exampleDialogue);

					DreadMonThree = true;
				}
			}

            if (VoidBlasterHits >= 0)
			{
				VoidBlasterHitsTime++;
				if(VoidBlasterHitsTime >= 100)
				{
					VoidBlasterHits = 0;
                    VoidBlasterHitsTime = 0;

                }

			}


			if (Aurorean >= 0.5f)
            {
                AuroreanBool = true;
            }
            if (Aurorean <= 0f)
            {
                AuroreanBool = false;

            }

            if (AuroreanBool)
			{
                AuroreanB += 0.02f;
                Aurorean -= 0.02f;

			}
			else
            {
                AuroreanB -= 0.02f;
                Aurorean += 0.02f;
            }

            Player player = Main.LocalPlayer;
            if (!player.active)
				return;
			MyPlayer CVA = player.GetModPlayer<MyPlayer>();
			if (Dead)
            {
                HMArmorTime = 0;
                HMArmor = false;
				CameraaaTime = 0;
				Cameraaa = false;
				Dead = false;

            }



            //player.extraAccessorySlots = extraAccSlots; dont actually use, it'll fuck things up
            if (WindRuneOn && !Player.HasBuff(ModContent.BuffType<GintzelSheild>() ))
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/SwordSlice"), Player.position);
                WindRuneOn = false;
                player.AddBuff(ModContent.BuffType<GintzelSheildCD>(), 900);
            }
            if (!WindRune && Player.HasBuff(ModContent.BuffType<GintzelSheild>()))
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/SwordSlice"), Player.position);
                player.ClearBuff(ModContent.BuffType<GintzelSheild>());
            }
            if (GHE)
            {
                if (GHETarget.active)
                {
                    GHETime++;
                    if (GHETime >= 30)
                    {
                        Vector2 direction = Vector2.Normalize(GHETarget.Center - Player.Center) * 8.5f;
                        GHETime = 0;
                        GHEVector.X = Main.rand.NextFloat(GHETarget.Center.X - 130, GHETarget.Center.X + 130);
                        GHEVector.Y = Main.rand.NextFloat(GHETarget.Center.Y - 130, GHETarget.Center.Y + 130);
                        var EntitySource = GHETarget.GetSource_FromThis();
                    //    Projectile.NewProjectile(EntitySource, GHEVector.X, GHEVector.Y, direction.X, direction.Y, ModContent.ProjectileType<GhostExcaliburProj>(), 42, 1, Player.whoAmI, 0, 0);
                    }
                }
            }

            bool expertMode = Main.expertMode;
            if (NPC.AnyNPCs(ModContent.NPCType<DreadMire>()) || NPC.AnyNPCs(ModContent.NPCType<DreadMiresHeart>()))
            {

            }
            else
            {
                heart = false;
                heartDead = 0;
            }
            if (NPC.AnyNPCs(ModContent.NPCType<IrradiatedNest>()))
            {

            }
            else
            {
                IrradiatedKilled = 0;
            }
            if (NPC.AnyNPCs(ModContent.NPCType<SingularityFragment>()) || 
				NPC.AnyNPCs(ModContent.NPCType<ALCADHOLE>()) || 
				NPC.AnyNPCs(ModContent.NPCType<SupernovaFragment>()) ||
                NPC.AnyNPCs(ModContent.NPCType<VerliaB>()))
            {
                SingularityFragment = true;
            }
			else if (ZoneIlluria)
			{
				SingularityFragment = true;
			}
			else
            {
                SingularityFragment = false;
            }


			if (SingularityFragment || NiiviFight)
            {
                if (Main.shimmerAlpha <= 1)
                {
                    Main.shimmerAlpha += 0.02f;
                }
                else
                {
                    Main.shimmerAlpha = 1.02f;
                }
                if (Main.shimmerBrightenDelay <= 0.2f)
                {
                    Main.shimmerBrightenDelay += 0.05f;
                }
                else
                {
                    Main.shimmerBrightenDelay = 0.811f;
                }
                if (Main.shimmerDarken <= 1.4f)
                {
                    Main.shimmerDarken += 0.06f;
                }
                else
                {
                    Main.shimmerDarken = 1.41f;
                }
            }
            else
            {
                if (Main.shimmerAlpha >= 0)
                {
                    Main.shimmerAlpha -= 0.01f;
                }
                else
                {
                    Main.shimmerAlpha = 0f;
                }
                if (Main.shimmerBrightenDelay >= 0f)
                {
                    Main.shimmerBrightenDelay -= 0.01f;
                }
                else
                {
                    Main.shimmerBrightenDelay = 0f;
                }
                if (Main.shimmerDarken >= 0f)
                {
                    Main.shimmerDarken -= 0.01f;
                }
                else
                {
                    Main.shimmerDarken = 0f;
                }
            }

            if (AssassinsSlash)
            {
                AssassinsTime++;
                if (AssassinsTime >= 8)
                {
                    AssassinsSlashes += 1;
                    if (AssassinsSlashes >= 7)
                    {
                        for (int i = 0; i < 14; i++)
                        {
                            Dust.NewDustPerfect(AssassinsSlashnpc.Center, ModContent.DustType<SmokeDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, default(Color), 1f).noGravity = true;
                        }
                        AssassinsSlashnpc = null;
                        AssassinsSlashes = 0;
                        AssassinsTime = 0;
                        AssassinsSlash = false;
                    }
                    if (AssassinsSlashnpc.active == false)
                    {
                        AssassinsSlashnpc = null;
                        AssassinsSlashes = 0;
                        AssassinsTime = 0;
                        AssassinsSlash = false;
                    }
                    AssassinsTime = 0;
                    var EntitySource = AssassinsSlashnpc.GetSource_FromThis();


                    Projectile.NewProjectile(EntitySource, AssassinsSlashnpc.Center.X, AssassinsSlashnpc.Center.Y, 0, 0, ModContent.ProjectileType<AssassinsSpawnEffect>(), Player.HeldItem.damage * 2, 1, Player.whoAmI, 0, 0);
                    Projectile.NewProjectile(EntitySource, AssassinsSlashnpc.Center.X, AssassinsSlashnpc.Center.Y, 0, 0, ModContent.ProjectileType<AssassinsSlashProj>(), 0, 1, Player.whoAmI, 0, 0);
                }
            }

			if (SwordComboSlash > 5)
            {
				SwordComboSlash = 0;
            }

            if (HMArmor)
            {
                HMArmorTime++;
                if (HMArmorTime <= 1)
                {
                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/ArcharilitDrone3"), player.position);
					var EntitySource = Player.GetSource_FromThis();

					Projectile.NewProjectile(EntitySource, player.Center.X, player.Center.Y, 0, 0, ModContent.ProjectileType<HMArncharMinionRightProj>(), Player.HeldItem.damage * 2, 1, Player.whoAmI, 0, 0);
                    Projectile.NewProjectile(EntitySource, player.Center.X, player.Center.Y, 0, 0, ModContent.ProjectileType<HMArncharMinionLeftProj>(), Player.HeldItem.damage * 2, 1, Player.whoAmI, 0, 0);
                    player.AddBuff(ModContent.BuffType<HMMinionBuff>(), 99999);
                }

            }
            else
            {
                player.ClearBuff(ModContent.BuffType<HMMinionBuff>());
                HMArmorTime = 0;
            }



			if (Cameraaa)
			{
				CameraaaTime++;
				if (CameraaaTime <= 1)
				{
					SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/DMHeart__Vomit3"), player.position);
					var EntitySource = Player.GetSource_FromThis();

					Projectile.NewProjectile(EntitySource, player.Center.X, player.Center.Y, 0, 0, ModContent.ProjectileType<SmileForCamera>(), Player.HeldItem.damage * 0, 1, Player.whoAmI, 0, 0);
				
					player.AddBuff(ModContent.BuffType<CameraMinBuff>(), 99999);
				}

			}
			else
			{
				player.ClearBuff(ModContent.BuffType<CameraMinBuff>());
				CameraaaTime = 0;
			}


			if (FCArmor)
            {
                FCArmorTime++;
                if (FCArmorTime <= 1)
                {
                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/CorsageRune1"), Player.position);
                    var EntitySource = Player.GetSource_FromThis();
                    Projectile.NewProjectile(EntitySource, player.Center.X, player.Center.Y, 0, 0, ModContent.ProjectileType<FCMinionProj>(), Player.HeldItem.damage * 2, 1, Player.whoAmI, 0, 0);
                    player.AddBuff(ModContent.BuffType<FCMinionBuff>(), 99999);
                }

            }
            else
            {
                player.ClearBuff(ModContent.BuffType<FCMinionBuff>());
                FCArmorTime = 0;
            }
			
			if (ZoneAcid　|| ZoneLab)
            {
                if (player.wet)
                {
                    player.AddBuff(ModContent.BuffType<Irradiation>(), 30);
                }

				//Update Rain
                Main.raining = true;

				//That way, if it is already raining, it won't be overriden
				//And if it is not raining, it'll just be permanent until you leave the biome
				if (Main.rainTime <= 2)
					Main.rainTime = 2;
				Main.maxRaining = 0.8f;
                Main.maxRain = 140;


				//Create Gores
                float goreScale = Main.rand.NextFloat(0.5f, 0.9f);
                int x = (int)(Main.windSpeedCurrent > 0 ? Main.screenPosition.X - 100 : Main.screenPosition.X + Main.screenWidth + 100);
                int y = (int)Main.screenPosition.Y + Main.rand.Next(-100, Main.screenHeight);
                int a = Gore.NewGore(Player.GetSource_FromThis(), new Vector2(x, y), Vector2.Zero, GoreID.TreeLeaf_Jungle, goreScale);

                Main.gore[a].rotation = 0f;
                Main.gore[a].velocity.Y = Main.rand.NextFloat(1f, 3f);
            }


            if (ZoneAbyss)
            {
                player.AddBuff(ModContent.BuffType<DarkHold>(), 10);
            }

			if (ZoneAurelus)
			{
				player.AddBuff(ModContent.BuffType<DarkHold>(), 10);
			}
			
			if (ZoneIshtar && !DownedBossSystem.downedZuiBoss)
			{
				player.AddBuff(ModContent.BuffType<SigfriedsInsanity>(), 10);
			}

			if (CorsageTime >= 1)
            {
                var entitySource = Player.GetSource_FromThis();
                if (Main.rand.NextBool(5))
                {
                    int a = Gore.NewGore(entitySource, new Vector2(Player.Center.X + Main.rand.Next(-10, 10), Player.Center.Y + Main.rand.Next(-10, 10)), Player.velocity, 911);
                    Main.gore[a].timeLeft = 20;
                    Main.gore[a].scale = Main.rand.NextFloat(.5f, 1f);
                }
                Player.lifeRegen += 30;
                CorsageTime++;
                if (CorsageTime >= 300)
                {

                    CorsageTime = 0;
                }
            }







			if (HikersBSpawn && HikersBCooldown <= 0)
			{
				Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Player.velocity * -1.1f, ModContent.ProjectileType<Stump>(), 10, 1f, Player.whoAmI);
				HikersBCooldown = 30;
			}




			if (NotiaB && NotiaBCooldown == 301)
			{
				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Arcaneup"));
				for (int j = 0; j < 1; j++)
				{
					Vector2 speed = Main.rand.NextVector2Circular(0.1f, 1f);
					Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, speed * 3, ModContent.ProjectileType<Noti>(), 120, 1f, Player.whoAmI);
				}


			}
		/*	if (NotiaB && NotiaBCooldown > 300)
			{
				Player.GetDamage(DamageClass.Magic) *= 2f;
				Player.GetDamage(DamageClass.Ranged) *= 2f;

			}*/
			if (NotiaB && NotiaBCooldown == 420)
			{
				NotiaBCooldown = 0;


			}

			if (Daedstruck && DaedstruckBCooldown == 0)
			{
				DaedstruckBCooldown = 600;
				Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Player.velocity * 0f, ModContent.ProjectileType<LightBomb>(), 30, 1f, Player.whoAmI);

			}
	
			
			if (MasteryMagic && MasteryMagicBCooldown <= 0)
			{
				Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Player.velocity * -1f, ModContent.ProjectileType<MasteryofMagic>(), 0, 1f, Player.whoAmI);

				Player.AddBuff(ModContent.BuffType<MasteryMagic>(), 1000);
				MasteryMagicBCooldown = 1000;
			}







			if (GovheilB && GovheilBCooldown == 301)
			{
				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Arcaneup"));
				for (int j = 0; j < 1; j++)
				{
					Vector2 speed = Main.rand.NextVector2Circular(0.1f, 1f);
					Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, speed * 3, ModContent.ProjectileType<GovheilBows>(), 20, 1f, Player.whoAmI);
				}
			}

			/*if (GovheilB && GovheilBCooldown > 300)
			{
				Player.GetDamage(DamageClass.Magic) *= 2f;
				Player.GetDamage(DamageClass.Summon) *= 2f;

			}*/
			if (GovheilB && GovheilBCooldown == 540)
			{
				GovheilBCooldown = 0;


			}



			if (GovheilC && GovheilBCooldown == 301)
			{
				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Arcaneup"));
				for (int j = 0; j < 1; j++)
				{
					Vector2 speed = Main.rand.NextVector2Circular(0.1f, 1f);
					Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, speed * 3, ModContent.ProjectileType<GovheilSwords>(), 25, 1f, Player.whoAmI);
				}


			}

			if (DucanB && DucanBCooldown == 520)
			{
				DucanBCooldown = 0;


			}



			if (DucanB && DucanBCooldown == 301)
			{
				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Arcaneup"));
				for (int j = 0; j < 1; j++)
				{
					Vector2 speed = Main.rand.NextVector2Circular(0.1f, 1f);
					Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, speed * 3, 
						ModContent.ProjectileType<Dulcans>(), 60, 1f, Player.whoAmI);
				}


			}

			/*		if (GovheilC && GovheilBCooldown > 300)
					{
						Player.GetDamage(DamageClass.Ranged) *= 2f;
						Player.GetDamage(DamageClass.Melee) *= 2f;

					}*/
			if (GovheilC && GovheilBCooldown == 520)
			{
				GovheilBCooldown = 0;


			}

			#region 321smile


			if (ThreeTwoOneSmile && ThreeTwoOneSmileBCooldown == 180)
			{
				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Three"));
				for (int j = 0; j < 5; j++)
				{
					Vector2 speed = Main.rand.NextVector2Circular(1f, 1f);
					Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, speed * 5, ModContent.ProjectileType<Paint2>(), 25, 1f, Player.whoAmI);
				}


			}

			if (ThreeTwoOneSmile && ThreeTwoOneSmileBCooldown == 120)
			{
				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Two"));
				for (int j = 0; j < 5; j++)
				{
					Vector2 speed = Main.rand.NextVector2Circular(1f, 1f);
					Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, speed * 5, ModContent.ProjectileType<Paint3>(), 25, 1f, Player.whoAmI);
				}


			}

			if (ThreeTwoOneSmile && ThreeTwoOneSmileBCooldown == 60)
			{
				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/One"));
				for (int j = 0; j < 5; j++)
				{
					Vector2 speed = Main.rand.NextVector2Circular(1f, 1f);
					Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, speed * 5, ModContent.ProjectileType<Paint2>(), 25, 1f, Player.whoAmI);
				}


			}

			if (ThreeTwoOneSmile && ThreeTwoOneSmileBCooldown == 0)
			{
				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/zero"));
				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Binding_Abyss_Spawn"));
				for (int j = 0; j < 5; j++)
				{
					Vector2 speed = Main.rand.NextVector2Circular(1f, 1f);
					Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, speed * 5, ModContent.ProjectileType<Paint3>(), 25, 1f, Player.whoAmI);

					
				}
				Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Player.velocity, ModContent.ProjectileType<Artbar>(), 0, 1f, Player.whoAmI);
				ThreeTwoOneSmileBCooldown = 1720 + PPPaintTime;
			}

			if (ThreeTwoOneSmile && PaintdropBCooldown == 0)
            {
				RandomOrig3 = new Vector2(-15, (Main.rand.NextFloat(0f, 20f)));
				Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center + RandomOrig3, Player.velocity * 0f, 
					ModContent.ProjectileType<Meatball4>(), 0, 1f, Player.whoAmI);

				PaintdropBCooldown = 25;
			}


			#endregion


			if (Boots)
			{
				if (Player.controlJump)
				{
					const float jumpSpeed = 6.01f;
					if (Player.gravDir == 1)
					{
						Player.velocity.Y -= Player.gravDir * 1f;
						if (Player.velocity.Y <= -jumpSpeed) Player.velocity.Y = -jumpSpeed;
						Dust.NewDust(new Vector2(Player.position.X, Player.position.Y + Player.height), Player.width, 0, ModContent.DustType<Dusts.Sparkle>());
					}
					else
					{
						Player.velocity.Y += Player.gravDir * 0.5f;
						if (Player.velocity.Y >= jumpSpeed) Player.velocity.Y = jumpSpeed;
					}
				}
			}

			
			if (EventWorld.GintzingBoss)
			{
				player.AddBuff(ModContent.BuffType<Gintzingwinds>(), 100);
			}

			if (Player.HasBuff<Gintzingwinds>()) 
            {
				if (ModContent.GetInstance<LunarVeilClientConfig>().ParticlesToggle == true)
                {
					for (int j = 0; j < 1; j++)
					{
						RandomOrig3 = new Vector2(Player.width / 2, Player.height / 2) + new Vector2(Main.rand.NextFloat(-900f, 900f), (Main.rand.NextFloat(-600f, 600f)));
						RandomOrig2 = new Vector2(Player.width / 2, Player.height / 2) + new Vector2(Main.rand.NextFloat(-1600f, 1600f), (Main.rand.NextFloat(-900f, 900f)));
						RandomOrig = new Vector2(Player.width / 2, Player.height / 2) + new Vector2(Main.rand.NextFloat(-1800f, 1800f), (Main.rand.NextFloat(-1200f, 1200f)));

						Vector2 speed = new Vector2(4, 0);
						ParticleManager.NewParticle(Player.Center - RandomOrig, speed, ParticleManager.NewInstance<WindParticle>(), Color.RoyalBlue, Main.rand.NextFloat(0.2f, 0.8f));
					}
				}
			

				Main.GraveyardVisualIntensity = 0.8f;
				Main.windPhysicsStrength = 90;
			}

			if (ModContent.GetInstance<LunarVeilClientConfig>().ParticlesToggle == true && ZoneMechanics)
			{
				Main.GraveyardVisualIntensity = 0.6f;


			}

			if (ModContent.GetInstance<LunarVeilClientConfig>().ParticlesToggle == true && (ZoneFable || ZoneMorrow))
			{
				Main.GraveyardVisualIntensity = 0.4f;
				Main.windPhysicsStrength = 50;


				GoldenRingCooldown++;

				GoldenSparkleCooldown++;
				RayCooldown++;



				for (int j = 0; j < 1; j++)
				{
					RandomOrig3 = new Vector2(Player.width / 2, Player.height / 2) + new Vector2(Main.rand.NextFloat(-900f, 900f), (Main.rand.NextFloat(-600f, 600f)));
					RandomOrig2 = new Vector2(Player.width / 2, Player.height / 2) + new Vector2(Main.rand.NextFloat(-1600f, 1600f), (Main.rand.NextFloat(-900f, 900f)));
					RandomOrig = new Vector2(Player.width / 2, Player.height / 2) + new Vector2(Main.rand.NextFloat(-1800f, 1800f), (Main.rand.NextFloat(-1200f, 1200f)));

					Vector2 speed = Main.rand.NextVector2Circular(1f, 1f);
					Vector2 speed2 = Main.rand.NextVector2Circular(0.1f, 0.1f);
					ParticleManager.NewParticle(Player.Center - RandomOrig, speed2 * 3, ParticleManager.NewInstance<FabledParticle>(), Color.RoyalBlue, Main.rand.NextFloat(0.2f, 0.8f));


				}


				for (int j = 0; j < 2; j++)
				{
					RandomOrig3 = new Vector2(Player.width / 2, Player.height / 2) + new Vector2(Main.rand.NextFloat(-900f, 900f), (Main.rand.NextFloat(-600f, 600f)));
					RandomOrig2 = new Vector2(Player.width / 2, Player.height / 2) + new Vector2(Main.rand.NextFloat(-1600f, 1600f), (Main.rand.NextFloat(-900f, 900f)));
					RandomOrig = new Vector2(Player.width / 2, Player.height / 2) + new Vector2(Main.rand.NextFloat(-1800f, 1800f), (Main.rand.NextFloat(-1200f, 1200f)));

					Vector2 speed = Main.rand.NextVector2Circular(1f, 1f);
					Vector2 speed2 = Main.rand.NextVector2Circular(0.1f, 0.1f);
					ParticleManager.NewParticle(Player.Center - RandomOrig2, speed * 2, ParticleManager.NewInstance<SparkleTrailParticle>(), Color.RoyalBlue, Main.rand.NextFloat(0.2f, 0.8f));


				}

				for (int j = 0; j < 1; j++)
				{
					RandomOrig3 = new Vector2(Player.width / 2, Player.height / 2) + new Vector2(Main.rand.NextFloat(-900f, 900f), (Main.rand.NextFloat(-600f, 600f)));
					RandomOrig2 = new Vector2(Player.width / 2, Player.height / 2) + new Vector2(Main.rand.NextFloat(-1600f, 1600f), (Main.rand.NextFloat(-900f, 900f)));
					RandomOrig = new Vector2(Player.width / 2, Player.height / 2) + new Vector2(Main.rand.NextFloat(-1800f, 1800f), (Main.rand.NextFloat(-1200f, 1200f)));

					Vector2 speed = Main.rand.NextVector2Circular(1f, 1f);
					Vector2 speed2 = Main.rand.NextVector2Circular(0.1f, 0.1f);
					ParticleManager.NewParticle(Player.Center - RandomOrig3, speed * 0.5f, ParticleManager.NewInstance<FabledParticle>(), Color.RoyalBlue, Main.rand.NextFloat(0.2f, 0.8f));


				}
				if (GoldenRingCooldown > 2)
                {
					for (int j = 0; j < 1; j++)
					{
						RandomOrig3 = new Vector2(Player.width / 2, Player.height / 2) + new Vector2(Main.rand.NextFloat(-900f, 900f), (Main.rand.NextFloat(-600f, 600f)));
						RandomOrig2 = new Vector2(Player.width / 2, Player.height / 2) + new Vector2(Main.rand.NextFloat(-1600f, 1600f), (Main.rand.NextFloat(-900f, 900f)));
						RandomOrig = new Vector2(Player.width / 2, Player.height / 2) + new Vector2(Main.rand.NextFloat(-1800f, 1800f), (Main.rand.NextFloat(-1200f, 1200f)));

						Vector2 speed = Main.rand.NextVector2Circular(1f, 1f);
						Vector2 speed2 = Main.rand.NextVector2Circular(0.1f, 0.1f);
						ParticleManager.NewParticle(Player.Center - RandomOrig3, speed * 1, ParticleManager.NewInstance<GoldRingParticle>(), Color.RoyalBlue, Main.rand.NextFloat(0.2f, 0.8f));

						GoldenRingCooldown = 0;
					}
				}

				if (GoldenSparkleCooldown > 100)
				{
					for (int j = 0; j < 1; j++)
					{
						RandomOrig3 = new Vector2(Player.width / 2, Player.height / 2) + new Vector2(Main.rand.NextFloat(-900f, 900f), (Main.rand.NextFloat(-600f, 600f)));
						RandomOrig2 = new Vector2(Player.width / 2, Player.height / 2) + new Vector2(Main.rand.NextFloat(-1600f, 1600f), (Main.rand.NextFloat(-900f, 900f)));
						RandomOrig = new Vector2(Player.width / 2, Player.height / 2) + new Vector2(Main.rand.NextFloat(-1800f, 1800f), (Main.rand.NextFloat(-1200f, 1200f)));

						Vector2 speed = Main.rand.NextVector2Circular(1f, 1f);
						Vector2 speed2 = Main.rand.NextVector2Circular(0.1f, 0.1f);
						ParticleManager.NewParticle(Player.Center - RandomOrig2, speed2 * 3, ParticleManager.NewInstance<GoldSparkleParticle>(), Color.RoyalBlue, Main.rand.NextFloat(0.2f, 0.8f));

						GoldenSparkleCooldown = 0;
					}
				}





				if (ModContent.GetInstance<LunarVeilClientConfig>().ParticlesToggle == true && ZoneFable)
				{


					if (RayCooldown > 1000)
					{
						for (int j = 0; j < 1; j++)
						{
							RandomOrig3 = new Vector2(Player.width / 2, Player.height / 2) + new Vector2(Main.rand.NextFloat(-3000f, 3000f), (Main.rand.NextFloat(700f, 700f)));


							Vector2 speed = Main.rand.NextVector2Circular(1f, 1f);
							Vector2 speed2 = Main.rand.NextVector2Circular(0.1f, 0.1f);

							Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center - RandomOrig3, speed2 * 1, ModContent.ProjectileType<FabledSunray>(), 1, 1f, Player.whoAmI);

							RayCooldown = 0;
						}
					}

					if (RayCooldown == 500)
					{
						for (int j = 0; j < 1; j++)
						{
							RandomOrig3 = new Vector2(Player.width / 2, Player.height / 2) + new Vector2(Main.rand.NextFloat(-3000f, 3000f), (Main.rand.NextFloat(700f, 800f)));


							Vector2 speed = Main.rand.NextVector2Circular(1f, 1f);
							Vector2 speed2 = Main.rand.NextVector2Circular(0.1f, 0.1f);

							Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center - RandomOrig3, speed2 * 1, ModContent.ProjectileType<FabledColoredSunray>(), 1, 1f, Player.whoAmI);


						}
					}
				}
			}





			if (ModContent.GetInstance<LunarVeilClientConfig>().ParticlesToggle == true && (ZoneAcid || ZoneGovheil || ZoneVeri) && !ZoneLab)
			{
				Main.windPhysicsStrength = 90;
				Main.GraveyardVisualIntensity = 0.4f;
				GoldenRingCooldown++;

				GoldenSparkleCooldown++;
				for (int j = 0; j < 5; j++)
				{
					RandomOrig3 = new Vector2(Player.width / 2, Player.height / 2) + new Vector2(Main.rand.NextFloat(-900f, 900f), (Main.rand.NextFloat(-600f, 600f)));
					RandomOrig2 = new Vector2(Player.width / 2, Player.height / 2) + new Vector2(Main.rand.NextFloat(-1600f, 1600f), (Main.rand.NextFloat(-900f, 900f)));
					RandomOrig = new Vector2(Player.width / 2, Player.height / 2) + new Vector2(Main.rand.NextFloat(-1800f, 1800f), (Main.rand.NextFloat(-1200f, 1200f)));

					Vector2 speed = Main.rand.NextVector2Circular(1f, 1f);
					Vector2 speed2 = Main.rand.NextVector2Circular(0.1f, 0.1f);
					ParticleManager.NewParticle(Player.Center - RandomOrig2, speed * 2, ParticleManager.NewInstance<morrowstar>(), Color.RoyalBlue, Main.rand.NextFloat(0.2f, 0.8f));


				}

			}


			if (ModContent.GetInstance<LunarVeilClientConfig>().ParticlesToggle == true && ZoneVillage)
			{
				Main.GraveyardVisualIntensity = 0.1f;
				Main.windPhysicsStrength = 50;


				

				for (int j = 0; j < 3; j++)
				{
					RandomOrig3 = new Vector2(Player.width / 2, Player.height / 2) + new Vector2(Main.rand.NextFloat(-1500f, 1500f), (Main.rand.NextFloat(-600f, 600f)));
					RandomOrig2 = new Vector2(Player.width / 2, Player.height / 2) + new Vector2(Main.rand.NextFloat(-1600f, 1600f), (Main.rand.NextFloat(-900f, 900f)));
					RandomOrig = new Vector2(Player.width / 2, Player.height / 2) + new Vector2(Main.rand.NextFloat(-1800f, 1800f), (Main.rand.NextFloat(-1200f, 1200f)));

					Vector2 speed = Main.rand.NextVector2Circular(1f, 1f);
					Vector2 speed2 = Main.rand.NextVector2Circular(0.1f, 0.1f);
					ParticleManager.NewParticle(Player.Center - RandomOrig, speed2 * 3, ParticleManager.NewInstance<CrystalParticle2>(), Color.Orange, Main.rand.NextFloat(0.2f, 0.8f));
				}
			}



			if (ModContent.GetInstance<LunarVeilClientConfig>().ParticlesToggle == true && ZoneVeil)
			{
				Main.GraveyardVisualIntensity = 0.1f;
				Main.windPhysicsStrength = 50;




				for (int j = 0; j < 3; j++)
				{
					RandomOrig3 = new Vector2(Player.width / 2, Player.height / 2) + new Vector2(Main.rand.NextFloat(-1500f, 1500f), (Main.rand.NextFloat(-600f, 600f)));
					RandomOrig2 = new Vector2(Player.width / 2, Player.height / 2) + new Vector2(Main.rand.NextFloat(-1600f, 1600f), (Main.rand.NextFloat(-900f, 900f)));
					RandomOrig = new Vector2(Player.width / 2, Player.height / 2) + new Vector2(Main.rand.NextFloat(-1800f, 1800f), (Main.rand.NextFloat(-1200f, 1200f)));

					Vector2 speed = Main.rand.NextVector2Circular(1f, 1f);
					Vector2 speed2 = Main.rand.NextVector2Circular(0.1f, 0.1f);
					ParticleManager.NewParticle(Player.Center - RandomOrig, speed2 * 3, ParticleManager.NewInstance<BoreParticle>(), Color.Orange, Main.rand.NextFloat(0.1f, 0.9f));
				}
			}



			if (ModContent.GetInstance<LunarVeilClientConfig>().VanillaParticlesToggle == true && Player.ZoneHallow)
			{
				Main.GraveyardVisualIntensity = 0.4f;
				Main.windPhysicsStrength = 50;


				GoldenRingCooldown++;

				GoldenSparkleCooldown++;
				RayCooldown++;

				if (RayCooldown > 1000)
				{
					for (int j = 0; j < 1; j++)
					{
						float xRand = Main.rand.NextFloat(-500f, 500f);
                        float yRand = Main.rand.NextFloat(800, 1000f);
                        if (Main.rand.NextBool(2))
						{
                            yRand = -yRand;
                        }
			
                        RandomOrig3 = new Vector2(xRand, yRand);


						Vector2 speed2 = Main.rand.NextVector2Circular(0.1f, 0.1f);
						Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center - RandomOrig3, 
							speed2 * 1, ModContent.ProjectileType<CrystalRay>(), 1, 1f, Player.whoAmI);

						RayCooldown = 0;
					}


					for (int j = 0; j < 1; j++)
                    {
                        float xRand = Main.rand.NextFloat(-1000f, 1000f);
                        float yRand = Main.rand.NextFloat(800, 1000f);
                        if (Main.rand.NextBool(2))
                        {
                            yRand = -yRand;
                        }

                        RandomOrig3 = new Vector2(xRand, yRand);
						Vector2 speed2 = Main.rand.NextVector2Circular(0.1f, 0.1f);
						Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center - RandomOrig3, speed2 * 1, 
							ModContent.ProjectileType<CrystalRay2>(), 1, 1f, Player.whoAmI);

						RayCooldown = 0;
					}
				}


				if (RayCooldown > 900)
				{
					for (int j = 0; j < 1; j++)
					{
						float xRand = Main.rand.NextFloat(-500f, 500f);
                        float yRand = Main.rand.NextFloat(800, 1000f);
                        if (Main.rand.NextBool(2))
						{
                            yRand = -yRand;
                        }
			
                        RandomOrig3 = new Vector2(xRand, yRand);

						Vector2 speed2 = Main.rand.NextVector2Circular(0.1f, 0.1f);
						Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center - RandomOrig3, speed2 * 1,
							ModContent.ProjectileType<CrystalRay4>(), 1, 1f, Player.whoAmI);

						RayCooldown = 0;
					}


					for (int j = 0; j < 1; j++)
					{
                        float xRand = Main.rand.NextFloat(-1000f, 1000f);
                        float yRand = Main.rand.NextFloat(800, 1000f);
                        if (Main.rand.NextBool(2))
                        {
                            yRand = -yRand;
                        }
                        RandomOrig3 = new Vector2(xRand, yRand);
                        Vector2 speed2 = Main.rand.NextVector2Circular(0.1f, 0.1f);
						Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center - RandomOrig3, speed2 * 1, 
							ModContent.ProjectileType<CrystalRay2>(), 1, 1f, Player.whoAmI);

						RayCooldown = 0;
					}
				}
				if (RayCooldown == 500)
				{
					for (int j = 0; j < 1; j++)
					{
                        float xRand = Main.rand.NextFloat(-700f, 700f);
                        float yRand = Main.rand.NextFloat(800, 1000f);
                        if (Main.rand.NextBool(2))
                        {
                            yRand = -yRand;
                        }

                        RandomOrig3 = new Vector2(xRand, yRand);

                        Vector2 speed2 = Main.rand.NextVector2Circular(0.1f, 0.1f);

						Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center - RandomOrig3, speed2 * 1, ModContent.ProjectileType<CrystalRay>(), 1, 1f, Player.whoAmI);

						RayCooldown = 0;
					}


					for (int j = 0; j < 1; j++)
					{
                        float xRand = Main.rand.NextFloat(-600f, 800f);
                        float yRand = Main.rand.NextFloat(800, 1000f);
                        if (Main.rand.NextBool(2))
                        {
                            yRand = -yRand;
                        }

                        RandomOrig3 = new Vector2(xRand, yRand);
                        Vector2 speed2 = Main.rand.NextVector2Circular(0.1f, 0.1f);

						Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center - RandomOrig3, speed2 * 1, 
							ModContent.ProjectileType<CrystalRay2>(), 1, 1f, Player.whoAmI);

						RayCooldown = 0;
					}
				}


				if (RayCooldown == 250)
				{
					for (int j = 0; j < 1; j++)
					{
                        float xRand = Main.rand.NextFloat(-500f, 900f);
                        float yRand = Main.rand.NextFloat(800, 1000f);
                        if (Main.rand.NextBool(2))
                        {
                            yRand = -yRand;
                        }

                        RandomOrig3 = new Vector2(xRand, yRand);
						Vector2 speed2 = Main.rand.NextVector2Circular(0.1f, 0.1f);
						Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center - RandomOrig3, speed2 * 1, 
							ModContent.ProjectileType<CrystalRay3>(), 1, 1f, Player.whoAmI);
					}


					for (int j = 0; j < 1; j++)
					{
                        float xRand = Main.rand.NextFloat(-900f, 500f);
                        float yRand = Main.rand.NextFloat(800, 1000f);
                        if (Main.rand.NextBool(2))
                        {
                            yRand = -yRand;
                        }

                        RandomOrig3 = new Vector2(xRand, yRand);
                        Vector2 speed = Main.rand.NextVector2Circular(1f, 1f);
						Vector2 speed2 = Main.rand.NextVector2Circular(0.1f, 0.1f);

						Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center - RandomOrig3, speed2 * 1, 
							ModContent.ProjectileType<CrystalRay4>(), 1, 1f, Player.whoAmI);


					}
				}

				if (RayCooldown == 750)
				{
					for (int j = 0; j < 1; j++)
					{
                        float xRand = Main.rand.NextFloat(-500, 500f);
                        float yRand = Main.rand.NextFloat(800, 1000f);
                        if (Main.rand.NextBool(2))
                        {
                            yRand = -yRand;
                        }

                        RandomOrig3 = new Vector2(xRand, yRand);
						Vector2 speed2 = Main.rand.NextVector2Circular(0.1f, 0.1f);

						Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center - RandomOrig3, speed2 * 1, 
							ModContent.ProjectileType<CrystalRay3>(), 1, 1f, Player.whoAmI);
					}

					for (int j = 0; j < 1; j++)
                    {
                        float xRand = Main.rand.NextFloat(-1000f, 2000f);
                        float yRand = Main.rand.NextFloat(800, 1000f);
                        if (Main.rand.NextBool(2))
                        {
                            yRand = -yRand;
                        }

                        RandomOrig3 = new Vector2(xRand, yRand);
						Vector2 speed2 = Main.rand.NextVector2Circular(0.1f, 0.1f);
						Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center - RandomOrig3, speed2 * 1, 
							ModContent.ProjectileType<CrystalRay4>(), 1, 1f, Player.whoAmI);
					}
				}


				for (int j = 0; j < 3; j++)
				{
					RandomOrig3 = new Vector2(Player.width / 2, Player.height / 2) + new Vector2(Main.rand.NextFloat(-1400f, 1200f), (Main.rand.NextFloat(-600f, 600f)));
					RandomOrig2 = new Vector2(Player.width / 2, Player.height / 2) + new Vector2(Main.rand.NextFloat(-1600f, 1600f), (Main.rand.NextFloat(-900f, 900f)));
					RandomOrig = new Vector2(Player.width / 2, Player.height / 2) + new Vector2(Main.rand.NextFloat(-1800f, 1800f), (Main.rand.NextFloat(-1200f, 1200f)));

					Vector2 speed = Main.rand.NextVector2Circular(1f, 1f);
					Vector2 speed2 = Main.rand.NextVector2Circular(0.1f, 0.1f);
					ParticleManager.NewParticle(Player.Center - RandomOrig, speed2 * 3, ParticleManager.NewInstance<CrystalParticle1>(), Color.Orange, Main.rand.NextFloat(0.2f, 0.8f));
				}

				for (int j = 0; j < 2; j++)
				{
					RandomOrig3 = new Vector2(Player.width / 2, Player.height / 2) + new Vector2(Main.rand.NextFloat(-1500f, 1500f), (Main.rand.NextFloat(-600f, 600f)));
					RandomOrig2 = new Vector2(Player.width / 2, Player.height / 2) + new Vector2(Main.rand.NextFloat(-1600f, 1600f), (Main.rand.NextFloat(-900f, 900f)));
					RandomOrig = new Vector2(Player.width / 2, Player.height / 2) + new Vector2(Main.rand.NextFloat(-1800f, 1800f), (Main.rand.NextFloat(-1200f, 1200f)));

					Vector2 speed = Main.rand.NextVector2Circular(1f, 1f);
					Vector2 speed2 = Main.rand.NextVector2Circular(0.1f, 0.1f);
					ParticleManager.NewParticle(Player.Center - RandomOrig, speed2 * 3, ParticleManager.NewInstance<CrystalParticle2>(), Color.Orange, Main.rand.NextFloat(0.2f, 0.8f));
				}

			}







			if (ModContent.GetInstance<LunarVeilClientConfig>().ParticlesToggle == true && ZoneAlcadzia)
			{
				Main.windPhysicsStrength = 90;
				Main.GraveyardVisualIntensity = 0.4f;
				GoldenRingCooldown++;

				GoldenSparkleCooldown++;
				for (int j = 0; j < 5; j++)
				{
					RandomOrig3 = new Vector2(Player.width / 2, Player.height / 2) + new Vector2(Main.rand.NextFloat(-900f, 900f), (Main.rand.NextFloat(-600f, 600f)));
					RandomOrig2 = new Vector2(Player.width / 2, Player.height / 2) + new Vector2(Main.rand.NextFloat(-1600f, 1600f), (Main.rand.NextFloat(-900f, 900f)));
					RandomOrig = new Vector2(Player.width / 2, Player.height / 2) + new Vector2(Main.rand.NextFloat(-1800f, 1800f), (Main.rand.NextFloat(-1200f, 1200f)));

					Vector2 speed = Main.rand.NextVector2Circular(1f, 1f);
					Vector2 speed2 = Main.rand.NextVector2Circular(0.1f, 0.1f);
					ParticleManager.NewParticle(Player.Center - RandomOrig2, speed * 2, ParticleManager.NewInstance<FabledParticle2>(), Color.RoyalBlue, Main.rand.NextFloat(0.2f, 0.8f));


				}


				for (int j = 0; j < 5; j++)
				{
					RandomOrig3 = new Vector2(Player.width / 2, Player.height / 2) + new Vector2(Main.rand.NextFloat(-900f, 900f), (Main.rand.NextFloat(-600f, 600f)));
					RandomOrig2 = new Vector2(Player.width / 2, Player.height / 2) + new Vector2(Main.rand.NextFloat(-1600f, 1600f), (Main.rand.NextFloat(-900f, 900f)));
					RandomOrig = new Vector2(Player.width / 2, Player.height / 2) + new Vector2(Main.rand.NextFloat(-1800f, 1800f), (Main.rand.NextFloat(-1200f, 1200f)));

					Vector2 speed = Main.rand.NextVector2Circular(1f, 1f);
					Vector2 speed2 = Main.rand.NextVector2Circular(0.1f, 0.1f);
					ParticleManager.NewParticle(Player.Center - RandomOrig2, speed * 2, ParticleManager.NewInstance<morrowstar>(), Color.RoyalBlue, Main.rand.NextFloat(0.2f, 0.8f));


				}


				for (int j = 0; j < 5; j++)
				{
					RandomOrig3 = new Vector2(Player.width / 2, Player.height / 2) + new Vector2(Main.rand.NextFloat(-900f, 900f), (Main.rand.NextFloat(-600f, 600f)));
					RandomOrig2 = new Vector2(Player.width / 2, Player.height / 2) + new Vector2(Main.rand.NextFloat(-1600f, 1600f), (Main.rand.NextFloat(-900f, 900f)));
					RandomOrig = new Vector2(Player.width / 2, Player.height / 2) + new Vector2(Main.rand.NextFloat(-1800f, 1800f), (Main.rand.NextFloat(-1200f, 1200f)));

					Vector2 speed = Main.rand.NextVector2Circular(1f, 1f);
					Vector2 speed2 = Main.rand.NextVector2Circular(0.1f, 0.1f);
					ParticleManager.NewParticle(Player.Center - RandomOrig2, speed * 2, ParticleManager.NewInstance<MoonTrailParticle2>(), Color.RoyalBlue, Main.rand.NextFloat(0.2f, 0.8f));
				}

			}

			


			if (ModContent.GetInstance<LunarVeilClientConfig>().ParticlesToggle == true && (ZoneAbyss || ZoneAurelus || ZoneIlluria))
			{
			
				


				GoldenRingCooldown++;

				GoldenSparkleCooldown++;
				RayCooldown++;



				for (int j = 0; j < 2; j++)
				{
					RandomOrig3 = new Vector2(Player.width / 2, Player.height / 2) + new Vector2(Main.rand.NextFloat(-900f, 900f), (Main.rand.NextFloat(-600f, 600f)));
					RandomOrig2 = new Vector2(Player.width / 2, Player.height / 2) + new Vector2(Main.rand.NextFloat(-1600f, 1600f), (Main.rand.NextFloat(-900f, 900f)));
					RandomOrig = new Vector2(Player.width / 2, Player.height / 2) + new Vector2(Main.rand.NextFloat(-1800f, 1800f), (Main.rand.NextFloat(-1200f, 1200f)));

					Vector2 speed = Main.rand.NextVector2Circular(1f, 1f);
					Vector2 speed2 = Main.rand.NextVector2Circular(0.1f, 0.1f);
					ParticleManager.NewParticle(Player.Center - RandomOrig, speed2 * 3, ParticleManager.NewInstance<FabledParticle2>(), Color.RoyalBlue, Main.rand.NextFloat(0.2f, 0.8f));


				}


				for (int j = 0; j < 1; j++)
				{
					RandomOrig3 = new Vector2(Player.width / 2, Player.height / 2) + new Vector2(Main.rand.NextFloat(-900f, 900f), (Main.rand.NextFloat(-600f, 600f)));
					RandomOrig2 = new Vector2(Player.width / 2, Player.height / 2) + new Vector2(Main.rand.NextFloat(-1600f, 1600f), (Main.rand.NextFloat(-900f, 900f)));
					RandomOrig = new Vector2(Player.width / 2, Player.height / 2) + new Vector2(Main.rand.NextFloat(-1800f, 1800f), (Main.rand.NextFloat(-1200f, 1200f)));

					Vector2 speed = Main.rand.NextVector2Circular(1f, 1f);
					Vector2 speed2 = Main.rand.NextVector2Circular(0.1f, 0.1f);
					ParticleManager.NewParticle(Player.Center - RandomOrig2, speed * 2, ParticleManager.NewInstance<IceyParticle>(), Color.RoyalBlue, Main.rand.NextFloat(0.2f, 0.8f));


				}

				for (int j = 0; j < 2; j++)
				{
					RandomOrig3 = new Vector2(Player.width / 2, Player.height / 2) + new Vector2(Main.rand.NextFloat(-900f, 900f), (Main.rand.NextFloat(-600f, 600f)));
					RandomOrig2 = new Vector2(Player.width / 2, Player.height / 2) + new Vector2(Main.rand.NextFloat(-1600f, 1600f), (Main.rand.NextFloat(-900f, 900f)));
					RandomOrig = new Vector2(Player.width / 2, Player.height / 2) + new Vector2(Main.rand.NextFloat(-1800f, 1800f), (Main.rand.NextFloat(-1200f, 1200f)));

					Vector2 speed = Main.rand.NextVector2Circular(1f, 1f);
					Vector2 speed2 = Main.rand.NextVector2Circular(0.1f, 0.1f);
					ParticleManager.NewParticle(Player.Center - RandomOrig3, speed * 0.5f, ParticleManager.NewInstance<FabledParticle2>(), Color.RoyalBlue, Main.rand.NextFloat(0.2f, 0.8f));


				}
				if (GoldenRingCooldown > 2)
				{
					for (int j = 0; j < 1; j++)
					{
						RandomOrig3 = new Vector2(Player.width / 2, Player.height / 2) + new Vector2(Main.rand.NextFloat(-900f, 900f), (Main.rand.NextFloat(-600f, 600f)));
						RandomOrig2 = new Vector2(Player.width / 2, Player.height / 2) + new Vector2(Main.rand.NextFloat(-1600f, 1600f), (Main.rand.NextFloat(-900f, 900f)));
						RandomOrig = new Vector2(Player.width / 2, Player.height / 2) + new Vector2(Main.rand.NextFloat(-1800f, 1800f), (Main.rand.NextFloat(-1200f, 1200f)));

						Vector2 speed = Main.rand.NextVector2Circular(1f, 1f);
						Vector2 speed2 = Main.rand.NextVector2Circular(0.1f, 0.1f);
						ParticleManager.NewParticle(Player.Center - RandomOrig3, speed * 1, ParticleManager.NewInstance<GoldRingParticle2>(), Color.RoyalBlue, Main.rand.NextFloat(0.2f, 0.8f));

						GoldenRingCooldown = 0;
					}
				}

				if (GoldenSparkleCooldown > 100)
				{
					for (int j = 0; j < 1; j++)
					{
						RandomOrig3 = new Vector2(Player.width / 2, Player.height / 2) + new Vector2(Main.rand.NextFloat(-900f, 900f), (Main.rand.NextFloat(-600f, 600f)));
						RandomOrig2 = new Vector2(Player.width / 2, Player.height / 2) + new Vector2(Main.rand.NextFloat(-1600f, 1600f), (Main.rand.NextFloat(-900f, 900f)));
						RandomOrig = new Vector2(Player.width / 2, Player.height / 2) + new Vector2(Main.rand.NextFloat(-1800f, 1800f), (Main.rand.NextFloat(-1200f, 1200f)));

						Vector2 speed = Main.rand.NextVector2Circular(1f, 1f);
						Vector2 speed2 = Main.rand.NextVector2Circular(0.1f, 0.1f);
						ParticleManager.NewParticle(Player.Center - RandomOrig2, speed2 * 3, ParticleManager.NewInstance<GoldRingParticle2>(), Color.RoyalBlue, Main.rand.NextFloat(0.2f, 0.8f));

						GoldenSparkleCooldown = 0;
					}
				}
			}

			bool spawnAuroreanParticles = EventWorld.Aurorean && (player.ZoneOverworldHeight || player.ZoneSkyHeight);
			if (ModContent.GetInstance<LunarVeilClientConfig>().ParticlesToggle == true && spawnAuroreanParticles)
            {
                Main.windPhysicsStrength = 50;
                GoldenRingCooldown++;
                GoldenSparkleCooldown++;
                RayCooldown++;

                for (int j = 0; j < 1; j++)
                {
                    RandomOrig3 = new Vector2(Player.width / 2, Player.height / 2) + new Vector2(Main.rand.NextFloat(-900f, 900f), (Main.rand.NextFloat(-600f, 600f)));
                    RandomOrig2 = new Vector2(Player.width / 2, Player.height / 2) + new Vector2(Main.rand.NextFloat(-1600f, 1600f), (Main.rand.NextFloat(-900f, 900f)));
                    RandomOrig = new Vector2(Player.width / 2, Player.height / 2) + new Vector2(Main.rand.NextFloat(-1800f, 1800f), (Main.rand.NextFloat(-1200f, 1200f)));

                    Vector2 speed = Main.rand.NextVector2Circular(1f, 1f);
                    Vector2 speed2 = Main.rand.NextVector2Circular(0.1f, 0.1f);
                    ParticleManager.NewParticle(Player.Center - RandomOrig, speed2 * 3, ParticleManager.NewInstance<FabledParticle2>(), Color.Orange, Main.rand.NextFloat(0.2f, 0.8f));
                }

                for (int j = 0; j < 1; j++)
                {
                    RandomOrig3 = new Vector2(Player.width / 2, Player.height / 2) + new Vector2(Main.rand.NextFloat(-900f, 900f), (Main.rand.NextFloat(-600f, 600f)));
                    RandomOrig2 = new Vector2(Player.width / 2, Player.height / 2) + new Vector2(Main.rand.NextFloat(-1600f, 1600f), (Main.rand.NextFloat(-900f, 900f)));
                    RandomOrig = new Vector2(Player.width / 2, Player.height / 2) + new Vector2(Main.rand.NextFloat(-1800f, 1800f), (Main.rand.NextFloat(-1200f, 1200f)));

                    Vector2 speed = Main.rand.NextVector2Circular(1f, 1f);
                    Vector2 speed2 = Main.rand.NextVector2Circular(0.1f, 0.1f);
                    ParticleManager.NewParticle(Player.Center - RandomOrig3, speed * 0.5f, ParticleManager.NewInstance<FabledParticle2>(), Color.HotPink, Main.rand.NextFloat(0.2f, 0.8f));
                }

				for (int j = 0; j < 3; j++)
				{
					RandomOrig3 = new Vector2(Player.width / 2, Player.height / 2) + new Vector2(Main.rand.NextFloat(-900f, 900f), (Main.rand.NextFloat(-600f, 600f)));
					RandomOrig2 = new Vector2(Player.width / 2, Player.height / 2) + new Vector2(Main.rand.NextFloat(-1600f, 1600f), (Main.rand.NextFloat(-900f, 900f)));
					RandomOrig = new Vector2(Player.width / 2, Player.height / 2) + new Vector2(Main.rand.NextFloat(-1800f, 1800f), (Main.rand.NextFloat(-1200f, 1200f)));

					Vector2 speed = Main.rand.NextVector2Circular(1f, 1f);
					Vector2 speed2 = Main.rand.NextVector2Circular(0.1f, 0.1f);
					ParticleManager.NewParticle(Player.Center - RandomOrig2, speed * 2, ParticleManager.NewInstance<MoonTrailParticle2>(), Color.RoyalBlue, Main.rand.NextFloat(0.2f, 0.8f));
				}

				for (int j = 0; j < 1; j++)
				{
					RandomOrig3 = new Vector2(Player.width / 2, Player.height / 2) + new Vector2(Main.rand.NextFloat(-900f, 900f), (Main.rand.NextFloat(-600f, 600f)));
					RandomOrig2 = new Vector2(Player.width / 2, Player.height / 2) + new Vector2(Main.rand.NextFloat(-1600f, 1600f), (Main.rand.NextFloat(-900f, 900f)));
					RandomOrig = new Vector2(Player.width / 2, Player.height / 2) + new Vector2(Main.rand.NextFloat(-1800f, 1800f), (Main.rand.NextFloat(-1200f, 1200f)));

					Vector2 speed = Main.rand.NextVector2Circular(1f, 1f);
					Vector2 speed2 = Main.rand.NextVector2Circular(0.1f, 0.1f);
					ParticleManager.NewParticle(Player.Center - RandomOrig, speed2 * 3, ParticleManager.NewInstance<FabledParticle3>(), Color.RoyalBlue, Main.rand.NextFloat(0.2f, 0.8f));
				}

				for (int j = 0; j < 1; j++)
				{
					RandomOrig3 = new Vector2(Player.width / 2, Player.height / 2) + new Vector2(Main.rand.NextFloat(-900f, 900f), (Main.rand.NextFloat(-600f, 600f)));
					RandomOrig2 = new Vector2(Player.width / 2, Player.height / 2) + new Vector2(Main.rand.NextFloat(-1600f, 1600f), (Main.rand.NextFloat(-900f, 900f)));
					RandomOrig = new Vector2(Player.width / 2, Player.height / 2) + new Vector2(Main.rand.NextFloat(-1800f, 1800f), (Main.rand.NextFloat(-1200f, 1200f)));

					Vector2 speed = Main.rand.NextVector2Circular(1f, 1f);
					Vector2 speed2 = Main.rand.NextVector2Circular(0.1f, 0.1f);
					ParticleManager.NewParticle(Player.Center - RandomOrig2, speed * 2, ParticleManager.NewInstance<GoldRingParticle2>(), Color.RoyalBlue, Main.rand.NextFloat(0.2f, 0.8f));
				}

				for (int j = 0; j < 1; j++)
				{
					RandomOrig3 = new Vector2(Player.width / 2, Player.height / 2) + new Vector2(Main.rand.NextFloat(-900f, 900f), (Main.rand.NextFloat(-600f, 600f)));
					RandomOrig2 = new Vector2(Player.width / 2, Player.height / 2) + new Vector2(Main.rand.NextFloat(-1600f, 1600f), (Main.rand.NextFloat(-900f, 900f)));
					RandomOrig = new Vector2(Player.width / 2, Player.height / 2) + new Vector2(Main.rand.NextFloat(-1800f, 1800f), (Main.rand.NextFloat(-1200f, 1200f)));

					Vector2 speed = Main.rand.NextVector2Circular(1f, 1f);
					Vector2 speed2 = Main.rand.NextVector2Circular(0.1f, 0.1f);
					ParticleManager.NewParticle(Player.Center - RandomOrig3, speed * 0.5f, ParticleManager.NewInstance<FabledParticle3>(), Color.RoyalBlue, Main.rand.NextFloat(0.2f, 0.8f));
				}

				if (GoldenRingCooldown > 2)
				{
					for (int j = 0; j < 2; j++)
					{
						RandomOrig3 = new Vector2(Player.width / 2, Player.height / 2) + new Vector2(Main.rand.NextFloat(-900f, 900f), (Main.rand.NextFloat(-600f, 600f)));
						RandomOrig2 = new Vector2(Player.width / 2, Player.height / 2) + new Vector2(Main.rand.NextFloat(-1600f, 1600f), (Main.rand.NextFloat(-900f, 900f)));
						RandomOrig = new Vector2(Player.width / 2, Player.height / 2) + new Vector2(Main.rand.NextFloat(-1800f, 1800f), (Main.rand.NextFloat(-1200f, 1200f)));

						Vector2 speed = Main.rand.NextVector2Circular(1f, 1f);
						Vector2 speed2 = Main.rand.NextVector2Circular(0.1f, 0.1f);
						ParticleManager.NewParticle(Player.Center - RandomOrig3, speed * 2, ParticleManager.NewInstance<GoldRingParticle3>(), Color.RoyalBlue, Main.rand.NextFloat(0.2f, 0.8f));

						GoldenRingCooldown = 0;
					}
				}

				if (GoldenSparkleCooldown > 100)
				{
					for (int j = 0; j < 1; j++)
					{
						RandomOrig3 = new Vector2(Player.width / 2, Player.height / 2) + new Vector2(Main.rand.NextFloat(-900f, 900f), (Main.rand.NextFloat(-600f, 600f)));
						RandomOrig2 = new Vector2(Player.width / 2, Player.height / 2) + new Vector2(Main.rand.NextFloat(-1600f, 1600f), (Main.rand.NextFloat(-900f, 900f)));
						RandomOrig = new Vector2(Player.width / 2, Player.height / 2) + new Vector2(Main.rand.NextFloat(-1800f, 1800f), (Main.rand.NextFloat(-1200f, 1200f)));

						Vector2 speed = Main.rand.NextVector2Circular(1f, 1f);
						Vector2 speed2 = Main.rand.NextVector2Circular(0.1f, 0.1f);
						ParticleManager.NewParticle(Player.Center - RandomOrig2, speed2 * 3, ParticleManager.NewInstance<GoldRingParticle3>(), Color.RoyalBlue, Main.rand.NextFloat(0.2f, 0.8f));
						GoldenSparkleCooldown = 0;
					}
				}


				GoldenRingCooldown++;
				GoldenSparkleCooldown++;
				for (int j = 0; j < 5; j++)
				{
					RandomOrig3 = new Vector2(Player.width / 2, Player.height / 2) + new Vector2(Main.rand.NextFloat(-900f, 900f), (Main.rand.NextFloat(-600f, 600f)));
					RandomOrig2 = new Vector2(Player.width / 2, Player.height / 2) + new Vector2(Main.rand.NextFloat(-1600f, 1600f), (Main.rand.NextFloat(-900f, 900f)));
					RandomOrig = new Vector2(Player.width / 2, Player.height / 2) + new Vector2(Main.rand.NextFloat(-1800f, 1800f), (Main.rand.NextFloat(-1200f, 1200f)));

					Vector2 speed = Main.rand.NextVector2Circular(1f, 1f);
					Vector2 speed2 = Main.rand.NextVector2Circular(0.1f, 0.1f);
					ParticleManager.NewParticle(Player.Center - RandomOrig2, speed * 2, ParticleManager.NewInstance<morrowstar>(), Color.RoyalBlue, Main.rand.NextFloat(0.2f, 0.8f));
				}
			}







            bool spawnAuroreanParticles2 = EventWorld.GreenSun && ZoneAcid && (player.ZoneOverworldHeight || player.ZoneSkyHeight);
            if (ModContent.GetInstance<LunarVeilClientConfig>().ParticlesToggle == true && spawnAuroreanParticles2)
            {
                Main.windPhysicsStrength = 50;
                GoldenRingCooldown++;
                GoldenSparkleCooldown++;
                RayCooldown++;

                for (int j = 0; j < 1; j++)
                {
                    RandomOrig3 = new Vector2(Player.width / 2, Player.height / 2) + new Vector2(Main.rand.NextFloat(-900f, 900f), (Main.rand.NextFloat(-600f, 600f)));
                    RandomOrig2 = new Vector2(Player.width / 2, Player.height / 2) + new Vector2(Main.rand.NextFloat(-1600f, 1600f), (Main.rand.NextFloat(-900f, 900f)));
                    RandomOrig = new Vector2(Player.width / 2, Player.height / 2) + new Vector2(Main.rand.NextFloat(-1800f, 1800f), (Main.rand.NextFloat(-1200f, 1200f)));

                    Vector2 speed = Main.rand.NextVector2Circular(1f, 1f);
                    Vector2 speed2 = Main.rand.NextVector2Circular(0.1f, 0.1f);
                    ParticleManager.NewParticle(Player.Center - RandomOrig, speed2 * 3, ParticleManager.NewInstance<FabledParticle2>(), Color.Orange, Main.rand.NextFloat(0.2f, 0.8f));
                }

                for (int j = 0; j < 1; j++)
                {
                    RandomOrig3 = new Vector2(Player.width / 2, Player.height / 2) + new Vector2(Main.rand.NextFloat(-900f, 900f), (Main.rand.NextFloat(-600f, 600f)));
                    RandomOrig2 = new Vector2(Player.width / 2, Player.height / 2) + new Vector2(Main.rand.NextFloat(-1600f, 1600f), (Main.rand.NextFloat(-900f, 900f)));
                    RandomOrig = new Vector2(Player.width / 2, Player.height / 2) + new Vector2(Main.rand.NextFloat(-1800f, 1800f), (Main.rand.NextFloat(-1200f, 1200f)));

                    Vector2 speed = Main.rand.NextVector2Circular(1f, 1f);
                    Vector2 speed2 = Main.rand.NextVector2Circular(0.1f, 0.1f);
                    ParticleManager.NewParticle(Player.Center - RandomOrig3, speed * 0.5f, ParticleManager.NewInstance<FabledParticle2>(), Color.HotPink, Main.rand.NextFloat(0.2f, 0.8f));
                }

                for (int j = 0; j < 1; j++)
                {
                    RandomOrig3 = new Vector2(Player.width / 2, Player.height / 2) + new Vector2(Main.rand.NextFloat(-900f, 900f), (Main.rand.NextFloat(-600f, 600f)));
                    RandomOrig2 = new Vector2(Player.width / 2, Player.height / 2) + new Vector2(Main.rand.NextFloat(-1600f, 1600f), (Main.rand.NextFloat(-900f, 900f)));
                    RandomOrig = new Vector2(Player.width / 2, Player.height / 2) + new Vector2(Main.rand.NextFloat(-1800f, 1800f), (Main.rand.NextFloat(-1200f, 1200f)));

                    Vector2 speed = Main.rand.NextVector2Circular(1f, 1f);
                    Vector2 speed2 = Main.rand.NextVector2Circular(0.1f, 0.1f);
                    ParticleManager.NewParticle(Player.Center - RandomOrig2, speed * 2, ParticleManager.NewInstance<MoonTrailParticle2>(), Color.RoyalBlue, Main.rand.NextFloat(0.2f, 0.8f));
                }

                for (int j = 0; j < 1; j++)
                {
                    RandomOrig3 = new Vector2(Player.width / 2, Player.height / 2) + new Vector2(Main.rand.NextFloat(-900f, 900f), (Main.rand.NextFloat(-600f, 600f)));
                    RandomOrig2 = new Vector2(Player.width / 2, Player.height / 2) + new Vector2(Main.rand.NextFloat(-1600f, 1600f), (Main.rand.NextFloat(-900f, 900f)));
                    RandomOrig = new Vector2(Player.width / 2, Player.height / 2) + new Vector2(Main.rand.NextFloat(-1800f, 1800f), (Main.rand.NextFloat(-1200f, 1200f)));

                    Vector2 speed = Main.rand.NextVector2Circular(1f, 1f);
                    Vector2 speed2 = Main.rand.NextVector2Circular(0.1f, 0.1f);
                    ParticleManager.NewParticle(Player.Center - RandomOrig, speed2 * 3, ParticleManager.NewInstance<FabledParticle3>(), Color.RoyalBlue, Main.rand.NextFloat(0.2f, 0.8f));
                }

                

                for (int j = 0; j < 1; j++)
                {
                    RandomOrig3 = new Vector2(Player.width / 2, Player.height / 2) + new Vector2(Main.rand.NextFloat(-900f, 900f), (Main.rand.NextFloat(-600f, 600f)));
                    RandomOrig2 = new Vector2(Player.width / 2, Player.height / 2) + new Vector2(Main.rand.NextFloat(-1600f, 1600f), (Main.rand.NextFloat(-900f, 900f)));
                    RandomOrig = new Vector2(Player.width / 2, Player.height / 2) + new Vector2(Main.rand.NextFloat(-1800f, 1800f), (Main.rand.NextFloat(-1200f, 1200f)));

                    Vector2 speed = Main.rand.NextVector2Circular(1f, 1f);
                    Vector2 speed2 = Main.rand.NextVector2Circular(0.1f, 0.1f);
                    ParticleManager.NewParticle(Player.Center - RandomOrig3, speed * 0.5f, ParticleManager.NewInstance<FabledParticle3>(), Color.RoyalBlue, Main.rand.NextFloat(0.2f, 0.8f));
                }




                if (GoldenRingCooldown > 2)
                {
                    for (int j = 0; j < 2; j++)
                    {
                        RandomOrig3 = new Vector2(Player.width / 2, Player.height / 2) + new Vector2(Main.rand.NextFloat(-900f, 900f), (Main.rand.NextFloat(-600f, 600f)));
                        RandomOrig2 = new Vector2(Player.width / 2, Player.height / 2) + new Vector2(Main.rand.NextFloat(-1600f, 1600f), (Main.rand.NextFloat(-900f, 900f)));
                        RandomOrig = new Vector2(Player.width / 2, Player.height / 2) + new Vector2(Main.rand.NextFloat(-1800f, 1800f), (Main.rand.NextFloat(-1200f, 1200f)));

                        Vector2 speed = Main.rand.NextVector2Circular(1f, 1f);
                        Vector2 speed2 = Main.rand.NextVector2Circular(0.1f, 0.1f);
                        ParticleManager.NewParticle(Player.Center - RandomOrig3, speed * 2, ParticleManager.NewInstance<GoldRingParticle3>(), Color.RoyalBlue, Main.rand.NextFloat(0.2f, 0.8f));

                        GoldenRingCooldown = 0;
                    }
                }

                if (GoldenSparkleCooldown > 100)
                {
                    for (int j = 0; j < 1; j++)
                    {
                        RandomOrig3 = new Vector2(Player.width / 2, Player.height / 2) + new Vector2(Main.rand.NextFloat(-900f, 900f), (Main.rand.NextFloat(-600f, 600f)));
                        RandomOrig2 = new Vector2(Player.width / 2, Player.height / 2) + new Vector2(Main.rand.NextFloat(-1600f, 1600f), (Main.rand.NextFloat(-900f, 900f)));
                        RandomOrig = new Vector2(Player.width / 2, Player.height / 2) + new Vector2(Main.rand.NextFloat(-1800f, 1800f), (Main.rand.NextFloat(-1200f, 1200f)));

                        Vector2 speed = Main.rand.NextVector2Circular(1f, 1f);
                        Vector2 speed2 = Main.rand.NextVector2Circular(0.1f, 0.1f);
                        ParticleManager.NewParticle(Player.Center - RandomOrig2, speed2 * 3, ParticleManager.NewInstance<GoldRingParticle3>(), Color.RoyalBlue, Main.rand.NextFloat(0.2f, 0.8f));
                        GoldenSparkleCooldown = 0;
                    }
                }


                GoldenRingCooldown++;
                GoldenSparkleCooldown++;
                for (int j = 0; j < 5; j++)
                {
                    RandomOrig3 = new Vector2(Player.width / 2, Player.height / 2) + new Vector2(Main.rand.NextFloat(-900f, 900f), (Main.rand.NextFloat(-600f, 600f)));
                    RandomOrig2 = new Vector2(Player.width / 2, Player.height / 2) + new Vector2(Main.rand.NextFloat(-1600f, 1600f), (Main.rand.NextFloat(-900f, 900f)));
                    RandomOrig = new Vector2(Player.width / 2, Player.height / 2) + new Vector2(Main.rand.NextFloat(-1800f, 1800f), (Main.rand.NextFloat(-1200f, 1200f)));

                    Vector2 speed = Main.rand.NextVector2Circular(1f, 1f);
                    Vector2 speed2 = Main.rand.NextVector2Circular(0.1f, 0.1f);
                    ParticleManager.NewParticle(Player.Center - RandomOrig2, speed * 2, ParticleManager.NewInstance<morrowstar>(), Color.RoyalBlue, Main.rand.NextFloat(0.2f, 0.8f));
                }

                GoldenRingCooldown++;
                GoldenSparkleCooldown++;
               
            }






            if (ModContent.GetInstance<LunarVeilClientConfig>().ParticlesToggle == true && (ZoneCinder || ZoneDrakonic))
			{
				Main.windPhysicsStrength = 50;
				Main.UseHeatDistortion = true;
				GoldenRingCooldown++;
				GoldenSparkleCooldown++;
				RayCooldown++;

				for (int j = 0; j < 3; j++)
				{
					RandomOrig3 = new Vector2(Player.width / 2, Player.height / 2) + new Vector2(Main.rand.NextFloat(-900f, 900f), (Main.rand.NextFloat(-600f, 600f)));
					RandomOrig2 = new Vector2(Player.width / 2, Player.height / 2) + new Vector2(Main.rand.NextFloat(-1600f, 1600f), (Main.rand.NextFloat(-900f, 900f)));
					RandomOrig = new Vector2(Player.width / 2, Player.height / 2) + new Vector2(Main.rand.NextFloat(-1800f, 1800f), (Main.rand.NextFloat(-1200f, 1200f)));

					Vector2 speed = Main.rand.NextVector2Circular(1f, 1f);
					Vector2 speed2 = Main.rand.NextVector2Circular(0.1f, 0.1f);
					ParticleManager.NewParticle(Player.Center - RandomOrig, speed2 * 3, ParticleManager.NewInstance<UnderworldParticle1>(), Color.Orange, Main.rand.NextFloat(0.2f, 0.8f));
				}



				for (int j = 0; j < 2; j++)
				{
					RandomOrig3 = new Vector2(Player.width / 2, Player.height / 2) + new Vector2(Main.rand.NextFloat(-900f, 900f), (Main.rand.NextFloat(-600f, 600f)));
					RandomOrig2 = new Vector2(Player.width / 2, Player.height / 2) + new Vector2(Main.rand.NextFloat(-1600f, 1600f), (Main.rand.NextFloat(-900f, 900f)));
					RandomOrig = new Vector2(Player.width / 2, Player.height / 2) + new Vector2(Main.rand.NextFloat(-1800f, 1800f), (Main.rand.NextFloat(-1200f, 1200f)));

					Vector2 speed = Main.rand.NextVector2Circular(1f, 1f);
					Vector2 speed2 = Main.rand.NextVector2Circular(0.1f, 0.1f);
					ParticleManager.NewParticle(Player.Center - RandomOrig, speed2 * 3, ParticleManager.NewInstance<FlameParticle>(), Color.RoyalBlue, Main.rand.NextFloat(0.2f, 0.8f));
				}

				for (int j = 0; j < 1; j++)
				{
					RandomOrig3 = new Vector2(Player.width / 2, Player.height / 2) + new Vector2(Main.rand.NextFloat(-900f, 900f), (Main.rand.NextFloat(-600f, 600f)));
					RandomOrig2 = new Vector2(Player.width / 2, Player.height / 2) + new Vector2(Main.rand.NextFloat(-1600f, 1600f), (Main.rand.NextFloat(-900f, 900f)));
					RandomOrig = new Vector2(Player.width / 2, Player.height / 2) + new Vector2(Main.rand.NextFloat(-1800f, 1800f), (Main.rand.NextFloat(-1200f, 1200f)));

					Vector2 speed = Main.rand.NextVector2Circular(1f, 1f);
					Vector2 speed2 = Main.rand.NextVector2Circular(0.1f, 0.1f);
					ParticleManager.NewParticle(Player.Center - RandomOrig2, speed * 2, ParticleManager.NewInstance<UnderworldParticle2>(), Color.RoyalBlue, Main.rand.NextFloat(0.2f, 0.8f));
				}

				for (int j = 0; j < 1; j++)
				{
					RandomOrig3 = new Vector2(Player.width / 2, Player.height / 2) + new Vector2(Main.rand.NextFloat(-900f, 900f), (Main.rand.NextFloat(-600f, 600f)));
					RandomOrig2 = new Vector2(Player.width / 2, Player.height / 2) + new Vector2(Main.rand.NextFloat(-1600f, 1600f), (Main.rand.NextFloat(-900f, 900f)));
					RandomOrig = new Vector2(Player.width / 2, Player.height / 2) + new Vector2(Main.rand.NextFloat(-1800f, 1800f), (Main.rand.NextFloat(-1200f, 1200f)));

					Vector2 speed = Main.rand.NextVector2Circular(1f, 1f);
					Vector2 speed2 = Main.rand.NextVector2Circular(0.1f, 0.1f);
					ParticleManager.NewParticle(Player.Center - RandomOrig2, speed * 2, ParticleManager.NewInstance<UnderworldParticle3>(), Color.RoyalBlue, Main.rand.NextFloat(0.2f, 0.8f));
				}
			}














			if (ModContent.GetInstance<LunarVeilClientConfig>().VanillaParticlesToggle == true && Player.ZoneUnderworldHeight)
			{
				Main.windPhysicsStrength = 50;
				GoldenRingCooldown++;
				GoldenSparkleCooldown++;
				RayCooldown++;

				for (int j = 0; j < 3; j++)
				{
					RandomOrig3 = new Vector2(Player.width / 2, Player.height / 2) + new Vector2(Main.rand.NextFloat(-900f, 900f), (Main.rand.NextFloat(-600f, 600f)));
					RandomOrig2 = new Vector2(Player.width / 2, Player.height / 2) + new Vector2(Main.rand.NextFloat(-1600f, 1600f), (Main.rand.NextFloat(-900f, 900f)));
					RandomOrig = new Vector2(Player.width / 2, Player.height / 2) + new Vector2(Main.rand.NextFloat(-1800f, 1800f), (Main.rand.NextFloat(-1200f, 1200f)));

					Vector2 speed = Main.rand.NextVector2Circular(1f, 1f);
					Vector2 speed2 = Main.rand.NextVector2Circular(0.1f, 0.1f);
					ParticleManager.NewParticle(Player.Center - RandomOrig, speed2 * 3, ParticleManager.NewInstance<UnderworldParticle1>(), Color.Orange, Main.rand.NextFloat(0.2f, 0.8f));
				}

				

				for (int j = 0; j < 2; j++)
				{
					RandomOrig3 = new Vector2(Player.width / 2, Player.height / 2) + new Vector2(Main.rand.NextFloat(-900f, 900f), (Main.rand.NextFloat(-600f, 600f)));
					RandomOrig2 = new Vector2(Player.width / 2, Player.height / 2) + new Vector2(Main.rand.NextFloat(-1600f, 1600f), (Main.rand.NextFloat(-900f, 900f)));
					RandomOrig = new Vector2(Player.width / 2, Player.height / 2) + new Vector2(Main.rand.NextFloat(-1800f, 1800f), (Main.rand.NextFloat(-1200f, 1200f)));

					Vector2 speed = Main.rand.NextVector2Circular(1f, 1f);
					Vector2 speed2 = Main.rand.NextVector2Circular(0.1f, 0.1f);
					ParticleManager.NewParticle(Player.Center - RandomOrig, speed2 * 3, ParticleManager.NewInstance<FlameParticle>(), Color.RoyalBlue, Main.rand.NextFloat(0.2f, 0.8f));
				}

				for (int j = 0; j < 1; j++)
				{
					RandomOrig3 = new Vector2(Player.width / 2, Player.height / 2) + new Vector2(Main.rand.NextFloat(-900f, 900f), (Main.rand.NextFloat(-600f, 600f)));
					RandomOrig2 = new Vector2(Player.width / 2, Player.height / 2) + new Vector2(Main.rand.NextFloat(-1600f, 1600f), (Main.rand.NextFloat(-900f, 900f)));
					RandomOrig = new Vector2(Player.width / 2, Player.height / 2) + new Vector2(Main.rand.NextFloat(-1800f, 1800f), (Main.rand.NextFloat(-1200f, 1200f)));

					Vector2 speed = Main.rand.NextVector2Circular(1f, 1f);
					Vector2 speed2 = Main.rand.NextVector2Circular(0.1f, 0.1f);
					ParticleManager.NewParticle(Player.Center - RandomOrig2, speed * 2, ParticleManager.NewInstance<UnderworldParticle2>(), Color.RoyalBlue, Main.rand.NextFloat(0.2f, 0.8f));
				}

				for (int j = 0; j < 1; j++)
				{
					RandomOrig3 = new Vector2(Player.width / 2, Player.height / 2) + new Vector2(Main.rand.NextFloat(-900f, 900f), (Main.rand.NextFloat(-600f, 600f)));
					RandomOrig2 = new Vector2(Player.width / 2, Player.height / 2) + new Vector2(Main.rand.NextFloat(-1600f, 1600f), (Main.rand.NextFloat(-900f, 900f)));
					RandomOrig = new Vector2(Player.width / 2, Player.height / 2) + new Vector2(Main.rand.NextFloat(-1800f, 1800f), (Main.rand.NextFloat(-1200f, 1200f)));

					Vector2 speed = Main.rand.NextVector2Circular(1f, 1f);
					Vector2 speed2 = Main.rand.NextVector2Circular(0.1f, 0.1f);
					ParticleManager.NewParticle(Player.Center - RandomOrig2, speed * 2, ParticleManager.NewInstance<UnderworldParticle3>(), Color.RoyalBlue, Main.rand.NextFloat(0.2f, 0.8f));
				}
			}

			if (Dice)
			{
				Timer++;
				if (Timer == 90 || DiceCooldown == 90)
				{
					var entitySource = player.GetSource_FromThis();

					switch (Main.rand.Next(5))
					{

						case 0:


							CombatText.NewText(player.getRect(), Color.YellowGreen, LangText.Misc("Dice.1"), true, false);
							for (int i = 0; i < player.inventory.Length; i++)

							{

								if (player.inventory[i].type == ModContent.ItemType<GambitToken>())

								{
									Item item = new Item();
									player.QuickSpawnItem(entitySource, ModContent.ItemType<GildedBag1>(), Main.rand.Next(1, 1));
									player.inventory[i].TurnToAir();
									player.inventory[i] = item;
									SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Kaboom"));


									Dice = false;
									break;

								}
							}
							break;

						case 1:


							CombatText.NewText(player.getRect(), Color.YellowGreen, LangText.Misc("Dice.2"), true, false);
							for (int i = 0; i < player.inventory.Length; i++)

							{

								if (player.inventory[i].type == ModContent.ItemType<GambitToken>())

								{
									Item item = new Item();
									player.QuickSpawnItem(entitySource, ModContent.ItemType<GildedBag1>(), Main.rand.Next(1, 2));
									player.inventory[i].TurnToAir();
									player.inventory[i] = item;
									SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Kaboom"));


									Dice = false;
									break;

								}
							}
							break;

						case 2:


							CombatText.NewText(player.getRect(), Color.YellowGreen, LangText.Misc("Dice.3"), true, false);
							for (int i = 0; i < player.inventory.Length; i++)

							{

								if (player.inventory[i].type == ModContent.ItemType<GambitToken>())

								{
									Item item = new Item();
									player.QuickSpawnItem(entitySource, ModContent.ItemType<GildedBag1>(), Main.rand.Next(0, 1));
									player.inventory[i].TurnToAir();
									player.inventory[i] = item;
									SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Kaboom"));


									Dice = false;
									break;

								}
							}
							break;

						case 3:


							CombatText.NewText(player.getRect(), Color.YellowGreen, LangText.Misc("Dice.4"), true, false);
							for (int i = 0; i < player.inventory.Length; i++)

							{

								if (player.inventory[i].type == ModContent.ItemType<GambitToken>())

								{
									Item item = new Item();

									player.inventory[i].TurnToAir();
									player.inventory[i] = item;
									SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Kaboom"));

									Dice = false;
									break;

								}
							}
							break;


						case 4:

							CombatText.NewText(player.getRect(), Color.YellowGreen, LangText.Misc("Dice.5"), true, false);
							for (int i = 0; i < player.inventory.Length; i++)

							{

								if (player.inventory[i].type == ModContent.ItemType<GambitToken>())

								{
									Item item = new Item();
									player.QuickSpawnItem(entitySource, ModContent.ItemType<GildedBag1>(), Main.rand.Next(2, 2));
									player.inventory[i].TurnToAir();
									player.inventory[i] = item;
									SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Kaboom"));


									Dice = false;
									break;

								}
							}
							break;

					}
					Timer = 0;

				}



			}


            #region//--------------------------------------------------------------------- Bridget lmaooo (1000 lines)



            for (int i = 0; i < player.inventory.Length; i++)
			{
				if (player.inventory[i].type == ModContent.ItemType<Bridget>())
				{
					Bridget++;
					if (Bridget > 1080)
                    {
						int combatText=-1;
						switch (Main.rand.Next(30))
						{

							case 0:

								combatText = CombatText.NewText(player.getRect(), Color.YellowGreen, LangText.Misc("Bridget.1"), true, false);
								Bridget = 0;
								SoundEngine.PlaySound(SoundID.LucyTheAxeTalk, player.position);
								break;

							case 1:

								combatText = CombatText.NewText(player.getRect(), Color.YellowGreen, LangText.Misc("Bridget.2"), true, false);
								Bridget = 0;
								SoundEngine.PlaySound(SoundID.LucyTheAxeTalk, player.position);
								break;

							case 2:

								combatText = CombatText.NewText(player.getRect(), Color.YellowGreen, LangText.Misc("Bridget.3"), true, false);
								Bridget = 0;
								SoundEngine.PlaySound(SoundID.LucyTheAxeTalk, player.position);
								break;

							case 4:

								combatText = CombatText.NewText(player.getRect(), Color.YellowGreen, LangText.Misc("Bridget.4"), true, false);
								Bridget = 0;
								SoundEngine.PlaySound(SoundID.LucyTheAxeTalk, player.position);
								break;

							case 5:

								combatText = CombatText.NewText(player.getRect(), Color.YellowGreen, LangText.Misc("Bridget.5"), true, false);
								Bridget = 0;
								SoundEngine.PlaySound(SoundID.LucyTheAxeTalk, player.position);
								break;

							case 6:

								combatText = CombatText.NewText(player.getRect(), Color.YellowGreen, LangText.Misc("Bridget.6"), true, false);
								Bridget = 0;
								SoundEngine.PlaySound(SoundID.LucyTheAxeTalk, player.position);
								break;

							case 7:

								combatText = CombatText.NewText(player.getRect(), Color.YellowGreen, LangText.Misc("Bridget.7"), true, false);
								Bridget = 0;
								SoundEngine.PlaySound(SoundID.LucyTheAxeTalk, player.position);
								break;


							case 8:

								combatText = CombatText.NewText(player.getRect(), Color.YellowGreen, LangText.Misc("Bridget.8"), true, false);
								Bridget = 0;
								SoundEngine.PlaySound(SoundID.LucyTheAxeTalk, player.position);
								break;


							case 9:

								combatText = CombatText.NewText(player.getRect(), Color.YellowGreen, LangText.Misc("Bridget.9"), true, false);
								Bridget = 0;
								SoundEngine.PlaySound(SoundID.LucyTheAxeTalk, player.position);
								break;

							case 10:

								combatText = CombatText.NewText(player.getRect(), Color.YellowGreen, LangText.Misc("Bridget.10"), true, false);
								Bridget = 0;
								SoundEngine.PlaySound(SoundID.LucyTheAxeTalk, player.position);
								break;

							case 11:

								combatText = CombatText.NewText(player.getRect(), Color.YellowGreen, LangText.Misc("Bridget.11"), true, false);
								Bridget = 0;
								SoundEngine.PlaySound(SoundID.LucyTheAxeTalk, player.position);
								break;

							case 12:

								combatText = CombatText.NewText(player.getRect(), Color.YellowGreen, LangText.Misc("Bridget.12"), true, false);
								Bridget = 0;
								SoundEngine.PlaySound(SoundID.LucyTheAxeTalk, player.position);
								break;

							case 13:

								combatText = CombatText.NewText(player.getRect(), Color.YellowGreen, LangText.Misc("Bridget.13"), true, false);
								Bridget = 0;
								SoundEngine.PlaySound(SoundID.LucyTheAxeTalk, player.position);
								break;

							case 14:

								combatText = CombatText.NewText(player.getRect(), Color.YellowGreen, LangText.Misc("Bridget.14"), true, false);
								Bridget = 0;
								SoundEngine.PlaySound(SoundID.LucyTheAxeTalk, player.position);
								break;


							case 15:

								combatText = CombatText.NewText(player.getRect(), Color.YellowGreen, LangText.Misc("Bridget.15"), true, false);
								Bridget = 0;
								SoundEngine.PlaySound(SoundID.LucyTheAxeTalk, player.position);
								break;

							case 16:

								combatText = CombatText.NewText(player.getRect(), Color.YellowGreen, LangText.Misc("Bridget.16"), true, false);
								Bridget = 0;
								SoundEngine.PlaySound(SoundID.LucyTheAxeTalk, player.position);
								break;

							case 17:

								combatText = CombatText.NewText(player.getRect(), Color.YellowGreen, LangText.Misc("Bridget.17"), true, false);
								Bridget = 0;
								SoundEngine.PlaySound(SoundID.LucyTheAxeTalk, player.position);
								break;

							case 18:

								combatText = CombatText.NewText(player.getRect(), Color.YellowGreen, LangText.Misc("Bridget.18"), true, false);
								Bridget = 0;
								SoundEngine.PlaySound(SoundID.LucyTheAxeTalk, player.position);
								break;

							case 19:

								combatText = CombatText.NewText(player.getRect(), Color.YellowGreen, LangText.Misc("Bridget.19"), true, false);
								Bridget = 0;
								SoundEngine.PlaySound(SoundID.LucyTheAxeTalk, player.position);
								break;

							case 20:

								combatText = CombatText.NewText(player.getRect(), Color.YellowGreen, LangText.Misc("Bridget.20"), true, false);
								Bridget = 0;
								SoundEngine.PlaySound(SoundID.LucyTheAxeTalk, player.position);
								break;

							case 21:

								combatText = CombatText.NewText(player.getRect(), Color.YellowGreen, LangText.Misc("Bridget.21"), true, false);
								Bridget = 0;
								SoundEngine.PlaySound(SoundID.LucyTheAxeTalk, player.position);
								break;

							case 22:

								combatText = CombatText.NewText(player.getRect(), Color.YellowGreen, LangText.Misc("Bridget.22"), true, false);
								Bridget = 0;
								SoundEngine.PlaySound(SoundID.LucyTheAxeTalk, player.position);
								break;

							case 23:

								combatText = CombatText.NewText(player.getRect(), Color.YellowGreen, LangText.Misc("Bridget.23"), true, false);
								Bridget = 0;
								SoundEngine.PlaySound(SoundID.LucyTheAxeTalk, player.position);
								break;


							case 24:

								combatText = CombatText.NewText(player.getRect(), Color.YellowGreen, LangText.Misc("Bridget.24"), true, false);
								Bridget = 0;
								SoundEngine.PlaySound(SoundID.LucyTheAxeTalk, player.position);
								break;

							case 25:

								combatText = CombatText.NewText(player.getRect(), Color.YellowGreen, LangText.Misc("Bridget.25"), true, false);
								Bridget = 0;
								SoundEngine.PlaySound(SoundID.LucyTheAxeTalk, player.position);
								break;


							case 26:

								combatText = CombatText.NewText(player.getRect(), Color.YellowGreen, LangText.Misc("Bridget.26"), true, false);
								Bridget = 0;
								SoundEngine.PlaySound(SoundID.LucyTheAxeTalk, player.position);
								break;

							case 27:

								combatText = CombatText.NewText(player.getRect(), Color.YellowGreen, LangText.Misc("Bridget.27"), true, false);
								Bridget = 0;
								SoundEngine.PlaySound(SoundID.LucyTheAxeTalk, player.position);
								break;

							case 28:

								combatText = CombatText.NewText(player.getRect(), Color.YellowGreen, LangText.Misc("Bridget.28"), true, false);
								Bridget = 0;
								SoundEngine.PlaySound(SoundID.LucyTheAxeTalk, player.position);
								break;

							case 29:

								combatText = CombatText.NewText(player.getRect(), Color.YellowGreen, LangText.Misc("Bridget.29"), true, false);
								Bridget = 0;
								SoundEngine.PlaySound(SoundID.LucyTheAxeTalk, player.position);
								break;

						}

						//Checking if combatText not equal to -1 just incase it somehow didn't get set
						if(combatText != -1)
                        {
							//360 ticks = 6 seconds
							CombatText text = Main.combatText[combatText];
							text.lifeTime = 360;
						}
					}



						break;

				}













			}

            #endregion 
        }
        public const int CAMO_DELAY = 100;
		public override void PreUpdate()
		{
			

			


			if (Main.hasFocus)
				AddForegroundOrBackground();
		}

		bool Sirestiastalk;
		bool Zuitalk;
		public override void SaveData(TagCompound tag)
		{
			tag["Sirestiastalk"] = Sirestiastalk;
            tag["MonO"] = DreadMonOne;
            tag["MonTw"] = DreadMonTwo;
            tag["MonTh"] = DreadMonThree;
            tag["Zuitalk"] = Zuitalk;
		}

		public override void LoadData(TagCompound tag)
		{
            DreadMonOne = tag.GetBool("MonO");
            DreadMonTwo = tag.GetBool("MonTw");
            DreadMonThree = tag.GetBool("MonTh");
            Sirestiastalk = tag.GetBool("Sirestiastalk");
			Zuitalk = tag.GetBool("Zuitalk");
		}

      
        private void AddForegroundOrBackground()
		{
			if (ZoneIlluria || ZoneIshtar)
			{
				int leafFGChance = Starstrike.SpawnChance(Player);
				if (leafFGChance != -1 && Main.rand.NextBool(leafFGChance))
				{
					bool spawnForegroundItem = true;
					bool spawnOnPlayerLayer = true;
					Vector2 pos = Player.Center - new Vector2(Main.rand.Next(-(int)(Main.screenWidth * 2f), (int)(Main.screenWidth * 2f)), Main.screenHeight * 0.52f);
					ForegroundHelper.AddItem(new Starstrike(pos), spawnForegroundItem, spawnOnPlayerLayer);
				}



				int SnowFGChance = Snowstrike.SpawnChance(Player);
				if (SnowFGChance != -1 && Main.rand.NextBool(SnowFGChance))
				{
					bool spawnForegroundItem = true;
					bool spawnOnPlayerLayer = true;
					Vector2 pos = Player.Center - new Vector2(Main.rand.Next(-(int)(Main.screenWidth * 2f), (int)(Main.screenWidth * 2f)), Main.screenHeight * 0.52f);
					ForegroundHelper.AddItem(new Snowstrike(pos), spawnForegroundItem, spawnOnPlayerLayer);
				}
			}


			if (Main._shouldUseWindyDayMusic)
			{
				int leafFGChance = Cherryblossom.SpawnChance(Player);
				if (leafFGChance != -1 && Main.rand.NextBool(leafFGChance))
				{
					bool spawnForegroundItem = true;
					bool spawnOnPlayerLayer = true;
					Vector2 pos = Player.Center - new Vector2(Main.rand.Next(-(int)(Main.screenWidth * 2f), (int)(Main.screenWidth * 2f)), Main.screenHeight * 0.52f);
					ForegroundHelper.AddItem(new Cherryblossom(pos), spawnForegroundItem, spawnOnPlayerLayer);
				}



				
			}

			if (Main.raining && (Player.ZoneForest || ZoneVillage))
			{
				int leafFGChance = Cherryblossom.SpawnChance(Player);
				if (leafFGChance != -1 && Main.rand.NextBool(leafFGChance))
				{
					bool spawnForegroundItem = true;
					bool spawnOnPlayerLayer = true;
					Vector2 pos = Player.Center - new Vector2(Main.rand.Next(-(int)(Main.screenWidth * 2f), (int)(Main.screenWidth * 2f)), Main.screenHeight * 0.52f);
					ForegroundHelper.AddItem(new Cherryblossom(pos), spawnForegroundItem, spawnOnPlayerLayer);
				}




			}

			if ((Player.ZoneDesert))
			{
				int leafFGChance = Sandstrike.SpawnChance(Player);
				if (leafFGChance != -1 && Main.rand.NextBool(leafFGChance))
				{
					bool spawnForegroundItem = true;
					bool spawnOnPlayerLayer = true;
					Vector2 pos = Player.Center - new Vector2(Main.rand.Next(-(int)(Main.screenWidth * 2f), (int)(Main.screenWidth * 2f)), Main.screenHeight * 0.52f);
					ForegroundHelper.AddItem(new Sandstrike(pos), spawnForegroundItem, spawnOnPlayerLayer);
				}




			}
		}



		public int Shake = 0;

        public override void OnHitNPCWithItem(Item item, NPC target, NPC.HitInfo hit, int damageDone)/* tModPorter If you don't need the Item, consider using OnHitNPC instead */
		{
			if (Player.HeldItem.DamageType == DamageClass.Ranged && TAuraSpawn && TAuraCooldown <= 0)
			{
				Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Player.velocity * -4, ProjectileID.SpikyBall, 30, 1f, Player.whoAmI);
				Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Player.velocity * 4, ProjectileID.SpikyBall, 20, 1f, Player.whoAmI);
				Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Player.velocity, ProjectileID.SpikyBall, 50, 1f, Player.whoAmI);
				TAuraCooldown = 600;

			}

			if (BroochFrile && FrileBDCooldown <= 0)
			{

				Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Player.velocity, ModContent.ProjectileType<FrileBroochP>(), 4, 1f, Player.whoAmI);
				FrileBDCooldown = 1;

			}

			if (Lovestruck)
			{

				Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Player.velocity, ModContent.ProjectileType<LovestruckP>(), 4, 1f, Player.whoAmI);


			}


		}

		public override void OnHitNPCWithProj(Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)/* tModPorter If you don't need the Projectile, consider using OnHitNPC instead */
		{
			if (Lovestruck && LovestruckBCooldown <= 0)
			{

				Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Player.velocity, ModContent.ProjectileType<LovestruckP>(), 4, 1f, Player.whoAmI);
				LovestruckBCooldown = 30;

			}

			if (Player.HeldItem.DamageType == DamageClass.Ranged && TAuraSpawn && TAuraCooldown <= 0)
			{
				Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Player.velocity * -4, ProjectileID.SpikyBall, 30, 1f, Player.whoAmI);
				Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Player.velocity * 4, ProjectileID.SpikyBall, 20, 1f, Player.whoAmI);
				Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Player.velocity, ProjectileID.SpikyBall, 50, 1f, Player.whoAmI);
				TAuraCooldown = 600;
			}
		}


		public override void OnHitByNPC(NPC npc, Player.HurtInfo hurtInfo)
		{
			if (ThornedBook)
			{
				if (npc.type != ModContent.NPCType<SingularityFragment>() && npc.type != ModContent.NPCType<Rek>() && npc.type != ModContent.NPCType<SupernovaFragment>())
                {
					npc.SimpleStrikeNPC(hurtInfo.Damage * 5, hurtInfo.HitDirection, crit: false, hurtInfo.Knockback);
				}	
			}

			if (Lovestruck)
			{
				Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Player.velocity, ModContent.ProjectileType<LovestruckP>(), 4, 1f, Player.whoAmI);
				if (npc.type != ModContent.NPCType<SingularityFragment>() && npc.type != ModContent.NPCType<Rek>())
				{
					npc.SimpleStrikeNPC(hurtInfo.Damage * 3, hurtInfo.HitDirection, crit: false, hurtInfo.Knockback);
				}
			}


			if (ADisease)
            {
				switch (Main.rand.Next(8))
				{
					case 0:

						npc.AddBuff((BuffID.Poisoned), 120);
						break;
					case 1:

						npc.AddBuff((BuffID.Slow), 120);

						break;
					case 2:

						npc.AddBuff((BuffID.OnFire3), 240);

						break;
					case 3:

						npc.AddBuff((BuffID.OnFire), 120);

						break;
					case 4:

						npc.AddBuff((BuffID.Frostburn2), 240);

						break;
					case 5:

						npc.AddBuff((BuffID.BrainOfConfusionBuff), 240);

						break;

					case 6:

						npc.AddBuff((BuffID.Lovestruck), 240);

						break;
					case 7:

						npc.AddBuff((ModContent.BuffType<Wounded>()), 240);

						break;
				}
			}
		}

        public override void PostUpdateEquips()
        {
			//Terric Setbonus

			if (Teric)
			{
				TericGramTime++;
				if (TericGramLevel == 2)
				{
					Lighting.AddLight(Player.Center, Color.DarkRed.ToVector3() * 0.5f * Main.essScale);
					Player.GetCritChance(DamageClass.Magic) += 21;
				}
				if (TericGramLevel == 1)
				{
					Lighting.AddLight(Player.Center, Color.DarkRed.ToVector3() * 0.25f * Main.essScale);
					Player.GetCritChance(DamageClass.Magic) += 10;
				}

				if (TericGramTime >= 340)
				{
					TericGramTime = 0;
					if (TericGramLevel < 2)
					{
						if (TericGramLevel == 1)
						{
							var EntitySource = Player.GetSource_FromThis();
							NPC.NewNPC(EntitySource, (int)Player.Center.X, (int)Player.Center.Y, ModContent.NPCType<TericGramNPC2>());
							TericGramLevel += 1;
						}
						else
						{
							Lighting.AddLight(Player.Center, Color.DarkRed.ToVector3() * 0.75f * Main.essScale);
							var EntitySource = Player.GetSource_FromThis();
							NPC.NewNPC(EntitySource, (int)Player.Center.X, (int)Player.Center.Y, ModContent.NPCType<TericGramNPC>());
							TericGramLevel += 1;
						}
					}
				}
			}
			else
			{
				TericGramTime = 0;
				TericGramLevel = 0;
			}

			//Sap Container's Effect
			if (ArcaneM)
            {
				if (ArcaneMCooldown == 601)
				{
					SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/ArcaneExplode"));
					for (int j = 0; j < 7; j++)
					{
						Vector2 speed = Main.rand.NextVector2Circular(1f, 1f);
						ParticleManager.NewParticle(Player.Center, speed * 3, 
							ParticleManager.NewInstance<ArcanalParticle>(), Color.RoyalBlue, Main.rand.NextFloat(0.2f, 0.8f));
					}
				}

				if (ArcaneMCooldown > 600)
				{
					Player.GetDamage(DamageClass.Magic) *= 1.25f;
				}

				if (ArcaneMCooldown > 720)
				{
					ArcaneMCooldown = 0;
				}
			}
			else
			{
				ArcaneMCooldown = 0;
			}
			

			if (StealthRune)
			{
				if (StealthTime <= 1800)
				{
					StealthTime++;
				}
				else
				{
					if (Main.rand.NextBool(5))
                    {
                        int d = Dust.NewDust(Player.position, Player.width, Player.height,
                                ModContent.DustType<GlowDust>(), newColor: Color.Red, Scale: 0.8f);
                        Main.dust[d].noGravity = true;
              
						if (Main.rand.NextBool(5))
						{
                            Dust.NewDust(Player.position, Player.width, Player.height,
                              ModContent.DustType<GunFlash>(), newColor: Color.Red, Scale: 0.8f);

                        }
                    }
                }

				float maxDamageIncrease = 0.33f;
				float stealthProgress = StealthTime / 1800;
				float damageIncrease = stealthProgress * maxDamageIncrease;
				Player.GetDamage(DamageClass.Generic) += damageIncrease;
			}

			if (SpiritPendent && ZoneAbyss)
			{
				Player.GetDamage(DamageClass.Generic) += 250 / 1500f;
			}


			if (ThreeTwoOneSmile && ThreeTwoOneSmileBCooldown > 1480)
			{
				Player.GetDamage(DamageClass.Generic) += PPPaintDMG;
				Player.GetCritChance(DamageClass.Generic) = 100f;


				if (PPPaintI)
				{
					PPPaintDMG2 = 15;
				}

				if (PPPaintI && PPPaintII)
				{
					PPPaintDMG2 = 50;
				}

				if (PPPaintI && PPPaintII && PPPaintIII)
				{
					PPPaintDMG2 = 150;
				}
			}

			if (GovheilC && GovheilBCooldown > 300)
			{
				Player.GetDamage(DamageClass.Ranged) *= 1.5f;
				Player.GetDamage(DamageClass.Melee) *= 1.5f;

			}

			if (GovheilB && GovheilBCooldown > 300)
			{
				Player.GetDamage(DamageClass.Magic) *= 1.1f;
				Player.GetDamage(DamageClass.Summon) *= 1.1f;

			}

			if (DucanB && DucanBCooldown > 350)
			{
				Player.GetDamage(DamageClass.Melee) *= 1.1f;
				

			}

			if (NotiaB && NotiaBCooldown > 300)
			{
				Player.GetDamage(DamageClass.Magic) *= 1.2f;
				Player.GetDamage(DamageClass.Ranged) *= 1.2f;
			}

			if (SpiritPendent && ZoneAbyss)
			{
				Player.GetDamage(DamageClass.Magic) += 250 / 150f;
				Player.GetDamage(DamageClass.Summon) += 250 / 1500f;
				Player.GetDamage(DamageClass.Throwing) += 250 / 1500f;
				Player.GetDamage(DamageClass.Ranged) += 250 / 1500f;
				Player.GetDamage(DamageClass.Melee) += 250 / 1500f;
			}
		}

        public override bool PreItemCheck()
		{
			if (Player.selectedItem != lastSelectedI)
			{
				SwordComboR = 0;
				SwordCombo = 0;
				lastSelectedI = Player.selectedItem;
			}
			if (SwordComboR > 0)
			{
				SwordComboR--;
				if (SwordComboR == 0)
				{
					SwordCombo = 0;
				}
			}




			return true;
		}

	}
}
