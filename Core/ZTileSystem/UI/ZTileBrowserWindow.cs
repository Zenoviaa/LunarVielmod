using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ReLogic.Content;
using Stellamod.Common.ItemBrowser;
using Stellamod.Core.Tooltips;
using Stellamod.Core.Utilities;
using Stellamod.Helpers;
using Stellamod.UI;
using System;
using System.Reflection;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace Stellamod.Core.ZTileSystem.UI;

/// <summary>
/// The full window of the item browser
/// </summary>
public class ZTileBrowserWindow : UIPanel
{
    private UIScrollbar _scrollbar;
    private ZTileBrowserMenu _inventoryMenu;
    private ItemBrowserTabMenu _tabMenu;
    private UIInputTextField _textBox;
    static ZTileBrowserWindow()
    {
        // Don't run this on the server
    }


    public ZTileBrowserWindow() : base()
    {
        _scrollbar = new FancyScrollbar();
        _inventoryMenu = new ZTileBrowserMenu(_scrollbar);
        _textBox = new UIInputTextField("Search...");
    }

    public string SearchFilter => _textBox.Text;
    public int RelativeLeft => ScreenHelper.TrueScreenWidth / 2 - (int)Width.Pixels / 2;
    public int RelativeTop => ScreenHelper.TrueScreenHeight / 2 - (int)Height.Pixels / 2;

    public override void OnInitialize()
    {
        base.OnInitialize();
        Width.Pixels = 512;
        Height.Pixels = 384;
        Left.Pixels = RelativeLeft;
        Top.Pixels = RelativeTop;
        BackgroundColor = Color.Transparent;
        BorderColor = Color.Transparent;


        _inventoryMenu.HAlign = 0.5f;
        _inventoryMenu.VAlign = 0.5f;
        Append(_inventoryMenu);
       // Append(_xButton);

        //Scrollbar
        _scrollbar.Width.Set(20, 0);
        _scrollbar.Height.Set(340, 0);
        _scrollbar.Left.Set(0, 0.95f);
        _scrollbar.Top.Set(0, 0f);

        float maxViewSize = 48 * 8f;
        _scrollbar.SetView(0, maxViewSize);
        Append(_scrollbar);

        _textBox.HAlign = 0.5f;
        _textBox.VAlign = 0.1f;
        _textBox.Width.Pixels = 128;
        Append(_textBox);
        SetPos();
    }

    public override void OnDeactivate()
    {
        base.OnDeactivate();
        if (!Main.gameMenu)
        {
            SoundEngine.PlaySound(SoundID.MenuClose);
        }
    }

    public void ResetCategories()
    {
        _tabMenu.SetCategory(null);
    }

    private void SetPos()
    {
        Width.Pixels = 512;
        Height.Pixels = 384;
        Left.Pixels = _pos.X;
        Top.Pixels = _pos.Y;
        _textBox.VAlign = 0f;
        _inventoryMenu.HAlign = 0.5f;
        _inventoryMenu.VAlign = 0.25f;

    }
    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        _scrollbar.Left.Set(0, 0.98f);
        _scrollbar.Top.Set(0, 0.1f);
        //Constantly lock the UI in the position regardless of resolution changes
        _inventoryMenu.SetSearchFilter(SearchFilter);
        SetPos();
    }


    private bool _isDragging;
    private Vector2? _drag = null;
    private Vector2 _pos;
    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        base.DrawSelf(spriteBatch);

        //Draw the back panel thing zemmie said to add
        Rectangle rectangle = GetDimensions().ToRectangle();
        Utils.DrawInvBG(spriteBatch, rectangle, new Color(23, 25, 81, 255) * 0.925f);


        this.QuickMouseInteraction();
        var config = ModContent.GetInstance<LunarVeilClientConfig>();
        Vector2 ratioPos = new Vector2(config.EnchantmentMenuX, config.EnchantmentMenuY);
        if (ratioPos.X < 0f || ratioPos.X > 100f)
        {
            ratioPos.X = 50;
        }

        if (ratioPos.Y < 0f || ratioPos.Y > 100f)
        {
            ratioPos.Y = 3;
        }

        Vector2 drawPos = ratioPos;
        _pos.X = drawPos.X = (int)(drawPos.X * 0.01f * Main.screenWidth);
        _pos.Y = drawPos.Y = (int)(drawPos.Y * 0.01f * Main.screenHeight);

        Rectangle mouseRect = new Rectangle((int)Main.MouseScreen.X, (int)Main.MouseScreen.Y, 8, 8);
        Vector2 size = new Vector2(Width.Pixels, Height.Pixels / 8);
        Rectangle barRect = Utils.CenteredRectangle(drawPos + size / 2, size * Main.UIScale);

        MouseState ms = Mouse.GetState();
        Vector2 mousePos = Main.MouseScreen;
        Vector2 newScreenRatioPosition = ratioPos;
        if (ms.LeftButton == ButtonState.Pressed && !_isDragging && barRect.Intersects(mouseRect))
        {
            _isDragging = true;
        }

        //Handle dragging
        if (_isDragging)
        {

            if (!_drag.HasValue)
                _drag = mousePos - drawPos;

            Vector2 newCorner = mousePos - _drag.GetValueOrDefault(Vector2.Zero);
            // Convert the new corner position into a screen ratio position.
            newScreenRatioPosition.X = (100f * newCorner.X) / Main.screenWidth;
            newScreenRatioPosition.Y = (100f * newCorner.Y) / Main.screenHeight;

            // Compute the change in position. If it is large enough, actually move the meter
            Vector2 delta = newScreenRatioPosition - ratioPos;
            if (Math.Abs(delta.X) >= 0.05f || Math.Abs(delta.Y) >= 0.05f)
            {
                config.EnchantmentMenuX = newScreenRatioPosition.X;
                config.EnchantmentMenuY = newScreenRatioPosition.Y;
            }

            if (ms.LeftButton == ButtonState.Released)
            {
                _isDragging = false;
                _drag = null;
                MethodInfo saveMethodInfo = typeof(ConfigManager).GetMethod("Save", BindingFlags.Static | BindingFlags.NonPublic);
                if (saveMethodInfo is not null)
                    saveMethodInfo.Invoke(null, new object[] { config });
            }
        }
    }
}
