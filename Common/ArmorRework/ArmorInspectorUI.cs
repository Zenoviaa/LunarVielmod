using Stellamod.Core.Tooltips;
using Terraria;
using Terraria.GameContent.UI.Elements;

namespace Stellamod.Common.ArmorRework
{
    public class ArmorInspectorUI : UIPanel
    {
        public ArmorStatSummaryUI summaryUI;
        public ArmorLoreUI loreUI;
        public ArmorPreviewUI previewUI;
        public ArmorInspectorUI() : base()
        {
            summaryUI = new();
            loreUI = new();
            previewUI = new();
        }
        public float alpha;
        public override void OnInitialize()
        {
            base.OnInitialize();
            Width.Pixels = 750;
            Height.Pixels = 300;
            BackgroundColor = Color.Transparent;
            BorderColor = Color.Transparent;
            Append(summaryUI);
            Append(loreUI);
            Append(previewUI);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            Height.Pixels = 400;
          
            Vector2 mouseScreen = Main.MouseScreen;
            mouseScreen.X += 64;

            Vector2 targetPoint = mouseScreen;
            targetPoint.X -= Width.Pixels;
            if (targetPoint.X < 64)
                targetPoint.X = 64;
            Left.Pixels = MathHelper.Lerp(targetPoint.X + 128, targetPoint.X, alpha);

            Top.Pixels = mouseScreen.Y + 8;
            summaryUI.Left.Set(0, 0);
            summaryUI.Top.Set(0, 0);
            previewUI.Left.Set(-300, 1);
            previewUI.Top.Set(0, 0);
            loreUI.Left.Set(-loreUI.Width.Pixels - 32, 1);
            loreUI.Top.Set(-loreUI.minHeight, 1);
       
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);
            Vector2 topLeft = GetDimensions().ToRectangle().TopLeft();
            Vector2 position = topLeft;
            Rectangle rectangle = ExpandableTooltip.GetBGRectangle((int)position.X, (int)position.Y, (int)Width.Pixels, (int)Height.Pixels);
            Utils.DrawInvBG(spriteBatch, rectangle, new Color(23, 25, 81, 255) * 0.925f * alpha);
        }
    }
}
