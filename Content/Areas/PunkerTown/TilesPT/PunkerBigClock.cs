using ReLogic.Content;
using Stellamod.Core.ZTileSystem;
using Stellamod.Helpers;
using System;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.PunkerTown.TilesPT
{
    public class PunkerBigClock : ZTile
    {
        private Asset<Texture2D> _miniWheelTextureAsset;
        private Asset<Texture2D> _hourHandTextureAsset;
        private Asset<Texture2D> _minuteHandTextureAsset;
        private Asset<Texture2D> _numeralsTextureAsset;
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            frameCount = 1;
            drawOrigin = TileDrawOrigin.Center;
            rotateSpeed = 0.005f;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 drawPosition, Vector2 screenPos, ZTileDrawParams drawParams)
        {
            base.PostDraw(spriteBatch, drawPosition, screenPos, drawParams);
     
            _miniWheelTextureAsset ??= ModContent.Request<Texture2D>(Texture + "_MiniWheel");
            _hourHandTextureAsset ??= ModContent.Request<Texture2D>(Texture + "_HourHand");
            _minuteHandTextureAsset ??= ModContent.Request<Texture2D>(Texture + "_MinuteHand");
            _numeralsTextureAsset ??= ModContent.Request<Texture2D>(Texture + "_Numerals");

            spriteBatch.Draw(_miniWheelTextureAsset.Value, drawPosition, null, drawParams.lightColor, Main.GlobalTimeWrappedHourly * -0.1f, _miniWheelTextureAsset.Size() / 2f, drawParams.tileData.scale, SpriteEffects.None, 0);

            float time = Utils.GetDayTimeAs24FloatStartingFromMidnight();
            float progress = time / 24f;

            float hourRotation = progress * MathHelper.TwoPi * 2;
            float minuteRotation = progress * MathHelper.TwoPi * 24;

       

            Color numeralsColor = Color.White;
            numeralsColor.A = 0;
            numeralsColor *= ExtraMath.Osc(0.5f, 1f);
            numeralsColor *= 0.5f;
            spriteBatch.Draw(_numeralsTextureAsset.Value, drawPosition, null, numeralsColor, 0, _numeralsTextureAsset.Size() / 2f, drawParams.tileData.scale, SpriteEffects.None, 0);

            spriteBatch.Draw(_hourHandTextureAsset.Value, drawPosition, null, drawParams.lightColor, hourRotation, new Vector2(_hourHandTextureAsset.Width() / 2f, _hourHandTextureAsset.Height()), drawParams.tileData.scale, SpriteEffects.None, 0);
            spriteBatch.Draw(_minuteHandTextureAsset.Value, drawPosition, null, drawParams.lightColor, minuteRotation, new Vector2(_minuteHandTextureAsset.Width() / 2f, _minuteHandTextureAsset.Height()), drawParams.tileData.scale, SpriteEffects.None, 0);

        }
    }
}
