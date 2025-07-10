using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Core.Effects;
using Terraria.ModLoader;

namespace Stellamod.Core.Backgrounds
{
    public class CustomBGLayer
    {
        public CustomBGLayer()
        {
            DrawScale = 1f;
            BlendState = BlendState.AlphaBlend;
        }
        public Asset<Texture2D> Texture;
        public float Parallax;
        public Vector2 DrawOffset;
        public float DrawScale;
        public BlendState BlendState;
        public Shader Shader;
        public void SetTexture(string texturePath)
        {
            Texture = ModContent.Request<Texture2D>("Stellamod/" + texturePath);
        }
    }
}
