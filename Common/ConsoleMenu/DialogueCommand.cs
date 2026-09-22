using Stellamod.Common.GooberDialogue;
using Stellamod.Core.TileOverlaySystem;
using Stellamod.NPCs.Town;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Core;

namespace Stellamod.Common.ConsoleMenu;

public class DialogueCommand : ConsoleCommand
{
    public override string GetCommandName()
    {
        return "dialogue";
    }
    public override Arguments GetArguments()
    {
        return null;
    }
    public override bool Invoke(params string[] args)
    {
        CutsceneHandler.SpeakerNPCs[0] = NPC.FindFirstNPC(ModContent.NPCType<Zui>());
        CutsceneHandler.Play(ModContent.GetInstance<ZuiTestCutscene>());
        return true;
    }
}
