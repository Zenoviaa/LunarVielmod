using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Helpers;
using Stellamod.Systems.MiscellaneousMath;

namespace Stellamod.Common.Foggy
{
    internal class Fog
    {
        public Asset<Texture2D> texture;
        public Point tilePosition;
        public Vector2 position;
        public Vector2 scale;
        public Vector2 startScale;
        public Vector2 offset;
        public Color color;
        public Color startColor;
        public float rotation;
        public float scrollSpeed;
        public float pulseWidth;

        public void Update()
        {
            float p = MathUtil.Osc(0f, 1f, speed: 1.0f, offset: position.X + position.Y);
            float ep = Easing.SpikeOutCirc(p);
            color = Color.Lerp(startColor * 0.95f, startColor, ep);
            scale = Vector2.Lerp(startScale * pulseWidth, startScale, ep);
        }
    }
}
