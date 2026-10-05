using ReLogic.Content;
using Stellamod.Common.DialogueTowning;
using Stellamod.Common.GooberDialogue;
using Stellamod.Core;
using Stellamod.Core.DialogueSystem;

namespace Stellamod.Content.Dialogue
{
    public class ListUmDialogue : BaseDialogue
    {
        public override void GetTalkingParameters(ref SpeechBoxTalkingParameters parameters, int lineNumber)
        {
            parameters.profile = GooberDialoguePresets.List;
        }
        public override int GetLength()
        {
            return 5;
        }
    }
    public class ListWhyHereDialogue : BaseDialogue
    {
        public override void GetTalkingParameters(ref SpeechBoxTalkingParameters parameters, int lineNumber)
        {
            parameters.profile = GooberDialoguePresets.List;
        }
        public override int GetLength()
        {
            return 3;
        }
    }
    public class ListZuiDialogue : BaseDialogue
    {
        public override void GetTalkingParameters(ref SpeechBoxTalkingParameters parameters, int lineNumber)
        {
            parameters.profile = GooberDialoguePresets.List;
        }
        public override int GetLength()
        {
            return 8;
        }
    }
    public class ListAloneDialogue : BaseDialogue
    {
        public override void GetTalkingParameters(ref SpeechBoxTalkingParameters parameters, int lineNumber)
        {
            parameters.profile = GooberDialoguePresets.List;
        }
        public override int GetLength()
        {
            return 8;
        }
    }
}
