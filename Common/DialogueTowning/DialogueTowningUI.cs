using Stellamod.Common.Shaders;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.UI.Chat;

namespace Stellamod.Common.DialogueTowning;

public enum DialogueBoxState : byte
{
    Speaking = 0,
    Shrinking = 1,
    WaitingForTalk = 2,
    Expanding = 3,

}
public class DialogueTowningUI : UIPanel
{
    private string _localizedText;
    private float _timer;
    private float _stateTimer;
    private float _nextCharInScale;
    private float _scale;
    private int _textIndex;
    private DialogueBoxState _state;

    public int RelativeLeft => Main.screenWidth / 2 - 76;
    public int RelativeTop => Main.screenHeight - 220;
    public Vector2 DrawPos => new Vector2(Left.Pixels, Top.Pixels);
    public float TimeBetweenTexts { get; set; } = 0.015f;

    public float DefaultTextSpeed => 0.025f;
    public string LocalizedText
    {
        get
        {
            return _localizedText;
        }
        set
        {
            _localizedText = value;
        }
    }

    public SpeechBoxTalkingParameters TalkingParameters
    {
        get;
        set
        {
            field = value;
            _localizedText = field.text;
        }
    }

    public Vector2 Offset { get; set; }

    public float Duration { get; set; }
    public float Alpha { get; set; }

    public float ScaleTime => 0.5f;
    public override void OnInitialize()
    {
        base.OnInitialize();
        _state = DialogueBoxState.Expanding;
        Width.Pixels = 700;
        Height.Pixels = 200;
        Left.Pixels = RelativeLeft;
        Top.Pixels = RelativeTop;
        BackgroundColor = Color.Transparent;
        BorderColor = Color.Transparent;
        Duration = 0.5f;

    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        //Constantly lock the UI in the position regardless of resolution changes
        Left.Pixels = RelativeLeft - Width.Pixels / 2;
        Top.Pixels = RelativeTop;
        switch (_state)
        {
            case DialogueBoxState.Speaking:
                AI_Speaking(gameTime);
                break;
            case DialogueBoxState.Expanding:
                AI_Expanding(gameTime);
                break;
            case DialogueBoxState.WaitingForTalk:
                AI_WaitingForTalk(gameTime);
                break;
            case DialogueBoxState.Shrinking:
                AI_Shrinking(gameTime);
                break;
        }
    }


    public void PrepareForTalkingOptions()
    {
        if (_state == DialogueBoxState.Shrinking || _state == DialogueBoxState.WaitingForTalk)
            return;

        SwitchState(DialogueBoxState.Shrinking);
    }
    public void PrepareForTalking()
    {
        if (_state == DialogueBoxState.Speaking || _state == DialogueBoxState.Expanding)
            return;

        SwitchState(DialogueBoxState.Expanding);
    }
    private void SwitchState(DialogueBoxState state)
    {
        _state = state;
        _stateTimer = 0;
    }

    private void AI_Shrinking(GameTime gameTime)
    {
        _stateTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
        float progress = _stateTimer / ScaleTime;
        float easing = EasingFunction.BezierEase(progress, new Vector2(0.8f, -0.4f), new Vector2(0.5f, 1f));
        _scale = MathHelper.Lerp(1f, 0f, easing);
        if (_stateTimer >= ScaleTime)
        {
            SwitchState(DialogueBoxState.WaitingForTalk);
        }
    }


    private void AI_WaitingForTalk(GameTime gameTime)
    {

    }


    private void AI_Expanding(GameTime gameTime)
    {
        _stateTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
        float progress = _stateTimer / ScaleTime;
        float easing = EasingFunction.BezierEase(progress, new Vector2(0.8f, -0.4f), new Vector2(0.5f, 1f));
        _scale = MathHelper.Lerp(0f, 1f, easing);
        if (_stateTimer >= ScaleTime)
        {
            SwitchState(DialogueBoxState.Speaking);
        }
    }

    private void AI_Speaking(GameTime gameTime)
    {
        _scale = 1f;
        if (!IsFinishedTyping())
        {
            _timer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (_timer >= DefaultTextSpeed)
            {
                _textIndex++;
                _timer = 0;
                if (_textIndex % 3 == 0 && TalkingParameters.profile.talkingSound.HasValue)
                    SoundEngine.PlaySound(TalkingParameters.profile.talkingSound.Value);
            }
            _nextCharInScale = _timer / DefaultTextSpeed;
        }
        else
        {
            _nextCharInScale = 0;
        }
    }

    private void DrawBackground(SpriteBatch spriteBatch)
    {
        //This is where the box is drawn
        spriteBatch.EndOut(out var oldParameters);
        var box = DialogueTownRenderer.RenderSpeechWindow(TalkingParameters with
        {
            size = new Vector2(1600, 252)
        });

        spriteBatch.Begin(oldParameters);
        var drawer = SpritebatchDrawer.FromTextureAsset(box, Main.screenPosition);
        drawer.drawOrigin = Vector2.Zero;
        drawer.worldPosition -= new Vector2(64);
        drawer.worldPosition += DrawPos;
        drawer.worldPosition += Offset;
        drawer.color = Color.White;
        drawer.color *= Alpha;
        spriteBatch.Draw(drawer);
    }

