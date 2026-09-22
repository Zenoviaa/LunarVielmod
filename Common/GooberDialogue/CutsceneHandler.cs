using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Core;

namespace Stellamod.Common.GooberDialogue;

//Which means to net sync a cutscene
//All we have to do is send the type, elapsed time, and the npc whoAmIs for speakers
//and any other data we may have in the future
//Perfect!
//I'll make a packet later

public class CutsceneHandler : ModSystem
{
    /// <summary>
    /// Plays a cutscene
    /// </summary>
    public static readonly CutsceneTimeline Timeline = new();
    public static ACutsceneType[] CutscenePrefabs;

    /// <summary>
    /// The player that is controlling and progressing the cutscene
    /// </summary>
    public static int CutsceneHostWhoAmI
    {
        get;
        set;
    }

    public static Player CutsceneHost => Main.player[CutsceneHostWhoAmI];

    public static int[] SpeakerNPCs = new int[16];

    public override void Load()
    {
        base.Load();
        GooberDialogueSystem.OnClearSpeechBubbles += UpdateTimeline;
    }
    public override void Unload()
    {
        base.Unload();
        GooberDialogueSystem.OnClearSpeechBubbles -= UpdateTimeline;
    }
    public override void PostSetupContent()
    {
        base.PostSetupContent();
        List<ACutsceneType> cutscenes = new();

        Type[] types = AssemblyManager.GetLoadableTypes(Mod.Code);

        foreach (Type type in types.Where(t => t.IsClass && t.IsSubclassOf(typeof(ACutsceneType)) && !t.IsAbstract))
        {
            ACutsceneType overlayType = Activator.CreateInstance(type) as ACutsceneType;
            overlayType.Type = cutscenes.Count;
            cutscenes.Add(overlayType);
        }
        CutscenePrefabs = cutscenes.ToArray();
    }
    private void UpdateTimeline()
    {
        Timeline.Update();

        //If the cutscene host is no longer active, shift the host, this will prevent a softlock from disconnecting
        if(Main.netMode == NetmodeID.Server)
        {
            if (!CutsceneHost.active)
            {
                foreach(var player in Main.ActivePlayers)
                {
                    CutsceneHostWhoAmI = player.whoAmI;
                    break;
                }
                SendCutsceneSync();
            }
        }
    }

    public static void SendCutsceneSync()
    {
        var packet = Stellamod.Instance.GetPacket();
        packet.Write((byte)MessageType.CutsceneSync);
        packet.Write(Timeline.Cutscene == null ? -1 : Timeline.Cutscene.Type);
        packet.Write(Timeline.ElapsedTime);
        packet.Write(Timeline.CutsceneIndex);
        for(int i = 0; i < SpeakerNPCs.Length; i++)
        {
            packet.Write(SpeakerNPCs[i]);
        }
        packet.Write(CutsceneHostWhoAmI);
        packet.Send(-1);
    }

    public static void ReceiveCutsceneSync(BinaryReader reader, int whoAmI)
    {
        int sceneType = reader.ReadInt32();
        if(sceneType != -1)
        {
            Timeline.Cutscene = CutscenePrefabs[sceneType];
       //     Main.NewText($"Set Cutscene Type {sceneType}");
        }
        Timeline.ElapsedTime = reader.ReadSingle();
        Timeline.CutsceneIndex = reader.ReadInt32();
        for(int i = 0; i < SpeakerNPCs.Length; i++)
        {
            SpeakerNPCs[i] = reader.ReadInt32();
        }
        CutsceneHostWhoAmI = reader.ReadInt32();
        if (Main.netMode == NetmodeID.Server)
        {
            //Forward to all connected clients
            SendCutsceneSync();
        }
    }

    public static void Next()
    {
        Timeline.Next();
        if (Main.netMode != NetmodeID.SinglePlayer)
        {
            SendCutsceneSync();
        }
    }

    public static void Play(ACutsceneType type)
    {
        Timeline.Play(type);
        if (Main.netMode != NetmodeID.SinglePlayer)
        {
            SendCutsceneSync();
        }
    }

    public override void PostUpdateEverything()
    {
        base.PostUpdateEverything();

    }
}

