using Microsoft.Xna.Framework;
using Stellamod.Core.Particles;
using Stellamod.Helpers;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.GameContent.UI.Elements;

namespace Stellamod.Common.WeaponUpgrade.UI
{
    public class WeaponUpgradeUI : UIPanel
    {
        private float _particleSpawnTimer;
        private UIPanel _panel;
   
        public FurnaceBackground upgradeBackground;
        public WeaponUpgradeSlot reforgeSlot;
        public UpgradeButton reforgeButton;
        public MaterialToUse pearl;

        public int RelativeLeft => Main.screenWidth / 2 - (int)(Width.Pixels / 2) - 64;
        public int RelativeTop => Main.screenHeight / 2 - (int)(Height.Pixels / 2) - 64;
        public override void OnInitialize()
        {
            base.OnInitialize();
            Width.Pixels = 252;
            Height.Pixels = 252;
            Left.Pixels = RelativeLeft;
            Top.Pixels = RelativeTop;
            BackgroundColor = Color.Transparent;
            BorderColor = Color.Transparent;

            upgradeBackground = new FurnaceBackground();
            Append(upgradeBackground);

            _panel = new UIPanel();
            _panel.Width.Pixels = Width.Pixels;
            _panel.Height.Pixels = Height.Pixels;
            _panel.BackgroundColor = Color.Transparent;
            _panel.BorderColor = Color.Transparent;
            Append(_panel);

            reforgeSlot = new WeaponUpgradeSlot();
            
            float pixels = 64;
            reforgeSlot.Left.Set(0, 0);
            reforgeSlot.Top.Set(0, 0);
            reforgeSlot.Left.Pixels = pixels;
            reforgeSlot.Top.Pixels = pixels;
            _panel.Append(reforgeSlot);

            reforgeButton = new UpgradeButton();
            reforgeButton.Left.Set(0, 0f);
            reforgeButton.Top.Set(0, 0f);
            reforgeButton.Left.Pixels = 32;
            reforgeButton.Top.Pixels = 48;
            _panel.Append(reforgeButton);

            pearl = new MaterialToUse();
            pearl.Left.Set(0, 1f);
            pearl.Top.Set(0, 0f);
            pearl.Left.Pixels = -32;
            pearl.Top.Pixels = 64;
            _panel.Append(pearl);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            Left.Pixels = RelativeLeft;
            Top.Pixels = RelativeTop;
            Top.Pixels += ExtraMath.Osc(0f, 4f);
            if (Main.gameInactive)
                return;

            _particleSpawnTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if(_particleSpawnTimer > 0.5f)
            {
                Rectangle spawnRect = GetDimensions().ToRectangle();
                Vector2 pos = new Vector2();
                pos.X = Main.rand.Next(spawnRect.Left, spawnRect.Right);
                pos.Y = Main.rand.Next(spawnRect.Top, spawnRect.Bottom);
                Vector2 velocity = -Vector2.UnitY * Main.rand.NextFloat(3f, 7f);
                velocity = velocity.RotatedByRandom(MathHelper.ToRadians(75));
                DustParticle dp = DustParticle.SpawnInUI(pos, velocity, Color.White, Scale: 0.5f);
                dp.innerColor = Color.Lerp(Color.Yellow, Color.Red, Main.rand.NextFloat(0f, 1f));
                dp.outerColor = Color.Red;
                _particleSpawnTimer -= 0.1f;
            }

            //idk why i originally did this, but changing it will break all the ui so just leave it
            upgradeBackground.Left.Set(0, 0.1f);
            upgradeBackground.Top.Set(0, 0.2f);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);
        }
        protected override void DrawChildren(SpriteBatch spriteBatch)
        {
            base.DrawChildren(spriteBatch);
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
          //  spriteBatch.GraphicsDevice.SetRenderTarget(new RenderTarget2D(spriteBatch.GraphicsDevice, 10, 10));
           //spriteBatch.GraphicsDevice.SetRenderTarget(null);
        }
    }
}
