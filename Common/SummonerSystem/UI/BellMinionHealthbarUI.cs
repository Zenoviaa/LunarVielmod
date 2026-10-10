using Microsoft.Xna.Framework.Graphics.PackedVector;
using ReLogic.Content;
using Stellamod.Helpers;
using Stellamod.Trails;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI.Chat;
using static Terraria.GameContent.Animations.IL_Actions.Sprites;

namespace Stellamod.Common.SummonerSystem.UI
{
    public class BellMinionHealthbarUI : UIPanel
    {
        private List<AbstractBellSummon> _minions;
        private Asset<Texture2D> _healthBarTextureAsset;
        public BellMinionHealthbarUI() : base()
        {
            _healthBarTextureAsset = ModContent.Request<Texture2D>(this.GetType().DirectoryHere() + "/HealthBar");
            _minions = new List<AbstractBellSummon>();
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            _minions.Clear();
            Player player = Main.LocalPlayer;
            foreach(var projectile in Main.ActiveProjectiles)
            {
                if (projectile.owner != player.whoAmI)
                    continue;
                if(projectile.ModProjectile is AbstractBellSummon bellSummon)
                {
         
                    _minions.Add(bellSummon);
                }
            }
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            //  base.DrawSelf(spriteBatch);
            var config = ModContent.GetInstance<LunarVeilClientConfig>();
            if (config.DisableSummonHealthbar)
                return;

            Vector2 topLeft = GetDimensions().ToRectangle().TopLeft();
            float yOffsetPer = 48;
            int repeats = 48;
            var maxBarRect = new Rectangle(0, 0, 48 * 2, _healthBarTextureAsset.Value.Height / 2);
            var maxBarWhiteRect = new Rectangle(0, _healthBarTextureAsset.Value.Height / 2, 48 * 2, _healthBarTextureAsset.Value.Height / 2);

            Vector2 GetOffset(in int i)
            {
                return i * yOffsetPer * -Vector2.UnitY;
            }
            using (spriteBatch.Ctx(spriteBatch.Parameters with { samplerState = SamplerState.AnisotropicWrap }))
            {
                //Draw Display Names
                for (int i = 0; i < _minions.Count; i++)
                {
                    var bellSummon = _minions[i];
                    var offset = GetOffset(i);
                    ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, bellSummon.DisplayName.Value,
                        topLeft + offset - new Vector2(2, 24), Color.White, 0, Vector2.Zero, Vector2.One);
                }

                //Draw Bars
                for (int i = 0; i < _minions.Count; i++)
                {
                    var bellSummon = _minions[i];
                    var minion = bellSummon.GetAttachedNPC();
                    var healthPercent = (float)minion.life / (float)minion.lifeMax;
                    var offset = GetOffset(i);
                    var rect = maxBarRect;

                    var backgroundRect = maxBarWhiteRect;
                    backgroundRect.Width += 4;

                    var shrunkenBackgroundRect = backgroundRect;
                    shrunkenBackgroundRect.Width = (int)((float)backgroundRect.Width * healthPercent);

                    spriteBatch.Draw(_healthBarTextureAsset.Value, topLeft + offset + new Vector2(-2, 0), backgroundRect, Color.DarkGray, 0, Vector2.Zero, new Vector2(1f, 1f), SpriteEffects.None, 0);
                    spriteBatch.Draw(_healthBarTextureAsset.Value, topLeft + offset + new Vector2(-2, 0), shrunkenBackgroundRect, Color.White, 0, Vector2.Zero, new Vector2(1f, 1f), SpriteEffects.None, 0);
                    spriteBatch.Draw(_healthBarTextureAsset.Value, topLeft + offset , rect, Color.Black, 0, Vector2.Zero, new Vector2(1f, 1f), SpriteEffects.None, 0);

                    var healthColor = Color.Lerp(Color.Red, Color.White, healthPercent);
                    var shrunkenRect = rect;
                    shrunkenRect.Width = (int)((float)rect.Width * healthPercent);
                    spriteBatch.Draw(_healthBarTextureAsset.Value, topLeft + offset, shrunkenRect, healthColor, 0, Vector2.Zero, new Vector2(1f, 1f), SpriteEffects.None, 0);
                }

                for (int i = 0; i < _minions.Count; i++)
                {
                    var bellSummon = _minions[i];
                    var minion = bellSummon.GetAttachedNPC();
                    var healthPercent = (float)minion.life / (float)minion.lifeMax;
                    var offset = GetOffset(i);
                    var minionFraming = bellSummon.GetIcon();
                    spriteBatch.Draw(minionFraming.texture, topLeft + offset - new Vector2(16, 0), minionFraming.frame, Color.White, 0, minionFraming.frame.Size() / 2f, 1, SpriteEffects.None, 0);       
                }
            }
        }

    }
}
