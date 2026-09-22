namespace Stellamod.Common.GooberDialogue;

public class ZuiTestCutscene : ACutsceneType
{
    public ZuiTestCutscene()
    {

    }

    public override void Create()
    {
        ref var zuiSpeaker = ref CutsceneHandler.SpeakerNPCs[0];
        Add(GooberDialoguePresets.SpeechBubbleZui(
            "Hey there little guy, did you get lost? Would you like to have some of my magic?", zuiSpeaker));
        Add(GooberDialoguePresets.SpeechBubbleZui(
            "Don't worry, I'm not going to kidnap you in my white van, not like I have one or anything...", zuiSpeaker));
        Add(GooberDialoguePresets.SpeechBubbleZui(
            "It's okay though! I'm here to help haha... yeah...", zuiSpeaker));
    }
}

