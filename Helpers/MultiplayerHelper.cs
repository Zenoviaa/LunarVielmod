using Microsoft.Xna.Framework;
using Stellamod.Core.DungeonGeneration;
using Stellamod.Core.SilkSystem;
using Stellamod.Helpers;
using Stellamod.Items.Accessories.Players;
using Stellamod.Items.Weapons.Melee;
using Stellamod.NPCs.Bosses.GothiviaTheSun.GOS;
using Stellamod.NPCs.Bosses.IrradiaNHavoc.Irradia;
using Stellamod.NPCs.Bosses.Verlia.Projectiles;
using Stellamod.NPCs.Colosseum.Common;
using Stellamod.NPCs.Town;
using Stellamod.UI.Dialogue;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Chat;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Stellamod
{
    public static class MultiplayerHelper
    {
        public static bool IsHost => Main.netMode != NetmodeID.MultiplayerClient;
        public static void WriteItemList(this BinaryWriter writer, List<Item> arr)
        {
            writer.Write(arr.Count);
            for (int i = 0; i < arr.Count; i++)
            {
                writer.Write(arr[i].type);
            }
        }

        public static List<Item> ReadItemList(this BinaryReader reader)
        {
            int length = reader.ReadInt32();
            List<Item> itemList = new List<Item>();
            for (int i = 0; i < length; i++)
            {
                itemList.Add(new Item(reader.ReadInt32()));
            }
            return itemList;
        }

        public static ModPacket WriteToPacket(int capacity, MessageType type, Action<ModPacket> action)
        {
            ModPacket packet = Stellamod.Instance.GetPacket(capacity);
            packet.Write((byte)type);
            action?.Invoke(packet);
            return packet;
        }

        public static void HandlePacket(BinaryReader reader, int whoAmI)
        {
            var id = (MessageType)reader.ReadByte();
            byte player;
            switch (id)
            {
                case MessageType.Dodge:
                    VixylPlayer.HandleExampleDodgeMessage(reader, whoAmI);
                    break;
                case MessageType.BossSpawnFromClient:
                    if (Main.netMode == NetmodeID.Server)
                    {
                        player = reader.ReadByte();
                        int bossType = reader.ReadInt32();
                        int TileCordsX = reader.ReadInt32();
                        int TileCordsY = reader.ReadInt32();
                        int npcCenterX = reader.ReadInt32();
                        int npcCenterY = reader.ReadInt32();

                        if (NPC.AnyNPCs(bossType))
                            return;

                        int npcID = NPC.NewNPC(new EntitySource_TileBreak(TileCordsX, TileCordsY), TileCordsX, TileCordsY, bossType);
                        Main.npc[npcID].netUpdate2 = true;

                    }
                    break;
                case MessageType.CompleteMerenaQuest:
                    var questType = (MerenaQuestSystem.QuestType)reader.ReadByte();
                    MerenaQuestSystem.HandleCompleteQuest(questType);
                    break;

                case MessageType.CompleteZuiQuest:
                    ZuiQuestSystem.QuestsCompleted++;
                    break;

                case MessageType.CreatePortal:
                    float altarX = reader.ReadSingle();
                    float altarY = reader.ReadSingle();
                    int left = reader.ReadInt32();
                    int top = reader.ReadInt32();
                    TeleportSystem.CreatePortal(new Vector2(altarX, altarY), left, top);
                    if (Main.netMode == NetmodeID.Server)
                    {
                        TeleportSystem.RefreshPortals();
                    }
                    break;


                case MessageType.StartBossFromDialogue:
                    StartBossFromDialogue((DialogueType)reader.ReadInt32());
                    break;

                case MessageType.StartDialogue:
                    StartDialogue((DialogueType)reader.ReadInt32());
                    break;

                case MessageType.STARBLOCK:
                    //EventWorld.Aurorean = false;
                    if (Main.netMode == NetmodeID.Server)
                    {
                        NetworkText auroeanStarfallEnded = NetworkText.FromLiteral("The Aurorean Starfall has been blocked! :(");
                        ChatHelper.BroadcastChatMessage(auroeanStarfallEnded, new Color(234, 96, 114));
                    }

                    break;

                case MessageType.BreakString:
                    if (Main.netMode == NetmodeID.Server)
                    {
                        int x = reader.ReadInt32();
                        int y = reader.ReadInt32();
                        SilkManager.DestroySilk(x, y);
                    }
                    break;

                case MessageType.DashPlayerSync:
                    byte playernumber = reader.ReadByte();
                    DashPlayer dashPlayer = Main.player[playernumber].GetModPlayer<DashPlayer>();
                    dashPlayer.ReceivePlayerSync(reader);

                    if (Main.netMode == NetmodeID.Server)
                    {
                        // Forward the changes to the other clients
                        dashPlayer.SyncPlayer(-1, whoAmI, false);
                    }
                    break;

                case MessageType.ResetColosseum:
                    if (Main.netMode == NetmodeID.Server)
                    {
                        ColosseumSystem colosseumSystem = ModContent.GetInstance<ColosseumSystem>();
                        colosseumSystem.Reset();
                    }
                    break;
                case MessageType.HandleDoor:
                    if(Main.netMode == NetmodeID.Server)
                    {
                        int x = (int)reader.ReadInt32();
                        int y = (int)reader.ReadInt32();
                        Point tilePosition = new Point(x, y);
                        int d = (int)reader.ReadInt32();
                        if(d == -1)
                        {
                            DungeonGenerationHelper.RemoveDoorInWorld(tilePosition);
                        }
                        else
                        {
                            Door door = (Door)d;
                            DungeonGenerationHelper.PlaceDoorInWorld(tilePosition, door);
                        }
                    }
                    break;
            }
        }

        private static void StartBossFromDialogue(DialogueType dialogueType)
        {
            switch (dialogueType)
            {
                case DialogueType.Start_Verlia:
                    foreach (NPC npc in Main.ActiveNPCs)
                    {
                        if (npc.type == ModContent.NPCType<StarteV>())
                        {
                            StarteV verlia = npc.ModNPC as StarteV;
                            verlia.State = StarteV.ActionState.Death;
                            verlia.ResetTimers();
                        }
                    }
                    break;

                case DialogueType.Start_Irradia:
                    foreach (NPC npc in Main.ActiveNPCs)
                    {
                        if (npc.type == ModContent.NPCType<StartIrradia>())
                        {
                            StartIrradia verlia = npc.ModNPC as StartIrradia;
                            verlia.State = StartIrradia.ActionState.Death;
                            verlia.ResetTimers();
                        }
                    }

                    break;
                case DialogueType.Start_Goth:
                    foreach (NPC npc in Main.ActiveNPCs)
                    {
                        if (npc.type == ModContent.NPCType<StartGoth>())
                        {
                            StartGoth verlia = npc.ModNPC as StartGoth;
                            verlia.State = StartGoth.ActionState.Death;
                            verlia.ResetTimers();
                        }
                    }

                    break;
            }
        }
        private static void StartDialogue(DialogueType dialogueType)
        {
            if (Main.netMode == NetmodeID.Server)
                return;
            switch (dialogueType)
            {
                case DialogueType.Start_Verlia:
                    {
                        DialogueSystem dialogueSystem = ModContent.GetInstance<DialogueSystem>();

                        //2. Create a new instance of your dialogue
                        VerliasDialogue exampleDialogue = new VerliasDialogue();

                        //3. Start it
                        dialogueSystem.StartDialogue(exampleDialogue);
                    }
                    break;
                case DialogueType.Start_Irradia:
                    {
                        DialogueSystem dialogueSystem = ModContent.GetInstance<DialogueSystem>();

                        //2. Create a new instance of your dialogue
                        IrradiaDialogue exampleDialogue = new IrradiaDialogue();

                        //3. Start it
                        dialogueSystem.StartDialogue(exampleDialogue);
                    }
                    break;
                case DialogueType.Start_Goth:
                    {
                        DialogueSystem dialogueSystem = ModContent.GetInstance<DialogueSystem>();

                        //2. Create a new instance of your dialogue
                        GothiviaDialogue exampleDialogue = new GothiviaDialogue();

                        //3. Start it
                        dialogueSystem.StartDialogue(exampleDialogue);
                    }
                    break;
            }
        }

        public static void SpawnBossFromClient(byte whoAmI, int type, int x, int y) =>
            Stellamod.WriteToPacket(Stellamod.Instance.GetPacket(), (byte)MessageType.BossSpawnFromClient, whoAmI, type, x, y).Send(-1);
    }
}