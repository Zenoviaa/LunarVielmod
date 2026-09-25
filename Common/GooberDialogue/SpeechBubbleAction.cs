using Stellamod.Core.Camera;
using Terraria;

namespace Stellamod.Common.GooberDialogue;

public record class SpeechBubbleAction(
    SpeakerParameters Speaker) : ACutsceneAction
{
    private SpeechBubble _bubble;

    private Entity GetParent()
    {
        if (Speaker.speakerNpcWhoAmI != -1)
            return Main.npc[Speaker.speakerNpcWhoAmI];
        return null;
    }
    public override void Update(float elapsedTime)
    {
        base.Update(elapsedTime);
        var speaker = Speaker;
        Entity parent = GetParent();
        if (parent != null)
        {
            speaker.bubblePosition = parent.TopRight + new Vector2(18, -18);
            CameraTargetSystem.AddTarget(parent.Center);
        }
        _bubble = GooberDialogueSystem.GetSpeechBubble();
        _bubble.speaker = speaker;
        if (IsFinishedSpeaking(elapsedTime))
            _bubble.showArrow = true;
        GooberDialogueUtility.EvaluateSpeaking(
            new SpeakingData(_bubble, elapsedTime, Speaker.profile.timeBetweenTexts, Speaker.profile.talkingSound));
    }

    public bool IsFinishedSpeaking(float elapsedTime)
    {
     //   Main.NewText(_bubble);
        return GooberDialogueUtility.IsFinishedSpeaking(
            new SpeakingData(_bubble, elapsedTime, Speaker.profile.timeBetweenTexts, Speaker.profile.talkingSound));
    }

    public override bool IsComplete(float elapsedTime)
    {
        return CutsceneHandler.CutsceneHost.ShouldProgressCutscene && IsFinishedSpeaking(elapsedTime);
    }
}

