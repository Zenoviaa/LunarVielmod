using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Core.ArmorShop;
using System;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.ModLoader;
using Terraria.UI;

namespace Stellamod.UI.ArmorShopSystem
{
    public class BuyArmorButton : UIPanel
    {
        public event Action<int> OnEmptyMouseover;
        private readonly float _scale = 1f;
        public BuyArmorButton()
        {
            float scale = 1f;
            var asset = ModContent.Request<Texture2D>(
                $"{ArmorShopUISystem.RootTexturePath}BuyArmorButton", ReLogic.Content.AssetRequestMode.ImmediateLoad);
            Width.Set(asset.Width() * scale, 0f);
            Height.Set(asset.Height() * scale, 0f);
            OnLeftClick += OnButtonClick;
            OnMouseOver += OnMouseHover;
        }

        public ArmorShopSet armorSet;
        private void OnButtonClick(UIMouseEvent evt, UIElement listeningElement)
        {
            ArmorShopUISystem uiSystem = ModContent.GetInstance<ArmorShopUISystem>();
            if (uiSystem.CanPurchase(armorSet))
            {
                uiSystem.Purchase(armorSet);
            }

            // We can do stuff in here!
        }

        private void OnMouseHover(UIMouseEvent evt, UIElement listeningElement)
        {

        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            CalculatedStyle dimensions = GetDimensions();
            Rectangle rectangle = GetDimensions().ToRectangle();
            Point point = new Point((int)dimensions.X, (int)dimensions.Y);
            Vector2 pos = rectangle.TopLeft();
            Texture2D textureToDraw;
            if (IsMouseHovering)
            {
                textureToDraw = ModContent.Request<Texture2D>($"{ArmorShopUISystem.RootTexturePath}BuyArmorButtonSelected").Value;
            }
            else
            {
                textureToDraw = ModContent.Request<Texture2D>($"{ArmorShopUISystem.RootTexturePath}BuyArmorButton").Value;
            }
            bool contains = ContainsPoint(Main.MouseScreen);
            if (contains && !PlayerInput.IgnoreMouseInterface)
            {
                Main.LocalPlayer.mouseInterface = true;
            }
            Color drawColor = Color.White;
            ArmorShopUISystem uiSystem = ModContent.GetInstance<ArmorShopUISystem>();

            //Grey out when crafting won't make anything
            if (!uiSystem.CanPurchase(armorSet))
                drawColor = drawColor.MultiplyRGB(Color.Gray);


            spriteBatch.Draw(textureToDraw, pos, null, drawColor, 0f, Vector2.Zero, _scale, SpriteEffects.None, 0f);

        }
    }
}
