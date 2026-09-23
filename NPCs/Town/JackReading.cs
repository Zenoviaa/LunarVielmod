using Microsoft.Xna.Framework;
using Stellamod.Common.DialogueTowning;
using Stellamod.Common.GooberDialogue;
using Stellamod.Content.Areas.Fable.BossesFB.DaedusTheDevoted;
using Stellamod.Content.Areas.Fable.BossesFB.JackTheScholar;
using Stellamod.Core;
using Stellamod.Helpers;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace Stellamod.NPCs.Town
{
    public class JackReading : VeilTownNPC
    {
        private int _frame;
        public int NumberOfTimesTalkedTo = 0;
        public override string Texture => "Stellamod/Content/Areas/Fable/BossesFB/JackTheScholar/JackTheScholar";
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.npcFrameCount[Type] = 28;
        }

        public override void SetPointSpawnerDefaults(ref NPCPointSpawner spawner)
        {
            spawner.structureToSpawnIn = "Structures/Fable";
            spawner.spawnTileOffset = new Point(190, -70);
        }

        public override void SetDefaults()
        {
            NPC.friendly = true; // NPC Will not attack player
            NPC.width = 54;
            NPC.height = 65;
            NPC.aiStyle = -1;
            NPC.damage = 90;
            NPC.defense = 42;
            NPC.lifeMax = 2000;
            NPC.npcSlots = 0;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.knockBackResist = 0.5f;
            NPC.dontTakeDamageFromHostiles = true;
            NPC.BossBar = Main.BigBossProgressBar.NeverValid;
            SpawnAtPoint = true;
            HasTownDialogue = true;
        }
        public override void AI()
        {
            base.AI();

        }

        public override void FindFrame(int frameHeight)
        {
            base.FindFrame(frameHeight);

            //Animation Speed
            NPC.frameCounter += 0.15f;
            if (NPC.frameCounter >= 1f)
            {
                _frame++;
                NPC.frameCounter = 0f;
            }
            if (_frame >= 4f)
            {
                _frame = 0;
            }

            NPC.frame.Y = frameHeight * _frame;
        }

 

        //This prevents the NPC from despawning
        public override bool CheckActive()
        {
            return true;
        }

        public override string GetChat()
        {
            WeightedRandom<string> chat = new WeightedRandom<string>();
            // These are things that the NPC has a chance of telling you when you talk to it.
            chat.Add(LangText.Chat(this, "Basic1"));


            NumberOfTimesTalkedTo++;
            if (NumberOfTimesTalkedTo >= 10)
            {
                //This counter is linked to a single instance of the NPC, so if ExamplePerson is killed, the counter will reset.
                chat.Add(LangText.Chat(this, "Basic2"));
            }

            return chat; // chat is implicitly cast to a string.
        }

        public override void OpenTownDialogue(ref SpeechBoxTalkingParameters talkingParameters, List<Tuple<string, Action>> buttons)
        {
            base.OpenTownDialogue(ref talkingParameters, buttons);
            talkingParameters.profile = GooberDialoguePresets.Jack;
            //Set buttons
            buttons.Add(new("Talk", Talk));
            buttons.Add(new("Challenge", Challenge));
        }

        private void Challenge()
        {
            CloseTownDialogue();
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                Main.NewText(LangText.Chat(this, "Challenge"), Color.Gold);
                NPC npc = NPC.NewNPCDirect(NPC.GetSource_FromThis(), (int)NPC.position.X, (int)NPC.position.Y,
                    ModContent.NPCType<JackTheScholar>());
                npc.netUpdate = true;
            }
            else
            {
                if (Main.netMode == NetmodeID.SinglePlayer)
                    return;

                MultiplayerHelper.SpawnBossFromClient((byte)Main.LocalPlayer.whoAmI,
                    ModContent.NPCType<JackTheScholar>(), (int)NPC.position.X, (int)NPC.position.Y);
            }

            //Spawn Boss
            NPC.Kill();
        }

    }
}
