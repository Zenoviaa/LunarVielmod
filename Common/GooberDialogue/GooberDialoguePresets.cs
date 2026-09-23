using Stellamod.Common.ConsoleMenu;
using Stellamod.Common.DialogueTowning;
using Stellamod.Core;

namespace Stellamod.Common.GooberDialogue;

public static class GooberDialoguePresets
{
    public static GooberProfile Delgrim => Default with
    {
        bigPortraitTextureAsset = AssetReferences.Core.DialogueSystem.Delgrim.Asset,
        portraitTextureAsset = null,
        name = "Delgrim"
    };
    public static GooberProfile Daedus => Default with
    {
        bigPortraitTextureAsset = AssetReferences.Core.DialogueSystem.Daedus.Asset,
        portraitTextureAsset = null,
        name = "Daedus"
    };
    public static GooberProfile Bulbtrifier => Default with
    {
        bigPortraitTextureAsset = AssetReferences.Core.DialogueSystem.Bulbtrifier.Asset,
        portraitTextureAsset = null,
        name = "Bulbtrifier"
    };
    public static GooberProfile Gilatine => Default with
    {
        bigPortraitTextureAsset = AssetReferences.Core.DialogueSystem.Gilatine.Asset,
        portraitTextureAsset = null,
        name = "Gilatine"
    };
    public static GooberProfile Gothivia => Default with
    {
        bigPortraitTextureAsset = AssetReferences.Core.DialogueSystem.Gothivia.Asset,
        portraitTextureAsset = null,
        name = "Gothivia"
    };
    public static GooberProfile Jack => Default with
    {
        bigPortraitTextureAsset = AssetReferences.Core.DialogueSystem.Jack.Asset,
        portraitTextureAsset = null,
        name = "Jack"
    };
    public static GooberProfile Jiitas => Default with
    {
        bigPortraitTextureAsset = AssetReferences.Core.DialogueSystem.Jiitas.Asset,
        portraitTextureAsset = null,
        name = "Jiitas"
    };
    public static GooberProfile List => Default with
    {
        bigPortraitTextureAsset = AssetReferences.Core.DialogueSystem.List.Asset,
        portraitTextureAsset = null,
        name = "List"
    };
    public static GooberProfile ManMan => Default with
    {
        bigPortraitTextureAsset = AssetReferences.Core.DialogueSystem.ManMan.Asset,
        portraitTextureAsset = null,
        name = "ManMan"
    };
    public static GooberProfile Mordred => Default with
    {
        bigPortraitTextureAsset = AssetReferences.Core.DialogueSystem.Mordred.Asset,
        portraitTextureAsset = null,
        name = "Mordred"
    };
    public static GooberProfile GardenerWilly => Default with
    {
        bigPortraitTextureAsset = AssetReferences.Core.DialogueSystem.GardenerWilly.Asset,
        portraitTextureAsset = null,
        name = "GardenerWilly"
    };

    public static GooberProfile Rysa => Default with
    {
        bigPortraitTextureAsset = AssetReferences.Core.DialogueSystem.Rysa.Asset,
        portraitTextureAsset = null,
        name = "Rysa"
    };

    public static GooberProfile Sirestias => Default with
    {
        bigPortraitTextureAsset = AssetReferences.Core.DialogueSystem.Sirestias.Asset,
        portraitTextureAsset = null,
        name = "Sirestias"
    };

    public static GooberProfile Veizal => Default with
    {
        bigPortraitTextureAsset = AssetReferences.Core.DialogueSystem.Veizal.Asset,
        portraitTextureAsset = null,
        name = "Veizal"
    };

    public static GooberProfile Veldris => Default with
    {
        bigPortraitTextureAsset = AssetReferences.Core.DialogueSystem.Veldris.Asset,
        portraitTextureAsset = null,
        name = "Veldris"
    };

    public static GooberProfile Verlia => Default with
    {
        bigPortraitTextureAsset = AssetReferences.Core.DialogueSystem.Verlia.Asset,
        portraitTextureAsset = null,
        name = "Verlia"
    };

    public static GooberProfile Bishinine => Default with
    {
        bigPortraitTextureAsset = AssetReferences.Core.DialogueSystem.Bishinine.Asset,
        portraitTextureAsset = null,
        name = "Bishinine"
    };

    public static GooberProfile Default => new()
    {
        startGradientColor = new Color(50, 50, 50),
        endGradientColor = new Color(0, 0, 0),
        outlineColor = Color.Red,
        bigPortraitTextureAsset = AssetReferences.Core.DialogueSystem.QuestionMark.Asset,
        portraitTextureAsset = AssetReferences.Content.GooberPortraits.ZuiMiniPortrait.Asset,
        timeBetweenTexts = 2,
        talkingSound = AssetReferences.Assets.Sounds.Voice.SpaeraTalk.Asset with { Volume = 0.07f },
        name = "Verlia"
    };

    public static GooberProfile Zui => new()
    {
        startGradientColor = new Color(240, 122, 35),
        endGradientColor = new Color(202, 68, 43),
        outlineColor = new Color(202, 68, 43),
        boxStyle = new ZuiDialogueStyle(),
        bigPortraitTextureAsset = AssetReferences.Core.DialogueSystem.Zui.Asset,
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
