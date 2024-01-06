
using Microsoft.Xna.Framework;
using Stellamod.Utilis;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

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

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
                new FlavorTextBestiaryInfoElement("A poisonous slime mutated from normal green slimes"),
            });
        }

        public override void SetDefaults()
        {
            NPC.aiStyle = 0;
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
            //bool expertMode = Main.expertMode;
            if (Main.rand.NextBool(45))
            {
                NPC.netUpdate = false;
            }

            NPC.TargetClosest(true);
            /*
            // What is this for?
            for (int index1 = 0; index1 < 6; ++index1)
            {
                float x = (NPC.Center.X - 22);
                float xnum2 = (NPC.Center.X + 22);
                float y = (NPC.Center.Y);
            }
            */

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
                if (SingularityFragment.LazerType == 1)
                {
                    Timer = -179f;
                }

                Utilities.NewProjectileBetter(NPC.Center.X, NPC.Center.Y, direction.X, direction.Y, ModContent.ProjectileType<VoidBeam>(), 250, 0f, -1, 0, NPC.whoAmI);
                Utilities.NewProjectileBetter(NPC.Center.X, NPC.Center.Y, direction.X, direction.Y, ModContent.ProjectileType<VoidBeamIN>(), 250, 0f, -1, 0, NPC.whoAmI);
                Lazer = true;
            }

            NPC.ai[1] = direction.X;
            NPC.ai[3] = direction.Y;
            if(SingularityFragment.LazerType == 0)
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