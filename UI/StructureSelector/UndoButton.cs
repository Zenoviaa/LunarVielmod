using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.WorldG.StructureManager;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace Stellamod.UI.StructureSelector
{
    internal class UndoButton : UIPanel
    {
        internal const int width = 42;
        internal const int height = 56;

        public override void OnInitialize()
        {
            base.OnInitialize();

            Width.Pixels = width;
            Height.Pixels = height;
            BackgroundColor = Color.Transparent;
            BorderColor = Color.Transparent;
            OnLeftClick += OnButtonClick;
        }

        private void OnButtonClick(UIMouseEvent evt, UIElement listeningElement)
        {
            SnapshotSystem system = ModContent.GetInstance<SnapshotSystem>();
            system.Undo();
            // We can do stuff in here!
            SoundEngine.PlaySound(SoundID.MenuTick);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            BackgroundColor = Color.Transparent;
            bool contains = ContainsPoint(Main.MouseScreen);
            if (contains && !PlayerInput.IgnoreMouseInterface)
            {
                Main.LocalPlayer.mouseInterface = true;
            }
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);
            CalculatedStyle dimensions = GetDimensions();
            Point point = new Point((int)dimensions.X, (int)dimensions.Y);
            Texture2D textureToDraw;
            if (IsMouseHovering)
            {
                textureToDraw = ModContent.Request<Texture2D>(StructureSelectorUISystem.RootTexturePath + "UndoSelected").Value;
            }
            else
            {
                textureToDraw = ModContent.Request<Texture2D>(StructureSelectorUISystem.RootTexturePath + "Undo").Value;
            }
            spriteBatch.Draw(textureToDraw, new Rectangle(point.X, point.Y, textureToDraw.Width, textureToDraw.Height), null, Color.White);
        }
    }
}
