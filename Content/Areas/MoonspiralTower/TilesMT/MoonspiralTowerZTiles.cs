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
            //TODO: Don't spam ModContent.Request
            Asset<Texture2D> texture = ModContent.Request<Texture2D>(Texture);

            Vector2 flagPosition = drawPosition;
            flagPosition.X += ExtraMath.Osc(0f, 4, speed: 3);
          //  flagPosition.Y -= texture.Height() * 0.5f;
            Vector2 drawOrigin = new Vector2(texture.Width() / 2f, 0f);
            BannerWavingShader wavingShader = BannerWavingShader.Instance;
            wavingShader.OscStrength = 0.1f;
            wavingShader.XOffset = 4;
            wavingShader.Time = Main.GlobalTimeWrappedHourly * 2 + drawParams.tilePosition.x;

            spriteBatch.Restart(effect: wavingShader.Effect);
            spriteBatch.Draw(texture.Value, flagPosition, null, drawParams.lightColor, 0, drawOrigin, 1, SpriteEffects.None, 0);
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

    public class HangingFairy : ZTile
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            drawOrigin = TileDrawOrigin.TopDown;
            frameCount = 1;

            //idk
            windSwayOffset = 0f;

            //The max it can sway
            windSwayMagnitude = 0.2f;

            //How fast it sways
            windSwaySpeed = 0.02f;
        }
    }
    public class MoonPot : ZTile
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            drawOrigin = TileDrawOrigin.BottomUp;
            frameCount = 1;
        }
    }
    public class HangingMoonLantern : ZTile
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            drawOrigin = TileDrawOrigin.TopDown;
            frameCount = 1;

            //idk
            windSwayOffset = 0f;

            //The max it can sway
            windSwayMagnitude = 0.2f;

            //How fast it sways
            windSwaySpeed = 0.02f;
        }
    }
    public class HangingWhiteMoonLantern : ZTile
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            drawOrigin = TileDrawOrigin.TopDown;
            frameCount = 1;

            //idk
            windSwayOffset = 0f;

            //The max it can sway
            windSwayMagnitude = 0.2f;

            //How fast it sways
            windSwaySpeed = 0.02f;
        }
    }
    
    public class StandingMoonPot : ZTile
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            drawOrigin = TileDrawOrigin.BottomUp;
            frameCount = 1;
        }
    }
    
    public class StandingMoonCandle : ZTile
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            drawOrigin = TileDrawOrigin.BottomUp;
            frameCount = 1;
        }
    }

    public class MoonRails : ZTile
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            drawOrigin = TileDrawOrigin.BottomUp;
            frameCount = 1;
        }
    }
}
