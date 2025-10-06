using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.ModLoader;
using Terraria.UI;

namespace Stellamod.Core.XixianFlaskSystem.UI
{
    public class FlaskSlotUIState : UIState
    {
        public FlaskSlotPanel panel;
        public FlaskSlotUIState() : base()
        {

        }

        public override void OnInitialize()
        {
            panel = new();
            Append(panel);
        }
    }
    public class FlaskSlotPanel : UIPanel
    {
        private UIPanel _panel;
        public FlaskSlot slot;


        public const int width = 432;
        public const int height = 280;

        public int RelativeLeft
        {
            get
            {
                if (!Main.playerInventory)
                {
                    return 412;
                }
                return 555;
            }
        }
        public int RelativeTop => 8;

        public override void OnInitialize()
        {
            base.OnInitialize();
            Width.Pixels = 48 * 5f;
            Height.Pixels = 48 * 16;
            Left.Pixels = RelativeLeft;
            Top.Pixels = RelativeTop;
            BackgroundColor = Color.Transparent;
            BorderColor = Color.Transparent;

            _panel = new UIPanel();
            _panel.Width.Pixels = Width.Pixels;
            _panel.Height.Pixels = Height.Pixels;
            _panel.BackgroundColor = Color.Transparent;
            _panel.BorderColor = Color.Transparent;
            Append(_panel);

            slot = new();
            _panel.Append(slot);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            //Constantly lock the UI in the position regardless of resolution changes
            Left.Pixels = RelativeLeft;
            Top.Pixels = RelativeTop;
        }
    }
    public class FlaskSlot : UIElement
    {
        private readonly int _context;
        private readonly float _scale;
        private Item _item;
        private UIText _durationText;
        private UIText _keybindText;
        private Asset<Texture2D> _slotTextureAsset;
        public FlaskSlot(int context = ItemSlot.Context.InventoryItem, float scale = 1f)
        {
            _context = context;
            _scale = scale;

            _slotTextureAsset = ModContent.Request<Texture2D>(
                $"{XixianFlaskUISystem.RootTexturePath}FlaskSlot", AssetRequestMode.ImmediateLoad);
            _durationText = new UIText("0");
            _keybindText = new UIText("");
            Width.Set(_slotTextureAsset.Width() * scale, 0f);
            Height.Set(_slotTextureAsset.Height() * scale, 0f);
            OnLeftClick += OpenUI;
        }

        public override void OnInitialize()
        {
            base.OnInitialize();
            _durationText.Left.Set(0, 0.1f);
            _durationText.Top.Set(0, 0.5f);
            Append(_durationText);
            Append(_keybindText);
        }

        private void OpenUI(UIMouseEvent evt, UIElement listeningElement)
        {
            //Don't open if haven't unlocked
            FlaskPlayer flaskPlayer = Main.LocalPlayer.GetModPlayer<FlaskPlayer>();
            if (!flaskPlayer.HasUnlockedFlask())
                return;

            XixianFlaskUISystem uiSystem = ModContent.GetInstance<XixianFlaskUISystem>();
            uiSystem.ToggleUI();

        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            List<string> keys = LunarVeilKeybinds.FlaskKeybind.GetAssignedKeys();
            if(keys.Count > 0)
            {
                _keybindText.SetText(keys[0]);
            }
            else
            {
                _keybindText.SetText("");
            }
            _keybindText.Left.Set(0, 0.75f);
            _keybindText.Top.Set(0, 0.75f);
                Player player = Main.LocalPlayer;
            int flaskBuffType = ModContent.BuffType<CannotUseFlask>();
            int buffIndex = player.FindBuffIndex(flaskBuffType);
            if (buffIndex == -1)
            {
                _durationText.SetText("");
                return;
            }

            int remainingTime = player.buffTime[buffIndex];
            float ticks = remainingTime;
            float seconds = ticks / 60f;
            _durationText.SetText(seconds.ToString("#.#"));
        }
        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            if (_item == null)
            {
                _item = ModContent.GetModItem(ModContent.ItemType<XixianFlask>()).Item.Clone();
            }

            float oldScale = Main.inventoryScale;
            Main.inventoryScale = _scale;
            Rectangle rectangle = GetDimensions().ToRectangle();
            bool contains = ContainsPoint(Main.MouseScreen);
            if (contains && !PlayerInput.IgnoreMouseInterface)
            {
                Main.LocalPlayer.mouseInterface = true;
                Main.hoverItemName = _item.HoverName;
                Main.HoverItem = _item;
            }

            //Draw Backing
            Color color2 = Main.inventoryBack;
            Vector2 pos = rectangle.TopLeft();

            Texture2D backingTexture = _slotTextureAsset.Value;
            int offset = (int)(backingTexture.Size().Y / 2);
            Vector2 centerPos = pos + rectangle.Size() / 2f;
            spriteBatch.Draw(backingTexture, rectangle.TopLeft(), null, color2, 0f, default, _scale, SpriteEffects.None, 0f);

            ItemSlot.DrawItemIcon(_item, _context, spriteBatch, centerPos, _scale, 32, Color.White);
            Main.inventoryScale = oldScale;
        }
    }
}
