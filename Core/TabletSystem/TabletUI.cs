using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Common.UI;
using Stellamod.Helpers;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace Stellamod.Core.TabletSystem
{
    public class TabletUI : UIPanel
    {
        public int RelativeLeft => Main.screenWidth / 2 - (int)Width.Pixels / 2;
        public int RelativeTop => Main.screenHeight / 2 - (int)Height.Pixels / 2;

        public Asset<Texture2D> TabletCardTexture;
        public Asset<Texture2D> InnerTexture;
        public UIText Title;
        public UIText Text;
        public CommonBackButton backButton;
        public Color TabletColor;
        public Vector2 DrawOffset;
        public float Alpha;
        public TabletUI() : base()
        {
            Alpha = 0f;
            TabletCardTexture = ModContent.Request<Texture2D>(this.GetType().DirectoryHere() + "/TabletCard");
            Text = new UIText("This is placeholder text");
            Title = new UIText("This is placeholder text", large: true);
        }

        public override void OnInitialize()
        {
            base.OnInitialize();
            Width.Pixels = 660;
            Height.Pixels = 492;
            Left.Pixels = RelativeLeft + DrawOffset.X;
            Top.Pixels = RelativeTop + DrawOffset.Y;
            TabletColor = Color.White;
            BackgroundColor = Color.Transparent;
            BorderColor = Color.Transparent;
            Title.HAlign = 0.5f;
            Title.Top.Pixels = 16;
            Text.Width.Pixels = Width.Pixels - 64;
            Text.Height = Height;
            Text.MarginLeft = 16;
            Text.MarginRight = 16;
            Text.DynamicallyScaleDownToWidth = true;
            Text.Top.Set(0, 0.75f);
            Text.IsWrapped = true;


            Title.TextColor = TabletColor * Alpha;
            Text.TextColor = TabletColor * Alpha;
            Append(Text);
            Append(Title);

            backButton = new CommonBackButton(ModContent.GetInstance<TabletUISystem>().CloseUI);
            backButton.alpha = Alpha;
            backButton.asXButton = true;
            backButton.Left.Pixels = Width.Pixels - 48;
            Append(backButton);
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
            Title.TextColor = TabletColor * Alpha;
            Text.TextColor = TabletColor * Alpha;
            backButton.alpha = Alpha;

            bool contains = ContainsPoint(Main.MouseScreen);
            if (contains && !PlayerInput.IgnoreMouseInterface)
            {
                Main.LocalPlayer.mouseInterface = true;
            }
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
          //  base.DrawSelf(spriteBatch);
            CalculatedStyle dimensions = GetDimensions();
            Point point = new Point((int)dimensions.X, (int)dimensions.Y);
            Rectangle drawRectangle = new Rectangle(point.X, point.Y,
                TabletCardTexture.Value.Width, TabletCardTexture.Value.Height);
            drawRectangle.Location += new Point(0, (int)VectorHelper.Osc(-2f, 2f));


            Color backingColor = Color.Lerp(Color.White, Color.Black, 0.75f) * Alpha;
            Color frontColor = Color.White * Alpha;
            spriteBatch.Draw(InnerTexture.Value, drawRectangle, null, frontColor);
            spriteBatch.Draw(TabletCardTexture.Value, drawRectangle, null, backingColor);
        }
    }
}
