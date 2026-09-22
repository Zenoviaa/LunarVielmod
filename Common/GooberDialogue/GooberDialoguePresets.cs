using Stellamod.Common.ConsoleMenu;
using Stellamod.Core;

namespace Stellamod.Common.GooberDialogue;

public static class GooberDialoguePresets
{
    public static GooberProfile Zui => new()
    {
        startGradientColor = new Color(240, 122, 35),
        endGradientColor = new Color(202, 68, 43),
        outlineColor = new Color(202, 68, 43),
        portraitTextureAsset = AssetReferences.Content.GooberPortraits.ZuiMiniPortrait.Asset,
        timeBetweenTexts = 2,
        talkingSound = AssetReferences.Assets.Sounds.Voice.SpaeraTalk.Asset with { Volume = 0.07f },
        name = "Zui"
    };

    public static SpeakerParameters SpeakZui(string text)
    {
        GooberProfile profile = Zui;
        return new SpeakerParameters
        {
            profile = profile,
            text = text,
            textIndex = 0
        };
    }

    public static SpeechBubbleAction SpeechBubbleZui(string text, int zuiSpeaker)
    {
        return new SpeechBubbleAction(SpeakZui(text) with { speakerNpcWhoAmI = zuiSpeaker});
    }
}
