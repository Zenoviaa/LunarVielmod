using Stellamod.Common.DialogueTowning;
using Stellamod.Core;

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