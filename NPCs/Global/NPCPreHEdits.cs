using Stellamod.Buffs.Dusteffects;
using Stellamod.DropRules;
using Stellamod.Items.Accessories;
using Stellamod.Items.Accessories.Brooches;
using Stellamod.Items.Consumables;
using Stellamod.Items.Harvesting;
using Stellamod.Items.Materials;
using Stellamod.Items.Special.MinerLogs;
using Stellamod.Items.Weapons.Mage;
using Stellamod.Items.Weapons.Melee.Safunais;
using Stellamod.Items.Weapons.PowdersItem;
using Stellamod.Items.Weapons.Ranged;
using Stellamod.Items.Weapons.Ranged.Crossbows;
using Stellamod.Items.Weapons.Summon;
using Stellamod.NPCs.Acidic;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Global
{
    public class NPCPreHEdits : GlobalNPC
	{

		int Timerboss = 0;
		// ModifyNPCLoot uses a unique system called the ItemDropDatabase, which has many different rules for many different drop use cases.
		// Here we go through all of them, and how they can be used.
		// There are tons of other examples in vanilla! In a decompiled vanilla build, GameContent/ItemDropRules/ItemDropDatabase adds item drops to every single vanilla NPC, which can be a good resource.
		public override bool InstancePerEntity => true;
		public override void AI(NPC npc)
        {
			Timerboss++;
			if (npc.type == NPCID.Creeper)
			{
				if (Timerboss == 180)
				{
					if (StellaMultiplayer.IsHost)
					{
                        float speedXnpc = npc.velocity.X;
                        float speedYnpc = npc.velocity.Y;
                        Projectile.NewProjectile(npc.GetSource_FromThis(), npc.position.X, npc.position.Y, speedXnpc, speedYnpc,
							ProjectileID.GoldenShowerHostile, 13, 0f, Owner: Main.myPlayer);
                    }


                    Timerboss = 0;
				}
			}

			if (npc.type == NPCID.EaterofWorldsHead)
			{
				if (Timerboss == 100)
                {
					if (StellaMultiplayer.IsHost)
					{
						float speedXnpc = npc.velocity.X;
						float speedYnpc = npc.velocity.Y;
						Projectile.NewProjectile(npc.GetSource_FromThis(), npc.position.X, npc.position.Y, speedXnpc, speedYnpc,
							ProjectileID.CursedFlameHostile, 20, 0f, Owner: Main.myPlayer);
					}
					Timerboss = 0;
				}
			}

			if (npc.type == NPCID.SkeletronPrime)
			{
				if (npc.life > npc.lifeMax / 2)
				{
					if (Timerboss == 60)
                    {
						if (StellaMultiplayer.IsHost)
						{
							float speedXnpc = npc.velocity.X;
							float speedYnpc = npc.velocity.Y;
							Projectile.NewProjectile(npc.GetSource_FromThis(), npc.position.X, npc.position.Y, speedXnpc, speedYnpc, 
								ProjectileID.Skull, 40, 0f, Owner: Main.myPlayer);
						}
						Timerboss = 0;
					}
				}

                    
				if (npc.life < npc.lifeMax / 2)
                {

					if (Timerboss == 30)
					{
						if (StellaMultiplayer.IsHost)
						{
							float speedXnpc = npc.velocity.X;
							float speedYnpc = npc.velocity.Y;
							Projectile.NewProjectile(npc.GetSource_FromThis(), npc.position.X, npc.position.Y, speedXnpc, speedYnpc, 
								ProjectileID.DeathLaser, 50, 0f, Owner: Main.myPlayer);
						}
					}


					if (Timerboss == 60)
					{
						if (StellaMultiplayer.IsHost)
						{
                            float speedXnpc = npc.velocity.X;
                            float speedYnpc = npc.velocity.Y;

                            Projectile.NewProjectile(npc.GetSource_FromThis(), npc.position.X, npc.position.Y, speedXnpc, speedYnpc,
                                ProjectileID.Skull, 40, 0f, Owner: Main.myPlayer);
                        }


                        Timerboss = 0;
					}

				}
			}

			if (npc.type == NPCID.BrainofCthulhu)
			{
				npc.velocity *= 1.01f;
			}
		}

		

		public override void SetDefaults(NPC npc)
        {
			if (npc.type == NPCID.EyeofCthulhu)
			{
				npc.damage = 60;

				//Increase boss health by 50%
				float lifeMax = npc.lifeMax;
				lifeMax *= 1.6f;
				npc.lifeMax = (int)lifeMax;
			}

			if (npc.type == NPCID.QueenBee)
			{

				npc.damage = 60;

				//Increase boss health by 50%
				float lifeMax = npc.lifeMax;
				lifeMax *= 1.6f;
				npc.lifeMax = (int)lifeMax;
			}

			if (npc.type == NPCID.SkeletronHead)
			{
				npc.damage = 70;
				if (Main.expertMode)
				{
					npc.damage = 50;
				}

				if (Main.masterMode)
				{
					npc.damage = 50;
				}

				//Increase boss health by 50%
				float lifeMax = npc.lifeMax;
				lifeMax *= 1.7f;
				npc.lifeMax = (int)lifeMax;
			}

			if (npc.type == NPCID.SkeletronPrime)
			{
				
				if (Main.expertMode)
				{
					npc.damage = 90;
				}

				if (Main.masterMode)
				{
					npc.damage = 110;
				}


			}

			if (npc.type == NPCID.WallofFlesh)
			{
				npc.damage = 400;
		
				//Increase boss health by 50%
				float lifeMax = npc.lifeMax;
				lifeMax *= 1.9f;
				npc.lifeMax = (int)lifeMax;
			}

			if (npc.type == NPCID.WallofFleshEye)
			{
				npc.damage = 300;		
			}


			if (npc.type == NPCID.Creeper)
			{
				npc.life = 575;
				npc.lifeMax = 575;
				npc.damage = 50;		
			}

			if (npc.type == NPCID.EaterofWorldsTail)
			{
				npc.damage = 90;
			}

			if (npc.type == NPCID.EaterofWorldsHead)
			{
				npc.life = 1000;
				npc.lifeMax = 1000;
			}
		}

		public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
		{

			// We will now use the Guide to explain many of the other types of drop rules.
			

			if (npc.type == NPCID.GoblinSummoner)
			{
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ShadowFlamePowder>(), 1, 1, 1));

                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Gambit>(), 1, 1, 3)); // In conjunction with the above removal, this makes it so a guide with any name will drop the Green Cap.
			}


            if (npc.type == NPCID.GraniteFlyer)
            {


                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<VeiledScriptureMiner7>(), 5, 1, 1)); // In conjunction with the above removal, this makes it so a guide with any name will drop the Green Cap.
            }

            if (npc.type == NPCID.Nymph)
            {


                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<VeiledScriptureMiner2>(), 1, 1, 1)); // In conjunction with the above removal, this makes it so a guide with any name will drop the Green Cap.
            }

            if (npc.type == NPCID.RockGolem)
            {


                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<VeiledScriptureMiner3>(), 3, 1, 1)); // In conjunction with the above removal, this makes it so a guide with any name will drop the Green Cap.
            }

            if (npc.type == NPCID.Tim)
            {


                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<VeiledScriptureMiner4>(), 1, 1, 1)); // In conjunction with the above removal, this makes it so a guide with any name will drop the Green Cap.
            }


            if (npc.type == NPCID.SkeletronHead)
			{


				npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<BonedThrowBroochA>(), 1, 1, 1)); // In conjunction with the above removal, this makes it so a guide with any name will drop the Green Cap.
			}


			if (npc.type == NPCID.GoblinSorcerer)
			{


				npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Vinger>(), 15, 1, 1)); // In conjunction with the above removal, this makes it so a guide with any name will drop the Green Cap.
			}

			if (npc.type == NPCID.BlackRecluse)
			{


				npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<DustedSilk>(), 1, 1, 4)); // In conjunction with the above removal, this makes it so a guide with any name will drop the Green Cap.
			}

			if (npc.type == NPCID.BlackRecluseWall)
			{


				npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<DustedSilk>(), 1, 1, 4)); // In conjunction with the above removal, this makes it so a guide with any name will drop the Green Cap.
			}
			if (npc.type == NPCID.WallCreeper)
			{


				npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<DustedSilk>(), 1, 1, 3)); // In conjunction with the above removal, this makes it so a guide with any name will drop the Green Cap.
			}
            if (npc.type == NPCID.GoblinShark)
            {


                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<BloodPowder>(), 2, 1, 1)); // In conjunction with the above removal, this makes it so a guide with any name will drop the Green Cap.
            }
            if (npc.type == NPCID.WallCreeperWall)
			{
				npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<DustedSilk>(), 1, 1, 3)); // In conjunction with the above removal, this makes it so a guide with any name will drop the Green Cap.
			}

            if (npc.type == NPCID.BlueArmoredBones || 
				npc.type == NPCID.BlueArmoredBonesMace ||
                npc.type == NPCID.BlueArmoredBonesNoPants || 
				npc.type == NPCID.BlueArmoredBonesSword)
			{
				npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<DungeonCrossbow>(), 75, 1, 1)); // In conjunction with the above removal, this makes it so a guide with any name will drop the Green Cap.
			}

			if (npc.type == NPCID.AngryBones ||
				npc.type == NPCID.AngryBonesBig ||
				npc.type == NPCID.AngryBonesBigHelmet || 
				npc.type == NPCID.AngryBonesBigMuscle)
			{


				npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<DungeonCrossbow>(), 75, 1, 1)); // In conjunction with the above removal, this makes it so a guide with any name will drop the Green Cap.
			}



			//--------------------------------------------------------------------- Desert

			if (npc.type == NPCID.Antlion)
			{


				npcLoot.Add(ItemDropRule.Common(ItemID.SandBlock, 3, 1, 5)); // In conjunction with the above removal, this makes it so a guide with any name will drop the Green Cap.
				npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Stick>(), 3, 1, 5)); // In conjunction with the above removal, this makes it so a guide with any name will drop the Green Cap.

				npcLoot.Add(ItemDropRule.Common(ItemID.SandBoots, 25, 1, 1)); 
				npcLoot.Add(ItemDropRule.Common(ItemID.Sandgun, 40, 1, 1)); 
			}
			if (npc.type == NPCID.Vulture)
			{


				npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Heatspot>(), 20, 1, 1)); // In conjunction with the above removal, this makes it so a guide with any name will drop the Green Cap.
				npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Stick>(), 3, 1, 5)); // In conjunction with the above removal, this makes it so a guide with any name will drop the Green Cap.

			}

			if (npc.type == NPCID.FlyingAntlion)
			{


		
				npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<SwarmerStaff>(), 10, 1, 1)); // In conjunction with the above removal, this makes it so a guide with any name will drop the Green Cap.

			}

			//------------------------------------------------------------------------ OVERWORLD + RAI
			if (npc.type == NPCID.Zombie)
			{


				npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Stick>(), 3, 1, 5)); // In conjunction with the above removal, this makes it so a guide with any name will drop the Green Cap.
				npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<CondensedDirt>(), 3, 1, 5)); // In conjunction with the above removal, this makes it so a guide with any name will drop the Green Cap.
				npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<GrassDirtPowder>(), 30, 1, 1)); // In conjunction with the above removal, this makes it so a guide with any name will drop the Green Cap.
			}

			if (npc.type == NPCID.FlyingFish)
			{


				npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<FlyingFishBroochA>(), 20,  1, 1)); // In conjunction with the above removal, this makes it so a guide with any name will drop the Green Cap.
			}

			if (npc.type == NPCID.BlueSlime)
			{
				npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<SlimeBroochA>(), 300, 1, 1)); // In conjunction with the above removal, this makes it so a guide with any name will drop the Green Cap.
			}


			//------------------------------------------- BLOOD MOON

			if (npc.type == NPCID.BloodZombie)
			{


	// In conjunction with the above removal, this makes it so a guide with any name will drop the Green Cap.
				npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<CondensedDirt>(), 3, 1, 5)); // In conjunction with the above removal, this makes it so a guide with any name will drop the Green Cap.
				npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<BloodLamp>(), 200, 1, 1));
			}
			
			if (npc.type == NPCID.Drippler)
			{

				npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<BloodLamp>(), 300, 1, 1));
		 // In conjunction with the above removal, this makes it so a guide with any name will drop the Green Cap.
			

			}







			if (npc.type == ModContent.NPCType<AcidSlime>() || npc.type == ModContent.NPCType<AcidProbe>() || npc.type == ModContent.NPCType<AcidSpirit>() || npc.type == ModContent.NPCType<ToxicBoulder>())
			{
				LeadingConditionRule HardmodeDropRule = new LeadingConditionRule(new HardmodeDropRule());
				HardmodeDropRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<GraftedSoul>(), 1, 0, 5));
				npcLoot.Add(HardmodeDropRule);
			}

		}
	}
}