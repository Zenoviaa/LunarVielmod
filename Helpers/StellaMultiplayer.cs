using Microsoft.Xna.Framework;
using Stellamod.Helpers;
using Stellamod.Items.Weapons.Melee;
using Stellamod.NPCs.Town;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod
{
    public static class StellaMultiplayer
	{
		private struct Wait
		{
			public Func<bool> Condition { get; set; }
			public Action Result { get; set; }
		}

		private static List<Wait> _waits = new List<Wait>();

		public static bool IsHost => Main.netMode != NetmodeID.MultiplayerClient;
        public static void Load() => Main.OnTickForInternalCodeOnly += OnTick;

		public static void Unload()
		{
			Main.OnTickForInternalCodeOnly -= OnTick;
			_waits = null;
		}

		public static void OnTick()
		{
			if (_waits == null) return;

			for (int i = 0; i < _waits.Count; i++)
			{
				Wait wait = _waits[i];
				if (wait.Condition.Invoke())
				{
					wait.Result?.Invoke();
					_waits.RemoveAt(i--);
				}
			}
		}

		public static void WaitUntil(Func<bool> condition, Action whenTrue) => _waits.Add(new Wait() { Condition = condition, Result = whenTrue });

		// This is deprecated, DO NOT USE IT. Only here for compatability until later stages when we decide to swap it out for the new one.
		public static ModPacket WriteToPacket(ModPacket packet, byte msg, params object[] param)
		{
			packet.Write(msg);

			for (int m = 0; m < param.Length; m++)
			{
				object obj = param[m];
				if (obj is bool) packet.Write((bool)obj);
				else if (obj is byte) packet.Write((byte)obj);
				else if (obj is int) packet.Write((int)obj);
				else if (obj is float) packet.Write((float)obj);
				else if (obj is double) packet.Write((double)obj);
				else if (obj is short) packet.Write((short)obj);
				else if (obj is ushort) packet.Write((ushort)obj);
				else if (obj is sbyte) packet.Write((sbyte)obj);
				else if (obj is uint) packet.Write((uint)obj);
				else if (obj is decimal) packet.Write((decimal)obj);
				else if (obj is long) packet.Write((long)obj);
				else if (obj is string) packet.Write((string)obj);
			}
			return packet;
		}

		public static ModPacket WriteToPacket(int capacity, MessageType type)
		{
			ModPacket packet = Stellamod.Instance.GetPacket(capacity);
			packet.Write((byte)type);
			return packet;
		}

		public static ModPacket WriteToPacket(int capacity, MessageType type, Action<ModPacket> action)
		{
			ModPacket packet = Stellamod.Instance.GetPacket(capacity);
			packet.Write((byte)type);
			action?.Invoke(packet);
			return packet;
		}

		public static ModPacket WriteToPacketAndSend(int capacity, MessageType type, Action<ModPacket> beforeSend)
		{
			var packet = WriteToPacket(capacity, type, beforeSend);
			packet.Send();
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
                    bool con1 = reader.ReadBoolean();
                    bool con2 = reader.ReadBoolean();
                    bool con3 = reader.ReadBoolean();
                    bool con4 = reader.ReadBoolean();
                    bool con5 = reader.ReadBoolean();

                    if (!MerenaQuestSystem.KillVerliaCompleted)
                        MerenaQuestSystem.KillVerliaCompleted = con1;
                    if (!MerenaQuestSystem.ExploreMorrowedVillageCompleted)
                        MerenaQuestSystem.ExploreMorrowedVillageCompleted = con2;
                    if (!MerenaQuestSystem.Give100DustBagsCompleted)
                        MerenaQuestSystem.Give100DustBagsCompleted = con3;
                    if (!MerenaQuestSystem.MakeMagicPaperCompleted)
                        MerenaQuestSystem.MakeMagicPaperCompleted = con4;
                    if (!MerenaQuestSystem.MakeTomeOfInfiniteSorceryCompleted)
                        MerenaQuestSystem.MakeTomeOfInfiniteSorceryCompleted = con5;
                    break;

				case MessageType.CompleteZuiQuest:
                    bool ccon1 = reader.ReadBoolean();
                    bool ccon2 = reader.ReadBoolean();
                    bool ccon3 = reader.ReadBoolean();
                    bool ccon4 = reader.ReadBoolean();
                    bool ccon5 = reader.ReadBoolean();
                    int questsCompleted = reader.ReadInt32();

                    if (!ZuiQuestSystem.ThreeQuestsCompleted)
                        ZuiQuestSystem.ThreeQuestsCompleted = ccon1;
                    if (!ZuiQuestSystem.SixQuestsCompleted)
                        ZuiQuestSystem.SixQuestsCompleted = ccon2;
                    if (!ZuiQuestSystem.TenQuestsCompleted)
                        ZuiQuestSystem.TenQuestsCompleted = ccon3;
                    if (!ZuiQuestSystem.TwentyQuestsCompleted)
                        ZuiQuestSystem.TwentyQuestsCompleted = ccon4;
                    if (!ZuiQuestSystem.ThirtyQuestsCompleted)
                        ZuiQuestSystem.ThirtyQuestsCompleted = ccon5;
                    if (ZuiQuestSystem.QuestsCompleted <= questsCompleted)
                        ZuiQuestSystem.QuestsCompleted = questsCompleted;
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
			}
		}

		public static void SpawnBossFromClient(byte whoAmI, int type, int x, int y) => 
			Stellamod.WriteToPacket(Stellamod.Instance.GetPacket(), (byte)MessageType.BossSpawnFromClient, whoAmI, type, x, y).Send(-1);
	}
}