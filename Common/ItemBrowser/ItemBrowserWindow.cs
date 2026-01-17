using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ReLogic.Content;
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

namespace Stellamod.Common.ItemBrowser
{
    /// <summary>
    /// The full window of the item browser
    /// </summary>
    public class ItemBrowserWindow : UIPanel
    {
        private UIImage _backgroundSquare;
        private UIScrollbar _scrollbar;
        private UIScrollbar _sortingScrollbar;
        private XButton _xButton;
        private ItemBrowserMenu _inventoryMenu;
        private ItemBrowserTabMenu _tabMenu;
        private UIInputTextField _textBox;
        private ItemBrowserModFilterButton _modFilterButton;
        private readonly Asset<Texture2D> BackgroundSquareTexture;
        public ItemBrowserWindow() : base()
        {
            string texturePath = typeof(ItemBrowserWindow).DirectoryHere() + "/ItemBrowserMenu";
            BackgroundSquareTexture = ModContent.Request<Texture2D>(texturePath);
            _backgroundSquare = new UIImage(BackgroundSquareTexture)
            {
                HAlign = 0f,
                VAlign = 0f,
                AllowResizingDimensions = true,
                ScaleToFit = true,
            };

            _scrollbar = new FancyScrollbar();
            _sortingScrollbar = new FancyScrollbar();
            _xButton = new XButton(Close);
            _inventoryMenu = new ItemBrowserMenu(_scrollbar);
            _tabMenu = new ItemBrowserTabMenu(_inventoryMenu, _sortingScrollbar);
            _textBox = new UIInputTextField("Search...");
            _modFilterButton = new ItemBrowserModFilterButton(_inventoryMenu);
        }

        public string SearchFilter => _textBox.Text;

        public int RelativeLeft => ScreenHelper.TrueScreenWidth / 2 - (int)Width.Pixels / 2;
        public int RelativeTop => ScreenHelper.TrueScreenHeight / 2 - (int)Height.Pixels / 2;

        public override void OnInitialize()
        {
            base.OnInitialize();
            Width.Pixels = 704;
            Height.Pixels = 704;
            Left.Pixels = RelativeLeft;
            Top.Pixels = RelativeTop;
            BackgroundColor = Color.Transparent;
            BorderColor = Color.Transparent;


            Append(_backgroundSquare);


            _inventoryMenu.HAlign = 0.5f;
            _inventoryMenu.VAlign = 0.5f;
            Append(_inventoryMenu);

            _tabMenu.HAlign = 0.5f;
            _tabMenu.VAlign = 1f;
            Append(_tabMenu);
            Append(_xButton);

            //Scrollbar
            _scrollbar.Width.Set(20, 0);
            _scrollbar.Height.Set(340, 0);
            _scrollbar.Left.Set(0, 0.95f);
            _scrollbar.Top.Set(0, 0f);

            float maxViewSize = 48 * 8f;
            _scrollbar.SetView(0, maxViewSize);
            Append(_scrollbar);

            //Sorting Scrollbar
            _sortingScrollbar.Width.Set(20, 0);
            _sortingScrollbar.Height.Set(340, 0);
            _sortingScrollbar.Left.Set(0, 0.95f);
            _sortingScrollbar.Top.Set(0, 0f);

            _sortingScrollbar.SetView(0, maxViewSize);
            Append(_sortingScrollbar);
            _textBox.HAlign = 0.5f;
            _textBox.VAlign = 0.1f;
            _textBox.Width.Pixels = 128;
            Append(_textBox);
            Append(_modFilterButton);
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

        public void AddElements()
        {
            _inventoryMenu.AddElements(_tabMenu.Category);
        }
        private void SetPos()
        {
            Left.Pixels = _pos.X;
            Top.Pixels = _pos.Y;

            _backgroundSquare.Width = Width;
            _backgroundSquare.Height = Height;

            _inventoryMenu.HAlign = 0.5f;
            _inventoryMenu.VAlign = 0.25f;

            _tabMenu.VAlign = 0.8f;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            _modFilterButton.Left.Pixels = 64;
            _modFilterButton.Top.Pixels = 128;
            //Constantly lock the UI in the position regardless of resolution changes
            _inventoryMenu.SetSearchFilter(SearchFilter);
            SetPos();
        }

        private void Close()
        {
            ItemBrowserSystem itemBrowserSystem = ModContent.GetInstance<ItemBrowserSystem>();
            itemBrowserSystem.CloseUI();
        }

        private bool _isDragging;
        private Vector2? _drag = null;
        private Vector2 _pos;
        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);
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
}
