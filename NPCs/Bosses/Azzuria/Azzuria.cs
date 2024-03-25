using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.Azzuria
{
    internal partial class Azzuria : ModNPC
    {
        public enum ActionState
        {
            Roaming,
            Obliterate,
            Sleeping,
            BossFight
        }

        public ActionState State
        {
            get
            {
                return (ActionState)NPC.ai[0];
            }
            set
            {
                NPC.ai[0] = (float)value;
            }
        }

        ref float Timer => ref NPC.ai[1];
        ref float AttackTimer => ref NPC.ai[2];

        public override void SetStaticDefaults()
        {
            //Don't want her to be hit by any debuffs
            NPCID.Sets.ImmuneToRegularBuffs[Type] = true;
            NPCID.Sets.MPAllowedEnemies[Type] = true;
            NPCID.Sets.TrailCacheLength[Type] = Total_Segments;
            NPCID.Sets.TrailingMode[Type] = 2;
            NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers();
            drawModifiers.CustomTexturePath = "Stellamod/NPCs/Bosses/Sylia/SyliaPreview";
            drawModifiers.PortraitScale = 0.8f; // Portrait refers to the full picture when clicking on the icon in the bestiary
            drawModifiers.PortraitPositionYOverride = 0f;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            // Sets the description of this NPC that is listed in the bestiary
            bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> {
                new MoonLordPortraitBackgroundProviderBestiaryInfoElement(), // Plain black background
				new FlavorTextBestiaryInfoElement("Azzuria, The First Dragon")
            });
        }

        public override void SetDefaults()
        {
            //Stats
            NPC.lifeMax = 126000;
            NPC.defense = 24;
            NPC.damage = 150;
            NPC.width = (int)AzzuriaHeadSize.X;
            NPC.height = (int)AzzuriaHeadSize.Y;

            //It won't be considered a boss or take up slots until the fight actually starts
            //So the values are like this for now
            NPC.boss = false;
            NPC.npcSlots = 0f;
            NPC.aiStyle = -1;

            //She'll tile collide and have gravity while on the ground, but not while airborne.
            NPC.noTileCollide = true;
            NPC.noGravity = true;
            NPC.knockBackResist = 0;
            OrientArching();
        }

        public override bool CheckActive()
        {
            //Return false to prevents despawning.
            //She'll have custom code when she's in her fight phase
            return false;
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            //Return false for no Contact Damage
            return false;
        }

        public override void AI()
        {
            NPC.TargetClosest();
            switch (State)
            {
                case ActionState.Roaming:
                    AIRoaming();
                    break;
                case ActionState.Obliterate:
                    AIObliterate();
                    break;
                case ActionState.Sleeping:
                    AISleeping();
                    break;
                case ActionState.BossFight:
                    AIBossFight();
                    break; 
            }
        }
    }
}
