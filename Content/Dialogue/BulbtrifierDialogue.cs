using ReLogic.Content;
using Stellamod.Common.DialogueTowning;
using Stellamod.Common.GooberDialogue;
using Stellamod.Core;
using Stellamod.Core.DialogueSystem;

namespace Stellamod.Content.Dialogue;

public class BulbtrifierHiDialogue : BaseDialogue
{
    public override void GetTalkingParameters(ref SpeechBoxTalkingParameters parameters, int lineNumber)
    {
        parameters.profile = GooberDialoguePresets.Bulbtrifier;
    }
    public override int GetLength()
    {
        return 3;
    }
}

public class BulbtrifierWhoDialogue : BaseDialogue
{
    public override void GetTalkingParameters(ref SpeechBoxTalkingParameters parameters, int lineNumber)
    {
        parameters.profile = GooberDialoguePresets.Bulbtrifier;
    }
    public override int GetLength()
    {
        return 4;
    }
}
public class BulbtrifierHowMuchDialogue : BaseDialogue
{
    public override void GetTalkingParameters(ref SpeechBoxTalkingParameters parameters, int lineNumber)
    {
        parameters.profile = GooberDialoguePresets.Bulbtrifier;
    }
    public override int GetLength()
    {
        return 4;
    }
}
