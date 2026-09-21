namespace Stellamod.Common.GooberDialogue;

public class SpeechBubble
{
    public GooberDialogueParameters parameters;
    public float activeTimer;
    public float inOutTimer;
    public float EaseInOut => EasingFunction.OutCirc(inOutTimer / EaseTime);
    public static float EaseTime => 45;
}
