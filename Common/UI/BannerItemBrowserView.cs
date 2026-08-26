using Microsoft.Xna.Framework.Input;
using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Common.ArmorShop.UI;
using Stellamod.Common.Shaders;
using Stellamod.Core.Pixelation;
using Stellamod.Core.Tooltips;
using Stellamod.Core.Utilities;
using Stellamod.Helpers;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.UI.Chat;

namespace Stellamod.Common.UI
{
    public struct BannerDrawParameters
    {
        public Color color;
        public Vector2 position;
        public float scale;
    }
    public sealed class BannerItemBrowserView : UIPanel
    {
        private float _scale;
        private int _context;
        private bool _hovering;
        private bool _down;
        private Vector2 _offset;
        private Vector2 _velocity;
        private Vector2 _oldMousePos;
        private Vector2 _startMousePos;
        private Vector2 _startOffset;
        private float[] _scales;
        public BannerItemBrowserView(Item[] items, BannerShopParameters shopParameters)
        {
            _scale = 1f;
            _scale = 1f;
            _context = ItemSlot.Context.BankItem;

            //Set up the items we're going to iterate over
            Items = items;
            HoveringItem = new Item();
            HoveringItem.SetDefaults(ItemID.None);

            //Setup mouse interactions
            OnLeftClick += SpawnItem;
            OnLeftMouseDown += SetStartPosition;
            OnLeftMouseUp += AddVelocity;

            //Setup drawing
            ClothesLineTextureAsset = ModContent.Request<Texture2D>(this.GetTypeDirectoryWithSlash() + "Clothesline");
            SlotTextureAsset = ModContent.Request<Texture2D>(this.GetTypeDirectoryWithSlash() + "Banner");
            ShopParameters = shopParameters;
            if (shopParameters.SlotTextureOverride != null)
                SlotTextureAsset = shopParameters.SlotTextureOverride;
            Width.Set(32, 0f);
            Height.Set(32, 0f);
            SelectFunction = shopParameters.SelectItemFunction;
            ViewFunction = shopParameters.ViewItemFunction;
            IsSelectedFunction = shopParameters.SelectedItemFunction;
            DrawFunction = shopParameters.DrawFunction;
        }

        private void AddVelocity(UIMouseEvent evt, UIElement listeningElement)
        {
            Vector2 mouseScreen = Main.MouseScreen;
            Vector2 diff = mouseScreen - _oldMousePos;
            if (diff.Length() > 65)
                diff = diff.Resize(65);

            _velocity += diff;
            _down = false;
        }

        private void SetStartPosition(UIMouseEvent evt, UIElement listeningElement)
        {
            _startMousePos = Main.MouseScreen;
            _startOffset = _offset;
            _down = true;
        }

        private void SpawnItem(UIMouseEvent evt, UIElement listeningElement)
        {
            float d = Vector2.Distance(_startMousePos, Main.MouseScreen);
            if (d > 64)
                return;

            SelectFunction(HoveringItem);
        }

        public BannerShopParameters ShopParameters;
        public Asset<Texture2D> ClothesLineTextureAsset;
        public Asset<Texture2D> SlotTextureAsset;
        public readonly Action<Item> SelectFunction;
        public readonly Func<Item, bool> ViewFunction;
        public readonly Func<Item, bool> IsSelectedFunction;
        public readonly Action<SpriteBatch, Item, BannerDrawParameters> DrawFunction;
        public readonly Action<Item> HoverTooltipFunction;
        public readonly Action BuyFunction;
        public Item[] Items;
        public Item HoveringItem;
        public float transitionInterpolant;

        public override void Update(GameTime gameTime)
        {

            if (_down)
            {

                Vector2 mouseScreen = Main.MouseScreen;
                Vector2 diff = mouseScreen - _startMousePos;
                Vector2 newOffset = _startOffset + diff;
                _offset = newOffset;

            }
            _offset.X = MathHelper.Clamp(_offset.X, -Width.Pixels + 500, 0);
            _oldMousePos = Main.MouseScreen;
            _offset += _velocity;
      
            _offset.Y = 0;
            _velocity *= 0.94f;
          
        }
        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
         //   base.DrawSelf(spriteBatch);
            float oldScale = Main.inventoryScale;
            Main.inventoryScale = _scale;
            Rectangle rectangle = GetDimensions().ToRectangle();
            if (IsMouseHovering && !PlayerInput.IgnoreMouseInterface)
            {
                Main.LocalPlayer.mouseInterface = true;
            }

