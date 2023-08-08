
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Events;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using System;
using System.Collections.Generic;
using Steamworks;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;

using Stellamod.NPCs.Bosses.Jack;
using System.Reflection.Metadata;
using Stellamod.Utilis;
using Stellamod.Items.Consumables;
using Stellamod.Items.Materials;
using Stellamod.Items.Weapons.Ranged;
using Stellamod.Items.Weapons.Mage;
using Stellamod.Items.Weapons.Melee;
using Terraria.GameContent.ItemDropRules;
using Stellamod.Items.Weapons.Ranged;
using Stellamod.NPCs.Bosses.DreadMire;
using Terraria.GameContent.Bestiary;
using Stellamod.NPCs.Bosses.DreadMire.Heart;
using Stellamod.NPCs.Bosses.INest;

namespace Stellamod.NPCs.Bosses.singularityFragment
{

    public class LazerOrb : ModNPC
    {
        public float LazerAdd;
        public bool Lazer;
        public float Timer;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Dread Fire");
            NPCID.Sets.TrailingMode[NPC.type] = 0;
            NPCID.Sets.TrailCacheLength[NPC.type] = 5;
        }
        float Size = 3;
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
                new FlavorTextBestiaryInfoElement("A poisonous slime mutated from normal green slimes"),
            });
        }
        public override void SetDefaults()
        {
            NPC.width = 42;
            NPC.height = 42;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.damage = 15;
            NPC.buffImmune[BuffID.Confused] = true;
            NPC.buffImmune[BuffID.Poisoned] = true;
            NPC.buffImmune[BuffID.Venom] = true;
            NPC.dontCountMe = false;
            NPC.dontTakeDamage = true;
            NPC.dontCountMe = true;
            NPC.lifeMax = 200;
            NPC.alpha = 255;
        }
        public override bool PreAI()
        {
            bool expertMode = Main.expertMode;
            if (Main.rand.NextBool(45))
            {
                NPC.netUpdate = false;
            }
            NPC.TargetClosest(true);




            for (int index1 = 0; index1 < 6; ++index1)
            {
                float x = (NPC.Center.X - 22);
                float xnum2 = (NPC.Center.X + 22);
                float y = (NPC.Center.Y);
 
            }
            int parent = (int)NPC.ai[0];
            if (parent < 0 || parent >= Main.maxNPCs || !Main.npc[parent].active || Main.npc[parent].type != ModContent.NPCType<SingularityFragment>())
            {
                NPC.active = false;
                return false;
            }

            Vector2 direction = Vector2.Normalize(NPC.Center - Main.npc[parent].Center) * 8.5f;
            NPC.Center = Main.npc[parent].Center + NPC.ai[2] * Timer.ToRotationVector2();
            direction.Normalize();
            NPC.rotation = -direction.ToRotation();
            if (!Lazer)
            {
                LazerAdd += .006f;
                if (global::Stellamod.NPCs.Bosses.singularityFragment.SingularityFragment.LazerType == 1)
                {
                    Timer = -179f;
                }

                Utilities.NewProjectileBetter(NPC.Center.X, NPC.Center.Y, direction.X, direction.Y, ModContent.ProjectileType<VoidBeam>(), 250, 0f, -1, 0, NPC.whoAmI);
                Utilities.NewProjectileBetter(NPC.Center.X, NPC.Center.Y, direction.X, direction.Y, ModContent.ProjectileType<VoidBeamIN>(), 250, 0f, -1, 0, NPC.whoAmI);
                Lazer = true;
            }
            NPC.ai[1] = direction.X;
            NPC.ai[3] = direction.Y;

       

            if(global::Stellamod.NPCs.Bosses.singularityFragment.SingularityFragment.LazerType == 0)
            {
                Timer -= .01f + LazerAdd;
            }
            else
            {
                Timer += .01f + LazerAdd;
            }

            return false;
        }

        public override bool CheckActive()
        {
            return false;
        }
    }
}