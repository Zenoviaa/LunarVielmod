using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Common.QuestSystem;
using System;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.ModLoader;
using Terraria.ModLoader.UI.Elements;

namespace Stellamod.UI.CollectionSystem.Quests
{
    internal class ActiveQuestUI : UIPanel
    {
        private float _scale;
        private UIGrid _slotGrid;
        private UIText _descriptionText;
        private UIText _objectiveText;

        internal const int width = 480;
        internal const int height = 155;

        internal int RelativeLeft => Main.screenWidth / 2 - width / 2 + 280;
        internal int RelativeTop => Main.screenHeight / 2 - height / 2 - 196;

        public ActiveQuestUI(float scale = 1f) : base()
        {
            _scale = scale;
            //Set to air
            Glow = 1f;
        }

        public Quest Quest { get; set; }
        public float Glow { get; set; }
        public override void OnInitialize()
        {
            base.OnInitialize();
            Width.Pixels = 48 * 6f;
            Height.Pixels = 48 * 9;
            Left.Pixels = RelativeLeft;
            Top.Pixels = RelativeTop;
            BackgroundColor = Color.Transparent;
            BorderColor = Color.Transparent;

            _slotGrid = new UIGrid();
            _slotGrid.Width.Set(0, 1f);
            _slotGrid.Height.Set(0, 1f);
            _slotGrid.ListPadding = 2f;
            _slotGrid.Top.Pixels = 352;
            Append(_slotGrid);

            _descriptionText = new UIText("This is placeholder text", large: false);
            _descriptionText.Height.Pixels = Height.Pixels;
            _descriptionText.Width.Pixels = Width.Pixels;
            _descriptionText.IsWrapped = true;
            _descriptionText.Top.Pixels = 160;
            Append(_descriptionText);

            _objectiveText = new UIText("This is placeholder text", large: false);
            _objectiveText.Height.Pixels = Height.Pixels;
            _objectiveText.Width.Pixels = Width.Pixels;
            _objectiveText.IsWrapped = true;
            _objectiveText.Top.Pixels = 300;
            _objectiveText.ShadowColor = Color.Black;
            Append(_objectiveText);
        }

        public override void Recalculate()
        {
            //Recalculate the UI when there is some sort of update
            Left.Pixels = RelativeLeft;
            Top.Pixels = RelativeTop;
            _slotGrid?.Clear();
            if (Main.gameMenu)
                return;

            //We just need to get the number of unique materials since that's how we're sorting things

            Item[] rewards = Quest.Rewards.ToArray();

            for (int i = 0; i < rewards.Length; i++)
            {
                Item craft = rewards[i];
                QuestRewardSlot slot = new QuestRewardSlot();
                slot.Item = craft;
                slot.Glow = Glow;
                _slotGrid.Add(slot);
            }

            _slotGrid.Recalculate();
            base.Recalculate();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if(_descriptionText != null && Quest != null)
            {

                _descriptionText.SetText(Quest.Description);
            }

            if (_objectiveText != null && Quest != null)
            {

                _objectiveText.TextColor = Color.Lerp(Color.White, Main.DiscoColor, 0.2f);
                _objectiveText.SetText(Quest.Objective);
            }

    
            //Constantly lock the UI in the position regardless of resolution changes
            Left.Pixels = RelativeLeft;
            Top.Pixels = RelativeTop;
            Glow *= 0.95f;
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);
            //Draw Background
            //Draw Big Picture
            //Text

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
            Texture2D backgroundTexture = ModContent.Request<Texture2D>($"{CollectionBookUISystem.RootTexturePath}QuestImageBackground").Value;
            Texture2D bigPictureTexture = ModContent.Request<Texture2D>(Quest.BigTexture).Value;

            Vector2 backgroundDrawOffset = new Vector2(Width.Pixels / 2, 96);
            backgroundDrawOffset -= backgroundTexture.Size() / 2;

            Vector2 portraitDrawOffset = new Vector2(Width.Pixels / 2, 96);
            portraitDrawOffset -= bigPictureTexture.Size() / 2;
            spriteBatch.Draw(backgroundTexture, rectangle.TopLeft() + backgroundDrawOffset, null, color2, 0f, default, _scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(bigPictureTexture, rectangle.TopLeft() + portraitDrawOffset, null, color2, 0f, default, _scale, SpriteEffects.None, 0f);
            Main.inventoryScale = oldScale;
        }
    }
}
