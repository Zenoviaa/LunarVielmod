using Microsoft.Xna.Framework.Input;
using ReLogic.Content;
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
using Terraria.ModLoader.UI.Elements;

namespace Stellamod.Common.MagicSystem.UI
{
    public class EnchantmentMenu : UIPanel
    {
        private UIGrid _grid;
        private UIGrid _timedGrid;
        private UIImage _backgroundSquare;
        private XButton _xButton;

        private InventoryMenu _inventoryMenu;

        private StaffSlot _staffSlot;
        private ElementSlot _elementSlot;

        private readonly Asset<Texture2D> BackgroundSquareTexture;
        public EnchantmentMenu() : base()
        {
            string texturePath = typeof(EnchantmentMenu).DirectoryHere() + "/EnchantingMenu";
            BackgroundSquareTexture = ModContent.Request<Texture2D>(texturePath);

            _grid = new UIGrid();
            _timedGrid = new UIGrid();
            _inventoryMenu = new InventoryMenu();
            _backgroundSquare = new UIImage(BackgroundSquareTexture)
            {
                HAlign = 0f,
                VAlign = 0f,
                AllowResizingDimensions = true,
                ScaleToFit = true,
            };

            _xButton = new XButton(Close);
            _elementSlot = new ElementSlot();
            _staffSlot = new StaffSlot();
        }

        public void UseContext(StaffEditingContext ctx)
        {
            _grid.Clear();
            _timedGrid.Clear();
            for (int i = 0; i < ctx.staffToEdit.GetCombinedNormalSlotCount(Main.LocalPlayer); i++)
            {
                var slot = new EnchantmentSlot(i, isTimedSlot: false);
                slot.SetContext(ctx);
                _grid.Add(slot);
            }

            for (int i = 0; i < ctx.staffToEdit.GetCombinedTimedSlotCount(Main.LocalPlayer); i++)
            {
                var slot = new EnchantmentSlot(i, isTimedSlot: true);
                slot.SetContext(ctx);
                _timedGrid.Add(slot);
            }

            _grid.Recalculate();
            _timedGrid.Recalculate();
            _staffSlot.SetContext(ctx);
            _elementSlot.SetContext(ctx);
        }

        public void Rebuild()
        {
            _inventoryMenu?.SetEnchantments();
        }


        public override void OnInitialize()
        {
            base.OnInitialize();
            Width.Pixels = 704;
            Height.Pixels = 704;
            BackgroundColor = Color.Transparent;
            BorderColor = Color.Transparent;


            Append(_backgroundSquare);

            _grid.Width.Set(0, 0.8f);
            _grid.Height.Set(0, 0.35f);
            _grid.HAlign = 0.5f;
            _grid.VAlign = 0.65f;
            _grid.ListPadding = 2f;
            _grid.OverflowHidden = false;
            Append(_grid);

            _timedGrid.Width.Set(0, 0.8f);
            _timedGrid.Height.Set(0, 0.35f);
            _timedGrid.HAlign = 0.5f;
            _timedGrid.VAlign = 1f;
            _timedGrid.ListPadding = 2f;
            Append(_timedGrid);

            _staffSlot.HAlign = 0.05f;
            _staffSlot.VAlign = 0.05f;
            Append(_staffSlot);

            _elementSlot.HAlign = 0.19f;
            _elementSlot.VAlign = 0.19f;
            Append(_elementSlot);

            _inventoryMenu.HAlign = 0.9f;
            _inventoryMenu.VAlign = 0.05f;
            Append(_inventoryMenu);
            Append(_xButton);
            SetPos();
        }

        public override void OnDeactivate()
        {
            base.OnDeactivate();
            if (!Main.gameMenu)
            {
                //   _staffSlot.ReturnItem();
                SoundEngine.PlaySound(SoundID.MenuClose);
            }
        }


        private void SetPos()
        {
            Left.Pixels = _pos.X;
            Top.Pixels = _pos.Y;

            _backgroundSquare.Width = Width;
            _backgroundSquare.Height = Height;


        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            //Constantly lock the UI in the position regardless of resolution changes
            SetPos();


        }

        private void Close()
        {
            MagicUISystem uiSystem = ModContent.GetInstance<MagicUISystem>();
            uiSystem.CloseUI();
        }

        private bool _isDragging;
        private Vector2? _drag = null;
        private Vector2 _pos;
        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);
           // bool mouseInteract = this.QuickMouseInteraction();
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
            Vector2 size = new Vector2(Width.Pixels, Height.Pixels / 4);
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

