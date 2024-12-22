using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.UI;

namespace Stellamod.UI
{
    internal class UIInputTextField : UITextPanel<string>
    {
        internal bool focused = false;
        private int _cursor;
        private int _frameCount;
        private int _maxLength = 60;
        private string _mText;
        public UIInputTextField(string text, float textScale = 1, bool large = false) : base("", textScale, large)
        {
            _mText = text;
            SetPadding(4);
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            Focus();
            base.LeftClick(evt);
        }

        public void Unfocus()
        {
            if (focused)
            {
                focused = false;
                Main.blockInput = false;
            }
        }

        public void Focus()
        {
            if (!focused)
            {
                Main.clrInput();
                focused = true;
                Main.blockInput = true;
            }
        }

        public override void Update(GameTime gameTime)
        {
            Vector2 MousePosition = new Vector2((float)Main.mouseX, (float)Main.mouseY);
            if (!ContainsPoint(MousePosition) && Main.mouseLeft)
            {
                Unfocus();
            }
            bool contains = ContainsPoint(Main.MouseScreen);
            if (contains && !PlayerInput.IgnoreMouseInterface)
            {
                Main.LocalPlayer.mouseInterface = true;
            }
            base.Update(gameTime);
        }

        public void WriteAll(string text)
        {
            bool changed = text != Text;
            base.SetText(text);
            this._cursor = text.Length;
            Recalculate();
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Rectangle hitbox = GetDimensions().ToRectangle();

            Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, hitbox, Color.Lerp(Color.CadetBlue, Color.Black, 0.4f) * 0.75f);
            if (focused)
            {
                Terraria.GameInput.PlayerInput.WritingText = true;
                Main.instance.HandleIME();
                WriteAll(Main.GetInputText(Text));
            }

            CalculatedStyle innerDimensions2 = base.GetInnerDimensions();
            Vector2 pos2 = innerDimensions2.Position();
            if (IsLarge)
            {
                pos2.Y -= 10f * TextScale * TextScale;
            }
            else
            {
                pos2.Y -= 2f * TextScale;
            }

            if (IsLarge)
            {
                Utils.DrawBorderStringBig(spriteBatch, Text, pos2, TextColor, TextScale, 0f, 0f, -1);
                return;
            }
            Utils.DrawBorderString(spriteBatch, Text, pos2, TextColor, TextScale, 0f, 0f, -1);
            this._frameCount++;

            CalculatedStyle innerDimensions = base.GetInnerDimensions();
            Vector2 pos = innerDimensions.Position();
            DynamicSpriteFont spriteFont = base.IsLarge ? FontAssets.DeathText.Value : FontAssets.MouseText.Value;
            Vector2 vector = new Vector2(spriteFont.MeasureString(base.Text.Substring(0, this._cursor)).X, base.IsLarge ? 32f : 16f) * base.TextScale;
            if (base.IsLarge)
            {
                pos.Y -= 8f * base.TextScale;
            }
            else
            {
                pos.Y -= 1f * base.TextScale;
            }
            if (Text.Length == 0)
            {
                Vector2 size = new Vector2(spriteFont.MeasureString(_mText.ToString()).X, IsLarge ? 32f : 16f) * TextScale;
                pos.X += 5;
                if (base.IsLarge)
                {
                    Utils.DrawBorderStringBig(spriteBatch, _mText, pos, Color.Gray, base.TextScale, 0f, 0f, -1);
                    return;
                }
                Utils.DrawBorderString(spriteBatch, _mText, pos, Color.Gray, base.TextScale, 0f, 0f, -1);
                pos.X -= 5;
            }

            if (!focused)
                return;

            pos.X += vector.X - (base.IsLarge ? 8f : 4f) * base.TextScale + 6f;
            if ((this._frameCount %= 40) > 20)
            {
                return;
            }
            if (base.IsLarge)
            {
                Utils.DrawBorderStringBig(spriteBatch, "|", pos, base.TextColor, base.TextScale, 0f, 0f, -1);
                return;
            }
            Utils.DrawBorderString(spriteBatch, "|", pos, base.TextColor, base.TextScale, 0f, 0f, -1);
        }
    }
}

