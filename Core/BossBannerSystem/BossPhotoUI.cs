using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Helpers;
using Terraria;
using Terraria.GameContent.UI.Elements;

namespace Stellamod.Core.BossBannerSystem
{
    public class BossPhotoUI : UIPanel
    {
        private readonly BossPageUI _parent;
        public BossPhotoUI(BossPageUI parent)
        {
            _parent = parent;
        }
        public Asset<Texture2D> BossPhotoTextureAsset;
        public override void OnInitialize()
        {
            base.OnInitialize();
            Width.Pixels = 376;
            Height.Pixels = 186;
        }

        public void SetBossPage(BossPage bossPage)
        {
            BossPhotoTextureAsset = bossPage.RequestBossPhoto();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);
            Asset<Texture2D> texture = BossPhotoTextureAsset;
            Rectangle rectangle = UIHelper.MouseInterfaceInteraction(this);
            spriteBatch.Draw(texture.Value, rectangle.TopLeft(), null, Color.White, 0f, default, 1, SpriteEffects.None, 0f);
        }
    }
}
