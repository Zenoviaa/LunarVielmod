using ReLogic.Content;
using Stellamod.Common.Shaders;
using Stellamod.Core.ZTileSystem;
using Stellamod.Helpers;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.MoonspiralTower.TilesMT
{
    public class MoonFrostedChainLarge : ZTile
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            drawOrigin = TileDrawOrigin.TopDown;
            frameCount = 1;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 drawPosition, Vector2 screenPos, ZTileDrawParams drawParams)
        {
            //TODO: Verlet Integration chain
            return true;
        }
    }

    public class MoonFrostedChain : ZTile
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            drawOrigin = TileDrawOrigin.TopDown;
            frameCount = 1;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 drawPosition, Vector2 screenPos, ZTileDrawParams drawParams)
        {
            //TODO: Verlet Integration chain
            return true;
        }
    }

    public class MoonBanner : ZTile
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            drawOrigin = TileDrawOrigin.TopDown;
            frameCount = 1;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 drawPosition, Vector2 screenPos, ZTileDrawParams drawParams)
        {
            Asset<Texture2D> flagTexture = ModContent.Request<Texture2D>(Texture);

            Vector2 flagPosition = drawPosition;
            flagPosition.X += ExtraMath.Osc(0f, 4, speed: 3);
            Vector2 drawOrigin = new Vector2(flagTexture.Width() / 2f, 0f);
            BannerWavingShader wavingShader = BannerWavingShader.Instance;
            wavingShader.OscStrength = 0.1f;
            wavingShader.XOffset = 5;
            wavingShader.Time = Main.GlobalTimeWrappedHourly * 2;

            Asset<Texture2D> tileTextureAsset = ModContent.Request<Texture2D>(Texture);
            spriteBatch.Restart(effect: wavingShader.Effect);
            spriteBatch.Draw(flagTexture.Value, flagPosition, null, drawParams.lightColor, 0, drawOrigin, 1, SpriteEffects.None, 0);
            spriteBatch.RestartDefaults();
            return false;
        }
    }
    public class MoonPedestal : ZTile
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            drawOrigin = TileDrawOrigin.BottomUp;
            frameCount = 1;
        }
    }
    public class MoonspiralPillars : ZTile
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            drawOrigin = TileDrawOrigin.BottomUp;
            frameCount = 1;
        }
    }

    public class StainedGlassPane : ZTile
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            drawOrigin = TileDrawOrigin.BottomUp;
            frameCount = 3;
        }
    }
}
