namespace Stellamod.Common.GooberDialogue;

public class SpeechBubble
{
    public GooberDialogueParameters parameters;
    public float activeTimer;
    public float inOutTimer;
    public float EaseInOut => EasingFunction.OutCirc(inOutTimer / EaseTime);
    public float Scale
    {
        get
        {
            float inScale = EasingFunction.OutSine(inOutTimer / EaseTime);
            return inScale;
        }
    }
    public static float EaseTime => 45;
}
