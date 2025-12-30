using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Content.Items.MoonlightMagic;
using Stellamod.Helpers;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameInput;
using Terraria.ModLoader;
using Terraria.UI;

namespace Stellamod.Common.MagicSystem.UI
{
    public class EnchantmentInventorySlot : UIElement
    {
        private int _context;
        private readonly float _scale;
        private static EnchantmentComparer _comparer;
        public EnchantmentInventorySlot()
        {
            _comparer ??= new();
            _scale = 1;
            _context = ItemSlot.Context.BankItem;
            OnLeftClick += On_LeftClick;
            
            Item = new Item();
            Item.SetDefaults(0);

            string texturePath = GetType().DirectoryHere() + "/EnchantmentSlot";
            BackgroundTexture = ModContent.Request<Texture2D>(texturePath, ReLogic.Content.AssetRequestMode.ImmediateLoad);
            Width.Set(BackgroundTexture.Width() * 0.5f, 0f);
            Height.Set(BackgroundTexture.Height() * 0.5f, 0f);
        }

        public EnchantmentInventorySlot(Item item) : this()
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
            {
                Player player = Main.LocalPlayer;
                player.QuickSpawnItem(new EntitySource_DropAsItem(player), Main.mouseItem);
                Main.mouseItem = new Item();
                Main.mouseItem.SetDefaults(0);
            }
            AdvancedMagicPlayer magicPlayer = Main.LocalPlayer.GetModPlayer<AdvancedMagicPlayer>();
            if (!magicPlayer.IsUnlocked(Item))
            {
                return;
            }
            Main.mouseItem = Item.Clone();
            SoundStyle grab = AssetRegistry.Sounds.MagicWand.EnchantmentGrab;
            grab.PitchVariance = 0.15f;
            SoundEngine.PlaySound(grab);
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
            if (IsMouseHovering && !PlayerInput.IgnoreMouseInterface && !Main.LocalPlayer.mouseInterface)
            {
                Main.LocalPlayer.mouseInterface = true;
                HandleMouseItem();
            }

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

            Vector2 slotSize = slotTexture.Size();
            spriteBatch.Draw(slotTexture, iconCenterPos, null, color2, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0f);

            Color iconColor = Main.mouseItem.IsAir ? Color.White : Color.Lerp(Color.White, Color.Black, 0.8f);
            AdvancedMagicPlayer magicPlayer = Main.LocalPlayer.GetModPlayer<AdvancedMagicPlayer>();
            if (!magicPlayer.IsUnlocked(Item))
            {
                iconColor = Color.Lerp(Color.White, Color.Black, 0.8f);
            }
            ItemSlot.DrawItemIcon(Item, _context, spriteBatch, centerPos, 2, 16, iconColor);
            Main.inventoryScale = oldScale;
        }

        public int GetElementType()
        {
            if (Item.ModItem is BaseEnchantment enchantment)
                return enchantment.GetElementType();
            return 0;
        }


        public override int CompareTo(object obj)
        {
            if (obj is EnchantmentInventorySlot slot)
            {
                int compareElement = GetElementType().CompareTo(slot.GetElementType());
                if (compareElement == 0)
                {
                    return Item.ModItem.DisplayName.Value.CompareTo(slot.Item.ModItem.DisplayName.Value);
                }
                return compareElement;
            }
            return base.CompareTo(obj);
        }
    }
}
