using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Helpers;
using Stellamod.UI.CollectionSystem;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;

namespace Stellamod.Common.BossBannerSystem
{
    public class BossPhotoUI : UIPanel
    {
        private readonly BossPageUI _parent;
        public BossPhotoUI(BossPageUI parent)
        {
            _parent = parent;
        }
        public Asset<Texture2D> BossIconTextureAsset;
        public Asset<Texture2D> BossPhotoTextureAsset;
        public override void OnInitialize()
        {
            base.OnInitialize();
            Width.Pixels = 376;
            Height.Pixels = 186;
            BackgroundColor = Color.Transparent;
            BorderColor = Color.Transparent;
        }

        public void SetBossPage(BossPage bossPage)
        {
            BossPhotoTextureAsset = bossPage.RequestBossPhoto();
            BossIconTextureAsset = bossPage.RequestBossIcon();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {

            base.DrawSelf(spriteBatch);

            Rectangle rectangle = UIHelper.MouseInterfaceInteraction(this);
            BackgroundColor = Color.Transparent;
            Asset<Texture2D> texture = BossPhotoTextureAsset;

            Color color2 = Main.inventoryBack;
            Vector2 pos = rectangle.TopLeft();
            pos.X -= 32;

            bool isHidden = _parent.BossPage.IsHidden();
            if (isHidden)
            {
                texture = BossBanner.RequestFogTexture();
            }

            Texture2D backgroundTexture = ModContent.Request<Texture2D>($"{CollectionBookUISystem.RootTexturePath}QuestImageBackground").Value;
            Texture2D bigPictureTexture = texture.Value;
            if (bigPictureTexture == null)
                bigPictureTexture = ModContent.Request<Texture2D>($"{CollectionBookUISystem.RootTexturePath}MissingBigImage").Value;
            Texture2D overlayTexture = ModContent.Request<Texture2D>($"{CollectionBookUISystem.RootTexturePath}QuestTop").Value;

            //Draw the background thingy
            Vector2 backgroundDrawOffset = new Vector2(Width.Pixels / 2, 96);
            backgroundDrawOffset -= backgroundTexture.Size() / 2;
            spriteBatch.Draw(backgroundTexture, pos + backgroundDrawOffset, null, color2, 0f, default, 1, SpriteEffects.None, 0f);

            //Draw the big picture for the portrait in the quest book
            float scale = 0.86f;
            Vector2 portraitDrawOffset = new Vector2(Width.Pixels / 2, 96);
            portraitDrawOffset -= bigPictureTexture.Size() / 2;
            portraitDrawOffset *= scale / 2;
            portraitDrawOffset.Y += 12;
            spriteBatch.Draw(bigPictureTexture, pos + portraitDrawOffset, null, Color.White, 0f, default, 1 * scale, SpriteEffects.None, 0f);


            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, default, default, default, default, Main.UIScaleMatrix);

            Vector2 overlayDrawOffset = new Vector2(Width.Pixels / 2, 96);
            overlayDrawOffset -= overlayTexture.Size() / 2;
            overlayDrawOffset *= scale / 2;
            overlayDrawOffset.Y += 12;
            spriteBatch.Draw(overlayTexture, pos + overlayDrawOffset, null, Color.White, 0f, default, 1 * scale, SpriteEffects.None, 0f);

            spriteBatch.End();
            spriteBatch.Begin(default, default, default, default, default, default, Main.UIScaleMatrix);



            Texture2D zuiGlow = ModContent.Request<Texture2D>(TextureRegistry.ZuiEffect).Value;
            Color zuiGlowColor = Color.White;
            zuiGlowColor.A = 0;

            Vector2 topLeft = rectangle.TopLeft();
            Vector2 glowPos = topLeft + BossIconTextureAsset.Size() / 2f;
            glowPos.Y += 8;
            zuiGlowColor *= ExtraMath.Osc(0.5f, 1f);
            spriteBatch.Draw(zuiGlow, glowPos, null, zuiGlowColor, 0f, zuiGlow.Size() / 2f, 0.5f, SpriteEffects.None, 0f);

            Color drawColor = Color.White;
            if (isHidden)
            {
                drawColor = Color.Black;
            }

            if (_parent.BossPage.HasUnclaimedRewards())
            {
                UIHelper.QuickOutline(spriteBatch, BossIconTextureAsset.Value, topLeft, Main.DiscoColor, 1);
            }
            spriteBatch.Draw(BossIconTextureAsset.Value, topLeft, null, drawColor, 0f, default, 1, SpriteEffects.None, 0f);

        }
    }
}
