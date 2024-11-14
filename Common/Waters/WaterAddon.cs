using Microsoft.Xna.Framework.Graphics;
using Stellamod.Common.LoadingSystems;

namespace Stellamod.Common.Waters
{
    public abstract class WaterAddon : IOrderedLoadable
    {
        public float Priority => 1f;

        /// <summary>
        /// call Main.SpriteBatch.Begin with the parameters you want for the front of water. Primarily used for applying shaders
        /// </summary>
        public abstract void SpritebatchChange();
        /// <summary>
        /// call Main.SpriteBatch.Begin with the parameters you want for the back of water. Primarily used for applying shaders
        /// </summary>
        public abstract void SpritebatchChangeBack();

        public abstract bool Visible { get; }

        public abstract Texture2D BlockTexture(Texture2D normal, int x, int y);

        public void Load()
        {
            WaterAddonHandler.addons.Add(this);
        }

        public void Unload() { }
    }
}
