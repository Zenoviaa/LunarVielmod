namespace Stellamod.Common.GooberDialogue;

/// <summary>
/// Wrapper for a speech bubble class so we can automatically update the active timer when accessing it
/// </summary>
public class SpeechBubbleWrapper
{
    private readonly SpeechBubble _bubble;
    public SpeechBubbleWrapper(SpeechBubble bubble)
    {
        _bubble = bubble;
        _bubble.activeTimer = 10;
    }

    public SpeechBubble Bubble
    {
        get
        {
            _bubble.activeTimer = 10;
            return _bubble;
        }
    }
}
