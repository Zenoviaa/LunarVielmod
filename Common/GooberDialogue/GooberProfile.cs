using ReLogic.Content;
using Terraria.Audio;

namespace Stellamod.Common.GooberDialogue;

public struct SpeakerParameters
{
    public GooberProfile profile;
    public string text;
    public Vector2 bubblePosition;
    public int textIndex;
    public int speakerNpcWhoAmI;

}

/// <summary>
/// Parameters for a speech bubble that's going to be drawn in the world
/// </summary>
public struct GooberProfile
{
    public Asset<Texture2D> bigPortraitTextureAsset;
    public Asset<Texture2D> portraitTextureAsset;
    public SoundStyle? talkingSound;
    public Color startGradientColor;
    public Color endGradientColor;
    public Color outlineColor;

    public float timeBetweenTexts;
    public string name;
}

