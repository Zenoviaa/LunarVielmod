using ReLogic.Graphics;
using Stellamod.Core;
using Terraria;
using Terraria.Audio;
using Terraria.UI.Chat;

namespace Stellamod.Common.GooberDialogue;

public record struct SpeakingData(SpeechBubble Bubble, float ElapsedTime, float TimeBetweenTexts, SoundStyle? TalkingSound);
public class GooberDialogueUtility
{
    public static void EvaluateSpeaking(SpeakingData speakingData)
    {
        int textIndex = (int)(speakingData.ElapsedTime / speakingData.TimeBetweenTexts);
        if(textIndex % 3 == 0 && 
            textIndex < speakingData.Bubble.speaker.text.Length && 
            speakingData.TalkingSound.HasValue)
        {
            SoundEngine.PlaySound(speakingData.TalkingSound);
        }
       
        speakingData.Bubble.speaker.textIndex = textIndex;
    }
    public static bool IsFinishedSpeaking(SpeakingData speakingData)
    {
        int textIndex = (int)(speakingData.ElapsedTime / speakingData.TimeBetweenTexts);
        return textIndex >= speakingData.Bubble.speaker.text.Length ;
    }
    public static void DrawColorCodedStringShadowDoNotIgnoreColors(SpriteBatch spriteBatch, DynamicSpriteFont font, TextSnippet[] snippets, Vector2 position, Color baseColor, float rotation, Vector2 origin, Vector2 baseScale, float maxWidth = -1f, float spread = 2f)
    {
        var shadowDirections = ChatManager.ShadowDirections;
        for (int i = 0; i < shadowDirections.Length; i++)
        {
            ChatManager.DrawColorCodedString(spriteBatch, font, snippets, position + shadowDirections[i] * spread, baseColor, rotation, origin, baseScale, out var _, maxWidth, ignoreColors: false);
        }
    }
    public static Vector2 DrawColorCodedStringWithShadowDoNotIgnoreColors(SpriteBatch spriteBatch, DynamicSpriteFont font, string text, Vector2 position, Color baseColor, float rotation, Vector2 origin, Vector2 baseScale, float maxWidth = -1f, float spread = 2f)
    {
        TextSnippet[] snippets = ChatManager.ParseMessage(text, baseColor).ToArray();
        ChatManager.ConvertNormalSnippets(snippets);
        DrawColorCodedStringShadowDoNotIgnoreColors(spriteBatch, font, snippets, position, new Color(0, 0, 0, baseColor.A), rotation, origin, baseScale, maxWidth, spread);
        int hoveredSnippet;
        return ChatManager.DrawColorCodedString(spriteBatch, font, snippets, position, Color.White, rotation, origin, baseScale, out hoveredSnippet, maxWidth);
    }
}
