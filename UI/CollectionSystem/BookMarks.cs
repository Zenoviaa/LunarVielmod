using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameInput;
using Terraria.ModLoader;
using Terraria.UI;

namespace Stellamod.UI.CollectionSystem
{
    internal abstract class BaseBookMark : UIElement
    {
        private readonly float _scale = 1f;
        public virtual string TextureAsset { get; }
        internal BaseBookMark()
        {
            float scale = 1f;
            var asset = ModContent.Request<Texture2D>(
                $"{CollectionBookUISystem.RootTexturePath}{TextureAsset}", ReLogic.Content.AssetRequestMode.ImmediateLoad);
            Width.Set(asset.Width() * scale, 0f);
            Height.Set(asset.Height() * scale, 0f);
            OnLeftClick += OnButtonClick;
            OnMouseOver += OnMouseHover;
        }

        protected abstract void Trigger(CollectionBookUISystem uiSystem);
        private void OnButtonClick(UIMouseEvent evt, UIElement listeningElement)
        {
            CollectionBookUISystem uiSystem = ModContent.GetInstance<CollectionBookUISystem>();
            Trigger(uiSystem);
            // We can do stuff in here!
        }

        private void OnMouseHover(UIMouseEvent evt, UIElement listeningElement)
        {

        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            bool contains = ContainsPoint(Main.MouseScreen);
            if (contains && !PlayerInput.IgnoreMouseInterface)
            {
                Main.LocalPlayer.mouseInterface = true;
            }

        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            CalculatedStyle dimensions = GetDimensions();
            Point point = new Point((int)dimensions.X, (int)dimensions.Y);
            Texture2D textureToDraw;
            if (IsMouseHovering)
            {
                textureToDraw = ModContent.Request<Texture2D>($"{CollectionBookUISystem.RootTexturePath}{TextureAsset}Selected").Value;
            }
            else
            {
                textureToDraw = ModContent.Request<Texture2D>($"{CollectionBookUISystem.RootTexturePath}{TextureAsset}").Value;
            }


            Color drawColor = Color.White;
            Rectangle rect = new Rectangle(point.X, point.Y, textureToDraw.Width, textureToDraw.Height);
            CollectionBookUISystem uiSystem = ModContent.GetInstance<CollectionBookUISystem>();
            rect.Location += uiSystem.collectionBookUI.bookUI.book.Offset.ToPoint();
            float rotation = 0;
            spriteBatch.Draw(textureToDraw, rect, null, drawColor, rotation, Vector2.Zero, SpriteEffects.None, 0);
        }
    }


    //Doing it this way to save copy and pasting
    internal class CollectionTab : BaseBookMark
    {
        public CollectionTab() : base() { }
        public override string TextureAsset => "CollectionTab";
        protected override void Trigger(CollectionBookUISystem uiSystem)
        {
            uiSystem.OpenCollectionTabUI();
        }
    }


    internal class LoreTab : BaseBookMark
    {
        public LoreTab() : base() { }
        public override string TextureAsset => "LoreTab";
        protected override void Trigger(CollectionBookUISystem uiSystem)
        {
            uiSystem.OpenLoreTabUI();
        }
    }


    internal class QuestTab : BaseBookMark
    {
        public QuestTab() : base() { }
        public override string TextureAsset => "QuestTab";
        protected override void Trigger(CollectionBookUISystem uiSystem)
        {
            uiSystem.OpenQuestsTabUI();
        }
    }
}
