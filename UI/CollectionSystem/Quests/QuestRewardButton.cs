using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Common.QuestSystem;
using System;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.ModLoader;
using Terraria.UI;

namespace Stellamod.UI.CollectionSystem.Quests
{
    internal class QuestRewardButton : UIPanel
    {
        private UIText _text;
        internal event Action<int> OnEmptyMouseover;
        private readonly float _scale = 1f;
        internal QuestRewardButton()
        {
            float scale = 1f;
            var asset = ModContent.Request<Texture2D>(
                $"{CollectionBookUISystem.RootTexturePath}QuestRewardButton", ReLogic.Content.AssetRequestMode.ImmediateLoad);

    
            Width.Set(asset.Width() * scale, 0f);
            Height.Set(asset.Height() * scale, 0f);
            OnLeftClick += OnButtonClick;
            OnMouseOver += OnMouseHover;
        }

        public override void OnInitialize()
        {
            base.OnInitialize();
            _text = new UIText("Collect", large: false);
            _text.Height.Pixels = Height.Pixels;
            _text.Width.Pixels = Width.Pixels;
            _text.Left.Pixels = 12;
            _text.Top.Pixels = -2;
            _text.HAlign = 0.5f;
            _text.IsWrapped = false;
            _text.ShadowColor = Color.Black;
            _text.TextColor = Color.Goldenrod;
            Append(_text);
        }

        public Quest Quest { get; set; }
        private void OnButtonClick(UIMouseEvent evt, UIElement listeningElement)
        {
            QuestPlayer questPlayer = Main.LocalPlayer.GetModPlayer<QuestPlayer>();
            questPlayer.CollectQuestReward(Quest);

            // We can do stuff in here!
        }

        private void OnMouseHover(UIMouseEvent evt, UIElement listeningElement)
        {

        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            Top.Pixels = 380;
            bool contains = ContainsPoint(Main.MouseScreen);
            if (contains && !PlayerInput.IgnoreMouseInterface)
            {
                Main.LocalPlayer.mouseInterface = true;
            }

            QuestPlayer questPlayer = Main.LocalPlayer.GetModPlayer<QuestPlayer>();
            if (questPlayer.CompletedQuests.Contains(Quest))
            {
                if(_text != null)
                    _text.SetText("");
            }
            else
            {
                if (_text != null)
                    _text.SetText("Collect");
            }
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            QuestPlayer questPlayer = Main.LocalPlayer.GetModPlayer<QuestPlayer>();
            if (questPlayer.CompletedQuests.Contains(Quest))
                return;

            CalculatedStyle dimensions = GetDimensions();
            Point point = new Point((int)dimensions.X, (int)dimensions.Y);
            Texture2D textureToDraw;
            if (IsMouseHovering)
            {
                textureToDraw = ModContent.Request<Texture2D>($"{CollectionBookUISystem.RootTexturePath}QuestRewardButtonSelected").Value;
            }
            else
            {
                textureToDraw = ModContent.Request<Texture2D>($"{CollectionBookUISystem.RootTexturePath}QuestRewardButton").Value;
            }


            Color drawColor = Color.White;
            Rectangle rect = new Rectangle(point.X, point.Y, textureToDraw.Width, textureToDraw.Height);
            Vector2 drawPos = new Vector2(rect.TopLeft().X, rect.TopLeft().Y);
            float rotation = 0;
            spriteBatch.Draw(textureToDraw, drawPos, null, drawColor, rotation, Vector2.Zero, 1.5f, SpriteEffects.None, 0);
        }
    }
}
