using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Stellamod.Common.DialogueTowning;

public enum GameFlag : byte
{
    VerliaAwakeningCutscene
}


public class GameFlags : ModSystem
{
    public static bool[] gameFlags = new bool[256];
    public static void ResetFlags()
    {
        for (int i = 0; i < gameFlags.Length; i++)
        {
            gameFlags[i] = false;
        }
        if (Main.netMode != NetmodeID.SinglePlayer)
        {
            int clientToIgnore = Main.LocalPlayer.whoAmI;
            Stellamod.WriteToPacket(Stellamod.Instance.GetPacket(),
                (byte)MessageType.GameFlagReset).Send(ignoreClient: clientToIgnore);
        }
    }

    public override void ClearWorld()
    {
        base.ClearWorld();
        ResetFlags();
    }

    public override void SaveWorldData(TagCompound tag)
    {
        tag["gameFlags"] = gameFlags;
    }

    public override void LoadWorldData(TagCompound tag)
    {
        gameFlags = tag.Get<bool[]>("gameFlags");
    }


    public static bool IsCleared(GameFlag flag)
    {
        return IsCleared((int)flag);
    }

    public static bool IsCleared(int id)
    {
        return gameFlags[id];
    }

    public static void ClearFlag(GameFlag flag)
    {
        ClearFlag((int)flag);
    }

    public static void HandleFlagClearMessage(BinaryReader reader, int whoAmI)
    {
        int flag = reader.ReadInt32();
        ClearFlag(flag);
    }
    public static void HandleFlagResetMessage(BinaryReader reader, int whoAmI)
    {
        ResetFlags();
    }

    public static void ClearFlag(int id)
    {
        NPC.SetEventFlagCleared(ref gameFlags[id], -1);
        if (Main.netMode != NetmodeID.SinglePlayer)
        {
            int clientToIgnore = Main.LocalPlayer.whoAmI;
            Stellamod.WriteToPacket(Stellamod.Instance.GetPacket(),
                (byte)MessageType.GameFlagClear, id).Send(ignoreClient: clientToIgnore);
        }
    }

    public override void NetSend(BinaryWriter writer)
    {
        base.NetSend(writer);
        int numBytes = gameFlags.Length / 8;
        int j = 0;
        for (int i = 0; i < numBytes; i++)
        {
            BitsByte b = new BitsByte
            {
                [0] = gameFlags[j],
                [1] = gameFlags[j + 1],
                [2] = gameFlags[j + 2],
                [3] = gameFlags[j + 3],
                [4] = gameFlags[j + 4],
                [5] = gameFlags[j + 5],
                [6] = gameFlags[j + 6],
                [7] = gameFlags[j + 7]
            };
            writer.Write(b);
            j += 8;
        }
    }
    public override void NetReceive(BinaryReader reader)
    {
        base.NetReceive(reader);
        int numBytes = gameFlags.Length / 8;
        int j = 0;
        for (int i = 0; i < numBytes; i++)
        {
            BitsByte flags = reader.ReadByte();
            gameFlags[j] = flags[0];
            gameFlags[j + 1] = flags[1];
            gameFlags[j + 2] = flags[2];
            gameFlags[j + 3] = flags[3];
            gameFlags[j + 4] = flags[4];
            gameFlags[j + 5] = flags[5];
            gameFlags[j + 6] = flags[6];
            gameFlags[j + 7] = flags[7];
            j += 8;
        }
    }
}