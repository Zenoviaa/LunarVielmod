using ReLogic.Content;
using Stellamod.Common.Shaders;
using Stellamod.Core.Utilities;
using Stellamod.Core.ZTileSystem;
using Stellamod.Helpers;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.MoonspiralTower.TilesMT
{

    public class MoonFrostedChainLarge : AbstractZTileChain
    {

    }

    public class MoonFrostedChain : AbstractZTileChain
    {

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
    public class RotatingPaneLarge : ZTile
    {
        private Asset<Texture2D> _outlineTextureAsset;
        private Asset<Texture2D> _mediumTextureAsset;
        private Asset<Texture2D> _smallTextureAsset;
        private Asset<Texture2D> _mediumOutlineTextureAsset;
        private Asset<Texture2D> _smallOutlineTextureAsset;
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            frameCount = 1;
            drawOrigin = TileDrawOrigin.Center;
            rotateSpeed = 0.005f;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 drawPosition, Vector2 screenPos, ZTileDrawParams drawParams)
        {
            _outlineTextureAsset ??= ModContent.Request<Texture2D>(Texture + "_Outline");
            _mediumTextureAsset ??= ModContent.Request<Texture2D>(Texture + "_InnerMedium");
            _smallTextureAsset ??= ModContent.Request<Texture2D>(Texture + "_InnerSmall");
            _mediumOutlineTextureAsset ??= ModContent.Request<Texture2D>(Texture + "_InnerMedium_Outline");
            _smallOutlineTextureAsset ??= ModContent.Request<Texture2D>(Texture + "_InnerSmall_Outline");
                   
            Asset<Texture2D> tileTextureAsset = ModContent.Request<Texture2D>(Texture);
            Color drawColor = Color.White * 0.75f;
            drawColor.A = 0;
            spriteBatch.Draw(tileTextureAsset.Value, drawPosition, null, drawColor, Main.GlobalTimeWrappedHourly * rotateSpeed * 24, tileTextureAsset.Size() * 0.5f, drawParams.tileData.scale, SpriteEffects.None, 0);

            
            Vector2 rotationOrigin = drawPosition + screenPos;
            Vector2 mediumOffset = -Vector2.UnitY * 212;
            mediumOffset = mediumOffset.RotatedBy(-Main.GlobalTimeWrappedHourly * 0.5f);
            Vector2 pos = rotationOrigin + mediumOffset;
            SpritebatchDrawer mediumSBDrawer = SpritebatchDrawer.FromTextureAsset(_mediumTextureAsset, pos);
            mediumSBDrawer.rotation = Main.GlobalTimeWrappedHourly * 0.5f;
            mediumSBDrawer.color.A = 0;
            spriteBatch.Draw(mediumSBDrawer);

            SpritebatchDrawer mediumOutlienDrawer = SpritebatchDrawer.FromTextureAsset(_mediumOutlineTextureAsset, pos);
           // spriteBatch.Draw(mediumOutlienDrawer);

            Vector2 smallRotationOrigin = pos;
            Vector2 smallOffset = -Vector2.UnitY * 64;
            smallOffset = smallOffset.RotatedBy(Main.GlobalTimeWrappedHourly * 1f);
            Vector2 pos2 = smallRotationOrigin + smallOffset;
            SpritebatchDrawer smallSBDrawer = SpritebatchDrawer.FromTextureAsset(
                _smallTextureAsset, pos2);
            smallSBDrawer.rotation = Main.GlobalTimeWrappedHourly * 1f;
            smallSBDrawer.color.A = 0;
            spriteBatch.Draw(smallSBDrawer);

            SpritebatchDrawer smallOutlineDrawer = SpritebatchDrawer.FromTextureAsset(_smallOutlineTextureAsset, pos2);
          //  spriteBatch.Draw(smallOutlineDrawer);

       //     spriteBatch.Draw(_outlineTextureAsset.Value, drawPosition, null, Color.White, Main.GlobalTimeWrappedHourly * rotateSpeed * 24, tileTextureAsset.Size() * 0.5f, drawParams.tileData.scale, SpriteEffects.None, 0);


            return false;
        }
        public override void PostDraw(SpriteBatch spriteBatch, Vector2 drawPosition, Vector2 screenPos, ZTileDrawParams drawParams)
        {
            base.PostDraw(spriteBatch, drawPosition, screenPos, drawParams);

        }
    }
    public class RotatingPaneMedium : ZTile
    {
        private Asset<Texture2D> _outlineTextureAsset;
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            frameCount = 1;
            drawOrigin = TileDrawOrigin.Center;
            rotateSpeed = 0.005f;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 drawPosition, Vector2 screenPos, ZTileDrawParams drawParams)
        {
            _outlineTextureAsset ??= ModContent.Request<Texture2D>(Texture + "_Outline");
            return false;
        }
    }
    public class RotatingPaneSmall: ZTile
    {
        private Asset<Texture2D> _outlineTextureAsset;
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            frameCount = 1;
            drawOrigin = TileDrawOrigin.Center;
            rotateSpeed = 0.005f;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 drawPosition, Vector2 screenPos, ZTileDrawParams drawParams)
        {
            _outlineTextureAsset ??= ModContent.Request<Texture2D>(Texture + "_Outline");
            return false;
        }
    }
    public class SingularDoor : ZTile
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            drawOrigin = TileDrawOrigin.BottomLeft;
            frameCount = 1;
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
    public class MoonCrystal : ZTile
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
    public class StainedGlassPaneTall : ZTile
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
