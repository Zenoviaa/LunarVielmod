

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Core.Shaders;
using Stellamod.Helpers;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Core.LunarLightingSystem
{
    public class TorchLightSource : ILightSource
    {
          private LightCaster _lightCaster;
          private Vector2 _position;
        private PointLight _pointLight;
        public TorchLightSource()
        {
            _lightCaster = new LightCaster();   
        }
       
        public void ReCalculateLights(PointLight pointLight)
        {
            _pointLight = pointLight;
            _position = pointLight.position;
            _lightCaster.CastLight((int)pointLight.radius, pointLight);
        }
        
        public void ReleaseLights()
        {
            _lightCaster.ReleaseLight();
        }


        private void DrawPointLight(SpriteBatch spriteBatch)
        {
            var shader = PointLightShader.Instance;
            Vector4 colorAndIntensity = new Vector4(_pointLight.color, _pointLight.intensity);
            Color color = new Color(colorAndIntensity);
            shader.LightRadius = 0.7f;


            //Convert to screen space
            Vector2 lightingWorldPosition = _pointLight.position;
            Vector2 lightingScreenPosition = lightingWorldPosition - Main.screenPosition;
            lightingScreenPosition.X /= Main.screenWidth;
            lightingScreenPosition.Y /= Main.screenHeight;
            shader.LightPosition = lightingScreenPosition;
            spriteBatch.Draw(LunarLighting.PointLightTexture, Vector2.Zero, color * ExtraMath.Osc(0.9f, 1f, speed: 2));
        }


        private void DrawRayCastLights(SpriteBatch spriteBatch)
        {
            Vector2 drawOrigin = _lightCaster.texture.Size() / 2f;
            Vector2 drawPosition = _pointLight.position - Main.screenPosition;

            float scale = 1 * LunarLighting.DownSamples;
            spriteBatch.Draw(_lightCaster.texture, drawPosition, null, Color.White, 0, drawOrigin, scale, SpriteEffects.None, 0);
            spriteBatch.Draw(_lightCaster.texture, drawPosition, null, Color.White, 0, drawOrigin, scale, SpriteEffects.None, 0);
            spriteBatch.Draw(_lightCaster.texture, drawPosition, null, Color.White, 0, drawOrigin, scale, SpriteEffects.None, 0);

            Texture2D glowColor = ModContent.Request<Texture2D>(TextureRegistry.ZuiEffect).Value;
            Color lightingColor = Lighting.GetColor((int)(_pointLight.position.X / 16), (int)(_pointLight.position.Y / 16));
            //spriteBatch.Draw(glowColor, drawPosition, null, lightingColor * ExtraMath.Osc(0.8f, 1f, speed: 4), 0, glowColor.Size() / 2f, 2, SpriteEffects.None, 0);
            //spriteBatch.Draw(glowColor, drawPosition, null, lightingColor, 0, glowColor.Size() / 2f, 0.3f, SpriteEffects.None, 0);

        }

        public void DrawLights(SpriteBatch spriteBatch)
        {
            DrawPointLight(spriteBatch);
        }


    }
}
