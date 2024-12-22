using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Stellamod.WorldG.StructureManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Audio;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria;

namespace Stellamod.UI.StructureSelector
{
    internal class SaveStructureButton : UIPanel
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
            string fileName = ModContent.GetInstance<StructureSelectorUISystem>().saveUIState.ui.FileName;
            StructureSelection selection = ModContent.GetInstance<StructureSelection>();
            selection.SaveSelection(fileName);
            ModContent.GetInstance<StructureSelectorUISystem>().ToggleUI();
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
                textureToDraw = ModContent.Request<Texture2D>(StructureSelectorUISystem.RootTexturePath + "SaveSelected").Value;
            }
            else
            {
                textureToDraw = ModContent.Request<Texture2D>(StructureSelectorUISystem.RootTexturePath + "Save").Value;
            }
            spriteBatch.Draw(textureToDraw, new Rectangle(point.X, point.Y, textureToDraw.Width, textureToDraw.Height), null, Color.White);
        }
    }
}