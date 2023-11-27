using Stellamod.Items.Accessories;
using Stellamod.Items.Accessories.Brooches;
using Stellamod.Items.Consumables;
using Stellamod.Items.Harvesting;
using Stellamod.Items.Materials;
using Stellamod.Items.Weapons.Mage;
using Stellamod.Items.Weapons.Melee.Safunais;
using Stellamod.Items.Weapons.PowdersItem;
using Stellamod.Items.Weapons.Ranged;
using Stellamod.Items.Weapons.Ranged.Crossbows;
using Stellamod.Items.Weapons.Summon;
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
					float speedXb = npc.velocity.X * Main.rand.NextFloat(0f, 0f) + Main.rand.NextFloat(0f, 0f);
					float speedYb = npc.velocity.Y * Main.rand.Next(0, 0) * 0.0f + Main.rand.Next(0, 0) * 0f;

					float speedXnpc = npc.velocity.X;
					float speedYnpc = npc.velocity.Y;

					int fireball = Projectile.NewProjectile(npc.GetSource_FromThis(), npc.position.X, npc.position.Y, speedXnpc, speedYnpc, ProjectileID.IchorSplash, 13, 0f, 0, 0f, 0f);
					Projectile ichor = Main.projectile[fireball];
					ichor.hostile = true;
					ichor.friendly = false;

					Timerboss = 0;
				}
			}

			if (npc.type == NPCID.EaterofWorldsHead)
			{
				if (Timerboss == 100)
				{

					float speedXb = npc.velocity.X * Main.rand.NextFloat(0f, 0f) + Main.rand.NextFloat(0f, 0f);
					float speedYb = npc.velocity.Y * Main.rand.Next(0, 0) * 0.0f + Main.rand.Next(0, 0) * 0f;

					float speedXnpc = npc.velocity.X;
					float speedYnpc = npc.velocity.Y;

					int fireball = Projectile.NewProjectile(npc.GetSource_FromThis(), npc.position.X, npc.position.Y, speedXnpc, speedYnpc, ProjectileID.CursedFlameFriendly, 20, 0f, 0, 0f, 0f);

					Projectile ichor = Main.projectile[fireball];
					ichor.hostile = true;
					ichor.friendly = false;
					Timerboss = 0;
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
				npc.damage = 50;

				//Increase boss health by 50%
				float lifeMax = npc.lifeMax;
				lifeMax *= 1.5f;
				npc.lifeMax = (int)lifeMax;
			}

			if (npc.type == NPCID.QueenBee)
			{

				npc.damage = 60;

				//Increase boss health by 50%
				float lifeMax = npc.lifeMax;
				lifeMax *= 1.5f;
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
				lifeMax *= 1.5f;
				npc.lifeMax = (int)lifeMax;
			}

			if (npc.type == NPCID.WallofFlesh)
			{
				npc.damage = 240;
		
				//Increase boss health by 50%
				float lifeMax = npc.lifeMax;
				lifeMax *= 1.5f;
				npc.lifeMax = (int)lifeMax;
			}

			if (npc.type == NPCID.WallofFleshEye)
			{
				npc.damage = 140;		
			}

			if (npc.type == NPCID.Creeper)
			{
				npc.life = 550;
				npc.lifeMax = 550;
				npc.damage = 30;		
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


				npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Gambit>(), 1, 1, 3)); // In conjunction with the above removal, this makes it so a guide with any name will drop the Green Cap.
			}


			if (npc.type == NPCID.GoblinSorcerer)
			{


				npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Vinger>(), 300, 1, 1)); // In conjunction with the above removal, this makes it so a guide with any name will drop the Green Cap.
			}
			if (npc.type == NPCID.BlackRecluse)
			{


				npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<DustedSilk>(), 1, 1, 3)); // In conjunction with the above removal, this makes it so a guide with any name will drop the Green Cap.
			}
			if (npc.type == NPCID.WallCreeper)
			{


				npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<DustedSilk>(), 1, 1, 2)); // In conjunction with the above removal, this makes it so a guide with any name will drop the Green Cap.
			}





			//------------------------------------------- Dungeon

			if (npc.type == NPCID.BlueArmoredBones)
			{


				npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<DungeonCrossbow>(), 100, 1, 1)); // In conjunction with the above removal, this makes it so a guide with any name will drop the Green Cap.
			}
			if (npc.type == NPCID.AngryBones)
			{


				npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<DungeonCrossbow>(), 100, 1, 1)); // In conjunction with the above removal, this makes it so a guide with any name will drop the Green Cap.
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


				npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Heatspot>(), 100, 1, 1)); // In conjunction with the above removal, this makes it so a guide with any name will drop the Green Cap.
				npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Stick>(), 3, 1, 5)); // In conjunction with the above removal, this makes it so a guide with any name will drop the Green Cap.

			}

			if (npc.type == NPCID.FlyingAntlion)
			{


		
				npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<SwarmerStaff>(), 50, 1, 1)); // In conjunction with the above removal, this makes it so a guide with any name will drop the Green Cap.

			}

			//------------------------------------------------------------------------ OVERWORLD + RAIN



			if (npc.type == NPCID.AngryNimbus)
			{



				npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<CloudBow>(), 100, 1, 1)); // In conjunction with the above removal, this makes it so a guide with any name will drop the Green Cap.

			}

			if (npc.type == NPCID.PinkJellyfish)
			{



				npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<JellyBow>(), 50, 1, 1)); // In conjunction with the above removal, this makes it so a guide with any name will drop the Green Cap.
				npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<JellyStaff>(), 50, 1, 1)); // In conjunction with the above removal, this makes it so a guide with any name will drop the Green Cap.
				npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<JellyTome>(), 90, 1, 1)); // In conjunction with the above removal, this makes it so a guide with any name will drop the Green Cap.
			}

			if (npc.type == NPCID.GreenJellyfish)
			{



				npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<JellyBow>(), 50, 1, 1)); // In conjunction with the above removal, this makes it so a guide with any name will drop the Green Cap.
				npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<JellyStaff>(), 50, 1, 1)); // In conjunction with the above removal, this makes it so a guide with any name will drop the Green Cap.
				npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<JellyTome>(), 90, 1, 1)); // In conjunction with the above removal, this makes it so a guide with any name will drop the Green Cap.
			}

			if (npc.type == NPCID.MartianWalker)
			{



				npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Dragoniper>(), 50, 1, 1)); // In conjunction with the above removal, this makes it so a guide with any name will drop the Green Cap.
			// In conjunction with the above removal, this makes it so a guide with any name will drop the Green Cap.
			}

			if (npc.type == NPCID.BlueJellyfish)
			{



				npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<JellyBow>(), 50, 1, 1)); // In conjunction with the above removal, this makes it so a guide with any name will drop the Green Cap.
				npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<JellyStaff>(), 50, 1, 1)); // In conjunction with the above removal, this makes it so a guide with any name will drop the Green Cap.
				npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<JellyTome>(), 90, 1, 1)); // In conjunction with the above removal, this makes it so a guide with any name will drop the Green Cap.
			}


			if (npc.type == NPCID.Zombie)
			{


				npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Stick>(), 3, 1, 5)); // In conjunction with the above removal, this makes it so a guide with any name will drop the Green Cap.
				npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<CondensedDirt>(), 3, 1, 5)); // In conjunction with the above removal, this makes it so a guide with any name will drop the Green Cap.
				npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<GrassDirtPowder>(), 30, 1, 1)); // In conjunction with the above removal, this makes it so a guide with any name will drop the Green Cap.
			}

			if (npc.type == NPCID.FlyingFish)
			{


				npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<FlyingFishBroochA>(), 15, 1, 1)); // In conjunction with the above removal, this makes it so a guide with any name will drop the Green Cap.
			}

			if (npc.type == NPCID.GreenSlime)
			{


				npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<SlimeBroochA>(), 300, 1, 1)); // In conjunction with the above removal, this makes it so a guide with any name will drop the Green Cap.
			}


			//------------------------------------------- BLOOD MOON

			if (npc.type == NPCID.BloodZombie)
			{


	// In conjunction with the above removal, this makes it so a guide with any name will drop the Green Cap.
				npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<CondensedDirt>(), 3, 1, 5)); // In conjunction with the above removal, this makes it so a guide with any name will drop the Green Cap.
				npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<BloodLamp>(), 1000, 1, 1));
			}
			
			if (npc.type == NPCID.Drippler)
			{

				npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<BloodLamp>(), 600, 1, 1));
		 // In conjunction with the above removal, this makes it so a guide with any name will drop the Green Cap.
			

			}
			
		}
	}
}