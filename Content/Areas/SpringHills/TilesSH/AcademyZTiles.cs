using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Common.Shaders;
using Stellamod.Core.Pixelation;
using Stellamod.Core.ZTileSystem;
using Stellamod.Helpers;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.SpringHills.TilesSH
{
    public abstract class AbstractFlagPostZTile : ZTile
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            frameCount = 1;
            drawOrigin = TileDrawOrigin.BottomUp;
        }

        public override void PreDraw(SpriteBatch spriteBatch, Vector2 drawPosition, Vector2 screenPos, ZTileDrawParams drawParams)
        {
            base.PreDraw(spriteBatch, drawPosition, screenPos, drawParams);

            void RenderFlagPost(SpriteBatch spriteBatch, Vector2 screenPos)
            {
                Asset<Texture2D> flagTexture = ModContent.Request<Texture2D>(Texture + "_Flag");
                int segmentCount = 12;
                int segmentWidth = flagTexture.Width() / segmentCount;

                Vector2 flagPosition = drawPosition;
                flagPosition.Y += ExtraMath.Osc(0f, 4, speed: 3);
                Vector2 drawOrigin = new Vector2(0, flagTexture.Height() / 2f);
                FlagWavingShader wavingShader = FlagWavingShader.Instance;
                wavingShader.OscStrength = 0.1f;
                wavingShader.XOffset = 5;
                wavingShader.Time = Main.GlobalTimeWrappedHourly * 2;

                Asset<Texture2D> tileTextureAsset = ModContent.Request<Texture2D>(Texture);
                float yOffset = tileTextureAsset.Height() / 2f;
                flagPosition.Y -= yOffset;
                flagPosition.Y += flagTexture.Height() / 4f;
                flagPosition.Y += 8;
                spriteBatch.Restart(effect: wavingShader.Effect);
                spriteBatch.Draw(flagTexture.Value, flagPosition, null, drawParams.lightColor, 0, drawOrigin, 1, SpriteEffects.None, 0);
                spriteBatch.RestartDefaults();
            }

            PixelationManager.QueueSpritebatchDrawAction(RenderFlagPost, DrawLayer.BehindTiles);
        }
    }

    public class AcademyFlagPost : AbstractFlagPostZTile
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            frameCount = 1;
            drawOrigin = TileDrawOrigin.BottomUp;
        }
     

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 drawPosition, Vector2 screenPos, ZTileDrawParams drawParams)
        {
            base.PostDraw(spriteBatch, drawPosition, screenPos, drawParams);

        }
    }
}
