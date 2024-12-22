using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Helpers;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;

namespace Stellamod.UI.DialogueTowning
{
    public class DialogueTowningUI : UIPanel
    {
        private float _timer;
        private int _textIndex;
        private UIText _text;

        internal int RelativeLeft => Main.screenWidth / 2;
        internal int RelativeTop => Main.screenHeight - 300;
        internal Vector2 DrawPos => new Vector2(Left.Pixels, Top.Pixels);


        public float TimeBetweenTexts { get; set; } = 0.015f;
        public string LocalizedText { get; set; }
        public SoundStyle? TalkingSound { get; set; } = null;
        public Asset<Texture2D> Portrait { get; set; }
        public Vector2 Offset { get; set; }

        public float Duration { get; set; }
        public override void OnInitialize()
        {
            base.OnInitialize();
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
            float drawScale = 1f;
            Color drawColor = Color.White.MultiplyRGB(Color.Gray);
            spriteBatch.Draw(texture, drawPos, null, drawColor, rotation, drawOrigin, drawScale, SpriteEffects.None, 0);
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
            spriteBatch.Draw(texture, finalDrawPos, null, Color.White, rotation, drawOrigin, drawScale, SpriteEffects.None, 0);
        }

        private bool IsFinishedTyping()
        {
            return _textIndex > LocalizedText.Length;
        }

        public void ResetText()
        {

            _text.SetText(string.Empty);
            _textIndex = 0;
        }

        public void ResetText(string text, string portrait, float timeBetweenTexts = 0.015f, SoundStyle? talkingSound = null)
        {
            _text.SetText(string.Empty);
            _textIndex = 0;
            LocalizedText = LangText.TownDialogue(text);
            TalkingSound = talkingSound;
            Portrait = ModContent.Request<Texture2D>(DialogueTowningUISystem.RootTexturePath + portrait);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);
            DrawBackground(spriteBatch);
            DrawPortrait(spriteBatch);
        }
    }
}
