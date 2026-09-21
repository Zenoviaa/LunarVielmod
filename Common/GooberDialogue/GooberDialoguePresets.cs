using Stellamod.Core;

namespace Stellamod.Common.GooberDialogue;

public static class GooberDialoguePresets
{
    public static GooberDialogueParameters Zui => new()
    {
        startGradientColor = new Color(240, 122, 35),
        endGradientColor = new Color(202, 68, 43),
        outlineColor = new Color(202, 68, 43),
        portraitTextureAsset = AssetReferences.Content.GooberPortraits.ZuiMiniPortrait.Asset,
        bubblePosition = Vector2.Zero,
        name = "You ain't put no text",
        text = string.Empty
    };
}
