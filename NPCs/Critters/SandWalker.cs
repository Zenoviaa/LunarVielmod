using MonoMod.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader.Utilities;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using Stellamod.Helpers;

namespace Stellamod.NPCs.Critters
{
    internal class SandWalker
    {   /// <summary>
        /// This file shows off a critter npc. The unique thing about critters is how you can catch them with a bug net.
        /// The important bits are: Main.npcCatchable, NPC.catchItem, and Item.makeNPC.
        /// We will also show off adding an item to an existing RecipeGroup (see ExampleRecipes.AddRecipeGroups).
        /// Additionally, this example shows an involved IL edit.
        /// </summary>
        public class SandWalkerNPC : ModNPC
        {
            private const int ClonedNPCID = NPCID.Frog; // Easy to change type for your modder convenience
            public override void SetStaticDefaults()
            {
                Main.npcFrameCount[Type] = 8;
                Main.npcCatchable[Type] = true; // This is for certain release situations

                // These three are typical critter values
                NPCID.Sets.CountsAsCritter[Type] = true;
                NPCID.Sets.TakesDamageFromHostilesWithoutBeingFriendly[Type] = true;
                NPCID.Sets.TownCritter[Type] = true;

                // The frog is immune to confused
                NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;

                // This is so it appears between the frog and the gold frog
                NPCID.Sets.NormalGoldCritterBestiaryPriority.Insert(NPCID.Sets.NormalGoldCritterBestiaryPriority.IndexOf(ClonedNPCID) + 1, Type);
            }

            public override void SetDefaults()
            {
                // width = 12;
                // height = 10;
                // aiStyle = 7;
                // damage = 0;
                // defense = 0;
                // lifeMax = 5;
                // HitSound = SoundID.NPCHit1;
                // DeathSound = SoundID.NPCDeath1;
                // catchItem = 2121;
                // Sets the above
                NPC.CloneDefaults(ClonedNPCID);
                NPC.width = 22;
                NPC.height = 16;
                NPC.catchItem = ModContent.ItemType<SandWalkerItem>();
                AIType = ClonedNPCID;
            }

            public override void FindFrame(int frameHeight)
            {
                if(NPC.velocity.X == 0)
                {
                    //Don't animate while still, they don't have idle animation
                    NPC.frame.Y = 0;
                }
                else
                {
                    NPC.frameCounter += 0.3f;
                    NPC.frameCounter %= Main.npcFrameCount[NPC.type];
                    int frame = (int)NPC.frameCounter;
                    NPC.frame.Y = frame * frameHeight;
                }
            }

            public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
            {
                bestiaryEntry.AddTags(BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheUnderworld,
                    new FlavorTextBestiaryInfoElement(LangText.Bestiary(this, "The most adorable goodest spicy child. Do not dare be mean to him!")));
            }

            public override float SpawnChance(NPCSpawnInfo spawnInfo)
            {
                return SpawnCondition.OverworldDayDesert.Chance * 0.1f;
            }

            public override void HitEffect(NPC.HitInfo hit)
            {
                if (NPC.life <= 0)
                {
                    for (int i = 0; i < 6; i++)
                    {
                        Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Worm, 2 * hit.HitDirection, -2f);
                        if (Main.rand.NextBool(2))
                        {
                            dust.noGravity = true;
                            dust.scale = 1.2f * NPC.scale;
                        }
                        else
                        {
                            dust.scale = 0.7f * NPC.scale;
                        }
                    }
                 //   Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>($"{Name}_Gore_Head").Type, NPC.scale);
                  //  Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>($"{Name}_Gore_Leg").Type, NPC.scale);
                }
            }

            public override bool PreAI()
            {
                // Kills the NPC if it hits water, honey or shimmer
                if (NPC.wet && !Collision.LavaCollision(NPC.position, NPC.width, NPC.height))
                { // NPC.lavawet not 100% accurate for the frog
                  // These 3 lines instantly kill the npc without showing damage numbers, dropping loot, or playing DeathSound. Use this for instant deaths
                    NPC.life = 0;
                    NPC.HitEffect();
                    NPC.active = false;
                    SoundEngine.PlaySound(SoundID.NPCDeath16, NPC.position); // plays a fizzle sound
                }

                return true;
            }

            public override void PostAI()
            {
                NPC.spriteDirection = NPC.direction;
            }
        }

        public class SandWalkerItem : ModItem
        {
            public override void SetDefaults()
            {
                // useStyle = 1;
                // autoReuse = true;
                // useTurn = true;
                // useAnimation = 15;
                // useTime = 10;
                // maxStack = CommonMaxStack;
                // consumable = true;
                // width = 12;
                // height = 12;
                // makeNPC = 361;
                // noUseGraphic = true;

                // Cloning ItemID.Frog sets the preceding values
                Item.CloneDefaults(ItemID.Frog);
                Item.makeNPC = ModContent.NPCType<SandWalkerNPC>();
                Item.value += Item.buyPrice(0, 0, 30, 0); // Make this critter worth slightly more than the frog
                Item.rare = ItemRarityID.Blue;
                Item.bait = 25;
            }
        }
    }
}
