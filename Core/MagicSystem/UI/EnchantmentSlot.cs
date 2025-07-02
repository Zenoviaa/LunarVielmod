using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Core.Helpers;
using Terraria;
using Terraria.GameInput;
using Terraria.ModLoader;
using Terraria.UI;

namespace Stellamod.Core.MagicSystem.UI
{
    internal class EnchantmentSlot : UIElement
    {
        private StaffEditingContext _ctx;
        private readonly int _index;
        private readonly bool _isTimedSlot;
        private readonly int _context;
        private readonly float _scale;
        internal Item Item;
        internal EnchantmentSlot(int index, bool isTimedSlot, int context = ItemSlot.Context.BankItem, float scale = 1f)
        {
            _context = context;
            _scale = scale;
            _index = index;
            _isTimedSlot = isTimedSlot;

            Item = new Item();
            Item.SetDefaults(0);

            string texturePath = GetEnchantmentCardTexturePath();
            EnchantmentCardAsset = ModContent.Request<Texture2D>(texturePath, ReLogic.Content.AssetRequestMode.ImmediateLoad);
            Width.Set(EnchantmentCardAsset.Width() * scale, 0f);
            Height.Set(EnchantmentCardAsset.Height() * scale, 0f);
        }



        public Asset<Texture2D> EnchantmentCardAsset { get; private set; }
        public void SetContext(StaffEditingContext ctx)
        {
            _ctx = ctx;
            Item = _ctx.GetEnchantment(_index);
        }

        public string GetEnchantmentCardTexturePath()
        {
            string texturePath = GetType().DirectoryHere() + "/EnchantmentCard";
            return texturePath;
        }

        /// <summary>
        /// Returns true if this item can be placed into the slot (either empty or a pet item)
        /// </summary>
        internal bool Valid(Item item)
        {
            if (_isTimedSlot)
            {
                if (item.ModItem is Enchantment enchantment && enchantment.isTimedEnchantment)
                {
                    return true;
                }
                if (item.IsAir)
                {
                    return true;
                }
            }
            else
            {
                if (item.ModItem is Enchantment enchantment && !enchantment.isTimedEnchantment)
                {
                    return true;
                }

                if (item.IsAir)
                {
                    return true;
                }
            }

            return false;
        }

        internal void HandleMouseItem()
        {
            if (Valid(Main.mouseItem))
            {
                //Handles all the click and hover actions based on the context
                ItemSlot.Handle(ref Item, _context);

                //Save Item 
                _ctx.SetEnchantment(Item, _index);

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

            //Enchantment Card
            color2 = Color.LightGoldenrodYellow;

            Texture2D cardTexture = _isTimedSlot
                ? ModContent.Request<Texture2D>(GetType().DirectoryHere() + "/TimedEnchantmentCard").Value
                : ModContent.Request<Texture2D>(GetType().DirectoryHere() + "/EnchantmentCard").Value;

            int offset = (int)(cardTexture.Size().Y / 2);
            Vector2 centerPos = pos + rectangle.Size() / 2f;
            spriteBatch.Draw(cardTexture, rectangle.TopLeft(), null, color2, 0f, default(Vector2), _scale, SpriteEffects.None, 0f);


            //Enchantment Slot
            Texture2D slotTexture = _isTimedSlot
                ? ModContent.Request<Texture2D>(GetType().DirectoryHere() + "/TimedEnchantmentSlot").Value
                : ModContent.Request<Texture2D>(GetType().DirectoryHere() + "/EnchantmentSlot").Value;
            Vector2 drawOrigin = slotTexture.Size() / 2;
            Vector2 iconCenterPos = rectangle.TopLeft() + cardTexture.Size() / 2;
            spriteBatch.Draw(slotTexture, iconCenterPos, null, color2, 0f, drawOrigin, _scale, SpriteEffects.None, 0f);
            ItemSlot.DrawItemIcon(Item, _context, spriteBatch, centerPos, _scale * 2, 32, Color.White);
            Main.inventoryScale = oldScale;
        }
    }
}
