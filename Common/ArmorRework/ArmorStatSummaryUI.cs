using Stellamod.Core.Tooltips;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;

namespace Stellamod.Common.ArmorRework
{
    /// <summary>
    /// Creates a state inspector for a piece of armor
    /// </summary>
    public class ArmorStatSummaryUI : UIPanel
    {
        private bool _setBonusActive;
        private UIText _summaryText;
        private List<TooltipLine> _lines;
        public ArmorStatSummaryUI() : base()
        {
            _summaryText = new UIText("Maidenless...");
        }
        public override void OnInitialize()
        {
            base.OnInitialize();
            _summaryText.Width.Pixels = Width.Pixels;
            _summaryText.Height.Pixels = Height.Pixels;
            Width.Pixels = 384;
            Height.Pixels = 128;
            BackgroundColor = Color.Transparent;
            BorderColor = Color.Transparent;
            Append(_summaryText);
        }

        public float alpha;
        public void SetTooltips(List<TooltipLine> statLines)
        {
            _lines = statLines;
        }

        public void SetTooltips(List<TooltipLine> statLines, string setBonus)
        {
            _lines = statLines;
            _summaryText.Width.Pixels = Width.Pixels - 100;
            _summaryText.Height.Pixels = Height.Pixels;
            _summaryText.IsWrapped = true;


            Vector2 topLeft = GetDimensions().ToRectangle().TopLeft();
  
            Rectangle rectangle = ExpandableTooltip.GetBGRectangle(statLines, (int)topLeft.X, (int)topLeft.Y, 1);
            _summaryText.Top.Pixels = rectangle.Height;
      
            _summaryText.SetText(setBonus);
        }
        public void SetTooltips(List<TooltipLine> statLines, string setBonus, bool setBonusActive)
        {
            _setBonusActive = setBonusActive;
            _lines = statLines;
            _summaryText.Width.Pixels = Width.Pixels - 100;
            _summaryText.Height.Pixels = Height.Pixels;
            _summaryText.IsWrapped = true;


            Vector2 topLeft = GetDimensions().ToRectangle().TopLeft();

            Rectangle rectangle = ExpandableTooltip.GetBGRectangle(statLines, (int)topLeft.X, (int)topLeft.Y, 1);
            _summaryText.Top.Pixels = rectangle.Height;

            _summaryText.SetText(setBonus);
   
        }


        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);
            if (_lines == null)
                return;

            _summaryText.TextColor = _setBonusActive ? Color.Green * alpha : Color.Lerp(Color.White, Color.Black, 0.75f) * alpha;
            Vector2 topLeft = GetDimensions().ToRectangle().TopLeft();
            Vector2 position = new Vector2(0, 0);
            position += topLeft;


            int height = (int)_summaryText.MinHeight.Pixels;
            Rectangle tooltipRectangle = ExpandableTooltip.GetBGRectangle(_lines, (int)topLeft.X, (int)topLeft.Y, alpha);
            Rectangle rectangle = ExpandableTooltip.GetBGRectangle((int)position.X, (int)position.Y + (int)_summaryText.Top.Pixels, (int)_summaryText.Width.Pixels, height);

            Rectangle combinedRectangle = tooltipRectangle;
            combinedRectangle.Width =Math.Max(rectangle.Width, tooltipRectangle.Width);
            combinedRectangle.Height = (int)Parent.Height.Pixels;
            Utils.DrawInvBG(spriteBatch, combinedRectangle, new Color(23, 25, 81, 255) * 0.925f * alpha);

            ExpandableTooltip.DrawExpandableTooltip(spriteBatch, _lines, (int)topLeft.X, (int)topLeft.Y, alpha, false, Main.LocalPlayer.GetModPlayer<ArmorStatsPlayer>().RequestIconTexture);

        }
    }
}
