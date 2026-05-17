using Stellamod.Content.Areas.MoonspiralTower.CariyaBoss;
using Stellamod.Core.DialogueSystem;
using Stellamod.Core.Utilities;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Dialogue;

public class CariyaStartDialogue : BaseDialogue
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        CloseOnComplete = true;
    }

    public override int GetLength()
    {
        return 6;
    }

    public override void OnStart()
    {
        base.OnStart();
        int index = NPC.FindFirstNPC(ModContent.NPCType<CariyaSitting>());
        if (index == -1)
            return;

        NPCUtilities.ChangeNPCAIFromClient(index, ai1: 1);
    }

    public override void OnComplete()
    {
        base.OnComplete();
        int index = NPC.FindFirstNPC(ModContent.NPCType<CariyaSitting>());
        if (index == -1)
            return;

        Vector2 position = Main.npc[index].position;
        position.Y += Main.npc[index].height * 0.5f;
        NPCUtilities.SpawnNPCFromClient<Cariya>(position);
    }
}
