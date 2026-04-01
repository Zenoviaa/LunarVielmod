using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Helpers;
using Stellamod.UI.CollectionSystem;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;

namespace Stellamod.Common.BossBannerSystem
{
    /// <summary>
    /// Opens a boss page
    /// </summary>
    public class BossButton : UIPanel
    {
        private BossPage _bossPage;
        private BossPageUI _ui;
        public BossButton(BossPageUI ui, BossPage bossPage)
        {
            _ui = ui;
            _bossPage = bossPage;
            OnLeftClick += OpenBossPage;
        }

        public override void OnInitialize()
        {
            base.OnInitialize();
            Width.Pixels = 36;
            Height.Pixels = 36;
            BackgroundColor = Color.Transparent;
            BorderColor = Color.Transparent;
        }

        private void OpenBossPage(UIMouseEvent evt, UIElement listeningElement)
        {
            CollectionBookUISystem uiSystem = ModContent.GetInstance<CollectionBookUISystem>();
            uiSystem.OpenBossPageUI(_bossPage);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);
            Rectangle rectangle = UIHelper.MouseInterfaceInteraction(this);
            Vector2 topLeft = rectangle.TopLeft();
            Asset<Texture2D> bossIcon = _bossPage.RequestBossIcon();
            float drawScale = 0.5f;
            Color drawColor = _bossPage.IsHidden() ? Color.Black : Color.White;
            if (_bossPage.HasUnclaimedRewards())
            {
                UIHelper.QuickOutline(spriteBatch, bossIcon.Value, topLeft, Main.DiscoColor, drawScale);
            }

            if (IsMouseHovering)
            {
                UIHelper.QuickOutline(spriteBatch, bossIcon.Value, topLeft, Color.Yellow, drawScale);
            }
    
            Width.Pixels = bossIcon.Width() * drawScale;
            Height.Pixels = bossIcon.Height() * drawScale;
            spriteBatch.Draw(bossIcon.Value, topLeft, null, drawColor, 0, Vector2.Zero, drawScale, SpriteEffects.None, 0);
          
        }
    }
}
