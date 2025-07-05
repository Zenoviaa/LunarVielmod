using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Core.Effects;
using Stellamod.Core.Helpers;
using Stellamod.Core.Helpers.Math;
using System;

namespace Stellamod.Core.Foggy
{
    internal class Fog
    {
        public Asset<Texture2D> texture;
        public Action<Fog> updateFunc;
        public Func<Shader> shaderFunc;
        public BlendState blendState;
        public Shader shader;
        public string texturePath;
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
        public Fog()
        {
            blendState = BlendState.AlphaBlend;
        }

        public void Update()
        {
            float p = VectorHelper.Osc(0f, 1f, speed: 1.0f, offset: position.X + position.Y);
            float ep = EasingFunction.QuadraticBump(p);
            color = Color.Lerp(startColor * 0.95f, startColor, ep);
            scale = Vector2.Lerp(startScale * pulseWidth, startScale, ep);
            if (updateFunc != null)
            {
                updateFunc(this);
            }
        }
    }
}
