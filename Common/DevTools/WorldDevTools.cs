using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Common.DevTools;

public class WorldDevTools : ModSystem
{
    public static bool DisableNPCSpawns
    {
        get => field;
        set
        {
            var old = field;
            field = value;
            if (!old && field)
            {
                ClearNPCs();
            }
        }
    }

    public static bool FreezeTime
    {
        get => field;
        set
        {
            field = value;
        }
    }

    public override void ModifyTimeRate(ref double timeRate, ref double tileUpdateRate, ref double eventUpdateRate)
    {
        base.ModifyTimeRate(ref timeRate, ref tileUpdateRate, ref eventUpdateRate);
        if (FreezeTime)
        {
            timeRate = 0;
            tileUpdateRate = 0;
            eventUpdateRate = 0;
        }
    }
    public static void ClearItems()
    {
        foreach (var item in Main.ActiveItems)
        {
            item.active = false;
        }
    }
    public static void SetTimeNoSync(double time, bool isNight)
    {
        Main.time = time;
        Main.dayTime = !isNight;
    }
    public static void SetTime(double time, bool isNight)
    {
        Main.time = time;
        Main.dayTime = !isNight;
        if (Main.netMode == NetmodeID.MultiplayerClient)
        {
            SendTimeSyncPacket();
        }
    }

    public static void HandleDisableNPCSpawnsPacket(BinaryReader reader)
    {
        var areSpawnsEnabled = reader.ReadBoolean();
        DisableNPCSpawns = areSpawnsEnabled;
        if (Main.netMode == NetmodeID.Server)
        {
            SendDisableNPCSpawnsPacket();
        }
    }


    public static void ToggleClearItems()
    {
        ClearItems();
        if (Main.netMode == NetmodeID.MultiplayerClient)
        {
            SendClearItemsPacket();
        }
    }

    public static void ToggleNPCSpawns()
    {
        DisableNPCSpawns = !DisableNPCSpawns;
        if (Main.netMode == NetmodeID.MultiplayerClient)
        {
            SendDisableNPCSpawnsPacket();
        }

    }

    public static void ToggleFreezeTime()
    {
        FreezeTime = !FreezeTime;
        if (Main.netMode == NetmodeID.MultiplayerClient)
        {
            SendFreezeTimePacket();
        }
    }
    public static void HandleTimeSyncPacket(BinaryReader reader)
    {
        var time = reader.ReadDouble();
        var isNight = reader.ReadBoolean();
        Main.time = time;
        Main.dayTime = !isNight;
        if (Main.netMode == NetmodeID.Server)
        {
            SendTimeSyncPacket();
        }
    }

    public static void HandleClearItemsPacket(BinaryReader reader)
    {
        if (Main.netMode == NetmodeID.Server)
        {
            SendClearItemsPacket();
        }
    }

    public static void HandleFreezeTimePacket(BinaryReader reader)
    {
        var areTimeFroze = reader.ReadBoolean();
        FreezeTime = areTimeFroze;
        if (Main.netMode == NetmodeID.Server)
        {
            SendFreezeTimePacket();
        }
    }
    public static void SendClearItemsPacket()
    {
        var packet = Stellamod.Instance.GetPacket();
        packet.Write((byte)MessageType.ClearItems);
        packet.Send(-1);
    }
    public static void SendTimeSyncPacket()
    {
        var packet = Stellamod.Instance.GetPacket();
        packet.Write((byte)MessageType.SetTime);
        packet.Write(Main.time);
        packet.Write(Main.dayTime);
        packet.Send(-1);
    }

    public static void SendFreezeTimePacket()
    {
        var packet = Stellamod.Instance.GetPacket();
        packet.Write((byte)MessageType.SetFrozenTime);
        packet.Write(FreezeTime);
        packet.Send(-1);
    }

    public static void SendDisableNPCSpawnsPacket()
    {
        var packet = Stellamod.Instance.GetPacket();
        packet.Write((byte)MessageType.SetDisableNPCSpawns);
        packet.Write(DisableNPCSpawns);
        packet.Send(-1);
    }

    public static void ClearNPCs()
    {
        foreach (var npc in Main.ActiveNPCs)
        {
            npc.life = 0;
            if (Main.netMode == NetmodeID.Server)
                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, npc.whoAmI, 0f, 0f, 0f, 0);
        }
    }


}
