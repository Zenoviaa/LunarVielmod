using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Content.Items.MoonlightMagic;
using Stellamod.Helpers;
using Terraria;
using Terraria.GameInput;
using Terraria.ModLoader;
using Terraria.UI;

namespace Stellamod.Common.MagicSystem.UI
{
    public class StaffSlot : UIElement
    {
        private StaffEditingContext _ctx;
        private readonly int _context;
        private readonly float _scale;
        public Item Item;

        public StaffSlot(int context = ItemSlot.Context.BankItem, float scale = 1f)
        {
            _context = context;
            _scale = scale;

            Item = new Item();
            Item.SetDefaults(0);


            string texturePath = GetEnchantmentCardTexturePath();
            EnchantmentCardAsset = ModContent.Request<Texture2D>(texturePath, ReLogic.Content.AssetRequestMode.ImmediateLoad);
            Width.Set(EnchantmentCardAsset.Width() * scale, 0f);
            Height.Set(EnchantmentCardAsset.Height() * scale, 0f);
        }

        public Asset<Texture2D> EnchantmentCardAsset { get; private set; }

        public string GetEnchantmentCardTexturePath()
        {
            string texturePath = GetType().DirectoryHere() + "/StaffSlot";
            return texturePath;
        }

        /// <summary>
        /// Returns true if this item can be placed into the slot (either empty or a pet item)
        /// </summary>
        public bool Valid(Item item)
        {

            if (item.ModItem is BaseStaff staff)
                return true;

            //If we want to replace with air, yeah.
            if (item.IsAir)
                return true;

            return false;
        }
        public void SetContext(StaffEditingContext ctx)
        {
            _ctx = ctx;
            if(_ctx.staffToEdit.Item.type == ModContent.ItemType<NoStaff>())
            {
                Item = new Item();
                Item.SetDefaults(0);
            } else
            {
                Item = _ctx.staffToEdit.Item;
            }
        
        }

        public void ReturnItem()
        {
            if (Item.IsAir)
                return;
            Player player = Main.LocalPlayer;
            player.QuickSpawnItemDirect(player.GetSource_FromThis(), Item);
        }

        public void HandleMouseItem()
        {
            if (Valid(Main.mouseItem))
            {
                //Handles all the click and hover actions based on the context
                ItemSlot.Handle(ref Item, _context);
                //So when we switch a staff we need to create a new editing context.
                //So we need something to hold that context.

                if (Main.mouseLeftRelease && Main.mouseLeft)
                {
                    MagicUISystem magicUISystem = ModContent.GetInstance<MagicUISystem>();
                    if (Item.IsAir)
                    {
                        magicUISystem.EmptyUI();
                    }
                    else if (Item.ModItem is BaseStaff staff)
                    {
                        magicUISystem.OpenUI(staff);
                    }
                }
            }
        }


        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            float oldScale = Main.inventoryScale;
            Main.inventoryScale = _scale;
            Rectangle rectangle = GetDimensions().ToRectangle();
            if (IsMouseHovering && !PlayerInput.IgnoreMouseInterface)
            {
                Main.LocalPlayer.mouseInterface = true;
                HandleMouseItem();
            }

            //Draw item icon
            Vector2 pos = rectangle.TopLeft();
            Vector2 centerPos = pos + rectangle.Size() / 2f;

            Texture2D cardTexture = EnchantmentCardAsset.Value;
            int offset = (int)(cardTexture.Size().Y / 2);
            spriteBatch.Draw(cardTexture, rectangle.TopLeft(), null, Color.White, 0f, default(Vector2), _scale, SpriteEffects.None, 0f);

            Item item = _ctx.staffToEdit.Item;
            ItemSlot.DrawItemIcon(item, _context, spriteBatch, centerPos, _scale * 2, 32, Color.White);
            Main.inventoryScale = oldScale;
        }
    }
}
