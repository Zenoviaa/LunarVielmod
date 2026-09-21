using ReLogic.Content;

namespace Stellamod.Common.GooberDialogue;

/// <summary>
/// Parameters for a speech bubble that's going to be drawn in the world
/// </summary>
public struct GooberDialogueParameters
{
    public Asset<Texture2D> portraitTextureAsset;
    public Color startGradientColor;
    public Color endGradientColor;
    public Color outlineColor;
    public Vector2 bubblePosition;
    public int textIndex;
    public string text;
    public string name;
}
