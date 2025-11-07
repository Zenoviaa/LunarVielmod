

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace Stellamod.Core.LunarLightingSystem
{
    public class TorchLightSource : ILightSource
    {
        private LightCaster _lightCaster;
        private Vector2 _position;
        public TorchLightSource()
        {
            _lightCaster = new LightCaster();   
        }
       
        public void ReCalculateLights(PointLight pointLight)
        {
            _position = pointLight.position;
            _lightCaster.CastLight((int)pointLight.radius, pointLight);
        }
        
        public void ReleaseLights()
        {
            _lightCaster.ReleaseLight();
        }

        public void DrawLights(SpriteBatch spriteBatch)
        {
            Vector2 drawOrigin = _lightCaster.texture.Size() / 2f;
            Vector2 drawPosition = _position - Main.screenPosition;

            spriteBatch.Draw(_lightCaster.texture, drawPosition, null, Color.White, 0, drawOrigin, 1.75f , SpriteEffects.None, 0);
            spriteBatch.Draw(_lightCaster.texture, drawPosition, null, Color.White, 0, drawOrigin, 1.75f , SpriteEffects.None, 0);
            spriteBatch.Draw(_lightCaster.texture, drawPosition, null, Color.White, 0, drawOrigin, 1.75f , SpriteEffects.None, 0);

            Texture2D glowColor = ModContent.Request<Texture2D>(TextureRegistry.ZuiEffect).Value;
            Color lightingColor = Lighting.GetColor((int)(_position.X / 16), (int)(_position.Y / 16));
            spriteBatch.Draw(glowColor, drawPosition, null, lightingColor * ExtraMath.Osc(0.8f, 1f, speed: 4), 0, glowColor.Size()/2f, 2, SpriteEffects.None, 0);
            spriteBatch.Draw(glowColor, drawPosition, null, lightingColor, 0, glowColor.Size() / 2f, 0.3f, SpriteEffects.None, 0);

        }


    }
}
