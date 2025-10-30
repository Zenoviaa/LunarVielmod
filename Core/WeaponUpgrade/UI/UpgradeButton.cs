using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace Stellamod.Core.WeaponUpgrade.UI
{
    public class UpgradeButton : UIPanel
    {
        public event Action<int> OnEmptyMouseover;
        private readonly float _scale = 1f;
        public UpgradeButton()
        {
            float scale = 1f;
            var asset = ModContent.Request<Texture2D>(
                $"{WeaponUpgradeUISystem.RootTexturePath}ReforgeButton", ReLogic.Content.AssetRequestMode.ImmediateLoad);
            Width.Set(asset.Width() * scale, 0f);
            Height.Set(asset.Height() * scale, 0f);
            OnLeftClick += OnButtonClick;
            OnMouseOver += OnMouseHover;
        }

        private void OnButtonClick(UIMouseEvent evt, UIElement listeningElement)
        {
            WeaponUpgradeUISystem uiSystem = ModContent.GetInstance<WeaponUpgradeUISystem>();
            if (uiSystem.CanReforge())
            {
                uiSystem.Reforge();
                SoundStyle sound = SoundID.Item37;
                SoundEngine.PlaySound(sound);
                FXUtil.ShakeCamera(Main.LocalPlayer.position, 1024, 8);
            }

            // We can do stuff in here!
        }

        private void OnMouseHover(UIMouseEvent evt, UIElement listeningElement)
        {

        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {

            CalculatedStyle dimensions = GetDimensions();
            Point point = new Point((int)dimensions.X, (int)dimensions.Y);
            Texture2D textureToDraw;
            if (IsMouseHovering)
            {
                textureToDraw = ModContent.Request<Texture2D>($"{WeaponUpgradeUISystem.RootTexturePath}ReforgeButtonSelected").Value;
            }
            else
            {
                textureToDraw = ModContent.Request<Texture2D>($"{WeaponUpgradeUISystem.RootTexturePath}ReforgeButton").Value;
            }
            bool contains = ContainsPoint(Main.MouseScreen);
            if (contains && !PlayerInput.IgnoreMouseInterface)
            {
                Main.LocalPlayer.mouseInterface = true;
            }
            Color drawColor = Color.White;
            WeaponUpgradeUISystem uiSystem = ModContent.GetInstance<WeaponUpgradeUISystem>();

            //Grey out when crafting won't make anything
            if (!uiSystem.CanReforge())
                drawColor = drawColor.MultiplyRGB(Color.Gray);

            Rectangle rect = new Rectangle(point.X, point.Y, textureToDraw.Width, textureToDraw.Height);
            rect.Location += new Point(0, (int)VectorHelper.Osc(-8f, 8f, 1f));
            float rotation = 0;


            spriteBatch.Draw(textureToDraw, rect, null, drawColor, rotation, Vector2.Zero, SpriteEffects.None, 0);
        }
    }
}