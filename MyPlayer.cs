using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using Stellamod.Assets.Biomes;
using Stellamod.Brooches;
using Stellamod.Buffs;
using Stellamod.Buffs.Charms;
using Stellamod.Dusts;
using Stellamod.Items.Accessories.Runes;
using Stellamod.Items.Armors.Daedia;
using Stellamod.Items.Armors.Govheil;
using Stellamod.Items.Armors.Lovestruck;
using Stellamod.Items.Armors.Verl;
using Stellamod.Items.Consumables;
using Stellamod.NPCs.Bosses.singularityFragment;
using Stellamod.NPCs.Bosses.DreadMire.Heart;
using Stellamod.NPCs.Bosses.DreadMire;
using Stellamod.NPCs.Bosses.INest;
using Stellamod.Particles;
using Stellamod.Projectiles;

using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Stellamod.Items.Weapons.Summon;
using Stellamod.Projectiles.Swords;
using Stellamod.Projectiles.Gun;
using Stellamod.WorldG;
using Terraria.GameContent;
using Stellamod.NPCs.Bosses.Verlia;

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
		public int SwordComboR;
		public int lastSelectedI;
		public bool Lovestruck;
		public int LovestruckBCooldown = 0;
		public bool ADisease;
		public bool ZoneFable = false;
		private Vector2 RandomOrig;
		private Vector2 RandomOrig2;
		private Vector2 RandomOrig3;
		public int GoldenRingCooldown = 0;
		public int GoldenSparkleCooldown = 0;
		public int RayCooldown = 0;
		public int VerliaBDCooldown = 5;
		public bool GovheilB;
		public bool GovheilC;
		public int GovheilBCooldown = 0;
		public bool Daedstruck;
		public int DaedstruckBCooldown = 1;













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
		//---------------------------------------------------------------------------------------------------------------








		public float screenFlash;
        private float screenFlashSpeed = 0.05f;
        private Vector2? screenFlashCenter;
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
		public bool ZoneAcid;
        public float AssassinsSlashes;
        public float AssassinsTime;
        public bool AssassinsSlash;
        public NPC AssassinsSlashnpc;
        public bool StealthRune;
        public bool SingularityFragment;
        public float StealthTime;

        public bool CorsageRune;
        public float CorsageTime;

        public bool DetonationRune;

        public bool ShadowCharm = false;

		public bool ClamsPearl;

        public bool WindRuneOn;
        public bool WindRune;
        public bool ShadeRune = false;
        public bool RealityRune = false;
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


        public bool Dead;





        public void ShakeAtPosition(Vector2 position, float distance, float strength)
        {
            this.shakeDrama = strength * (1f - base.Player.Center.Distance(position) / distance) * 0.5f;
        }
        public override void ModifyScreenPosition()
        {
            if (this.shouldFocus)
            {
                if (this.focusLength > 0f)
                {
                    if (this.focusTransition <= 1f)
                    {
                        Main.screenPosition = Vector2.SmoothStep(this.startPoint, this.focusPoint, this.focusTransition += 0.05f);
                    }
                    else
                    {
                        Main.screenPosition = this.focusPoint;
                    }
                    this.focusLength -= 0.05f;
                }
                else if (this.focusTransition >= 0f)
                {
                    Main.screenPosition = Vector2.SmoothStep(base.Player.Center - new Vector2(Main.screenWidth / 2, Main.screenHeight / 2), this.focusPoint, this.focusTransition -= 0.05f);
                }
                else
                {
                    this.shouldFocus = false;
                }
            }
            if (this.shakeDrama > 0.5f)
            {
                this.shakeDrama *= 0.92f;
                Vector2 shake = new Vector2(Main.rand.NextFloat(this.shakeDrama), Main.rand.NextFloat(this.shakeDrama));
                Main.screenPosition += shake;
            }
        }
        public void FocusOn(Vector2 pos, float length)
        {
            if (base.Player.Center.Distance(pos) < 2000f)
            {
                this.focusPoint = pos - new Vector2(Main.screenWidth / 2, Main.screenHeight / 2);
                this.focusTransition = 0f;
                this.startPoint = Main.screenPosition;
                this.focusLength = length;
                this.shouldFocus = true;
            }
        }
        public override bool PreKill(double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
        {
			Dead = true;
            HMArmor = false;
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

        }
        public override void OnHitAnything(float x, float y, Entity victim)
        {
            if (GHE)
            {
                GHETarget = victim;
            }
            if (DetonationRune)
            {
                if (Main.rand.NextBool(5))
                {
                    var EntitySource = Player.GetSource_FromThis();
                    Projectile.NewProjectile(EntitySource, victim.Center.X, victim.Center.Y, 0, 0, ModContent.ProjectileType<DetonationBomb>(), Player.HeldItem.damage * 2, 1, Main.myPlayer, 0, 0);

                }
            }
            if (RealityRune)
            {
                if (Main.rand.NextBool(7))
                {
                    var EntitySource = Player.GetSource_FromThis();
                    Projectile.NewProjectile(EntitySource, Player.Center.X, Player.Center.Y, 0, 0, ModContent.ProjectileType<RealityBolt>(), Player.HeldItem.damage * 2, 1, Main.myPlayer, 0, 0);

                }
            }
        }


        public override void ModifyHurt(ref Player.HurtModifiers modifiers)/* tModPorter Override ImmuneTo, FreeDodge or ConsumableDodge instead to prevent taking damage */
		{
			if (WindRune && !Player.HasBuff(ModContent.BuffType<GintzelSheildCD>()) && !Player.HasBuff(ModContent.BuffType<GintzelSheild>()))
			{

				if (Main.rand.NextBool(4))
                {
                    var EntitySource = Player.GetSource_FromThis();
                    Projectile.NewProjectile(EntitySource, Player.Center.X, Player.Center.Y, 0, 0, ModContent.ProjectileType<WindeffectGintzl>(), Player.HeldItem.damage * 2, 1, Main.myPlayer, 0, 0);
                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Verispin"), Player.position);
                    Player.AddBuff(ModContent.BuffType<GintzelSheild>(), 400);
                    WindRuneOn = true;
  
                }
			}

			if (StealthRune && StealthTime >= 500)
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
			TAuraSpawn = false;
			HikersBSpawn = false;
			Player.lifeRegen += increasedLifeRegen;
			increasedLifeRegen = 0;
			ArcaneM = false;
			PlantH = false;
			ThornedBook = false;
			Dice = false;
			NotiaB = false;
			Lovestruck = false;
			ADisease = false;
			GovheilB = false;
			GovheilC = false;
			Daedstruck = false;

		BroochSpragald = false;
			BroochFrile = false;
			BroochFlyfish = false;
			BroochMorrow = false;
			BroochSlime = false;
			BroochDiari = false;
			BroochVerlia = false;









            SpiritPendent = false;
            GHE = false;
            ShadeRune = false;
            FCArmor = false;
            ClamsPearl = false;
            HMArmor = false;
            DetonationRune = false;
            CorsageRune = false;
            StealthRune = false;
            Leather = false;
            ShadowCharm = false;

            WindRune = false;



            if (SwordComboR <= 0)
			{
				SwordCombo = 0;
				SwordComboR = 0;
			}
			else
			{
				SwordComboR--;
			}
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





		public override void PostUpdateMiscEffects()
		{

			bool fable = (Player.ZoneOverworldHeight && ZoneFable);
			Player.ManageSpecialBiomeVisuals("Stellamod:GovheilSky", ZoneFable);

			base.Player.ManageSpecialBiomeVisuals("Stellamod:Acid", ZoneAcid);
			base.Player.ManageSpecialBiomeVisuals("Stellamod:Gintzing", EventWorld.Gintzing);


            base.Player.ManageSpecialBiomeVisuals("Stellamod:Verlia", NPC.AnyNPCs(ModContent.NPCType<VerliaB>()));
        }

		public static SpriteBatch spriteBatch = new SpriteBatch(Main.graphics.GraphicsDevice);
		public override IEnumerable<Item> AddStartingItems(bool mediumCoreDeath)
		{

			return (IEnumerable<Item>)(object)new Item[1]
			{
				new Item(ModContent.ItemType<SirestiasStarterBag>(), 1, 0),
	
			};
		}
		public override void PostUpdate()
        {
            Player player = Main.LocalPlayer;
            if (!player.active)
				return;
			MyPlayer CVA = player.GetModPlayer<MyPlayer>();
			if (Dead)
            {
                HMArmorTime = 0;
                HMArmor = false;
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
                        Projectile.NewProjectile(EntitySource, GHEVector.X, GHEVector.Y, direction.X, direction.Y, ModContent.ProjectileType<GhostExcaliburProj>(), 42, 1, Main.myPlayer, 0, 0);
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
            if (NPC.AnyNPCs(ModContent.NPCType<SingularityFragment>()))
            {
                SingularityFragment = true;
            }
            else
            {
                SingularityFragment = false;
            }
            if (SingularityFragment)
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


                    Projectile.NewProjectile(EntitySource, AssassinsSlashnpc.Center.X, AssassinsSlashnpc.Center.Y, 0, 0, ModContent.ProjectileType<AssassinsSpawnEffect>(), Player.HeldItem.damage * 2, 1, Main.myPlayer, 0, 0);
                    Projectile.NewProjectile(EntitySource, AssassinsSlashnpc.Center.X, AssassinsSlashnpc.Center.Y, 0, 0, ModContent.ProjectileType<AssassinsSlashProj>(), 0, 1, Main.myPlayer, 0, 0);
                }
            }



            if (HMArmor)
            {
                HMArmorTime++;
                if (HMArmorTime <= 1)
                {
                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/ArcharilitDrone3"), player.position);
					var EntitySource = Player.GetSource_FromThis();

					Projectile.NewProjectile(EntitySource, player.Center.X, player.Center.Y, 0, 0, ModContent.ProjectileType<HMArncharMinionRight>(), Player.HeldItem.damage * 2, 1, Main.myPlayer, 0, 0);
                    Projectile.NewProjectile(EntitySource, player.Center.X, player.Center.Y, 0, 0, ModContent.ProjectileType<HMArncharMinionLeft>(), Player.HeldItem.damage * 2, 1, Main.myPlayer, 0, 0);
                    player.AddBuff(ModContent.BuffType<HMMinionBuff>(), 99999);
                }

            }
            else
            {
                player.ClearBuff(ModContent.BuffType<HMMinionBuff>());
                HMArmorTime = 0;
            }


            if (FCArmor)
            {
                FCArmorTime++;
                if (FCArmorTime <= 1)
                {
                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/CorsageRune1"), Player.position);
                    var EntitySource = Player.GetSource_FromThis();
                    Projectile.NewProjectile(EntitySource, player.Center.X, player.Center.Y, 0, 0, ModContent.ProjectileType<FCMinion>(), Player.HeldItem.damage * 2, 1, Main.myPlayer, 0, 0);
                    player.AddBuff(ModContent.BuffType<FCBuff>(), 99999);
                }

            }
            else
            {
                player.ClearBuff(ModContent.BuffType<FCBuff>());
                FCArmorTime = 0;
            }
            if (ZoneAcid)
            {
                if (player.wet)
                {
                    player.AddBuff(ModContent.BuffType<Irradiation>(), 30);
                }
                Main.raining = true;
                Main.maxRaining = 0.8f;
                Main.maxRain = 140;
                Main.rainTime = 18000;
                var entitySource = Player.GetSource_FromThis();
                Main.raining = true;
                Main.maxRaining = 0.8f;
                Main.maxRain = 140;
                Main.rainTime = 18000;
                float goreScale = Main.rand.NextFloat(0.5f, 0.9f);
                int x = (int)(Main.windSpeedCurrent > 0 ? Main.screenPosition.X - 100 : Main.screenPosition.X + Main.screenWidth + 100);
                int y = (int)Main.screenPosition.Y + Main.rand.Next(-100, Main.screenHeight);
                int a = Gore.NewGore(entitySource, new Vector2(x, y), Vector2.Zero, GoreID.TreeLeaf_Jungle, goreScale);
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

			if (StealthRune)
            {
                if (StealthTime <= 500)
                {
                    StealthTime++;
                }
                else
                {
                    if (Main.rand.NextBool(5))
                    {
                        int dustnumber = Dust.NewDust(Player.position, Player.width, Player.height, DustID.Firework_Red, 0f, 0f, 150, Color.Gold, 1f);
                        Main.dust[dustnumber].velocity *= 0.3f;
                        Main.dust[dustnumber].noGravity = true;
                    }
                }
                Player.GetDamage(DamageClass.Magic) += StealthTime / 150f;
                Player.GetDamage(DamageClass.Summon) += StealthTime / 1500f;
                Player.GetDamage(DamageClass.Throwing) += StealthTime / 1500f;
                Player.GetDamage(DamageClass.Ranged) += StealthTime / 1500f;
                Player.GetDamage(DamageClass.Melee) += StealthTime / 1500f;

            }
            if (SpiritPendent && ZoneAbyss)
            {
                Player.GetDamage(DamageClass.Magic) += 250 / 150f;
                Player.GetDamage(DamageClass.Summon) += 250 / 1500f;
                Player.GetDamage(DamageClass.Throwing) += 250 / 1500f;
                Player.GetDamage(DamageClass.Ranged) += 250 / 1500f;
                Player.GetDamage(DamageClass.Melee) += 250 / 1500f;
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
            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Brooches
            if (BroochSpragald && SpragaldBCooldown <= 0)
			{
				Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Player.velocity * -1f, ModContent.ProjectileType<SpragaldBrooch>(), 0, 1f, Player.whoAmI);

				Player.AddBuff(ModContent.BuffType<Spragald>(), 1000);
				SpragaldBCooldown = 1000;
			}

			if (BroochFrile && FrileBCooldown <= 0)
			{
				Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Player.velocity * -1f, ModContent.ProjectileType<FrileBrooch>(), 0, 1f, Player.whoAmI);

				Player.AddBuff(ModContent.BuffType<IceBrooch>(), 1000);
				FrileBCooldown = 1000;
			}


			if (BroochFlyfish && FlyfishBCooldown <= 0)
			{
				Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Player.velocity * -1f, ModContent.ProjectileType<FlyfishBrooch>(), 0, 1f, Player.whoAmI);

				Player.AddBuff(ModContent.BuffType<Flyfish>(), 1000);
				FlyfishBCooldown = 1000;
			}

			if (BroochMorrow && MorrowBCooldown <= 0)
			{
				Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Player.velocity * -1f, ModContent.ProjectileType<MorrowedBrooch>(), 0, 1f, Player.whoAmI);

				Player.AddBuff(ModContent.BuffType<Morrow>(), 1000);
				MorrowBCooldown = 1000;
			}


			if (BroochSlime && SlimeBCooldown <= 0)
			{
				Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Player.velocity * -1f, ModContent.ProjectileType<SlimeBrooch>(), 0, 1f, Player.whoAmI);

				Player.AddBuff(ModContent.BuffType<Slimee>(), 1000);
				SlimeBCooldown = 1000;
			}

			if (BroochDiari && DiariBCooldown <= 0)
			{
				Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Player.velocity * -1f, ModContent.ProjectileType<DiariBrooch>(), 0, 1f, Player.whoAmI);

				Player.AddBuff(ModContent.BuffType<Diarii>(), 1000);
				DiariBCooldown = 1000;
			}

			if (BroochVerlia && VerliaBCooldown <= 0)
			{
				Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Player.velocity * -1f, ModContent.ProjectileType<VerliaBrooch>(), 0, 1f, Player.whoAmI);

				Player.AddBuff(ModContent.BuffType<VerliaBroo>(), 1000);
				VerliaBCooldown = 1000;
			}

			if (BroochGint && GintBCooldown <= 0)
			{
				Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Player.velocity * -1f, ModContent.ProjectileType<GintzlBrooch>(), 0, 1f, Player.whoAmI);

				Player.AddBuff(ModContent.BuffType<GintBroo>(), 1000);
				GintBCooldown = 1000;
			}


			//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~








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
			if (NotiaB && NotiaBCooldown > 300)
			{
				Player.GetDamage(DamageClass.Magic) *= 2f;
				Player.GetDamage(DamageClass.Ranged) *= 2f;

			}
			if (NotiaB && NotiaBCooldown == 420)
			{
				NotiaBCooldown = 0;


			}

			if (Daedstruck && DaedstruckBCooldown == 0)
			{
				DaedstruckBCooldown = 600;
				Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Player.velocity * 0f, ModContent.ProjectileType<LightBomb>(), 30, 1f, Player.whoAmI);

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
			if (GovheilB && GovheilBCooldown > 300)
			{
				Player.GetDamage(DamageClass.Magic) *= 2f;
				Player.GetDamage(DamageClass.Summon) *= 2f;

			}
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
			if (GovheilC && GovheilBCooldown > 300)
			{
				Player.GetDamage(DamageClass.Ranged) *= 2f;
				Player.GetDamage(DamageClass.Melee) *= 2f;

			}
			if (GovheilC && GovheilBCooldown == 520)
			{
				GovheilBCooldown = 0;


			}


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

			if (Player.InModBiome<MarrowSurfaceBiome>() && !Main.dayTime)
			{
				MusicLoader.GetMusicSlot(Mod, "Assets/Music/morrownight");
			}
			if (EventWorld.GintzingBoss)
			{
				player.AddBuff(ModContent.BuffType<Gintzingwinds>(), 100);
			}

			if (Player.HasBuff<Gintzingwinds>()) 
            {

				for (int j = 0; j < 1 ; j++)
				{
					RandomOrig3 = new Vector2(Player.width / 2, Player.height / 2) + new Vector2(Main.rand.NextFloat(-900f, 900f), (Main.rand.NextFloat(-600f, 600f)));
					RandomOrig2 = new Vector2(Player.width / 2, Player.height / 2) + new Vector2(Main.rand.NextFloat(-1600f, 1600f), (Main.rand.NextFloat(-900f, 900f)));
					RandomOrig = new Vector2(Player.width / 2, Player.height / 2) + new Vector2(Main.rand.NextFloat(-1800f, 1800f), (Main.rand.NextFloat(-1200f, 1200f)));

					Vector2 speed = Main.rand.NextVector2Circular(1f, 1f);
					Vector2 speed2 = Main.rand.NextVector2Square(1f, 1f);
					ParticleManager.NewParticle(Player.Center - RandomOrig, speed2 * 3, ParticleManager.NewInstance<windline>(), Color.RoyalBlue, Main.rand.NextFloat(0.2f, 0.8f));


				}

				Main.GraveyardVisualIntensity = 0.8f;
				Main.windPhysicsStrength = 90;


			}



			

			if (Player.InModBiome<FableBiome>())
			{
				Main.GraveyardVisualIntensity = 0.4f;
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
					ParticleManager.NewParticle(Player.Center - RandomOrig, speed2 * 3, ParticleManager.NewInstance<FabledParticle>(), Color.RoyalBlue, Main.rand.NextFloat(0.2f, 0.8f));


				}


				for (int j = 0; j < 4; j++)
				{
					RandomOrig3 = new Vector2(Player.width / 2, Player.height / 2) + new Vector2(Main.rand.NextFloat(-900f, 900f), (Main.rand.NextFloat(-600f, 600f)));
					RandomOrig2 = new Vector2(Player.width / 2, Player.height / 2) + new Vector2(Main.rand.NextFloat(-1600f, 1600f), (Main.rand.NextFloat(-900f, 900f)));
					RandomOrig = new Vector2(Player.width / 2, Player.height / 2) + new Vector2(Main.rand.NextFloat(-1800f, 1800f), (Main.rand.NextFloat(-1200f, 1200f)));

					Vector2 speed = Main.rand.NextVector2Circular(1f, 1f);
					Vector2 speed2 = Main.rand.NextVector2Circular(0.1f, 0.1f);
					ParticleManager.NewParticle(Player.Center - RandomOrig2, speed * 2, ParticleManager.NewInstance<SparkleTrailParticle>(), Color.RoyalBlue, Main.rand.NextFloat(0.2f, 0.8f));


				}

				for (int j = 0; j < 2; j++)
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

			if (Player.InModBiome<AcidBiome>())
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

				if (Player.InModBiome<AbyssBiome>() || Player.InModBiome<AurelusBiome>())
			{
			
				Main.windPhysicsStrength = 50;


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







			if (ArcaneM && ArcaneMCooldown == 601)
			{
				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Arcaneup"));
				for (int j = 0; j < 7; j++)
				{
					Vector2 speed = Main.rand.NextVector2Circular(1f, 1f);
					Vector2 speed2 = Main.rand.NextVector2CircularEdge(1f, 1f);
					ParticleManager.NewParticle(Player.Center, speed * 3, ParticleManager.NewInstance<ArcanalParticle>(), Color.RoyalBlue, Main.rand.NextFloat(0.2f, 0.8f));

				}


			}
			if (ArcaneM && ArcaneMCooldown > 600)
			{
				Player.GetDamage(DamageClass.Magic) *= 2f;


			}
			if (ArcaneM && ArcaneMCooldown == 720)
			{
				ArcaneMCooldown = 0;


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


							CombatText.NewText(player.getRect(), Color.YellowGreen, "Wohooo", true, false);
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


							CombatText.NewText(player.getRect(), Color.YellowGreen, "Omg, its something!", true, false);
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


							CombatText.NewText(player.getRect(), Color.YellowGreen, "Are you disappointed? You should be.", true, false);
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


							CombatText.NewText(player.getRect(), Color.YellowGreen, "Wow, you have no maidens and no luck..", true, false);
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

							CombatText.NewText(player.getRect(), Color.YellowGreen, "Sooo lucky!", true, false);
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
		}
		public const int CAMO_DELAY = 100;



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
			if (BroochVerlia && VerliaBDCooldown <= 0)
			{
				
				for (int d = 0; d < 4; d++)
				{
					float speedXa = Main.rand.NextFloat(.4f, .7f) + Main.rand.NextFloat(-1f, 1f);
					float speedYa = Main.rand.Next(10, 15) * 0.01f + Main.rand.Next(-1, 1);


					Vector2 speedea = Main.rand.NextVector2Circular(0.5f, 0.5f);

					Projectile.NewProjectile(Player.GetSource_OnHit(target), (int)target.Center.X, (int)target.Center.Y, speedXa, speedYa, ModContent.ProjectileType<VerliaBroochP>(), 10, 1f, Player.whoAmI);

					Projectile.NewProjectile(Player.GetSource_OnHit(target), (int)target.Center.X, (int)target.Center.Y, speedXa * 0.7f, speedYa * 0.6f, ModContent.ProjectileType<VerliaBroochP>(), 10, 1f, Player.whoAmI);
					Projectile.NewProjectile(Player.GetSource_OnHit(target), (int)target.Center.X, (int)target.Center.Y, speedXa * 0.5f, speedYa * 0.3f, ModContent.ProjectileType<VerliaBroochP2>(), 15, 1f, Player.whoAmI);
					Projectile.NewProjectile(Player.GetSource_OnHit(target), (int)target.Center.X, (int)target.Center.Y, speedXa * 1.3f, speedYa * 0.3f, ModContent.ProjectileType<VerliaBroochP2>(), 15, 1f, Player.whoAmI);
					Projectile.NewProjectile(Player.GetSource_OnHit(target), (int)target.Center.X, (int)target.Center.Y, speedXa * 1f, speedYa * 1.5f, ModContent.ProjectileType<VerliaBroochP3>(), 20, 1f, Player.whoAmI);
				}

				VerliaBDCooldown = 220;
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


			if (BroochFrile && FrileBDCooldown <= 0)
			{

				Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Player.velocity, ModContent.ProjectileType<FrileBroochP>(), 3, 1f, Player.whoAmI);
				FrileBDCooldown = 3;

			}

			if (BroochVerlia && VerliaBDCooldown <= 0)
			{

				for (int d = 0; d < 4; d++)
				{
					float speedXa = Main.rand.NextFloat(.4f, .7f) + Main.rand.NextFloat(-1f, 1f);
					float speedYa = Main.rand.Next(10, 15) * 0.01f + Main.rand.Next(-1, 1);


					Vector2 speedea = Main.rand.NextVector2Circular(0.5f, 0.5f);

					Projectile.NewProjectile(Player.GetSource_OnHit(target), (int)target.Center.X, (int)target.Center.Y, speedXa, speedYa, ModContent.ProjectileType<VerliaBroochP>(), 10, 1f, Player.whoAmI);

					Projectile.NewProjectile(Player.GetSource_OnHit(target), (int)target.Center.X, (int)target.Center.Y, speedXa * 0.7f, speedYa * 0.6f, ModContent.ProjectileType<VerliaBroochP>(), 10, 1f, Player.whoAmI);
					Projectile.NewProjectile(Player.GetSource_OnHit(target), (int)target.Center.X, (int)target.Center.Y, speedXa * 0.5f, speedYa * 0.3f, ModContent.ProjectileType<VerliaBroochP2>(), 15, 1f, Player.whoAmI);
					Projectile.NewProjectile(Player.GetSource_OnHit(target), (int)target.Center.X, (int)target.Center.Y, speedXa * 1.3f, speedYa * 0.3f, ModContent.ProjectileType<VerliaBroochP2>(), 15, 1f, Player.whoAmI);
					Projectile.NewProjectile(Player.GetSource_OnHit(target), (int)target.Center.X, (int)target.Center.Y, speedXa * 1f, speedYa * 1.5f, ModContent.ProjectileType<VerliaBroochP3>(), 20, 1f, Player.whoAmI);
				}

				VerliaBDCooldown = 220;
			}

		}


		public override void OnHitByNPC(NPC npc, Player.HurtInfo hurtInfo)
		{
			if (ThornedBook)
			{
				npc.SimpleStrikeNPC(hurtInfo.Damage * 7, hurtInfo.HitDirection, crit: false, hurtInfo.Knockback);
			}

			if (Lovestruck)
			{

				Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Player.velocity, ModContent.ProjectileType<LovestruckP>(), 4, 1f, Player.whoAmI);

				npc.SimpleStrikeNPC(hurtInfo.Damage * 5, hurtInfo.HitDirection, crit: false, hurtInfo.Knockback);
			
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
