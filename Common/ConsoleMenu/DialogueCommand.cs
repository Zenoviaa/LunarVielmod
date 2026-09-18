using Stellamod.Common.GooberDialogue;
using Terraria;

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
        SpeechBubbleWrapper wrapper = GooberDialogueSystem.CreateBubble();
        wrapper.Bubble.parameters = GooberDialoguePresets.Zui with
        {
            text = "This is placeholder text to test if the dialogue wraps around the speech bubble as expected...",
            name = "Zui",
            bubblePosition = Main.LocalPlayer.TopRight
        };
        GooberDialogueSpeaker speaker = new GooberDialogueSpeaker(wrapper);
        UpdateableSystem.Updateables.Add(speaker);
        return true;
    }
}
