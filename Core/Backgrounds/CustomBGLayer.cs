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
        public Rectangle? SourceRct;
        public float Parallax;
        public Vector2 DrawOffset;
        public float DrawScale;
        public BlendState BlendState;
        public IShader Shader;
        public Vector2 ParallaxOffset;
        public float SnapParallaxYFactor;
        public void SetTexture(string texturePath)
        {
            Texture = ModContent.Request<Texture2D>("Stellamod/" + texturePath, AssetRequestMode.ImmediateLoad);
        }
    }
}
