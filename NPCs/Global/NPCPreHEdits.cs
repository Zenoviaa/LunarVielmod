using Stellamod.Items.Accessories;
using Stellamod.Items.Accessories.Brooches;
using Stellamod.Items.Special.MinerLogs;
using Stellamod.Items.Weapons.Mage;
using Stellamod.Items.Weapons.Melee.Safunais;
using Stellamod.Items.Weapons.PowdersItem;
using Stellamod.Items.Weapons.Ranged.Crossbows;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Global
{
    public class NPCPreHEdits : GlobalNPC
    {
        public override bool InstancePerEntity => true;


        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {

            // We will now use the Guide to explain many of the other types of drop rules.

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

            if (npc.type == NPCID.GoblinShark)
            {


                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<BloodPowder>(), 2, 1, 1)); // In conjunction with the above removal, this makes it so a guide with any name will drop the Green Cap.
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


                npcLoot.Add(ItemDropRule.Common(ItemID.SandBlock, 3, 1, 5)); // In conjunction with the above removal, this makes it so a guide with any name will drop the Green Cap. // In conjunction with the above removal, this makes it so a guide with any name will drop the Green Cap.

                npcLoot.Add(ItemDropRule.Common(ItemID.SandBoots, 25, 1, 1));
                npcLoot.Add(ItemDropRule.Common(ItemID.Sandgun, 40, 1, 1));
            }
            if (npc.type == NPCID.Vulture)
            {


                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Heatspot>(), 20, 1, 1)); // In conjunction with the above removal, this makes it so a guide with any name will drop the Green Cap.// In conjunction with the above removal, this makes it so a guide with any name will drop the Green Cap.

            }

            if (npc.type == NPCID.FlyingAntlion)
            {



                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<SwarmerStaff>(), 10, 1, 1)); // In conjunction with the above removal, this makes it so a guide with any name will drop the Green Cap.

            }
        }
    }
}