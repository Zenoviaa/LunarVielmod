using Stellamod.Common.BossBannerSystem;
using Stellamod.Core;
using Terraria;
using Terraria.GameInput;
using Terraria.ModLoader;
using Terraria.UI;

namespace Stellamod.UI.CollectionSystem;

public class BookIcon : UIElement
{
    private int ElementWidth => 60;
    private int ElementHeight => 76;
    public BookIcon()
    {
        Width.Set(ElementWidth, 0f);
        Height.Set(ElementHeight, 0f);
        OnLeftClick += OnButtonClick;
        OnMouseOver += OnMouseHover;
    }

    private void OnButtonClick(UIMouseEvent evt, UIElement listeningElement)
    {
        CollectionBookUISystem uiSystem = ModContent.GetInstance<CollectionBookUISystem>();
        uiSystem.ToggleUI();
    }

    private void OnMouseHover(UIMouseEvent evt, UIElement listeningElement)
    {

    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        bool contains = ContainsPoint(Main.MouseScreen);
        if (contains && !PlayerInput.IgnoreMouseInterface)
        {
            Main.LocalPlayer.mouseInterface = true;
        }
    }

    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        var dimensions = GetDimensions();
        var point = new Point((int)dimensions.X, (int)dimensions.Y);
        var rect = new Rectangle(point.X, point.Y, ElementWidth, ElementHeight);
        rect.Location += new Point(0, (int)VectorHelper.Osc(-8f, 8f, 1f));

        var bookDrawer = SpritebatchDrawer.FromTextureAsset(AssetReferences.UI.CollectionSystem.BookIcon.Asset, Main.screenPosition);
        bookDrawer.worldPosition += rect.Location.ToVector2();
        bookDrawer.color = Color.White;
        bookDrawer.VerticalFrame(0, 2);
        bookDrawer.drawOrigin = Vector2.Zero;
        spriteBatch.Draw(bookDrawer);

        var outline = false;
        if (BossPage.HasAnyUnclaimedRewards(Main.LocalPlayer))
        {  
            bookDrawer.color = Main.DiscoColor;
            outline = true;
        }
        outline |= IsMouseHovering;
        if (outline)
        {
            bookDrawer.VerticalFrame(1, 2);
            spriteBatch.Draw(bookDrawer);
        }
    }
}
