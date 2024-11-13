using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.ModLoader;

namespace Stellamod.Helpers
{
    internal static class TextureRegistry
    {
        public static string EmptyTexture => "Stellamod/Assets/Textures/Empty";
        public static string EmptyBigTexture => "Stellamod/Assets/Textures/EmptyBig";
        public static string FlowerTexture => "Stellamod/Effects/Masks/Flower";
        public static string FlyingSlashTexture => "Stellamod/Effects/Masks/FlyingSlash";
        public static string CircleOutline => "Stellamod/Effects/Masks/Extra_67";
        public static string NormalNoise1 => "Stellamod/Textures/NormalNoise1";
        public static string ZuiEffect => "Stellamod/Effects/Masks/ZuiEffect";
        public static string VoxTexture3 => "Stellamod/Assets/Effects/VoxTexture3";

        public static string VoxTexture4 => "Stellamod/Assets/Effects/VoxTexture5";

        public static string BoreParticleWhite => "Stellamod/Particles/BoreParticleWhite";
        public static Asset<Texture2D> StarNoise => ModContent.Request<Texture2D>("Stellamod/Textures/StarNoise");
        public static Asset<Texture2D> StarNoise2 => ModContent.Request<Texture2D>("Stellamod/Textures/StarNoise2");
        public static Asset<Texture2D> CloudNoise => ModContent.Request<Texture2D>("Stellamod/Textures/CloudNoise");
        public static Asset<Texture2D> CloudNoise2 => ModContent.Request<Texture2D>("Stellamod/Textures/CloudNoise2");
        public static Asset<Texture2D> CloudNoise3 => ModContent.Request<Texture2D>("Stellamod/Textures/CloudNoise3");
        public static Asset<Texture2D> CloudTexture => ModContent.Request<Texture2D>("Stellamod/Assets/Effects/CloudTexture");
        public static Asset<Texture2D> IrraTexture => ModContent.Request<Texture2D>("Stellamod/Assets/Effects/IrraTexture2");
    }
}
