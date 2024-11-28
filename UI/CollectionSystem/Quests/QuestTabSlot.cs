using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Common.QuestSystem;
using System;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.ModLoader;
using Terraria.ModLoader.UI.Elements;
using Terraria.UI;

namespace Stellamod.UI.CollectionSystem.Quests
{
    internal class QuestTabSlot : UIElement
    {
        private UIText _text;

        internal Quest Quest;

        private readonly int _context;
        private readonly float _scale;
        internal QuestTabSlot(float scale = 1f)
        {
            _scale = scale;
            var value = ModContent.Request<Texture2D>($"{CollectionBookUISystem.RootTexturePath}CollectionTabSlot",
                ReLogic.Content.AssetRequestMode.ImmediateLoad);
            Width.Set(value.Width() * scale, 0f);
            Height.Set(value.Height() * scale, 0f);
            OnLeftClick += OnButtonClick;
            OnMouseOver += OnMouseHover;
        }

        public float Glow { get; set; }
        public bool CompletedQuest { get; set; }
        public override void OnInitialize()
        {
            base.OnInitialize();
            Width.Pixels = 48 * 6f;
            Height.Pixels = 24;
            _text = new UIText("This is placeholder text", large: false);
            _text.Height.Pixels = Height.Pixels;
            _text.Width.Pixels = Width.Pixels;
            _text.Top.Pixels = 4;
            _text.IsWrapped = false;

            Append(_text);
        }
        private void OnButtonClick(UIMouseEvent evt, UIElement listeningElement)
        {

            CollectionBookUISystem uiSystem = ModContent.GetInstance<CollectionBookUISystem>();
            uiSystem.OpenQuestInfoUI(Quest);
        }

        private void OnMouseHover(UIMouseEvent evt, UIElement listeningElement)
        {

        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (_text != null && Quest != null)
            {
                _text.Top.Pixels = 4;
                _text.SetText(Quest.DisplayName);
                if (CompletedQuest)
                {
                    _text.TextColor = Color.Gray;
                }
            }

            bool contains = ContainsPoint(Main.MouseScreen);
            if (contains && !PlayerInput.IgnoreMouseInterface)
            {
                Main.LocalPlayer.mouseInterface = true;
            }
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);
            float oldScale = Main.inventoryScale;
            Main.inventoryScale = _scale;
            Rectangle rectangle = GetDimensions().ToRectangle();

            bool contains = ContainsPoint(Main.MouseScreen);
            if (contains && !PlayerInput.IgnoreMouseInterface)
            {
                Main.LocalPlayer.mouseInterface = true;
            }

            //Draw Backing
            Color color2 = Main.inventoryBack;
            Vector2 pos = rectangle.TopLeft();

            Vector2 centerPos = pos + rectangle.Size() / 2f;
            Texture2D slotTexture = ModContent.Request<Texture2D>($"{CollectionBookUISystem.RootTexturePath}CollectionTabSlot").Value;
            Texture2D questIconTexture = ModContent.Request<Texture2D>(Quest.IconTexture).Value;
            Texture2D questBackgroundTexture = ModContent.Request<Texture2D>($"{CollectionBookUISystem.RootTexturePath}QuestBackground").Value;

            spriteBatch.Draw(questBackgroundTexture, rectangle.TopLeft(), null, color2, 0f, default, _scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(slotTexture, rectangle.TopLeft(), null, color2, 0f, default, _scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(questIconTexture, rectangle.TopLeft() + questIconTexture.Size() / 4, null, Color.White, 0f, default, _scale, SpriteEffects.None, 0f);
            Main.inventoryScale = oldScale;
        }
    }
}
