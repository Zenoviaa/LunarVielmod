using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Items.MoonlightMagic;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.GameInput;
using Terraria.ModLoader;
using Terraria.UI;


namespace Stellamod.UI.AdvancedMagicSystem
{
    internal class AdvancedMagicElementSlot : UIElement
    {
        private Item _prevItem;
        private BaseStaff ActiveStaff => AdvancedMagicUISystem.Staff;
        private readonly int _context;
        private readonly float _scale;

        internal Item Item;
        internal event Action<int> OnEmptyMouseover;

        private int timer = 0;

        internal AdvancedMagicElementSlot(int context = ItemSlot.Context.BankItem, float scale = 1f)
        {
            _context = context;
            _scale = scale;
            var inventoryBack9 = TextureAssets.InventoryBack9;
            Width.Set(inventoryBack9.Width() * scale, 0f);
            Height.Set(inventoryBack9.Height() * scale, 0f);
        }

        /// <summary>
        /// Returns true if this item can be placed into the slot (either empty or a pet item)
        /// </summary>
        internal bool Valid(Item item)
        {
            return item.ModItem is BaseElement || item.IsAir;
        }

        internal void HandleMouseItem()
        {
            if (Valid(Main.mouseItem))
            {
                //Handles all the click and hover actions based on the context
                _prevItem = Item;
                ItemSlot.Handle(ref Item, _context);
                if (_prevItem != Item)
                {
                    SaveElementToStaff();
                }
            }
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            float oldScale = Main.inventoryScale;
            Main.inventoryScale = _scale;
            Rectangle rectangle = GetDimensions().ToRectangle();

            bool contains = ContainsPoint(Main.MouseScreen);

            if (contains && !PlayerInput.IgnoreMouseInterface)
            {
                Main.LocalPlayer.mouseInterface = true;
                HandleMouseItem();
            }

            //Draw Backing
            Color color2 = Main.inventoryBack;
            Vector2 pos = rectangle.TopLeft();

            Texture2D value = ModContent.Request<Texture2D>("Stellamod/UI/AdvancedMagicSystem/ElementSlot").Value;

            Vector2 centerPos = pos + rectangle.Size() / 2f;
            spriteBatch.Draw(value, rectangle.TopLeft(), null, color2, 0f, default(Vector2), _scale, SpriteEffects.None, 0f);

            if (Item.ModItem is BaseElement element)
            {

                /*
                int numberOfCloneImages = 6;

                Vector2 glowCenterPos = centerPos + new Vector2(12, 12);
                Main.DrawItemIcon(spriteBatch, Item, glowCenterPos, Color.White * 0.7f, sizeLimit);
                Color glowColor = element.GetElementColor();
                for (float i = 0; i < 1; i += 1f / numberOfCloneImages)
                {
                    float cloneImageDistance = MathF.Cos(Main.GlobalTimeWrappedHourly / 2.4f * MathF.Tau / 2f) + 0.5f;
                    cloneImageDistance = MathHelper.Max(cloneImageDistance, 0.1f);
                    Color color = glowColor * 0.4f;
                    color *= 1f - cloneImageDistance * 0.5f;
                    color.A = 0;
                    cloneImageDistance *= 3;
                    Vector2 drawPos = glowCenterPos + (i * MathF.Tau).ToRotationVector2() * (cloneImageDistance + 2f);
                    ItemSlot.DrawItemIcon(Item, _context, spriteBatch, drawPos, _scale, 69, color);
                }*/
            }

            ItemSlot.DrawItemIcon(Item, _context, spriteBatch, centerPos + new Vector2(12, 12), _scale, 64, Color.White);

            if (contains && Item.IsAir)
            {
                timer++;
                OnEmptyMouseover?.Invoke(timer);
            }
            else if (!contains)
            {
                timer = 0;
            }

            Main.inventoryScale = oldScale;
        }

        private void SaveElementToStaff()
        {
            if (ActiveStaff == null)
                return;
            ActiveStaff.primaryElement = Item.Clone();
        }

        public void Refresh()
        {
            if (ActiveStaff == null)
                return;
            Item = ActiveStaff.primaryElement.Clone();
        }
    }
}
