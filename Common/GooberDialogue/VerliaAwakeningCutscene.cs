using Stellamod.Common.DialogueTowning;
using Stellamod.Content.Areas.Tundra.MoonspiralTower.VerliaBoss;
using Stellamod.Core;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Common.GooberDialogue;

public class VerliaAwakeningCutscene : ACutsceneType
{
    public override void Create()
    {
        ref var verliaSpeaker = ref CutsceneHandler.SpeakerNPCs[0];
        for (var i = 0; i < 10; i++)
        {
            Add(GooberDialoguePresets.SpeechBubbleVerlia(
                LocalizationReferences.Mods.Stellamod.TownDialogue.VerliaFreeingDialogue.GetChildText($"Line{i}").Value, verliaSpeaker));
        }
        Add(GooberDialoguePresets.ClearFlag(GameFlag.VerliaAwakeningCutscene));
    }
}
public class VerliaKillingCutscene : ACutsceneType
{
    public override void Create()
    {
        ref var verliaSpeaker = ref CutsceneHandler.SpeakerNPCs[0];
        for (var i = 0; i < 7; i++)
        {
            Add(GooberDialoguePresets.SpeechBubbleVerlia(
                LocalizationReferences.Mods.Stellamod.TownDialogue.VerliaKillDialogue.GetChildText($"Line{i}").Value, verliaSpeaker));
        }
        Add(GooberDialoguePresets.Invoke(() =>
        {
            if (MultiplayerHelper.IsHost)
            {
                int index = NPC.FindFirstNPC(ModContent.NPCType<VerliaIdle>());
                if (index == -1)
                    return;

                Vector2 position = Main.npc[index].position;
                int x = (int)position.X;
                int y = (int)position.Y;
                NPCUtilities.SpawnNPCFromClient<Verlia>(position);
            }
        }));
    }
}