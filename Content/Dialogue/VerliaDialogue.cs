using ReLogic.Content;
using Stellamod.Common.DialogueTowning;
using Stellamod.Common.GooberDialogue;
using Stellamod.Core;
using Stellamod.Core.DialogueSystem;

namespace Stellamod.Content.Dialogue;


public class VerliaHappenedDialogue : BaseDialogue
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
    }
    public override void GetTalkingParameters(ref SpeechBoxTalkingParameters parameters, int lineNumber)
    {
        parameters.profile = GooberDialoguePresets.Verlia;
    }

    public override int GetLength()
    {
        return 6;
    }
}

public class VerliaFamilyDialogue : BaseDialogue
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
    }

    public override void GetTalkingParameters(ref SpeechBoxTalkingParameters parameters, int lineNumber)
    {
        parameters.profile = GooberDialoguePresets.Verlia;
    }

    public override int GetLength()
    {
        return 6;
    }
}

public class VerliaWingsDialogue : BaseDialogue
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
    }

    public override void GetTalkingParameters(ref SpeechBoxTalkingParameters parameters, int lineNumber)
    {
        parameters.profile = GooberDialoguePresets.Verlia;
    }

    public override int GetLength()
    {
        return 8;
    }
}