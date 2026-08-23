using Stellamod.Content.Areas.Tundra.MoonspiralTower.VerliaBoss;
using Stellamod.Core.DialogueSystem;
using Stellamod.Core.Utilities;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Dialogue;

public class VerliaKillDialogue : BaseDialogue
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        CloseOnComplete = true;
    }

    public override int GetLength()
    {
        return 7;
    }


    public override void OnComplete()
    {
        base.OnComplete();
        int index = NPC.FindFirstNPC(ModContent.NPCType<VerliaIdle>());
        if (index == -1)
            return;

        Vector2 position = Main.npc[index].position;
        int x = (int)position.X;
        int y = (int)position.Y;
        NPCUtilities.SpawnNPCFromClient<Verlia>(position);
    }
}