using Microsoft.Xna.Framework.Graphics;

namespace Stellamod.Core.LunarLightingSystem
{
    public interface ILightEmitter
    {
        /// <summary>
        /// The spritebatch is already initialized with additive drawing!
        /// </summary>
        /// <param name="spriteBatch"></param>
        void RenderLight(SpriteBatch spriteBatch);
    }
}
