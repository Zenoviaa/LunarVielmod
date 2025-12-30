using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Common.XixianFlaskSystem;
using Stellamod.Helpers;
using Terraria;
using Terraria.Audio;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace Stellamod.Common.XixianFlaskSystem.UI
{
    public class InsourceInventorySlot : UIElement
    {
        private int _context;
        private static float _scale => 2.5f;

        public InsourceInventorySlot()
        {
            _context = ItemSlot.Context.BankItem;
            OnLeftClick += On_LeftClick;
            Item = new Item();
            Item.SetDefaults(0);

            string texturePath = GetType().DirectoryHere() + "/InsourceSlot";
            BackgroundTexture = ModContent.Request<Texture2D>(texturePath, ReLogic.Content.AssetRequestMode.ImmediateLoad);
            Width.Set(BackgroundTexture.Width() * 0.5f, 0f);
            Height.Set(BackgroundTexture.Height() * 0.5f, 0f);
        }

        public InsourceInventorySlot(Item item) : this()
        {
            this.Item = item.Clone();
        }

        public Asset<Texture2D> BackgroundTexture;
        public Item Item;
        private void On_LeftClick(UIMouseEvent evt, UIElement listeningElement)
        {
            if (Item == null)
                return;
            if (!Main.mouseItem.IsAir)
                return;
            FlaskPlayer flaskPlayer = Main.LocalPlayer.GetModPlayer<FlaskPlayer>();
            if (!flaskPlayer.HasUnlocked(Item))
                return;
            Main.mouseItem = Item.Clone();
            SoundStyle clickSound = SoundID.MenuTick;
            SoundEngine.PlaySound(clickSound);
        }

        private void HandleMouseItem()
        {
            Main.HoverItem = Item;
            Main.hoverItemName = Item.HoverName;

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
            Vector2 centerPos = pos + (rectangle.Size() / 2f);

            //Enchantment Card
            color2 = Color.LightGoldenrodYellow;



            //Enchantment Slot
            Texture2D slotTexture = BackgroundTexture.Value;
            Vector2 drawOrigin = slotTexture.Size() / 2;
            Vector2 iconCenterPos = rectangle.TopLeft();

            Vector2 slotSize = slotTexture.Size();
            //   spriteBatch.Draw(slotTexture, iconCenterPos, null, color2, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0f);

            Color iconColor = Main.mouseItem.IsAir ? Color.White : Color.Lerp(Color.White, Color.Black, 0.8f);
            FlaskPlayer flaskPlayer = Main.LocalPlayer.GetModPlayer<FlaskPlayer>();
            if (!flaskPlayer.HasUnlocked(Item))
            {
                iconColor = Color.Lerp(Color.White, Color.Black, 0.8f);
            }
            ItemSlot.DrawItemIcon(Item, _context, spriteBatch, centerPos + new Vector2(8), Main.inventoryScale, 16, iconColor);
            Main.inventoryScale = oldScale;
        }

        public override int CompareTo(object obj)
        {
            return base.CompareTo(obj);
        }
    }
}