            if (IsMouseHovering && _hovering)
            {
                if (HoverTooltipFunction != null)
                {
                    HoverTooltipFunction(HoveringItem);
                }
                else
                {
                    Main.HoverItem = HoveringItem;
                    Main.hoverItemName = HoveringItem.HoverName;
                }
      
            }

            Vector2 topLeft = rectangle.TopLeft();
            topLeft += _offset;
            topLeft.X += 512;
            float availableWidth = GetInnerDimensions().Width;
            float listPadding = 8;
            Rectangle outerDimensions = new Rectangle(0, 0, 104, 210);
            Point mousePoint = Main.MouseScreen.ToPoint();
            string filter = string.Empty;
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

            _scales ??= new float[itemArr.Length];

            float elementWidth = outerDimensions.Width;
            float viewWidth = availableWidth;
            float elementHeight = outerDimensions.Height;

            //Calculate the maximum height of the grid
           
            float maximumWidth = itemArr.Length * (elementWidth + listPadding);
            Width.Pixels = maximumWidth + 32;



    
            Height.Pixels = 300;

            Texture2D slotTexture = SlotTextureAsset.Value;
            Color drawColor = Color.Lerp(Color.White, Color.Black, 0.75f);
            float drawScale = 1.2f;
            Vector2 drawOrigin = new Vector2(slotTexture.Width * 0.5f, 0f);
            Vector2 center = rectangle.Center();

            _hovering = false;
            //Now we're only loading the items that are in view! Yippee! Optimization!
            for (int i = 0; i < itemArr.Length; i++)
            {
                Item item = itemArr[i];


                //Remmeber 9 elements per row
                //We can use the modulus operator to get this to keep looping, since all elements are the same size
                float leftOffset = i * (elementWidth + listPadding);
                //Enchantment Card
                Vector2 tl = topLeft;
                tl.X += leftOffset;
                Vector2 centerPos = tl + new Vector2(24);
                Vector2 iconCenterPos = tl + slotTexture.Size() / 2;
                iconCenterPos.X = MathHelper.Lerp(center.X, iconCenterPos.X, transitionInterpolant);
                Vector2 diff = iconCenterPos - center;
   
                float xDist = MathF.Abs(center.X - iconCenterPos.X);

                float lerp = MathHelper.Clamp(xDist / 512, 0, 1);
                float lerp2 = MathHelper.Clamp(xDist / 1048, 0, 1);

                iconCenterPos -= diff * lerp2 * 0.1f;
                float extraScaleMul = MathHelper.Lerp(0.8f, 1f, 1f - lerp);
                bool isUnlocked = ViewFunction(item);
                bool isSelected = false;
                float banerLerp = 0.75f;
                if (IsSelectedFunction != null)
                {
                    isSelected = IsSelectedFunction(item);
                    if(isSelected)
                        banerLerp = 0.4f;
                }


                Color bannerColor = Color.Lerp(Color.White, Color.Black, banerLerp);
                Color iconColor = Color.White;
                if (!isUnlocked)
                    iconColor = Color.Lerp(iconColor, Color.Black, 0.8f);
                else
                {
                    bannerColor = Color.Lerp(bannerColor, Color.Transparent, lerp);
                    iconColor = Color.Lerp(iconColor, Color.Transparent, lerp);
                }
         

                spriteBatch.Draw(slotTexture, iconCenterPos - new Vector2(0, slotTexture.Height / 2), null, bannerColor * transitionInterpolant, 0f, drawOrigin, _scale * _scales[i] * extraScaleMul, SpriteEffects.None, 0f);

 
                if (isSelected)
                {
                    var whiteShader = SpriteWhiteShader.Instance;
                    float outlineOffset = 2;
                    Vector2 h = Vector2.UnitX * outlineOffset;
                    Vector2 v = Vector2.UnitY * outlineOffset;
                    RasterizerState rasterizerState = spriteBatch.GraphicsDevice.RasterizerState;

                    SamplerState anisotropicClamp = SamplerState.AnisotropicClamp;
                    spriteBatch.End();
                    spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, anisotropicClamp, DepthStencilState.None, Main.Rasterizer, whiteShader.Effect, Main.UIScaleMatrix);

                    if(ShopParameters.DrawWhitesFunction != null)
                    {
                        BannerDrawParameters drawParameters = new BannerDrawParameters();
                        drawParameters.position = iconCenterPos;
                        drawParameters.scale = drawScale * _scales[i] * extraScaleMul;
                        drawParameters.color = iconColor * transitionInterpolant;
                        ShopParameters.DrawWhitesFunction(spriteBatch, item, drawParameters);
                    }
                    else
                    {
                        Vector2 offset = new Vector2(2, 2);
                        ItemSlot.DrawItemIcon(item, _context, spriteBatch, iconCenterPos + new Vector2(offset.X, 0), drawScale * _scales[i] * extraScaleMul, 32, iconColor * transitionInterpolant);
                        ItemSlot.DrawItemIcon(item, _context, spriteBatch, iconCenterPos - new Vector2(offset.X, 0), drawScale * _scales[i] * extraScaleMul, 32, iconColor * transitionInterpolant);
                        ItemSlot.DrawItemIcon(item, _context, spriteBatch, iconCenterPos + new Vector2(0, offset.Y), drawScale * _scales[i] * extraScaleMul, 32, iconColor * transitionInterpolant);
                        ItemSlot.DrawItemIcon(item, _context, spriteBatch, iconCenterPos - new Vector2(0, offset.Y), drawScale * _scales[i] * extraScaleMul, 32, iconColor * transitionInterpolant);

                    }

                    spriteBatch.End();
                    spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, anisotropicClamp, DepthStencilState.None, rasterizerState, default, Main.UIScaleMatrix);

