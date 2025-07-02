using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Core.Helpers;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace Stellamod.Core.TabletSystem
{
    internal class TabletUI : UIPanel
    {
        internal int RelativeLeft => Main.screenWidth / 2 - (int)Width.Pixels / 2;
        internal int RelativeTop => Main.screenHeight / 2 - (int)Height.Pixels / 2;

        public Asset<Texture2D> TabletCardTexture;
        public Asset<Texture2D> InnerTexture;
        public UIText Title;
        public UIText Text;
        public Color TabletColor;
        public Vector2 DrawOffset;
        internal TabletUI() : base()
        {
            TabletCardTexture = ModContent.Request<Texture2D>(this.MyDirectory() + "/TabletCard");
            Text = new UIText("This is placeholder text");
            Title = new UIText("This is placeholder text", large: true);
        }

        public override void OnInitialize()
        {
            base.OnInitialize();
            Width.Pixels = 660;
            Height.Pixels = 492;
            TabletColor = Color.White;
            BackgroundColor = Color.Transparent;
            BorderColor = Color.Transparent;
            Title.HAlign = 0.5f;

            Text.Width.Pixels = Width.Pixels - 64;
            Text.Height = Height;
            Text.MarginLeft = 16;
            Text.MarginRight = 16;
            Text.DynamicallyScaleDownToWidth = true;
            Text.Top.Set(0, 0.75f);
            Text.IsWrapped = true;
            Append(Text);
            Append(Title);
        }


        public override void OnDeactivate()
        {
            base.OnDeactivate();
            if (!Main.gameMenu)
            {
                SoundEngine.PlaySound(SoundID.MenuClose);
            }
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            Left.Pixels = RelativeLeft + DrawOffset.X;
            Top.Pixels = RelativeTop + DrawOffset.Y;


            Title.TextColor = TabletColor;
            Text.TextColor = TabletColor;
            bool contains = ContainsPoint(Main.MouseScreen);
            if (contains && !PlayerInput.IgnoreMouseInterface)
            {
                Main.LocalPlayer.mouseInterface = true;
            }
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);
            CalculatedStyle dimensions = GetDimensions();
            Point point = new Point((int)dimensions.X, (int)dimensions.Y);
            Rectangle drawRectangle = new Rectangle(point.X, point.Y,
                TabletCardTexture.Value.Width, TabletCardTexture.Value.Height);
            drawRectangle.Location += new Point(0, (int)VectorHelper.Osc(-4f, 4f));

            spriteBatch.Draw(InnerTexture.Value, drawRectangle, null, TabletColor);
            spriteBatch.Draw(TabletCardTexture.Value, drawRectangle, null, TabletColor);

        }
    }
}
