using Microsoft.Xna.Framework.Graphics;
using Stellamod.UI.ArmorReforgeSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria;
using Microsoft.Xna.Framework;
using Stellamod.Common.ArmorShop;

namespace Stellamod.UI.ArmorShopSystem
{
    internal class BuyArmorButton : UIPanel
    {
        internal event Action<int> OnEmptyMouseover;
        private readonly float _scale = 1f;
        internal BuyArmorButton()
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
            Point point = new Point((int)dimensions.X, (int)dimensions.Y);
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

            Rectangle rect = new Rectangle(point.X, point.Y, textureToDraw.Width, textureToDraw.Height);
            float rotation = 0;


            spriteBatch.Draw(textureToDraw, rect, null, drawColor, rotation, Vector2.Zero, SpriteEffects.None, 0);
        }
    }
}