                    SpritebatchDrawer glowDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SimpleGlowCircle, Main.screenPosition + iconCenterPos);
                    glowDrawer.color = Color.White * 0.15f * (1f - lerp);
                    glowDrawer.color.A = 0;
                    glowDrawer.scale *= 0.26f;

                    spriteBatch.Draw(glowDrawer);
                }

                if(DrawFunction != null)
                {
                    BannerDrawParameters drawParameters = new BannerDrawParameters();
                    drawParameters.position = iconCenterPos;
                    drawParameters.scale = drawScale * _scales[i] * extraScaleMul;
                    drawParameters.color = iconColor * transitionInterpolant;
                    DrawFunction(spriteBatch, item, drawParameters);
                }
                else
                {
                    ItemSlot.DrawItemIcon(item, _context, spriteBatch, iconCenterPos,
                        drawScale * _scales[i] * extraScaleMul, 32, iconColor * transitionInterpolant);
                }
 
                if (HoveringItem.stack > 1)
                {
                    ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.ItemStack.Value, item.stack.ToString(),
                        centerPos + new Vector2(0, 2) * _scale, Color.White, 0f, Vector2.Zero, new Vector2(_scale), -1f, _scale);
                }

                //Check if hovering for tooltip
                Rectangle hoverRectangle = new Rectangle((int)tl.X, (int)tl.Y, 104, 210);
                if (hoverRectangle.Contains(mousePoint))
                {
                    _scales[i] = MathHelper.Lerp(_scales[i], 1.15f, 0.24f);
                    _hovering = true;

                    HoveringItem = item;
                    if(HoverTooltipFunction != null)
                    {
                        HoverTooltipFunction(item);
                    }
                    else
                    {
                        Main.HoverItem = item;
                        Main.hoverItemName = item.HoverName;
                    }
              
                }
                else
                {
                    _scales[i] = MathHelper.Lerp(_scales[i], 1f, 0.24f);
                }
            }
            Vector2 tl2 = rectangle.TopLeft();
            tl2.Y -= 16;
            tl2.X -= 64;
            SpritebatchDrawer drawer = SpritebatchDrawer.FromTextureAsset(ClothesLineTextureAsset, Main.screenPosition + tl2);
            drawer.color = Color.White * transitionInterpolant;
            drawer.drawOrigin = Vector2.Zero;
            drawer.sourceRect = new Rectangle(0, 0, 16, drawer.texture.Height);
            spriteBatch.Draw(drawer);

            Rectangle dstRect = new Rectangle((int)tl2.X + 16, (int)tl2.Y, Main.screenWidth + 64, drawer.texture.Height);
            drawer.sourceRect = new Rectangle(16, 0, 16, drawer.texture.Height);
            drawer.dstRect = dstRect;
            spriteBatch.Draw(drawer);
            Main.inventoryScale = oldScale;
        }


    }

}
