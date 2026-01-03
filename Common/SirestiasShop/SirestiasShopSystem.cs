using log4net.Filter;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using Microsoft.Xna.Framework.Input;
using MonoMod.Core.Platforms;
using ReLogic.Content;
using Stellamod.Common.ItemBrowser;
using Stellamod.Common.MagicSystem.UI;
using Stellamod.Common.Shaders;
using Stellamod.Core.Effects;
using Stellamod.Core.Tooltips;
using Stellamod.Core.Utilities;
using Stellamod.Helpers;
using Stellamod.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.UI;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.UI.Elements;
using Terraria.UI;
using Terraria.UI.Chat;
using static Terraria.GameContent.Animations.IL_Actions.Sprites;

namespace Stellamod.Common.SirestiasShop
{
    public class ForceShopTooltip : GlobalItem
    {
        public override bool InstancePerEntity => true;
        public bool showShopPrice;
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
       
            base.ModifyTooltips(item, tooltips);
            //guh

            if (!showShopPrice)
                return;

            if (item.shopSpecialCurrency == -1)
                return;
            string[] lines = new string[4];
            int currentLine = 0;
            CustomCurrencyManager.GetPrices(item, out long sell, out long buy);
            CustomCurrencyManager.GetPriceText(item.shopSpecialCurrency, lines, ref currentLine, buy);
            TooltipLine line = new TooltipLine(Mod, "BuyPrice", lines[0]);


