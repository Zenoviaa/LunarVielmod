using Stellamod.Common.ClassReworkSystem;
using Stellamod.Common.DungeonGeneration;
using Stellamod.Common.Players;
using Stellamod.Common.WaypointSystem;
using Stellamod.Content.Areas.Collosseum.Event.Common;
using Stellamod.Content.Areas.Fable.WeaponsFB;
using Stellamod.Content.Special.DeadRomancesExcalibur;
using Stellamod.Core;
using Stellamod.Core.PlayerLevelingSystem;
using Stellamod.Core.RibbonSystem;
using Stellamod.Core.ZTileSystem;
using Stellamod.Items;
using Stellamod.Items.Accessories.Players;
using Stellamod.Items.Weapons.Melee;
using Stellamod.NPCs.Bosses.IrradiaNHavoc.Irradia;
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
                case MessageType.ZTileSync:
                    ModContent.GetInstance<ZTileMap>().HandleZTileSyncPacket(reader);
                    break;
                case MessageType.BossDowned:
                    DownedBossRewardPlayer.HandleBossDownedMessage(reader, whoAmI);
                    break;
                case MessageType.RomanceDodge:
                    DeadRomancePlayer.HandleExampleDodgeMessage(reader, whoAmI);
                    break;
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
                case MessageType.SpawnNPC:
                    if (Main.netMode == NetmodeID.Server)
                    {
                        int npc = reader.ReadInt32();
                        int x = reader.ReadInt32();
                        int y = reader.ReadInt32();
                        int type = reader.ReadInt32();
                        float ai0 = reader.ReadSingle();
                        float ai1 = reader.ReadSingle();
                        float ai2 = reader.ReadSingle();
                        float ai3 = reader.ReadSingle();
                        int n = NPC.NewNPC(new EntitySource_Misc("Dragon"), x, y, type, ai0: ai0, ai1: ai1, ai2: ai2, ai3: ai3);
                        Main.npc[n].netUpdate = true;
                        Main.npc[n].netUpdate2 = true;
                    }
                    break;
                case MessageType.CompleteMerenaQuest:
                    var questType = (MerenaQuestSystem.QuestType)reader.ReadByte();
                    MerenaQuestSystem.HandleCompleteQuest(questType);
                    break;

                case MessageType.CompleteZuiQuest:
                    ZuiQuestSystem.QuestsCompleted++;
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
                case MessageType.DashPlayerSync:
                    {

                        byte playernumber = reader.ReadByte();
                        DashPlayer dashPlayer = Main.player[playernumber].GetModPlayer<DashPlayer>();
                        dashPlayer.ReceivePlayerSync(reader);

                        if (Main.netMode == NetmodeID.Server)
                        {
                            // Forward the changes to the other clients
                            dashPlayer.SyncPlayer(-1, whoAmI, false);
                        }
                    }
                    break;
                case MessageType.LevelingPlayerSync:
                    {
                        byte playernumber = reader.ReadByte();
                        LevelingPlayer levelingPlayer = Main.player[playernumber].GetModPlayer<LevelingPlayer>();
                        levelingPlayer.ReceivePlayerSync(reader);

                        if (Main.netMode == NetmodeID.Server)
                        {
                            // Forward the changes to the other clients
                            levelingPlayer.SyncPlayer(-1, whoAmI, false);
                        }
                    }
                    break;
                case MessageType.RecoilPlayerSync:
                    {

                        byte playernumber = reader.ReadByte();
                        RecoilPlayer recoilPlayer = Main.player[playernumber].GetModPlayer<RecoilPlayer>();
                        recoilPlayer.ReceivePlayerSync(reader);

                        if (Main.netMode == NetmodeID.Server)
                        {
                            // Forward the changes to the other clients
                            recoilPlayer.SyncPlayer(-1, whoAmI, false);
                        }
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
                    if (Main.netMode == NetmodeID.Server)
                    {
                        int x = reader.ReadInt32();
                        int y = reader.ReadInt32();
                        Point tilePosition = new Point(x, y);
                        int d = reader.ReadInt32();
                        if (d == -1)
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
                case MessageType.ScarecrowPlayerSync:
                    {
                        byte playernumber = reader.ReadByte();
                        ScarecrowSaberPlayer dashPlayer = Main.player[playernumber].GetModPlayer<ScarecrowSaberPlayer>();
                        dashPlayer.ReceivePlayerSync(reader);

                        if (Main.netMode == NetmodeID.Server)
                        {
                            // Forward the changes to the other clients
                            dashPlayer.SyncPlayer(-1, whoAmI, false);
                        }
                    }
                    break;
                case MessageType.BreakRibbon:
                    {
                        int rx = reader.ReadInt32();
                        int ry = reader.ReadInt32();
                        RibbonRenderer ribbonRenderer = ModContent.GetInstance<RibbonRenderer>();
                        ribbonRenderer.ReceiveBreakRibbonSync(rx, ry);
                    }

                    break;
                case MessageType.PlaceRibbon:
                    {
                        float x1 = reader.ReadSingle();
                        float y1 = reader.ReadSingle();
                        float x2 = reader.ReadSingle();
                        float y2 = reader.ReadSingle();
                        RibbonWandType wandType = (RibbonWandType)reader.ReadInt32();
                        RibbonRenderer ribbonRenderer = ModContent.GetInstance<RibbonRenderer>();
                        Vector2 start = new Vector2(x1, y1);
                        Vector2 end = new Vector2(x2, y2);
                        ribbonRenderer.ReceivePlaceRibbonSync(start, end, wandType);
                    }

                    break;
                case MessageType.PlaceDecoration:
                    {
                        ZRenderLayer layer = (ZRenderLayer)reader.ReadByte();
                        ZTilePosition tilePosition = new ZTilePosition();
                        tilePosition.x = reader.ReadUInt16();
                        tilePosition.y = reader.ReadUInt16();
                        tilePosition.z = reader.ReadUInt16();

                        ZTileInstanceData instanceData = new ZTileInstanceData();
                        instanceData.scale = reader.ReadSingle();
                        instanceData.flipX = reader.ReadBoolean();
                        instanceData.frameNumber = reader.ReadUInt16();
                        instanceData.rotation = (Rotation)reader.ReadByte();
                        instanceData.type = reader.ReadUInt16();
                        instanceData.value = reader.ReadByte();

                        ZTileMap tileMap = ModContent.GetInstance<ZTileMap>();
                        tileMap.Add(layer, tilePosition, instanceData);
                        if (Main.netMode == NetmodeID.Server)
                        {
                            //Forward all changes to other clients
                            tileMap.SyncPlaceTile(-1, whoAmI, layer, tilePosition, instanceData);
                        }
                    }

                    break;

                case MessageType.BreakDecoration:
                    {
                        Point tilePosition = new Point();
                        tilePosition.X = reader.ReadUInt16();
                        tilePosition.Y = reader.ReadUInt16();
                        ZTileMap tileMap = ModContent.GetInstance<ZTileMap>();
                        tileMap.KillAnyTile(tilePosition);
                        if (Main.netMode == NetmodeID.Server)
                        {
                            //Forward all changes to other clients
                            tileMap.SyncBreakTile(-1, whoAmI, tilePosition);
                        }
                    }

                    break;

                case MessageType.AggroSync:
                    {
                        int seed = reader.ReadInt32();
                        AggroSystem aggroSystem = ModContent.GetInstance<AggroSystem>();
                        aggroSystem.seed = seed;
                    }
                    break;

                case MessageType.CauldronSync:
                    {
                        Cauldron cauldron = ModContent.GetInstance<Cauldron>();

                        cauldron.HandleSyncPacket(reader);
                    }
                    break;
                case MessageType.WaypointActivate:
                    {
                        ModContent.GetInstance<OrganWaypointTracker>().HandleWaypointActivatePacket(reader);
                    }
                    break;
                case MessageType.ChangeNPCAI:
                    {
                        int npcWhoAmI = reader.ReadInt32();
                        float ai0 = reader.ReadSingle();
                        float ai1 = reader.ReadSingle();
                        float ai2 = reader.ReadSingle();
                        float ai3 = reader.ReadSingle();
                        NPCUtilities.HandleNPCAIChange(npcWhoAmI, ai0, ai1, ai2, ai3);
                    }
                    break;
                case MessageType.ClassReworkPlayerSync:
                    {
                        ClassReworkPlayer reworkPlayer = Main.LocalPlayer.GetModPlayer<ClassReworkPlayer>();
                        reworkPlayer.playerClass = (PlayerClass)reader.ReadSingle();
                    }
                    break;
            }
        }

        private static void StartBossFromDialogue(DialogueType dialogueType)
        {
            switch (dialogueType)
            {
                case DialogueType.Start_Verlia:
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
        public static void SpawnNPCFromClient(byte whoAmI, int type, int x, int y, float ai0 = 0, float ai1 = 0, float ai2 = 0, float ai3 = 0) =>
            Stellamod.WriteToPacket(Stellamod.Instance.GetPacket(), (byte)MessageType.SpawnNPC, whoAmI, type, x, y, ai0, ai1, ai2, ai3).Send(-1);
        public static void SpawnBossFromClient(byte whoAmI, int type, int x, int y, float ai0 = 0, float ai1 = 0, float ai2 = 0, float ai3 = 0) =>
            Stellamod.WriteToPacket(Stellamod.Instance.GetPacket(), (byte)MessageType.BossSpawnFromClient, whoAmI, type, x, y, ai0, ai1, ai2, ai3).Send(-1);
    }
}