using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.UI.Elements;

namespace Stellamod.Core.CollectionSystem.Medallion
{
    internal class DescriptionPageUI : UIPanel
    {
        private float _scale;
        internal const int width = 432;
        internal const int height = 155;

        internal int RelativeLeft => Main.screenWidth / 2 - width / 2 + 280;
        internal int RelativeTop => Main.screenHeight / 2 - height / 2 - 196;

        public DescriptionPageUI(float scale = 1f) : base()
        {
            _scale = scale;
            Description = new UIText(
                "This is placeholder text This is placeholder text This is placeholder text This is placeholder text This is placeholder text This is placeholder text This is placeholder text This is placeholder text This is placeholder text",
                textScale: 0.8f);
            BackgroundMiddle = new BackgroundDarkBigMiddle();
            //Set to air
            Glow = 1f;
        }

        public BackgroundDarkBigMiddle BackgroundMiddle { get; private set; }
        public UIText Description { get; set; }
        public float Glow { get; set; }
        public override void OnInitialize()
        {
            base.OnInitialize();
            Width.Pixels = 48 * 6f;
            Height.Pixels = 48 * 9;
            Left.Pixels = RelativeLeft;
            Top.Pixels = RelativeTop;
            BackgroundColor = Color.Transparent;
            BorderColor = Color.Transparent;
            Append(BackgroundMiddle);

            Description.Width.Pixels = Width.Pixels - 80;
            Description.IsWrapped = true;
            Description.MarginRight = 80;
            Append(Description);
        }


        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            //Constantly lock the UI in the position regardless of resolution changes
            Left.Pixels = RelativeLeft;
            Top.Pixels = RelativeTop;

            BackgroundMiddle.Left.Pixels = -24;
            BackgroundMiddle.Top.Pixels = 0;
            Description.Left.Pixels = -8;
            Description.Top.Pixels = 16;
            Glow *= 0.95f;
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);

        }
    }
}
