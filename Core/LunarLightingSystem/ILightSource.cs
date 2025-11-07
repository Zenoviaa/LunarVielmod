using Microsoft.Xna.Framework.Graphics;

namespace Stellamod.Core.LunarLightingSystem
{
    public interface ILightSource
    {
        void ReCalculateLights(PointLight pointLight);
        void ReleaseLights();
        void DrawLights(SpriteBatch spriteBatch);
    }
}
