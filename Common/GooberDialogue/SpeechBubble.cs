namespace Stellamod.Common.GooberDialogue;

public class SpeechBubble
{
    public SpeakerParameters speaker;
    public float activeTimer;
    public float inOutTimer;
    public float Scale
    {
        get
        {
            float inScale = EasingFunction.InOutCubic(inOutTimer / EaseTime);
            return inScale;
        }
    }
    public bool showArrow;
    public bool IsActive() => inOutTimer > 0;
    public bool IsValid() => speaker.profile.portraitTextureAsset != null;
    public static float EaseTime => 37;
}
