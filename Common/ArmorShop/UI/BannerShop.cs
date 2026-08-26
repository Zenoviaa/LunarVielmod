using Stellamod.Common.UI;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.UI;
using Terraria.UI.Chat;

namespace Stellamod.Common.ArmorShop.UI;

public class BannerShop
{
    public BannerShop(BannerShopParameters shopParameters, Action closeAction)
    {
        browserMenu = new BannerBrowserMenu(shopParameters);
        shopMenuUIState = new BannerShopMenuUIState(shopParameters, closeAction);
    }

    public BannerBrowserMenu browserMenu;
    public BannerShopMenuUIState shopMenuUIState;

    /// <summary>
    /// Creates a menu of all the items in the mod
    /// </summary>
    public class BannerBrowserMenu : UIPanel
    {
        private bool _initItems;

        public BannerBrowserMenu(BannerShopParameters shopParameters)
        {
            this.shopParameters = shopParameters;
            BackgroundColor = Color.Transparent;
            BorderColor = Color.Transparent;
        }
        public BannerShopParameters shopParameters;
        public BannerItemBrowserView View { get; private set; }
        public float BannerWidth => View.Width.Pixels;
        public float BannerHeight => View.Height.Pixels;
        public float textAlpha;
        public override void OnInitialize()
        {
            base.OnInitialize();
            Width.Pixels = 428;
            Height.Pixels = 236;
            // Append(_view);
        }

        private void Refresh()
        {
            if (Main.gameMenu)
                return;
            if (!_initItems || View == null)
            {
                RemoveAllChildren();
                View = new(
                    shopParameters.AvailableItemsFunction(), 
                    shopParameters);
                View.Width.Pixels = Width.Pixels;
                View.Height.Pixels = Height.Pixels;
                View.Activate();
                _initItems = true;
                Append(View);
            }

            base.Recalculate();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (View == null)
                Refresh();

            Width.Pixels = BannerWidth;
            Height.Pixels = BannerHeight;
        }
        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);
            Rectangle r = new Rectangle(0, 0, Main.screenWidth, Main.screenHeight);
            SpritebatchDrawer d = SpritebatchDrawer.FromTextureAsset(TextureAssets.BlackTile.Value, Vector2.Zero);
            d.dstRect = r;
            d.drawOrigin = Vector2.Zero;
            d.color = Color.Black * textAlpha * 0.8f;
            spriteBatch.Draw(d);

            string text = LangText.Common("DragHelp");
            Vector2 size = FontAssets.MouseText.Value.MeasureString(text);
            Vector2 centerPos = new Vector2(Main.screenWidth, Main.screenHeight) * 0.5f;
            centerPos.Y += 256;

            string text2 = LangText.Common(shopParameters.TooltipKey);
            Vector2 size2 = FontAssets.MouseText.Value.MeasureString(text2);
            ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, text2,
                centerPos + new Vector2(0, -412), Color.White * ExtraMath.Osc(0.8f, 1f, speed: 3) * textAlpha, 0f, size2 * 0.5f, new Vector2(1f), -1f, 1f);

            ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, text,
                centerPos + new Vector2(0, -382), Color.Lerp(Color.White, Color.Black, 0.5f) * ExtraMath.Osc(0.8f, 1f, speed: 3) * textAlpha, 0f, size * 0.5f, new Vector2(1f), -1f, 1f);

            string text3 = LangText.Common(shopParameters.TitleKey);
            Vector2 size3 = FontAssets.DeathText.Value.MeasureString(text3);
            ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.DeathText.Value, text3,
                centerPos + new Vector2(0, -452), Color.White * textAlpha, 0f, size3 * 0.5f, new Vector2(1f), -1f, 1f);
        }
    }


    /// <summary>
    /// The full window of the item browser
    /// </summary>
    public class BannerShopBrowserWindow : UIPanel
    {

        public BannerShopBrowserWindow(BannerBrowserMenu browserMenu) : base()
        {
            InventoryMenu = browserMenu;
        }

        public BannerBrowserMenu InventoryMenu { get; private set; }
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


            InventoryMenu.HAlign = 0.5f;
            InventoryMenu.VAlign = 0.5f;
            Append(InventoryMenu);
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

        private void SetPos()
        {
            Left.Pixels = 0;
            Top.Pixels = RelativeTop;
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            Width.Pixels = InventoryMenu.Width.Pixels;
            Height.Pixels = InventoryMenu.Height.Pixels;
            InventoryMenu.HAlign = 0.5f;
            InventoryMenu.VAlign = 0.25f;
            SetPos();
        }

    }

  
    public class BannerShopMenuUIState : UIState
    {
        public BannerShopBrowserWindow browserWindow;
        public CommonBackButton backButton;
        public CommonBackButton buyButton;
        public BannerShopMenuUIState(BannerShopParameters shopParameters, Action closeAction) : base()
        {
            ShopParameters = shopParameters;
            browserWindow = new(new BannerBrowserMenu(shopParameters));
            CloseAction = closeAction;
        }
        public BannerShopParameters ShopParameters;
        public readonly Action CloseAction;
        public float timer;
        public bool isOpen;
        private int RelativeLeft => Main.screenWidth / 2 - (int)Width.Pixels / 2;
        private int RelativeTop => Main.screenHeight / 2 - (int)Height.Pixels / 2 - 64;
        public override void OnInitialize()
        {
            Append(browserWindow);

            backButton = new CommonBackButton(Close); // );
            backButton.Left.Pixels = RelativeLeft;
            backButton.Top.Pixels = RelativeTop;
            Append(backButton);


            buyButton = new CommonBackButton(ShopParameters.BuyFunction, "Buy"); // );
            buyButton.Left.Pixels = RelativeLeft;
            buyButton.Top.Pixels = RelativeTop;
            Append(buyButton);
        }
        public override void OnActivate()
        {
            base.OnActivate();
            isOpen = true;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            float gap = 128;
            backButton.Left.Pixels = RelativeLeft - backButton.Width.Pixels / 2;
            backButton.Top.Pixels = RelativeTop + 256;
            backButton.Left.Pixels -= gap;

            buyButton.Left.Pixels = RelativeLeft - backButton.Width.Pixels / 2;
            buyButton.Left.Pixels += gap;
            buyButton.Top.Pixels = RelativeTop + 256;
            if (FullyClosed())
            {
                CloseAction();
            }
            if (isOpen)
            {
                timer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            else
            {
                timer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            float time = 0.5f;
            timer = MathHelper.Clamp(timer, 0, time);
            float t = EasingFunction.InOutSine(timer / time);
            if (browserWindow.InventoryMenu.View != null)
            {
                browserWindow.InventoryMenu.View.transitionInterpolant = t;
                browserWindow.InventoryMenu.textAlpha = t;
            }
        }
        public bool FullyClosed()
        {
            return !isOpen && timer <= 0;
            //    throw new NotImplementedException();
        }
        private void Close()
        {
            isOpen = false;
        }
    }
}
