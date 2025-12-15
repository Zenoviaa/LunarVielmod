using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Helpers;
using Terraria;
using Terraria.GameContent;
using Terraria.GameInput;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.UI.Chat;

namespace Stellamod.Core.ItemBrowser
{
    /// <summary>
    /// When clicked, spawns the item into your inventory
    /// </summary>
    public class ItemBrowserSlot : UIElement
    {
        private readonly int _context;
        private readonly float _scale;
        public Item Item;
        public Asset<Texture2D> SlotTextureAsset;
        
        public ItemBrowserSlot(Item itemToGrant, int context = ItemSlot.Context.BankItem, float scale = 1f)
        {
            _context = context;
            _scale = scale;

            Item = itemToGrant;


            string texturePath = this.GetType().DirectoryHere() + "/ItemBrowserSlot";
            SlotTextureAsset = ModContent.Request<Texture2D>(texturePath, ReLogic.Content.AssetRequestMode.ImmediateLoad);
            Width.Set(32 * scale, 0f);
            Height.Set(32 * scale, 0f);
            OnLeftClick += SpawnItem;
            OnRightClick += SpawnItemMaxStack;

        }

        private void SpawnItemMaxStack(UIMouseEvent evt, UIElement listeningElement)
        {
            Main.LocalPlayer.QuickSpawnItem(Main.LocalPlayer.GetSource_FromThis(), Item.type, Item.maxStack);
        }

        private void SpawnItem(UIMouseEvent evt, UIElement listeningElement)
        {
            Main.LocalPlayer.QuickSpawnItem(Main.LocalPlayer.GetSource_FromThis(), Item.type);
        }
        public override int CompareTo(object obj)
        {
            if(obj is ItemBrowserSlot otherSlot)
            {
                return Item.type.CompareTo(otherSlot.Item.type);
            }
            return base.CompareTo(obj);
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

            if (IsMouseHovering)
            {

                Main.HoverItem = Item;
                Main.hoverItemName = Item.HoverName;
            }


            Vector2 pos = rectangle.TopLeft();

            //Enchantment Card
            Vector2 centerPos = pos + rectangle.Size() / 2f;
            Color color2 = Main.inventoryBack;
            Texture2D slotTexture = SlotTextureAsset.Value;
            Vector2 drawOrigin = slotTexture.Size() / 2;
            Vector2 iconCenterPos = rectangle.TopLeft() + slotTexture.Size() / 2;
            spriteBatch.Draw(slotTexture, iconCenterPos, null, Color.Lerp(Color.White, Color.Black, 0.75f), 0f, drawOrigin, _scale, SpriteEffects.None, 0f);
            ItemSlot.DrawItemIcon(Item, _context, spriteBatch, centerPos, _scale * 1.25f, 32, Color.White);
            if (Item.stack > 1)
                ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.ItemStack.Value, Item.stack.ToString(),
                    centerPos + new Vector2(0, 2) * _scale, Color.White, 0f, Vector2.Zero, new Vector2(_scale), -1f, _scale);



            Main.inventoryScale = oldScale;
        }
    }
}
