using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Content.Items.MoonlightMagic;
using Stellamod.Core.Shaders;
using Stellamod.Helpers;
using Terraria;
using Terraria.Audio;
using Terraria.GameInput;
using Terraria.ModLoader;
using Terraria.UI;

namespace Stellamod.Core.MagicSystem.UI
{
    public class EnchantmentSlot : UIElement
    {
        private StaffEditingContext _ctx;
        private readonly int _index;
        private readonly bool _isTimedSlot;
        private readonly int _context;
        private readonly float _scale;
        private Item _oldItem;
        public Item Item;
        public EnchantmentSlot(int index, bool isTimedSlot, int context = ItemSlot.Context.BankItem, float scale = 1f)
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
        public bool Valid(Item item)
        {
            if (_isTimedSlot)
            {
                if (item.ModItem is BaseEnchantment enchantment && enchantment.isTimedEnchantment)
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
                if (item.ModItem is BaseEnchantment enchantment && !enchantment.isTimedEnchantment)
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

        public void HandleMouseItem()
        {
            if (Valid(Main.mouseItem))
            {
                //Handles all the click and hover actions based on the context

                ItemSlot.Handle(ref Item, _context);

                //Save Item 
                if (Main.mouseLeftRelease && Main.mouseLeft)
                {
                    _ctx.SetEnchantment(Item, _index);
                    SoundStyle place = AssetRegistry.Sounds.MagicWand.EnchantmentPlace;
                    place.PitchVariance = 0.15f;
                    SoundEngine.PlaySound(place);
                }
            }
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            float oldScale = Main.inventoryScale;
            Main.inventoryScale = _scale;
            Rectangle rectangle = GetDimensions().ToRectangle();
            bool contains = ContainsPoint(Main.MouseScreen);

            BaseEnchantment enchantment = Item.ModItem as BaseEnchantment;
            bool isSynergy = false;
            if(enchantment != null)
            {
                if(_ctx.staffToEdit != null)
                {
                    BaseElement element = _ctx.staffToEdit.GetElement().ModItem as BaseElement;
                    isSynergy = element.IsSynergizingWith(enchantment.GetElementType());
                }

            }

            if (IsMouseHovering && !PlayerInput.IgnoreMouseInterface)
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

            if (Item.ModItem is BaseEnchantment myEnchantment && isSynergy)
            {
                var myElement = ModContent.GetModItem(myEnchantment.GetElementType()) as BaseElement;
                var shader = FirePixelShader.Instance;
                shader.PrimaryColor = Color.Lerp(Color.White, new Color(255, 207, 79), 0.5f);
                shader.NoiseColor = myElement.GetElementColor();
                shader.Distortion = 0.0075f;
                shader.Speed = 10;
                shader.Power = 0.01f;
                shader.Apply();

                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, default, default, default, default, Main.UIScaleMatrix);

                shader.Data.Apply(null);

                spriteBatch.Draw(cardTexture, rectangle.TopLeft(), null, color2, 0f, default(Vector2), _scale, SpriteEffects.None, 0f);

                spriteBatch.End();
                spriteBatch.Begin(default, default, default, default, default, default, Main.UIScaleMatrix);

            }
            //Enchantment Slot
            Texture2D slotTexture = _isTimedSlot
                ? ModContent.Request<Texture2D>(GetType().DirectoryHere() + "/TimedEnchantmentSlot").Value
                : ModContent.Request<Texture2D>(GetType().DirectoryHere() + "/EnchantmentSlot").Value;
            Vector2 drawOrigin = slotTexture.Size() / 2;
            Vector2 iconCenterPos = rectangle.TopLeft() + cardTexture.Size() / 2;
            spriteBatch.Draw(slotTexture, iconCenterPos, null, color2, 0f, drawOrigin, _scale, SpriteEffects.None, 0f);
            ItemSlot.DrawItemIcon(Item, _context, spriteBatch, centerPos, _scale * 2, 32, Color.White);
           // spriteBatch.Restart(blendState: BlendState.Additive);

            if (!Item.IsAir)
            {
                Color colorOsc = Color.Lerp(Color.Black, Color.White, VectorHelper.Osc(0f, 1f));
                for (float f = 0; f < MathHelper.TwoPi; f += MathHelper.PiOver2)
                {
                    Vector2 drawPos = centerPos;
                    drawPos += f.ToRotationVector2() * VectorHelper.Osc(0f, 1f) * 10;
                    ItemSlot.DrawItemIcon(Item, _context, spriteBatch, drawPos, _scale * 2, 32, colorOsc * 0.03f);
                }


            }

            if (isSynergy)
            {
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, default, default, default, default, Main.UIScaleMatrix);
                Color colorOsc = Color.Lerp(Color.Black, Color.White, VectorHelper.Osc(0f, 1f));
                for (float f = 0; f < MathHelper.TwoPi; f += MathHelper.PiOver2)
                {
                    Vector2 drawPos = centerPos;
                    drawPos += f.ToRotationVector2() * VectorHelper.Osc(0f, 1f) * 10;
                    ItemSlot.DrawItemIcon(Item, _context, spriteBatch, drawPos, _scale * 2, 32, colorOsc * 0.5f);
                }
                spriteBatch.End();
                spriteBatch.Begin(default, default, default, default, default, default, Main.UIScaleMatrix);
            }

            //spriteBatch.RestartDefaults();


            if (IsMouseHovering && Item.IsAir)
            {

                SlotTooltipItem tooltipItem = ModContent.GetInstance<SlotTooltipItem>();
                tooltipItem.Reset();
                tooltipItem.isSynergy = isSynergy;
                tooltipItem.isTimedSlot = _isTimedSlot;
                Main.HoverItem = tooltipItem.Item;
                Main.hoverItemName = tooltipItem.Item.HoverName;
            }


            Asset<Texture2D> enchantmentOrb = ModContent.Request<Texture2D>(this.GetType().DirectoryHere() + "/EnchantmentOrb");
            Vector2 orbDrawPos = rectangle.TopLeft();
            orbDrawPos += new Vector2(-10, 0);
            // orbDrawPos += new Vector2(28, 17);
            Vector2 orbDrawOrigin = Vector2.Zero;
            float orbDrawScale = 1.25f;
            Color orbDrawColor = isSynergy ? Color.White : Color.Lerp(Color.White, Color.Black, 0.8f);
            spriteBatch.Draw(enchantmentOrb.Value, orbDrawPos, null, orbDrawColor, 0, orbDrawOrigin, orbDrawScale, SpriteEffects.None, 0);

            Rectangle orbHoverRectangle = new Rectangle((int)orbDrawPos.X, (int)orbDrawPos.Y, enchantmentOrb.Width(), enchantmentOrb.Height());
            if (orbHoverRectangle.Contains(Main.MouseScreen.ToPoint()))
            {
                SlotTooltipItem tooltipItem = ModContent.GetInstance<SlotTooltipItem>();
                tooltipItem.Reset();
                if (isSynergy)
                {
                    tooltipItem.isSynergy = true;
                }
                else
                {
                    tooltipItem.noSynergy = true;
                }

                Main.HoverItem = tooltipItem.Item;
                Main.hoverItemName = tooltipItem.Item.HoverName;
            }
            Main.inventoryScale = oldScale;
        }
    }
}
