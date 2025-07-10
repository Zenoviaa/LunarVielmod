using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.ModLoader;

namespace Stellamod.Core.Backgrounds
{
    public class CustomBGLayer
    {
        public CustomBGLayer(string texturePath, float parallax, Vector2 drawOffset)
        {
            Texture = ModContent.Request<Texture2D>("Stellamod/" + texturePath);
            Parallax = parallax;
            DrawOffset = drawOffset;
        }

        public Asset<Texture2D> Texture;
        public float Parallax;
        public Vector2 DrawOffset;
    }
}
