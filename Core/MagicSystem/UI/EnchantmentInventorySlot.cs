using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Core.Helpers;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace Stellamod.Core.MagicSystem.UI
{
    internal class EnchantmentInventorySlot : UIElement
    {
        private int _context;
        public EnchantmentInventorySlot()
        {
            _context = ItemSlot.Context.BankItem;
            OnLeftClick += On_LeftClick;


            string texturePath = GetType().DirectoryHere() + "/EnchantmentSlot";
            BackgroundTexture = ModContent.Request<Texture2D>(texturePath, ReLogic.Content.AssetRequestMode.ImmediateLoad);
            Width.Set(BackgroundTexture.Width(), 0f);
            Height.Set(BackgroundTexture.Height(), 0f);
        }

        public EnchantmentInventorySlot(Item item) : this()
        {
            this.Item = item;
        }

        public Asset<Texture2D> BackgroundTexture;
        public Item Item { get; set; }
        private void On_LeftClick(UIMouseEvent evt, UIElement listeningElement)
        {
            if (Item == null)
                return;
            if (!Main.mouseItem.IsAir)
                return;

            Main.mouseItem = Item.Clone();
            //TODO: Play UI click sound
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Rectangle rectangle = GetDimensions().ToRectangle();

            //Draw Backing
            Color color2 = Main.inventoryBack;
            Vector2 pos = rectangle.TopLeft();
            Vector2 centerPos = pos + rectangle.Size() / 2f;

            //Enchantment Card
            color2 = Color.LightGoldenrodYellow;



            //Enchantment Slot
            Texture2D slotTexture = BackgroundTexture.Value;
            Vector2 drawOrigin = slotTexture.Size() / 2;
            Vector2 iconCenterPos = rectangle.TopLeft();
            spriteBatch.Draw(slotTexture, iconCenterPos, null, color2, 0f, Vector2.Zero, 1, SpriteEffects.None, 0f);

            Color iconColor = Main.mouseItem.IsAir ? Color.White : Color.Lerp(Color.White, Color.Black, 0.8f);
            ItemSlot.DrawItemIcon(Item, _context, spriteBatch, centerPos, 2, 32, iconColor);


            //Handle hovering
            bool isHovering = rectangle.Contains(Main.MouseScreen.ToPoint());
            if (isHovering)
            {
                Main.hoverItemName = Item.Name;
                Main.HoverItem = Item;
            }
        }
    }
}
