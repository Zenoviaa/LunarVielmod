using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.ModLoader;

namespace Stellamod.Assets
{
    public static class AssetRegistry
    {
        public static class Textures
        {
            public static class Trails
            {
                public static Asset<Texture2D> BasicSlash_Wide1 = ModContent.Request<Texture2D>("Stellamod/Assets/Textures/Trails/BasicSlash_Wide1");
                public static Asset<Texture2D> BasicSlash_Wide2 = ModContent.Request<Texture2D>("Stellamod/Assets/Textures/Trails/BasicSlash_Wide2");
                public static Asset<Texture2D> BasicSlash_Wide3 = ModContent.Request<Texture2D>("Stellamod/Assets/Textures/Trails/BasicSlash_Wide3");
                public static Asset<Texture2D> BasicSlash_Wide4 = ModContent.Request<Texture2D>("Stellamod/Assets/Textures/Trails/BasicSlash_Wide4");
            }
        }
    }
}
