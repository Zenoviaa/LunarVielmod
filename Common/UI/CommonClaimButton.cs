using Stellamod.Core.Utilities;
using Stellamod.Helpers;
using System;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace Stellamod.Common.UI
{
    public class CommonClaimButton : UIPanel
    {
        private float _scale;
        private Action _closeFunction;
        private UIText _backText;
        public CommonClaimButton(Action closeFunction) : base()
        {
            _closeFunction = closeFunction;
            _backText = new UIText("Claim", large: true);
        }

        public bool alreadyClaimed;
        public bool notClaimed;
        public bool canClaim;
        public bool notBeaten;
        public override void OnInitialize()
        {
            base.OnInitialize();
            Width.Pixels = 160;
            Height.Pixels = 54;

            Append(_backText);
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            base.LeftClick(evt);
            _closeFunction();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            _backText.Width.Pixels = Width.Pixels;
            _backText.Height.Pixels = Height.Pixels;
            _backText.HAlign = 0.5f;
            _backText.SetText(LangText.Common("Claim"), _scale, true);
            _backText.TextColor = Color.White;

            Color faintColor = Color.White * 0.5f;
            if (alreadyClaimed)
            {
                _backText.SetText(LangText.Common("AlreadyClaimed"), _scale, true);
                _backText.TextColor = faintColor;
            }
            else if (notClaimed)
            {
                _backText.SetText(LangText.Common("NotClaimed"), _scale, true);
                _backText.TextColor = faintColor;
            }
            if (notBeaten)
            {
                _backText.SetText(LangText.Common("NotBeaten"), _scale, true);
                _backText.TextColor = faintColor;
            }
       
            BackgroundColor = Color.Lerp(Color.Blue, Color.Black, 1f) * 0f;
            BorderColor = Color.Lerp(Color.Purple, Color.Black, 0.8f) * 0f;
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);
            this.QuickMouseInteraction();
            if (IsMouseHovering)
            {
                _scale = MathHelper.Lerp(_scale, 0.8f, 0.3f);
            }
            else
            {
                _scale = MathHelper.Lerp(_scale, 0.5f, 0.3f);
            }
        }
    }
}
