using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Core.Helpers;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;

namespace Stellamod.Core.HealthbarSystem
{
    internal class BossHealthbarUI : UIPanel
    {
        private Vector2 _barFillScale;
        private Vector2 _redFillScale;
        private Vector2 _whiteFillScale;
        private float _whiteTimer;
        private float _redTimer;
        private float _oldFill;
        internal int RelativeLeft => (int)((Main.screenWidth / 2) - (Width.Pixels / 2));
        internal int RelativeTop => (int)(Main.screenHeight - Height.Pixels - 64);

        public BossHealthbarUI()
        {
            string directory = this.GetType().DirectoryHere();

            string barPath = directory + "/Healthbar";
            BarTextureAsset = ModContent.Request<Texture2D>(barPath, ReLogic.Content.AssetRequestMode.ImmediateLoad);

            string fillPath = directory + "/HealthbarFill";
            FillTextureAsset = ModContent.Request<Texture2D>(fillPath, ReLogic.Content.AssetRequestMode.ImmediateLoad);

            string barMoonPath = directory + "/HealthbarMoon";
            BarMoonTextureAsset = ModContent.Request<Texture2D>(barMoonPath, ReLogic.Content.AssetRequestMode.ImmediateLoad);

            BossFillTextureAsset = ModContent.Request<Texture2D>(fillPath, ReLogic.Content.AssetRequestMode.ImmediateLoad);
        }
        public override void OnInitialize()
        {
            base.OnInitialize();
            Width.Pixels = BarTextureAsset.Width();
            Height.Pixels = BarTextureAsset.Height();
            Left.Pixels = RelativeLeft;
            Top.Pixels = RelativeTop;
        }
        public Asset<Texture2D> BarTextureAsset;
        public Asset<Texture2D> FillTextureAsset;
        public Asset<Texture2D> BossFillTextureAsset;
        public Asset<Texture2D> BarMoonTextureAsset;
        public ScarletBoss TrackingNpc;
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            //Constantly lock the UI in the position regardless of resolution changes
            Left.Pixels = RelativeLeft;
            Top.Pixels = RelativeTop;
        }

        private float GetFill()
        {
            if (TrackingNpc == null)
                return 1;

            float life = TrackingNpc.NPC.life;
            float lifeMax = TrackingNpc.NPC.lifeMax;
            return life / lifeMax;
        }
        public bool IsTracking()
        {
            return TrackingNpc != null && TrackingNpc.NPC.active;
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Rectangle rectangle = GetDimensions().ToRectangle();
            Vector2 topLeft = rectangle.TopLeft();
            spriteBatch.Draw(BarTextureAsset.Value, topLeft, null, Color.White, 0f, default, 1f, SpriteEffects.None, 0f);

            Vector2 fillTopLeft = topLeft;
            fillTopLeft.Y += 20;
            fillTopLeft.X += 50;

            float fillAmount = GetFill();
            float width = (BarTextureAsset.Width() / 2) - 30;
            Vector2 maxScale = new Vector2(width, 1);
            Vector2 scale = Vector2.Lerp(new Vector2(1, 1), maxScale, fillAmount);
            if(_oldFill != fillAmount)
            {
                _whiteTimer = 100;
                _redTimer = 10;
                _oldFill = fillAmount;
            }

            _redTimer--;
            if(_redTimer <= 0)
            {
                _redFillScale = Vector2.Lerp(_redFillScale, scale, 0.1f);
            }

            _whiteTimer--;
            if(_whiteTimer <= 0)
            {
                _whiteFillScale = Vector2.Lerp(_whiteFillScale, scale, 0.1f);
            }
            _barFillScale = Vector2.Lerp(_barFillScale, scale, 0.1f);


            spriteBatch.Draw(FillTextureAsset.Value, fillTopLeft, null, Color.White, 0f, default, _whiteFillScale, SpriteEffects.None, 0f);
            spriteBatch.Draw(FillTextureAsset.Value, fillTopLeft, null, Color.Red, 0f, default, _redFillScale, SpriteEffects.None, 0f);
            if (IsTracking())
            {
                Asset<Texture2D> bossIconTexture = ModContent.Request<Texture2D>(TrackingNpc.Texture_BossIcon);
                spriteBatch.Draw(bossIconTexture.Value, topLeft + new Vector2(50, 58) / 2 + new Vector2(2), null, Color.White, 0f, bossIconTexture.Size() / 2, 1f, SpriteEffects.None, 0f);

                Asset<Texture2D> bossFillTexture = ModContent.Request<Texture2D>(TrackingNpc.Texture_BossBar);
                spriteBatch.Draw(bossFillTexture.Value, fillTopLeft, null, Color.White, 0f, default, _barFillScale, SpriteEffects.None, 0f);
            }
           

            spriteBatch.Draw(BarMoonTextureAsset.Value, topLeft, null, Color.White, 0f, default, 1f, SpriteEffects.None, 0f);
        }
    }
}
