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
            for(int i = 0; i < _minions.Count; i++)
            {
                AbstractBellSummon bellSummon = _minions[i];
                NPC minion = bellSummon.GetAttachedNPC();
                float healthPercent = (float)minion.life / (float)minion.lifeMax;
                Vector2 offset = i * yOffsetPer * -Vector2.UnitY;
                Color healthColor = Color.Lerp(Color.Red, Color.ForestGreen, healthPercent);

                ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, bellSummon.DisplayName.Value,
                    topLeft + offset - new Vector2(16, 24), Color.White, 0, Vector2.Zero, Vector2.One);


                Texture2D minionIcon = TextureAssets.Projectile[bellSummon.Type].Value;
                Vector2 scale = new Vector2(1f, 0.3f);

                Rectangle frame = bellSummon.Projectile.Frame();
                spriteBatch.Draw(minionIcon, topLeft + offset - new Vector2(frame.Width, 0), frame, Color.White, 0, frame.Size() / 2f, 1, SpriteEffects.None, 0);
                for (int r = 0; r < repeats; r++)
                {
                    Vector2 xOffset = Vector2.UnitX * r * _healthBarTextureAsset.Width();
                    spriteBatch.Draw(_healthBarTextureAsset.Value, topLeft + offset + xOffset - new Vector2(0, 2), null, Color.White, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
                    spriteBatch.Draw(_healthBarTextureAsset.Value, topLeft + offset + xOffset - new Vector2(0, -2), null, Color.White, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
                    spriteBatch.Draw(_healthBarTextureAsset.Value, topLeft + offset + xOffset - new Vector2(2, 0), null, Color.White, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
                    spriteBatch.Draw(_healthBarTextureAsset.Value, topLeft + offset + xOffset - new Vector2(-2, 0), null, Color.White, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
                }
                for (int r = 0; r < repeats; r++)
                {
                    Vector2 xOffset = Vector2.UnitX * r * _healthBarTextureAsset.Width();
                    spriteBatch.Draw(_healthBarTextureAsset.Value, topLeft + offset + xOffset, null, Color.Black, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
                }
                for (int r =0;r < repeats * healthPercent; r++)
                {
                    Vector2 xOffset = Vector2.UnitX * r * _healthBarTextureAsset.Width();
                    spriteBatch.Draw(_healthBarTextureAsset.Value, topLeft + offset + xOffset, null, healthColor, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
                }
            }    
        }
    }
}
