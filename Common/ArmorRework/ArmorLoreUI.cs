using Stellamod.Core.Tooltips;
using System;
using Terraria;
using Terraria.GameContent.UI.Elements;

namespace Stellamod.Common.ArmorRework
{
    /// <summary>
    /// Creates a lore inspector for a piece of armor
    /// </summary>
    public class ArmorLoreUI : UIPanel
    {
        private string _actualText;
        private UIText _loreText;
        public ArmorLoreUI() : base()
        {
            _loreText = new UIText("No Lore?");
        }

        public override void OnInitialize()
        {
            base.OnInitialize();
   
            Width.Pixels = 384;
            Height.Pixels = 128;
            BackgroundColor = Color.Transparent;
            BorderColor = Color.Transparent;
        
            _loreText.Width.Pixels = Width.Pixels;
            _loreText.Height.Pixels = Height.Pixels;
            _loreText.IsWrapped = true;
            _loreText.DynamicallyScaleDownToWidth = true;
            Append(_loreText);
        }
        public float alpha;
        public int minHeight;
        public void SetText(string text)
        {
            _actualText = text;

            //Calculate at full size
            //Then downscale if it goes over
            _loreText.SetText(_actualText, 1f, false);
            float num = 1f;
            float height = MathF.Min(_loreText.MinHeight.Pixels, 232);
            if (_loreText.MinHeight.Pixels > height)
                num = height / _loreText.MinHeight.Pixels;

            //This is a pretty hacky way to do it, and not really the best optimzied but it works so idc.
            //and this code doesn't need to be hyper efficient anyway
            //though i made a need
            _loreText.SetText(_actualText, num, false);
            int pixelHeight = (int)height;
            minHeight = pixelHeight;
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);
            _loreText.TextColor = Color.White * alpha;

            Vector2 position = GetDimensions().ToRectangle().TopLeft();
            Rectangle rectangle = ExpandableTooltip.GetBGRectangle((int)position.X, (int)position.Y, (int)Width.Pixels, minHeight);
            Utils.DrawInvBG(spriteBatch, rectangle, new Color(23, 25, 81, 255) * 0.925f * alpha);
        }
    }
}
