using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Core.DialogueSystem;
using Stellamod.Helpers;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace Stellamod.UI.DialogueTowning
{
    public class TalkingOptionsButtonGroupUI : UIPanel
    {
        private int _index;
        private TalkingOptionButtonUI[] _buttons;
        public int RelativeLeft => Main.screenWidth / 2 - (int)(Width.Pixels / 2) + 80;
        public int RelativeTop => Main.screenHeight - 300;
        public Vector2 DrawPos => new Vector2(Left.Pixels, Top.Pixels);

        public const int Max_Dialogue_Options = 4;
        public TalkingOptionsButtonGroupUI()
        {
            _buttons = new TalkingOptionButtonUI[Max_Dialogue_Options];
            for (int i = 0; i < _buttons.Length; i++)
            {
                _buttons[i] = new TalkingOptionButtonUI();
            }
        }
        public Vector2 Offset { get; set; }
        public float Alpha { get; set; }

        public override void OnInitialize()
        {
            base.OnInitialize();
            Width.Pixels = 480;
            Height.Pixels = 200;
            Left.Pixels = RelativeLeft;
            Top.Pixels = RelativeTop;
            BackgroundColor = Color.Transparent;
            BorderColor = Color.Transparent;

            for (int i = 0; i < _buttons.Length; i++)
            {
                var btn = _buttons[i];
                Append(_buttons[i]);
            }
        }

        public void ClearButtons()
        {
            _index = 0;

        }

        public void AddButton(ITalkingOption talkingOption)
        {
            _buttons[_index].SetTalkingOption(talkingOption);
            _index++;
        }


        public override void Update(GameTime gameTime)
        {
            BackgroundColor = Color.Transparent;

            base.Update(gameTime);
            //Constantly lock the UI in the position regardless of resolution changes
            Left.Pixels = RelativeLeft;
            Top.Pixels = RelativeTop;
            for (int y = 0; y < _buttons.Length; y++)
            {
                var btn = _buttons[y];
                btn.Top.Pixels = y * 48;
                btn.Alpha = y < _index ? 1 : 0;
                btn.Alpha *= Alpha;
            }
        }
    }
    public class TalkingOptionButtonUI : UIPanel
    {
        private float _alpha;
        private float _timer;
        private ITalkingOption _talkingOption;
        public TalkingOptionButtonUI()
        {
            Text = new UIText("This is placeholder text", 0.5f, true);
        }

        public override void OnInitialize()
        {
            base.OnInitialize();
            BackgroundColor = Color.Transparent;
            BorderColor = Color.Transparent;

            Width.Pixels = 214;
            Height.Pixels = 44;

            Text.Width.Set(0, 1);
            Text.Height.Set(0, 1);
            Text.TextColor = Color.White;
            Text.IsWrapped = false;
            Text.Top.Pixels = 0;
            Append(Text);

            OnLeftClick += OnButtonClick;
            OnMouseOver += OnMouseHover;
        }

        public void SetTalkingOption(ITalkingOption talkingOption)
        {
            _talkingOption = talkingOption;
            Text.SetText(talkingOption.GetDisplayName());
        }
        public UIText Text { get; set; }
        public float Alpha { get; set; }
        private void OnButtonClick(UIMouseEvent evt, UIElement listeningElement)
        {
            if (_alpha < 0.8f)
                return;

            SoundStyle soundStyle = SoundID.MenuTick;
            SoundEngine.PlaySound(soundStyle);
            _talkingOption?.Talk();
        }

        private void OnMouseHover(UIMouseEvent evt, UIElement listeningElement)
        {

        }


        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            _alpha = MathHelper.Lerp(_alpha, Alpha, 0.1f);
            Text.TextColor = Color.White * _alpha;
            Text.ShadowColor = Color.Black * _alpha;


  
            bool contains = ContainsPoint(Main.MouseScreen);
            if (contains && !PlayerInput.IgnoreMouseInterface)
            {
                Main.LocalPlayer.mouseInterface = true;
            }
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {

            base.DrawSelf(spriteBatch);
            Color drawColor = Color.White.MultiplyRGB(Color.DarkGray);
            drawColor *= 0.2f;
  
            if (IsMouseHovering)
            {
                float progress = _timer / 0.12f;
                Color colorToMultiplyBy = Color.Lerp(Color.White, Color.LightGoldenrodYellow, progress);
                drawColor = drawColor.MultiplyRGB(colorToMultiplyBy);
            }
            CalculatedStyle dimensions = GetDimensions();
            Point point = new Point((int)dimensions.X, (int)dimensions.Y);
            Texture2D textureToDraw = ModContent.Request<Texture2D>($"{DialogueTowningUISystem.RootTexturePath}DialogueBoxButton").Value;
            drawColor *= _alpha;

            spriteBatch.Draw(textureToDraw, point.ToVector2(), null,
                drawColor, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0f);
            _talkingOption?.Show(spriteBatch);

        }
    }



    /// <summary>
    /// Interface for creation a dialogue option on the talk menu
    /// </summary>
    public interface ITalkingOption
    {
        string GetDisplayName();
        void Talk();
        void Show(SpriteBatch spriteBatch);
    }

    public class DialogueTalkingOption : ITalkingOption
    {
        public DialogueTalkingOption(string localizedText, BaseDialogue dialogue)
        {
            LocalizedText = localizedText;
            Dialogue = dialogue;
        }

        public readonly string LocalizedText;
        public readonly BaseDialogue Dialogue;

        public string GetDisplayName()
        {
            return LocalizedText;
        }

        public void Talk()
        {
            DialogueSystemV2 dialogueSystem = ModContent.GetInstance<DialogueSystemV2>();
            dialogueSystem.StartDialogueSequence(Dialogue);
        }

        public void Show(SpriteBatch spriteBatch)
        {

        }
    }

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
        private float _scale;
        private int _textIndex;
        private UIText _text;
        private DialogueBoxState _state;

        public int RelativeLeft => Main.screenWidth / 2;
        public int RelativeTop => Main.screenHeight - 300;
        public Vector2 DrawPos => new Vector2(Left.Pixels, Top.Pixels);


        public float TimeBetweenTexts { get; set; } = 0.015f;
        public string LocalizedText
        {
            get
            {
                return _localizedText;
            }
            set
            {
                _localizedText = value;
                ParseCommands(ref _localizedText);
            }
        }

        public SoundStyle? TalkingSound { get; set; } = null;
        public Asset<Texture2D> Portrait { get; set; }
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

            _text = new UIText("This is placeholder text", large: false);
            _text.Height.Pixels = Height.Pixels;
            _text.Width.Pixels = Width.Pixels - 200;
            _text.Left.Pixels = 180;
            _text.Top.Pixels = 16;
            _text.IsWrapped = true;
            _text.MinWidth = _text.Width;
            Append(_text);

        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            //Constantly lock the UI in the position regardless of resolution changes
            Left.Pixels = RelativeLeft - Width.Pixels / 2;
            Top.Pixels = RelativeTop;
            _text.Left.Pixels = 180 + Offset.X;
            _text.Top.Pixels = 16 + Offset.Y;
            _text.TextColor = Color.White * Alpha;
        
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
                if (_timer >= TimeBetweenTexts)
                {

                    string realText = LocalizedText.Substring(0, _textIndex);
                    //Set text to white space
                    for (int i = 0; i < 128; i++)
                    {
                        realText += " ";
                    }

                    _text.SetText(realText);
                    _textIndex++;
                    _timer = 0;
                    if (_textIndex % 3 == 0)
                        SoundEngine.PlaySound(TalkingSound);
                }
            }
        }

        private void DrawBackground(SpriteBatch spriteBatch)
        {
            Texture2D texture = ModContent.Request<Texture2D>(DialogueTowningUISystem.RootTexturePath + "DialogueBoxTalk").Value;
            Vector2 drawPos = DrawPos;
            drawPos += Offset;

            float rotation = 0;
            Vector2 drawOrigin = new Vector2(0, 0);
            Vector2 drawScale = Vector2.One;
            Color drawColor = Color.White.MultiplyRGB(Color.Gray);
            drawColor *= Alpha;

            //Ok so basically we're gonna draw a part of the texture to get a cool effect
            //For now let's test how this looks
            //Might do something with a render target actually
            int openWidth = 150;
            Rectangle openRectangle = new Rectangle(0, 0, openWidth, texture.Height);

            int closeWidth = (int)((texture.Width - openWidth) * _scale);
            closeWidth += (int)(376 * (1f-_scale));
            Rectangle closeRectangle = new Rectangle(texture.Width - closeWidth, 0, closeWidth, texture.Height);


            Vector2 edgeDrawPos = drawPos;
           // edgeDrawPos.X += 16;
            edgeDrawPos.X += openRectangle.Width;
            spriteBatch.Draw(texture, edgeDrawPos, closeRectangle, drawColor, rotation, drawOrigin, drawScale, SpriteEffects.None, 0);
            spriteBatch.Draw(texture, drawPos, openRectangle, drawColor, rotation, drawOrigin, drawScale, SpriteEffects.None, 0);
           
        }

        private void DrawPortrait(SpriteBatch spriteBatch)
        {
            //Can't draw a portrait that doesn't exist.
            if (Portrait == null)
                return;
            Texture2D texture = Portrait.Value;
            Vector2 drawPos = DrawPos;
            Vector2 startDrawPos = drawPos;
            Vector2 endDrawPos = startDrawPos + new Vector2(0, 8);
            Vector2 finalDrawPos = Vector2.Lerp(startDrawPos, endDrawPos, VectorHelper.Osc(0f, 1f, speed: 1f));
            finalDrawPos += Offset;

            float rotation = 0;
            Vector2 drawOrigin = new Vector2(0, 0);
            float drawScale = 1f;

            spriteBatch.Draw(texture, finalDrawPos, null, Color.White * Alpha, rotation, drawOrigin, drawScale, SpriteEffects.None, 0);
        }


        private bool IsFinishedTyping()
        {
            return _textIndex > LocalizedText.Length;
        }

        public void ParseCommands(ref string text)
        {
            if (string.IsNullOrEmpty(text))
                return;

            while (text.Contains("["))
            {
                int indexOfLeftBracket = text.IndexOf("[") + 1;
                int indexOfEqual = text.IndexOf('=', indexOfLeftBracket) + 1;
                string command = text.Substring(indexOfLeftBracket, indexOfEqual - indexOfLeftBracket - 1);

                int indexOfRightBracket = text.IndexOf(']', indexOfLeftBracket) + 1;
                string action = text.Substring(indexOfEqual, indexOfRightBracket - indexOfEqual - 1);


                switch (command)
                {
                    case "PORTRAIT":
                        PortraitType portraitType = PortraitLoader.NameToType(action);
                        Portrait = PortraitLoader.LoadPortrait(portraitType);
                        break;
                }

                text = text.Substring(indexOfRightBracket, text.Length - indexOfRightBracket);
            }
        }

        public void ResetText()
        {
            _text.SetText(string.Empty);
            _textIndex = 0;
        }
        public void ClearText()
        {
            _text.SetText(string.Empty);
          //  _textIndex = 200;
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);
            DrawBackground(spriteBatch);
            DrawPortrait(spriteBatch);
        }
    }
}