            tooltips.Add(line);
        }
    }

    /// <summary>
    /// The view that lets you buy an item from sirestias's catalogue
    /// </summary>
    public class SirestiasShopBrowserView : UIPanel
    {
        private float[] _scaleLerps;
        private int _context;
        private float _scale;
        public SirestiasShopBrowserView() : base()
        {
            BackgroundColor = Color.Transparent;
            BorderColor = Color.Transparent;
            _scale = 1f;
            _context = ItemSlot.Context.ShopItem;
            Items = new Item[0];
            HoveringItem = new Item();
            HoveringItem.SetDefaults(0);
            string texturePath = this.GetType().DirectoryHere() + "/ShopItemSlot";
            SlotTextureAsset = ModContent.Request<Texture2D>(texturePath, AssetRequestMode.AsyncLoad);

            texturePath = this.GetType().DirectoryHere() + "/MiniPriceSlot";
            MiniPriceTextureAsset = ModContent.Request<Texture2D>(texturePath, AssetRequestMode.AsyncLoad);
            Width.Set(32, 0f);
            Height.Set(32, 0f);

            ElementsPerRow = 4;
            OnLeftClick += SpawnItem;
        }

        public Item[] Items;
        public Item HoveringItem;
        public Asset<Texture2D> SlotTextureAsset;
        public Asset<Texture2D> MiniPriceTextureAsset;
        public Asset<Texture2D> CurrencyTextureAsset;
        public string SearchFilter;
        public float ViewPosition;
        public int ElementsPerRow;

        private void SpawnItem(UIMouseEvent evt, UIElement listeningElement)
        {
    
            if (HoveringItem == null || HoveringItem.IsAir)
                return;

            Player player = Main.LocalPlayer;
            player.GetItemExpectedPrice(HoveringItem, out long calcForSelling, out long caclForBuying);
            if (CustomCurrencyManager.BuyItem(player, caclForBuying, HoveringItem.shopSpecialCurrency))
            {
                Main.mouseItem = HoveringItem.Clone();
                SoundEngine.PlaySound(SoundID.Coins);
            }
        }

        public void SetCatalogue(Item[] items)
        {
            Items = items;
        }
        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            float oldScale = Main.inventoryScale;
            Main.inventoryScale = _scale;
            Rectangle rectangle = GetDimensions().ToRectangle();
            if (IsMouseHovering && !PlayerInput.IgnoreMouseInterface)
            {
                Main.LocalPlayer.mouseInterface = true;
            }

            Vector2 topLeft = rectangle.TopLeft();
            float availableWidth = GetInnerDimensions().Width;
            float listPadding = 48;
            Rectangle outerDimensions = new Rectangle(0, 0, 32, 32);
            Point mousePoint = Main.MouseScreen.ToPoint();
            string filter = string.Empty;
            if (!string.IsNullOrEmpty(SearchFilter))
                filter = SearchFilter.TrimStart().ToLower();
            bool useFilter = !string.IsNullOrEmpty(filter);

            //We're basically just reusing the grid code here lol
            //There's currently 9 items per row
            //To optimize this, we can calculate the placement of an element with some simple math based on its index
            //Instead of using left and top variables
            //So let's do that


            //We only want to draw the items that are actually in view
            //So we should calculate a starting inde


            //Define our width variables
            Item[] itemArr = Items;
            if (_scaleLerps == null || _scaleLerps.Length != itemArr.Length)
                _scaleLerps = new float[itemArr.Length];

            if (itemArr.Length == 0 || itemArr == null)
                return;

            int elementsPerRow = ElementsPerRow;
            float elementWidth = outerDimensions.Width;
            float viewWidth = availableWidth;
            float elementHeight = outerDimensions.Height + 24;

            //Calculate the maximum height of the grid
            int itemRows = (itemArr.Length / elementsPerRow);
            float maximumHeight = itemRows * (elementHeight + listPadding);
            Height.Pixels = maximumHeight + 32;


            Texture2D slotTexture = SlotTextureAsset.Value;
            Color drawColor = Color.Lerp(Color.White, Color.Black, 0.15f);
            float drawScale = 1.2f;
            Vector2 drawOrigin = slotTexture.Size() / 2;

            //The view position is the y offset of the scrollbar
            //So to figure out where to start from
            //We just divide the offset by 
            //Caculate a starting and ending index for which items to draw
            int numRowsDownward = (int)(ViewPosition / (elementHeight + listPadding));
            int startIndex = numRowsDownward * elementsPerRow;
            int endIndex = startIndex + elementsPerRow * 6;

            Vector2 miniPriceDrawOrigin = MiniPriceTextureAsset.Size() / 2f;
            CurrencyTextureAsset = ModContent.GetInstance<SirestiasShopSystem>().SelectedCurrencyTextureAsset;
            //Now we're only loading the items that are in view! Yippee! Optimization!
            HoveringItem = null;

            /*
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, Main.Rasterizer, default, Main.UIScaleMatrix);

            */
            Asset<Texture2D> glow2TextureAsset = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/ZuiEffect");
            Asset<Texture2D> backGlowTextureAsset = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/Backglow");
            for (int i = startIndex; i < endIndex && i < itemArr.Length; i++)
            {
                Item item = itemArr[i];

                //Remmeber 9 elements per row
                //We can use the modulus operator to get this to keep looping, since all elements are the same size
                float leftOffset = i % elementsPerRow * (elementWidth + listPadding);
                float topOffset = i / elementsPerRow * (elementHeight + listPadding);

                //Enchantment Card
                Vector2 tl = topLeft;
                tl.X += leftOffset;
                tl.Y += topOffset;
                //Check if hovering for tooltip
                bool hovering = false;
                Rectangle hoverRectangle = new Rectangle((int)tl.X, (int)tl.Y, 72, 68);
                if (hoverRectangle.Contains(mousePoint))
                {
                    hovering = true;
                    _scaleLerps[i] = MathHelper.Lerp(_scaleLerps[i], 1.5f, 0.1f);

                    HoveringItem = item;
                    Main.HoverItem = item;
                    Main.hoverItemName = item.HoverName;
                }
                else
                {
                    _scaleLerps[i] = MathHelper.Lerp(_scaleLerps[i], 1f, 0.1f);
                }

                Vector2 centerPos = tl + new Vector2(16);

                Vector2 iconCenterPos = tl + slotTexture.Size() / 2;
                iconCenterPos.Y += (int)ExtraMath.Osc(0f, 2f, offset: i);

                spriteBatch.Draw(slotTexture, iconCenterPos + Vector2.UnitY * 8, null, Color.Black * 0.35f, 0f, drawOrigin, _scale * 0.75f, SpriteEffects.None, 0f);
                spriteBatch.Draw(slotTexture, iconCenterPos, null, drawColor, 0f, drawOrigin, _scale * 0.75f, SpriteEffects.None, 0f);

                Vector2 priceCenterPos = iconCenterPos;
                priceCenterPos.Y += 48;
                priceCenterPos.Y += (int)ExtraMath.Osc(0f, 2f, offset: i + 1);
                spriteBatch.Draw(MiniPriceTextureAsset.Value, priceCenterPos, null, drawColor, 0f, miniPriceDrawOrigin, _scale * 0.75f, SpriteEffects.None, 0f);

                if(CurrencyTextureAsset != null)
                {
                    Vector2 currencyDrawOrigin = CurrencyTextureAsset.Size() / 2f;

                    spriteBatch.Draw(CurrencyTextureAsset.Value, priceCenterPos + new Vector2(8, 0), null, drawColor, 0f, currencyDrawOrigin, _scale * 0.6f, SpriteEffects.None, 0f);

                }
                int storeValue = item.GetStoreValue();
                string priceString = storeValue.ToString();
                ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.ItemStack.Value, priceString,
                     priceCenterPos + new Vector2(-16, -8) * _scale, Color.White, 0f, Vector2.Zero, new Vector2(_scale), -1f, _scale);


                float scaleM = _scaleLerps[i];
                if (hovering)
                {
                    Color color = Color.LightSkyBlue;
                    color *= 0.5f;
                    color.A = 0;

                    float glowScale = ExtraMath.Osc(0.9f, 1f, 3, i);
                    Vector2 origin = glow2TextureAsset.Value.Size() / 2f;
                    spriteBatch.Draw(glow2TextureAsset.Value, iconCenterPos, null, color, 0, origin, glowScale * scaleM * 0.8f, SpriteEffects.None, 0);
                    color = Color.White;
                    color *= 0.5f;
                    color.A = 0;
                    spriteBatch.Draw(glow2TextureAsset.Value, iconCenterPos, null, color, 0, origin, glowScale * scaleM * 0.4f * 0.8f, SpriteEffects.None, 0);

                    RasterizerState rasterizerState = spriteBatch.GraphicsDevice.RasterizerState;

                    SamplerState anisotropicClamp = SamplerState.AnisotropicClamp;
                    spriteBatch.End();
                    spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, SpriteWhiteShader.Instance.Effect, Main.UIScaleMatrix);
                    ItemSlot.DrawItemIcon(item, _context, spriteBatch, iconCenterPos + -Vector2.UnitX * 2, drawScale * scaleM, 32, Color.White);
                    ItemSlot.DrawItemIcon(item, _context, spriteBatch, iconCenterPos + Vector2.UnitX * 2, drawScale * scaleM, 32, Color.White);
                    ItemSlot.DrawItemIcon(item, _context, spriteBatch, iconCenterPos + Vector2.UnitY * 2, drawScale * scaleM, 32, Color.White);
                    ItemSlot.DrawItemIcon(item, _context, spriteBatch, iconCenterPos + -Vector2.UnitY * 2, drawScale * scaleM, 32, Color.White);

                    spriteBatch.End();
                    spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, rasterizerState, default, Main.UIScaleMatrix);
                } else
                {
                    Color color = Color.White;
                    color *= 0.25f;
                    color.A = 0;

                    float glowScale = ExtraMath.Osc(1f, 1.3f, 1, i);
                    Vector2 origin = backGlowTextureAsset.Value.Size() / 2f;
                    spriteBatch.Draw(backGlowTextureAsset.Value, iconCenterPos, null, color, 0, origin, glowScale, SpriteEffects.None, 0);

                }

                    ItemSlot.DrawItemIcon(item, _context, spriteBatch, iconCenterPos, drawScale * scaleM, 32, Color.White);
                if (HoveringItem != null && HoveringItem.stack > 1)
                {
                    ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.ItemStack.Value, item.stack.ToString(),
                        centerPos + new Vector2(0, 2) * _scale, Color.White, 0f, Vector2.Zero, new Vector2(_scale), -1f, _scale);
                }


            }

            if (IsMouseHovering)
            {
                Main.HoverItem = HoveringItem;
           
                Main.hoverItemName = HoveringItem == null ? string.Empty : HoveringItem.HoverName;
            }

            Main.inventoryScale = oldScale;
        }
    }

    public class SirestiasCurrencyButton : UIPanel
    {
        private float _scaleMult;
        private Asset<Texture2D> _currencyTextureAsset;
        private UIText _currencyText;
        public SirestiasCurrencyButton(Asset<Texture2D> currencyTextureAsset) : base()
        {
            _currencyText = new UIText("0");
            _currencyTextureAsset = currencyTextureAsset;
            BackgroundColor = Color.Transparent;
            BorderColor = Color.Transparent;
            Width.Pixels = 32;
            Height.Pixels = 32;
        }
        public int CurrencyID = -1;
        public int TextureWidth => _currencyTextureAsset.Width();
        public int TextureHeight => _currencyTextureAsset.Height();
        public bool drawCurrencyText;

    public override void LeftClick(UIMouseEvent evt)
    {
        base.LeftClick(evt);
        SirestiasShopSystem uiSystem = ModContent.GetInstance<SirestiasShopSystem>();
        uiSystem.SetCurrency(CurrencyID);
        uiSystem.SelectedCurrencyTextureAsset = _currencyTextureAsset;
    }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);
            bool isHovering = this.QuickMouseInteraction();
            Vector2 topLeft = GetDimensions().ToRectangle().TopLeft();
            topLeft.Y += (int)ExtraMath.Osc(0f, 3f, offset: CurrencyID);
            Vector2 drawOrigin = _currencyTextureAsset.Size() / 2f;

        
            float s = 1.25f;
            Width.Pixels = TextureWidth * s * _scaleMult;
            Height.Pixels = TextureHeight * s * _scaleMult;
            bool isSelected = ModContent.GetInstance<SirestiasShopSystem>().SelectedCurrencyTextureAsset == _currencyTextureAsset;


            if (isHovering)
            {
                List<TooltipLine> tooltipLines = new List<TooltipLine>();

                string key = "RuinMedal";
                if (CurrencyID == Stellamod.EreshstylCurrencyID)
                    key = "Ereshstyl";
                else if (CurrencyID == Stellamod.NoHitCrystalCurrencyID)
                    key = "NoHitCrystal";
                else if (CurrencyID == -1)
                    key = "Coins";


                TooltipLine helpLine = new TooltipLine(Stellamod.Instance, "CurrencyName", LangText.Common(key));
                helpLine.OverrideColor = Color.Goldenrod;
                tooltipLines.Add(helpLine);


                ExpandableTooltipRenderer renderer = ModContent.GetInstance<ExpandableTooltipRenderer>();
                renderer.SetTooltipsToDraw(tooltipLines, 64, 16);

                _scaleMult = MathHelper.Lerp(_scaleMult, 1.2f, 0.1f);

                RasterizerState rasterizerState = spriteBatch.GraphicsDevice.RasterizerState;

                SamplerState anisotropicClamp = SamplerState.AnisotropicClamp;
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer,null, Main.UIScaleMatrix);
            
                spriteBatch.Draw(_currencyTextureAsset.Value, topLeft + drawOrigin + Vector2.UnitX * 2 , null, Color.White, 0, drawOrigin, s * _scaleMult, SpriteEffects.None, 0);
                spriteBatch.Draw(_currencyTextureAsset.Value, topLeft + drawOrigin - Vector2.UnitX * 2 , null, Color.White, 0, drawOrigin, s * _scaleMult, SpriteEffects.None, 0);
                spriteBatch.Draw(_currencyTextureAsset.Value, topLeft + drawOrigin + Vector2.UnitY * 2 , null, Color.White, 0, drawOrigin, s * _scaleMult, SpriteEffects.None, 0);
                spriteBatch.Draw(_currencyTextureAsset.Value, topLeft + drawOrigin - Vector2.UnitY * 2 , null, Color.White, 0, drawOrigin, s * _scaleMult, SpriteEffects.None, 0);

                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, rasterizerState, default, Main.UIScaleMatrix);
            } else if (isSelected)
            {
                /*
                RasterizerState rasterizerState = spriteBatch.GraphicsDevice.RasterizerState;

                SamplerState anisotropicClamp = SamplerState.AnisotropicClamp;
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, null, Main.UIScaleMatrix);

                spriteBatch.Draw(_currencyTextureAsset.Value, topLeft + drawOrigin + Vector2.UnitX * 2, null, Color.Yellow, 0, drawOrigin, s * _scaleMult, SpriteEffects.None, 0);
                spriteBatch.Draw(_currencyTextureAsset.Value, topLeft + drawOrigin - Vector2.UnitX * 2, null, Color.Yellow, 0, drawOrigin, s * _scaleMult, SpriteEffects.None, 0);
                spriteBatch.Draw(_currencyTextureAsset.Value, topLeft + drawOrigin + Vector2.UnitY * 2, null, Color.Yellow, 0, drawOrigin, s * _scaleMult, SpriteEffects.None, 0);
                spriteBatch.Draw(_currencyTextureAsset.Value, topLeft + drawOrigin - Vector2.UnitY * 2, null, Color.Yellow, 0, drawOrigin, s * _scaleMult, SpriteEffects.None, 0);

                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, rasterizerState, default, Main.UIScaleMatrix);
           */
           

            }
            else
            {
                _scaleMult = MathHelper.Lerp(_scaleMult, 1f, 0.1f);
            }
            spriteBatch.Draw(_currencyTextureAsset.Value, topLeft + drawOrigin, null, Color.White, 0, drawOrigin, s * _scaleMult, SpriteEffects.None, 0);

            if (!isSelected)
            {
                Color glowColor = Color.Lerp(Color.White, Color.Black, 0.5f); ;
                spriteBatch.Draw(_currencyTextureAsset.Value, topLeft + drawOrigin, null, glowColor, 0, drawOrigin, s * _scaleMult, SpriteEffects.None, 0);
            }
            else
            {
         
            }

            if (!drawCurrencyText)
                return;

            if(CurrencyID != -1)
            {
                Player player = Main.LocalPlayer;

                CustomCurrencyManager.TryGetCurrencySystem(CurrencyID, out CustomCurrencySystem system);
                bool overflowing = false;
                long num = system.CountCurrency(out overflowing, player.inventory);
                long num2 = system.CountCurrency(out overflowing, player.bank.item);
                long num3 = system.CountCurrency(out overflowing, player.bank2.item);
                long num4 = system.CountCurrency(out overflowing, player.bank3.item);
                long num5 = system.CountCurrency(out overflowing, player.bank4.item);
                long num6 = num + num2 + num3 + num4 + num5;

                string text = num6.ToString();
                Vector2 position = new Vector2(topLeft.X, topLeft.Y);
                position.X -= (int)(FontAssets.MouseText.Value.MeasureString(text).X);
                position.X -= 16;
                position.Y += 8;
                ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, text,
                    position, Color.White, 0, Vector2.Zero, Vector2.One);
            }
            else
            {
                Player player = Main.LocalPlayer;
                CurrencyHelper.CountCoins(player, out Wallet wallet);

                topLeft.X -= 42;
                topLeft.Y += 8;
                DrawCoinText(spriteBatch, TextureAssets.Coin[0].Value, topLeft, $"{wallet.copperCoins}");

                topLeft.X -= 42;
                DrawCoinText(spriteBatch, TextureAssets.Coin[1].Value, topLeft, $"{wallet.silverCoins}");

                topLeft.X -= 42;
                DrawCoinText(spriteBatch, TextureAssets.Coin[2].Value, topLeft, $"{wallet.goldCoins}");

                topLeft.X -= 42;
                DrawCoinText(spriteBatch, TextureAssets.Coin[3].Value, topLeft, $"{wallet.platinumCoins}");
            }
        }

        private void DrawCoinText(SpriteBatch spriteBatch, Texture2D coinTexture, Vector2 topLeft, string text)
        {
            Vector2 position = topLeft;
            int frameCount = 8;
            int frameHeight = coinTexture.Height / frameCount;
            Rectangle frame = new Rectangle(0, 0, coinTexture.Width, frameHeight);
            spriteBatch.Draw(coinTexture, topLeft - new Vector2(8), frame, Color.White);
            ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, text,
                    position, Color.White, 0, Vector2.Zero, Vector2.One);
        }
    }

    public class SirestiasGIF : UIPanel
    {
        private float _scale;
        private int _frameCount = 60;
        private int _frameIndex;
        private float _frameCounter;
        private Rectangle _frame;
        private Asset<Texture2D> _sirestiasAsset;
        private int FrameWidth => 159;
        private int FrameHeight => 127;
        public SirestiasGIF() : base()
        {
            _sirestiasAsset = ModContent.Request<Texture2D>(this.GetType().DirectoryHere() + "/SirestiasSitting");
        }
        public override void OnInitialize()
        {
            base.OnInitialize();
            Width.Pixels = 159 * 2;
            Height.Pixels = 127 * 2;
            BackgroundColor = Color.Transparent;
            BorderColor = Color.Transparent;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            _frameCounter += deltaTime;

            _scale = 1.1f;
            Width.Pixels = 159 * 2 * _scale;
            Height.Pixels = 127 * 2 * _scale;
            if (_frameCounter >= 0.05f)
            {
                _frameIndex++;
                _frameIndex = _frameIndex % _frameCount;
                _frameCounter = 0f;
                _frame = new Rectangle(0, _frameIndex * FrameHeight, FrameWidth, FrameHeight);
            }
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);
            Vector2 topLeft = GetDimensions().ToRectangle().TopLeft();

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, Main.Rasterizer, default, Main.UIScaleMatrix);

            spriteBatch.Draw(_sirestiasAsset.Value, topLeft, _frame, Color.White, 0, Vector2.Zero, 2 * _scale, SpriteEffects.None, 0);

        }
    }

    public class SirestiasShopRightCurrencyBar : UIPanel
    {
        private float _scale;
        private SirestiasCurrencyButton _ruinMedalsButton;
        private SirestiasCurrencyButton _ereshstylButton;
        private SirestiasCurrencyButton _noHitCrystalButton;
        private SirestiasCurrencyButton _coinsButton;
        public SirestiasShopRightCurrencyBar() : base()
        {
            _ruinMedalsButton = new SirestiasCurrencyButton(ModContent.Request<Texture2D>(this.GetType().DirectoryHere() + "/RuinMedal", AssetRequestMode.ImmediateLoad));
            _noHitCrystalButton = new SirestiasCurrencyButton(ModContent.Request<Texture2D>(this.GetType().DirectoryHere() + "/NoHitCrystal", AssetRequestMode.ImmediateLoad));
            _ereshstylButton = new SirestiasCurrencyButton(ModContent.Request<Texture2D>(this.GetType().DirectoryHere() + "/Ereshstyl", AssetRequestMode.ImmediateLoad));
            _coinsButton = new SirestiasCurrencyButton(ModContent.Request<Texture2D>(this.GetType().DirectoryHere() + "/Coin", AssetRequestMode.ImmediateLoad));
        }
        public override void OnInitialize()
        {
            base.OnInitialize();
            Width.Pixels = 26 * 2;
            Height.Pixels = 190 * 2;
            BackgroundColor = Color.Transparent;
            BorderColor = Color.Transparent;


            Append(_coinsButton);
            Append(_ruinMedalsButton);
            Append(_ereshstylButton);
            Append(_noHitCrystalButton);


        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            Left.Set(-Width.Pixels, 1);
          //  _gif.Top.Set(-_gif.Height.Pixels, 1);

            float listPadding = 64;
            _coinsButton.Left.Pixels = 4;
            _coinsButton.Top.Pixels = 8;

            _ruinMedalsButton.Left.Pixels = _coinsButton.Left.Pixels - (_ruinMedalsButton.TextureWidth * 0.5f - _coinsButton.TextureWidth * 0.5f);
            _ruinMedalsButton.Top.Pixels = _coinsButton.Top.Pixels + _coinsButton.TextureHeight * 2;

            _ereshstylButton.Left.Pixels = _ruinMedalsButton.Left.Pixels - (_ereshstylButton.TextureWidth * 0.5f - _ruinMedalsButton.TextureWidth * 0.5f);
            _ereshstylButton.Top.Pixels = _ruinMedalsButton.Top.Pixels + _ruinMedalsButton.TextureHeight + _ereshstylButton.TextureHeight;

            _noHitCrystalButton.Left.Pixels = _ereshstylButton.Left.Pixels - (_noHitCrystalButton.TextureWidth * 0.5f - _ereshstylButton.TextureWidth * 0.5f);
            _noHitCrystalButton.Top.Pixels = _ereshstylButton.Top.Pixels + _ereshstylButton.TextureHeight + _noHitCrystalButton.TextureHeight;


            _coinsButton.drawCurrencyText = true;
            _ruinMedalsButton.drawCurrencyText = true;
            _ereshstylButton.drawCurrencyText = true;
            _noHitCrystalButton.drawCurrencyText = true;

            _ruinMedalsButton.CurrencyID = Stellamod.MedalCurrencyID;
            _noHitCrystalButton.CurrencyID = Stellamod.NoHitCrystalCurrencyID;
            _ereshstylButton.CurrencyID = Stellamod.EreshstylCurrencyID;
            _coinsButton.CurrencyID = -1;
        }
        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);
           
        }
    }

    public class SirestiasShopCurrencyBar : UIPanel
    {
        private float _scale;

        private Asset<Texture2D> _backgroundTextureAsset;
        private SirestiasCurrencyButton _ruinMedalsButton;
        private SirestiasCurrencyButton _ereshstylButton;
        private SirestiasCurrencyButton _noHitCrystalButton;
        private SirestiasCurrencyButton _coinsButton;
        public SirestiasShopCurrencyBar() : base()
        {
            _backgroundTextureAsset = ModContent.Request<Texture2D>(this.GetType().DirectoryHere() + "/CurrencyBar");
            _ruinMedalsButton = new SirestiasCurrencyButton(ModContent.Request<Texture2D>(this.GetType().DirectoryHere() + "/RuinMedal", AssetRequestMode.ImmediateLoad));
            _noHitCrystalButton = new SirestiasCurrencyButton(ModContent.Request<Texture2D>(this.GetType().DirectoryHere() + "/NoHitCrystal", AssetRequestMode.ImmediateLoad));
            _ereshstylButton = new SirestiasCurrencyButton(ModContent.Request<Texture2D>(this.GetType().DirectoryHere() + "/Ereshstyl", AssetRequestMode.ImmediateLoad));
            _coinsButton = new SirestiasCurrencyButton(ModContent.Request<Texture2D>(this.GetType().DirectoryHere() + "/Coin", AssetRequestMode.ImmediateLoad));
        }

        public Asset<Texture2D> SelectedCurrencyTextureAsset;
        public override void OnInitialize()
        {
            base.OnInitialize();
            Width.Pixels = 190 * 2;
            Height.Pixels = 26 * 2;
            BackgroundColor = Color.Transparent;
            BorderColor = Color.Transparent;
         
            Append(_ruinMedalsButton);
            Append(_ereshstylButton);
            Append(_noHitCrystalButton);
            Append(_coinsButton);

        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            _scale = 0.86f;
                Left.Pixels = 54;
            Top.Pixels = 32;

            Width.Pixels = 190 * 2 * _scale;
            Height.Pixels = 26 * 2 * _scale;


            _ruinMedalsButton.Left.Pixels = 8;
            _ruinMedalsButton.Top.Pixels = 0;

            _ereshstylButton.Left.Pixels = _ruinMedalsButton.Left.Pixels + _ruinMedalsButton.TextureWidth * 2;
            _ereshstylButton.Top.Pixels = _ruinMedalsButton.Top.Pixels;

            _noHitCrystalButton.Left.Pixels = _ereshstylButton.Left.Pixels + _ereshstylButton.TextureWidth + _noHitCrystalButton.TextureWidth;
            _noHitCrystalButton.Top.Pixels = _ereshstylButton.Top.Pixels;

            _coinsButton.Left.Pixels = _noHitCrystalButton.Left.Pixels + _noHitCrystalButton.TextureWidth + _coinsButton.TextureWidth + 4;
            _coinsButton.Top.Pixels = _noHitCrystalButton.Top.Pixels;


            _ruinMedalsButton.CurrencyID = Stellamod.MedalCurrencyID;
            _noHitCrystalButton.CurrencyID = Stellamod.NoHitCrystalCurrencyID;
            _ereshstylButton.CurrencyID = Stellamod.EreshstylCurrencyID;
            _coinsButton.CurrencyID = -1;
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);
            Vector2 topLeft = GetDimensions().ToRectangle().TopLeft();
            spriteBatch.Draw(_backgroundTextureAsset.Value, topLeft, null, Color.White, 0, Vector2.Zero, _scale, SpriteEffects.None, 0);
        }      
    }

    public class SirestiasShopCatalogueWindow : UIPanel
    {
        private float _scale;
        private UIList _uiList;
        private UIPanel _panel;
        private UIGrid _grid;
        private UIScrollbar _scrollbar;
        private SirestiasShopBrowserView _browserView;
        private Asset<Texture2D> _catalogueWindowAsset;
        public SirestiasShopCatalogueWindow(UIScrollbar scrollbar) : base()
        {
            _grid = new UIGrid();
            _uiList = new UIList(); 
            _panel = new UIPanel();
            _scrollbar = scrollbar;
            _catalogueWindowAsset = ModContent.Request<Texture2D>(this.GetType().DirectoryHere() + "/CatalogueWindow");
            _browserView = new SirestiasShopBrowserView();
        }


        public void SetCatalogue(Item[] items)
        {
            _browserView.SetCatalogue(items);
            _grid.Recalculate();
        }

        public override void OnInitialize()
        {
            base.OnInitialize();
            Width.Pixels = 394 * 2;
            Height.Pixels = 272 * 2;
            BackgroundColor = Color.Transparent;
            BorderColor = Color.Transparent;

            _panel.Width.Pixels = Width.Pixels;
            _panel.Height.Pixels = Height.Pixels;
            _panel.BackgroundColor = Color.Transparent;
            _panel.BorderColor = Color.Transparent;
            Append(_panel);
     
            _grid.Left.Pixels = 10;
            _grid.Width.Set(0, 1f);
            _grid.Height.Set(0, 1f);
            _grid.HAlign = 0.5f;
            _grid.VAlign = 0.5f;
            _grid.ListPadding = 2;
            _panel.Append(_grid);

            _uiList.Width.Pixels = Width.Pixels;
            _uiList.Height.Pixels = Height.Pixels;
            _uiList.Add(_panel);
            _uiList.SetScrollbar(_scrollbar);
            Append(_uiList);

            Append(_browserView);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            _scale = 0.75f;
            Width.Pixels = 394 * 2 * _scale;
            Height.Pixels = 272 * 2 * _scale;
            Left.Pixels = 38;
            Top.Pixels = 90;

            _panel.Height.Pixels = _browserView.Height.Pixels + 32;
            float progress = _panel.Height.Pixels / Height.Pixels;
            progress = MathHelper.Clamp(progress, 0f, 1f);

            _scrollbar.Height.Set(Height.Pixels * progress, 0);
            float scrollRatio = _scrollbar.ViewPosition;

            _browserView.ViewPosition = scrollRatio;
            _browserView.Left.Pixels = 12;
            _browserView.Top.Pixels = 8;
            _browserView.Width.Pixels = Width.Pixels;

            //Hacky way to get invisible scrollbar when there's no need for it
            if (_panel.Height.Pixels < Height.Pixels)
            {
                _scrollbar.Top.Set(500000, 0f);
            }
            else
            {
                _scrollbar.Top.Set(0, 0.2f);
            }
            _scrollbar.Left.Set(0, 0.83f);
            _grid.ListPadding = 16;
        }


        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);
            Vector2 topLeft = GetDimensions().ToRectangle().TopLeft();
            spriteBatch.Draw(_catalogueWindowAsset.Value, topLeft, null, Color.White, 0, Vector2.Zero, _scale, SpriteEffects.None, 0);
        }
    }

    /// <summary>
    /// The big panel that contains every other shop element for Sirestias's shop
    /// </summary>
    public class SirestiasShopMainWindow : UIPanel
    {
        private float _inTimer;
        private UIImage _shopWindow;
        private UIScrollbar _scrollbar;
        private SirestiasShopRightCurrencyBar _rightCurrencyBar;
        private SirestiasShopCurrencyBar _currencyBar;
        private SirestiasShopCatalogueWindow _catalogueWindow;
        private SirestiasGIF _gif;
        public SirestiasShopMainWindow() : base()
        {
            _gif = new SirestiasGIF();
            _rightCurrencyBar = new SirestiasShopRightCurrencyBar();
            _currencyBar = new SirestiasShopCurrencyBar();
            _scrollbar = new FancyScrollbar();
            //_xButton = new XButton(Close);
            _catalogueWindow = new SirestiasShopCatalogueWindow(_scrollbar);
            _shopWindow = new UIImage(ModContent.Request<Texture2D>(this.GetType().DirectoryHere() + "/ShopWindow"));
        }

        private int RelativeLeft => Main.screenWidth / 2 - (int)Width.Pixels / 2;
        private int RelativeTop => Main.screenHeight / 2 - (int)Height.Pixels / 2;
        public override void OnInitialize()
        {
            base.OnInitialize();
            Width.Pixels = 394 * 2;
            Height.Pixels = 272 * 2;
            BackgroundColor = Color.Transparent;
            BorderColor = Color.Transparent;
            Append(_shopWindow);

          //  Append(_xButton);

            Append(_catalogueWindow);
            Append(_currencyBar);
            Append(_rightCurrencyBar);

            _gif.Left.Set(0, 1);
            _gif.Top.Set(0, 1);
            Append(_gif);

            //Scrollbar
            _scrollbar.Width.Set(20, 0);
            _scrollbar.Height.Set(340, 0);
            _scrollbar.Left.Set(0, 0.95f);
            _scrollbar.Top.Set(0, 0f);

            float maxViewSize = 48 * 8f;
            _scrollbar.SetView(0, maxViewSize);
            Append(_scrollbar);
        }

        public override void OnDeactivate()
        {
            base.OnDeactivate();
            if (!Main.gameMenu)
            {
                _inTimer = 0;
                SoundEngine.PlaySound(SoundID.MenuClose);
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            Left.Pixels = RelativeLeft;
            Top.Pixels = RelativeTop;
            _inTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            float ratio = _inTimer / 1f;
            float ease = EasingFunction.OutExpo(ratio);
            Top.Pixels += MathHelper.Lerp(400, 0, ease);

            _gif.Left.Set(-_gif.Width.Pixels, 1);
            _gif.Top.Set(-_gif.Height.Pixels, 1);
            
        }

        public void SetCatalogue(Item[] items)
        {
            _catalogueWindow.SetCatalogue(items);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);
            this.QuickMouseInteraction();
        }
    }

    public class BackButton : UIPanel
    {
        private float _scale;
        private Action _closeFunction;
        private UIText _backText;
        public BackButton(Action closeFunction) : base()
        {
            _closeFunction = closeFunction; 
            _backText = new UIText("Back", large: true);
        }
        private int RelativeLeft => Main.screenWidth / 2 - (int)Width.Pixels / 2;
        private int RelativeTop => Main.screenHeight / 2 - (int)Height.Pixels / 2;
        public override void OnInitialize()
        {
            base.OnInitialize();
            Width.Pixels = 128;
            Height.Pixels = 32;
            Append(_backText);
        }


        public override void LeftClick(UIMouseEvent evt)
        {
            base.LeftClick(evt);
            _closeFunction();
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            Left.Pixels = RelativeLeft;
            Top.Pixels = RelativeTop + 340;

            Width.Pixels = 160;
            Height.Pixels = 54;

            _backText.Width.Pixels = Width.Pixels;
            _backText.Height.Pixels = Height.Pixels;
            _backText.HAlign = 0.5f;
            _backText.SetText(LangText.Common("Back"));
            BackgroundColor = Color.Lerp(Color.Blue, Color.Black, 1f) * 0.5f;
            BorderColor = Color.Lerp(Color.Purple, Color.Black, 0.8f) * 0.5f;
        }
        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);
            this.QuickMouseInteraction();
            if (IsMouseHovering)
            {
                _scale = MathHelper.Lerp(_scale, 1.25f, 0.3f);
            }
            else
            {
                _scale = MathHelper.Lerp(_scale, 1f, 0.3f);
            }
            _backText.SetText(LangText.Common("Back"), _scale, true);
        }
    }

    public class SirestiasShopUIState : UIState
    {
        public SirestiasShopMainWindow shopWindow;
        public BackButton backButton;
        public SirestiasShopUIState() : base()
        {

        }

        public override void OnInitialize()
        {
            shopWindow = new SirestiasShopMainWindow();
            Append(shopWindow);

            backButton = new BackButton(ModContent.GetInstance<SirestiasShopSystem>().CloseUI);
            Append(backButton);
        }
    }


    [Autoload(Side = ModSide.Client)]
    public class SirestiasShopSystem : BaseUISystem
    {
        private bool _pressed;
        private UserInterface _userInterface;
        private GameTime _lastUpdateUiGameTime;
        public SirestiasShopUIState uiState;
        public override int uiSlot => Slot_MajorUI;
        public override void OnModLoad()
        {
            base.OnModLoad();

            _userInterface = new UserInterface();
            uiState = new();
            uiState.Activate();
        }

        public override void PreUpdateWorld()
        {
            base.PreUpdateWorld();

        }
        public Asset<Texture2D> SelectedCurrencyTextureAsset;

        public void OpenUI()
        {
            //Create a new editing context
            //Set the state of the interface.
            SetCurrency(Stellamod.MedalCurrencyID);
            _userInterface.SetState(uiState);
        }

        public void CloseUI()
        {

            _userInterface.SetState(null);
        }

        public override void UpdateUI(GameTime gameTime)
        {
            if (InputHelper.KeyDown(Keys.F2))
            {
                _pressed = true;
            }
            else if (_pressed && InputHelper.KeyUp(Keys.F2))
            {
                if (_userInterface.CurrentState == null)
                    OpenUI();
                else
                    CloseUI();

                _pressed = false;
            }
            _lastUpdateUiGameTime = gameTime;
            if (_userInterface?.CurrentState != null)
            {
                _userInterface.Update(gameTime);
            }
        }

        public void SetCurrency(int currencyID)
        {
            List<Item> catalogue = new List<Item>(35);
       
            for (int i =0; i < ItemSets.IsSoldBySirestias.Length; i++)
            {
                if (ItemSets.IsSoldBySirestias[i])
                {
                  
                    Item item = new Item(i);
                    item.isAShopItem = true;
                    item.GetGlobalItem<ForceShopTooltip>().showShopPrice = true;
                    if(item.shopSpecialCurrency == currencyID)
                        catalogue.Add(item);
                }
            }
    
            Item[] catalogueArr = catalogue.ToArray();
            uiState.shopWindow.SetCatalogue(catalogueArr);
        }

        public override void PreSaveAndQuit()
        {
            //Calls Deactivate and drops the item
            if (_userInterface.CurrentState != null)
            {
                //   RenamePetUI.saveItemInUI = true;
                _userInterface.SetState(null);
            }
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
            if (mouseTextIndex != -1)
            {
                layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
                    "Scarlet Sun: Sirestias's Shop",
                    delegate
                    {
                        if (_lastUpdateUiGameTime != null && _userInterface?.CurrentState != null)
                        {
                            SpriteBatch spriteBatch = Main.spriteBatch;
                            spriteBatch.End();
                            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, null, Main.UIScaleMatrix);
                            _userInterface.Draw(Main.spriteBatch, _lastUpdateUiGameTime);
                        }
                        return true;
                    },
                    InterfaceScaleType.UI));
            }
        }
    }
}
