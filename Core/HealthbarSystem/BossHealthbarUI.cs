using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Core.Effects;
using Stellamod.Core.Shaders;
using Stellamod.Helpers;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using static Terraria.GameContent.Animations.IL_Actions.Sprites;

namespace Stellamod.Core.HealthbarSystem
{
    public class BossHealthbarUI : UIPanel
    {
        private UIText _bossNameText;
        private Vector2 _barFillScale;
        private Vector2 _redFillScale;
        private Vector2 _whiteFillScale;

        private float _easeInAlpha;
        private float _easeInTimer;
        private float _whiteTimer;
        private float _redTimer;
        private float _oldFill;
        public int RelativeLeft => (int)((Main.screenWidth / 2) - (Width.Pixels / 2));
        public int RelativeTop => (int)(Main.screenHeight - Height.Pixels - 64);

        public BossHealthbarUI()
        {
            _bossNameText = new UIText("Boss");
            string directory = this.GetType().DirectoryHere();

            string barPath = directory + "/Healthbar";
            BarTextureAsset = ModContent.Request<Texture2D>(barPath, ReLogic.Content.AssetRequestMode.ImmediateLoad);

            string fillPath = directory + "/HealthbarFill";
            FillTextureAsset = ModContent.Request<Texture2D>(fillPath, ReLogic.Content.AssetRequestMode.ImmediateLoad);

            string edgePath = directory + "/HealthbarEdge";
            EdgeTextureAsset = ModContent.Request<Texture2D>(edgePath, ReLogic.Content.AssetRequestMode.ImmediateLoad);

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
            Append(_bossNameText);
        }
        public Asset<Texture2D> BarTextureAsset;
        public Asset<Texture2D> FillTextureAsset;
        public Asset<Texture2D> EdgeTextureAsset;
        public Asset<Texture2D> BossFillTextureAsset;
        public Asset<Texture2D> BarMoonTextureAsset;
        public ScarletBoss TrackingNpc;
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            //Constantly lock the UI in the position regardless of resolution changes
            Left.Pixels = RelativeLeft;
            Top.Pixels = RelativeTop;
            if (IsTracking())
            {
                float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
                _easeInTimer += deltaTime;
                _easeInAlpha = EasingFunction.InOutSine(_easeInTimer / 120f);
                Top.Pixels += MathHelper.Lerp(128, 0, _easeInAlpha);
            }


            _bossNameText.Left.Pixels = 48;
            _bossNameText.Top.Pixels = -10;
        }

        private float GetFill()
        {
            if (TrackingNpc == null)
                return 1;

            float life = TrackingNpc.NPC.life;
            float lifeMax = TrackingNpc.NPC.lifeMax;
            return life / lifeMax;
        }

        private string GetBossTitle()
        {
            if (TrackingNpc == null)
                return string.Empty;
            return TrackingNpc.DisplayName.Value;
        }

        public bool IsTracking()
        {
            return TrackingNpc != null && TrackingNpc.NPC.active;
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
   
            Rectangle rectangle = GetDimensions().ToRectangle();
            Vector2 topLeft = rectangle.TopLeft();
            spriteBatch.Draw(BarTextureAsset.Value, topLeft, null, Color.White * _easeInAlpha, 0f, default, 1f, SpriteEffects.None, 0f);

            Vector2 fillTopLeft = topLeft;
            fillTopLeft.Y += 20;
            fillTopLeft.X += 50;

            float fillAmount = GetFill();
            float width = (BarTextureAsset.Width() / 2) - 30;
            Vector2 maxScale = new Vector2(width, 1);
            Vector2 scale = Vector2.Lerp(new Vector2(1, 1), maxScale, fillAmount);
            if (_oldFill != fillAmount)
            {
                _whiteTimer = 25;
                _redTimer = 10;
                _oldFill = fillAmount;
            }

            _redTimer--;
            if (_redTimer <= 0)
            {
                _redFillScale = Vector2.Lerp(_redFillScale, scale, 0.1f);
            }

            _whiteTimer--;
            if (_whiteTimer <= 0)
            {
                _whiteFillScale = Vector2.Lerp(_whiteFillScale, scale, 0.1f);
            }
            _barFillScale = Vector2.Lerp(_barFillScale, scale, 0.1f);


            spriteBatch.Draw(FillTextureAsset.Value, fillTopLeft, null, Color.White * 0.25f * _easeInAlpha, 0f, default, _whiteFillScale, SpriteEffects.None, 0f);
            spriteBatch.Draw(FillTextureAsset.Value, fillTopLeft, null, Color.Red * _easeInAlpha, 0f, default, _redFillScale, SpriteEffects.None, 0f);
            if (IsTracking())
            {
                _bossNameText.SetText(GetBossTitle());
                Asset<Texture2D> bossIconTexture = ModContent.Request<Texture2D>(TrackingNpc.Texture_BossIcon);
                Asset<Texture2D> bossFillTexture = ModContent.Request<Texture2D>(TrackingNpc.Texture_BossBar);
                spriteBatch.Draw(bossIconTexture.Value, topLeft + new Vector2(50, 58) / 2 + new Vector2(2), null, Color.White * _easeInAlpha, 0f, bossIconTexture.Size() / 2, 1f, SpriteEffects.None, 0f);

                var shader = BossHealthbarShader.Instance;
                shader.InnerColor = Color.Transparent;
                shader.OuterColor = Color.White;
                shader.NoiseTexture = AssetRegistry.Textures.Noise.Perlin;

                spriteBatch.Draw(bossFillTexture.Value, fillTopLeft, null, Color.White * _easeInAlpha, 0f, default, _barFillScale, SpriteEffects.None, 0f);


                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, default, default, default, shader.Effect, Main.UIScaleMatrix);

        
                spriteBatch.Draw(bossFillTexture.Value, fillTopLeft, null, Color.White * _easeInAlpha, 0f, default, _barFillScale, SpriteEffects.None, 0f);
                spriteBatch.Draw(bossFillTexture.Value, fillTopLeft, null, Color.White * _easeInAlpha, 0f, default, _barFillScale, SpriteEffects.None, 0f);

                spriteBatch.End();
                spriteBatch.Begin(default, default, Main.DefaultSamplerState, DepthStencilState.Default, Main.Rasterizer, default, Main.UIScaleMatrix);

                Vector2 offset = new Vector2(_barFillScale.X * 2, 0);
                Vector2 edgeDrawPos = fillTopLeft + offset;
                spriteBatch.Draw(EdgeTextureAsset.Value, edgeDrawPos, null, Color.White * _easeInAlpha, 0f, default, 1f, SpriteEffects.None, 0f);
                for(float f = 0; f < 1f; f += 0.1f)
                {
                    Vector2 o = Vector2.UnitY.RotatedBy(f * MathHelper.TwoPi);
                    o *= ExtraMath.Osc(1, 2);
                    Vector2 drawPos = edgeDrawPos + o;
                    spriteBatch.Draw(EdgeTextureAsset.Value, drawPos, null, Color.White * 0.25f * _easeInAlpha, 0f, default, 1f, SpriteEffects.None, 0f);
                }
            }


            spriteBatch.Draw(BarMoonTextureAsset.Value, topLeft, null, Color.White * _easeInAlpha, 0f, default, 1f, SpriteEffects.None, 0f);
        }
    }
}