    private void DrawPortrait(SpriteBatch spriteBatch)
    {
        //Can't draw a portrait that doesn't exist.
        if (TalkingParameters.profile.bigPortraitTextureAsset == null)
            return;

        //Need to drawn with point clamp to look cleaner
        
        var spriteWhite = ShaderContent.GetInstance<SpriteWhiteShader>();
        var pos2 = DrawPos;
        pos2.Y += 6;
        pos2.Y += ExtraMath.Osc(-4, 4, 2);
        pos2.X += 8;
        pos2.X -= 100;
        pos2.Y -= 44;
        using (new SpritebatchContext(spriteBatch, spriteBatch.Parameters with { samplerState = SamplerState.PointClamp, effect = spriteWhite }))
        {
            Texture2D texture = TalkingParameters.profile.bigPortraitTextureAsset.Value;
            Vector2 drawPos = pos2;
            drawPos.Y -= 6;
            drawPos.X -= 8;
            Vector2 startDrawPos = drawPos;
            Vector2 endDrawPos = startDrawPos;

            Vector2 finalDrawPos = Vector2.Lerp(startDrawPos, endDrawPos, VectorHelper.Osc(0f, 1f, speed: 1f));
 
            finalDrawPos += Offset;
            finalDrawPos.Y = MathF.Floor(finalDrawPos.Y);
            finalDrawPos.X = MathF.Floor(finalDrawPos.X);
            float rotation = 0;
            Vector2 drawOrigin = new Vector2(0, 0);
            float drawScale = 1f;
            foreach (var offset in TextHelper.ShadowOffsets)
            {
                var pos = finalDrawPos + offset * 2;
           //     spriteBatch.Draw(texture, pos, null, Color.White * Alpha, rotation, drawOrigin, drawScale, SpriteEffects.None, 0);
            }

        }
        
        using (new SpritebatchContext(spriteBatch, spriteBatch.Parameters with { samplerState = SamplerState.PointClamp }))
        {
            Texture2D texture = TalkingParameters.profile.bigPortraitTextureAsset.Value;
            Vector2 drawPos = pos2;
            drawPos.Y -= 6;
            drawPos.X -= 8;
            Vector2 startDrawPos = drawPos;
            Vector2 endDrawPos = startDrawPos;

            Vector2 finalDrawPos = Vector2.Lerp(startDrawPos, endDrawPos, VectorHelper.Osc(0f, 1f, speed: 1f));
       
            finalDrawPos += Offset;
            finalDrawPos.Y = MathF.Floor(finalDrawPos.Y);
            finalDrawPos.X = MathF.Floor(finalDrawPos.X);
            float rotation = 0;
            Vector2 drawOrigin = new Vector2(0, 0);
            float drawScale = 1f;

            spriteBatch.Draw(texture, finalDrawPos, null, Color.White * Alpha, rotation, drawOrigin, drawScale, SpriteEffects.None, 0);
        }

    }


    private bool IsFinishedTyping()
    {
        var whiteSpaceCount = 0;
        for(var i = 0; i < LocalizedText.Length; i++)
        {
            if (LocalizedText[i] == ' ')
                whiteSpaceCount++;
        }

        var length = LocalizedText.Length;
        length -= whiteSpaceCount;
        return _textIndex > length;
    }

    public void ResetText()
    {
        _textIndex = 0;
    }
    public void ClearText()
    {
        _textIndex = 0;
    }

    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        base.DrawSelf(spriteBatch);

        DrawBackground(spriteBatch);
        DrawPortrait(spriteBatch);
        if (string.IsNullOrEmpty(LocalizedText))
            return;
        Rectangle ret = GetDimensions().ToRectangle();
        Vector2 pos = ret.TopLeft();
        pos.X += 100;

        var snippets = TextHelper.ParseMessage(FontAssets.DeathText.Value, LocalizedText, pos, Vector2.One * 0.5f, lineSpacing: 65, maxWidth: 650);
        for(int i = 0; i < snippets.Length; i++)
        {
            if(i > _textIndex)
            {
                snippets[i].characterColor = Color.Transparent;
            }
            else
            {
                snippets[i].characterColor = Color.White;
            }
        }

        if(_textIndex < snippets.Length)
        {
            ref var snipper = ref snippets[_textIndex];
            snipper.characterColor = Color.Lerp(Color.White * 0f, Color.White, EasingFunction.OutExpo(_nextCharInScale));
            snipper.scale *= MathHelper.Lerp(1.8f, 1f, _nextCharInScale);
        }
        TextHelper.DrawStringIndividually(FontAssets.DeathText.Value, spriteBatch, snippets);
    }
}
