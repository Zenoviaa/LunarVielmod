using Stellamod.Core;
using Terraria.Audio;

namespace Stellamod.Common.GooberDialogue;

public class GooberDialogueSpeaker :
    IUpdateable
{
    private int _textIndex;
    private float _timer;
    public GooberDialogueSpeaker(SpeechBubbleWrapper speechBubbleWrapper)
    {
        SpeechBubble = speechBubbleWrapper;
        timeBetweenTexts = 3;
        _timer = 0;
        talkingSound = AssetReferences.Assets.Sounds.AssassinsKnifeHit.Asset;
        isActive = true;
    }
    public bool isActive;
    public float timeBetweenTexts;
    public SoundStyle talkingSound;
    public readonly SpeechBubbleWrapper SpeechBubble;
    public bool IsActive => isActive;
    public bool IsFinishedTyping()
    {
        return _textIndex > SpeechBubble.Bubble.parameters.text.Length;
    }

    public void Reset()
    {
        _timer = 0;
        _textIndex = 0;
    }

    public void Update()
    {
        //  isActive = false;
        if (!IsFinishedTyping())
        {
            _timer++;
            if (_timer >= timeBetweenTexts)
            {
                SpeechBubble.Bubble.parameters.textIndex = _textIndex;
                _textIndex++;
                _timer = 0;
                if (_textIndex % 3 == 0)
                    SoundEngine.PlaySound(talkingSound);
            }
        }
        else
        {
            isActive = false;

        }

    }
}
