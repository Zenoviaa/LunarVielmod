using ReLogic.Content;
using Stellamod.Common.UI;
using Stellamod.Helpers;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.UI.Chat;

namespace Stellamod.Core.TabletSystem;

public class TabletUI : UIPanel
{
    public int RelativeLeft => Main.screenWidth / 2 - (int)Width.Pixels / 2;
    public int RelativeTop => Main.screenHeight / 2 - (int)Height.Pixels / 2;

    public Asset<Texture2D> TabletCardTexture;
    public Asset<Texture2D> InnerTexture;
    public UIText Title;
    public CommonBackButton backButton;
    public Color TabletColor;
    public string helpText;
    public TabletUI() : base()
    {
        TabletCardTexture = ModContent.Request<Texture2D>(this.GetType().DirectoryHere() + "/TabletCard");
        Title = new UIText("This is placeholder text", large: true);
    }

    public override void OnInitialize()
    {
        base.OnInitialize();
        Width.Pixels = 660;
        Height.Pixels = 492;
        Left.Pixels = RelativeLeft;
        Top.Pixels = RelativeTop;// + DrawOffset.Y;
        TabletColor = Color.White;
        BackgroundColor = Color.Transparent;
        BorderColor = Color.Transparent;
        Title.HAlign = 0.5f;
        Title.Top.Pixels = 16;
        Title.TextColor = TabletColor;
        Append(Title);

        backButton = new CommonBackButton(ModContent.GetInstance<TabletUISystem>().CloseUI);
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
        Left.Pixels = RelativeLeft;// + DrawOffset.X;
        Top.Pixels = RelativeTop;// + DrawOffset.Y;

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


        Color backingColor = Color.Lerp(Color.White, Color.Black, 0.75f);
        Color frontColor = Color.White;
        spriteBatch.Draw(InnerTexture.Value, drawRectangle, null, frontColor);
        spriteBatch.Draw(TabletCardTexture.Value, drawRectangle, null, backingColor);
        Vector2 textPosition = drawRectangle.BottomLeft();
        ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, helpText,
               textPosition + new Vector2(24, -120) * 1f, Color.White, 0f, Vector2.Zero, Vector2.One, Width.Pixels - 54, 1f);
    }
}
