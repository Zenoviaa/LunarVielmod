using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria.ModLoader;

namespace Stellamod.Helpers
{
    internal static class TextureRegistry
    {
        public static string PathHere(this ModType t)
        {
            string path = (t.GetType().Namespace).Replace('.', '/');
            return path;
        }
        public static string EmptyTexture => "Stellamod/Assets/Textures/Empty";
        public static string EmptyBigTexture => "Stellamod/Assets/Textures/EmptyBig";
        public static string EmptyGlowParticle => "Stellamod/Visual/Particles/GlowCircleBoomParticle";
        public static string EmptyLongGlowParticle => "Stellamod/Visual/Particles/GlowCircleLongBoomParticle";
        public static string FlowerTexture => "Stellamod/Assets/NoiseTextures/Flower";
        public static string FlyingSlashTexture => "Stellamod/Assets/NoiseTextures/FlyingSlash";
        public static string CircleOutline => "Stellamod/Assets/NoiseTextures/Extra_67";
        public static string NormalNoise1 => "Stellamod/Assets/NoiseTextures/NormalNoise1";
        public static string ZuiEffect => "Stellamod/Assets/NoiseTextures/ZuiEffect";
        public static string VoxTexture3 => "Stellamod/Assets/Effects/VoxTexture3";

        public static string VoxTexture4 => "Stellamod/Assets/Effects/VoxTexture5";

        public static string BoreParticleWhite => "Stellamod/Particles/BoreParticleWhite";
        public static Asset<Texture2D> BasicGlow => ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/BasicGlow");
        public static Asset<Texture2D> StarNoise => ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/StarNoise");
        public static Asset<Texture2D> StarNoise2 => ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/StarNoise2");
        public static Asset<Texture2D> CloudNoise => ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/CloudNoise");
        public static Asset<Texture2D> CloudNoise2 => ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/CloudNoise2");
        public static Asset<Texture2D> CloudNoise3 => ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/CloudNoise3");
        public static Asset<Texture2D> BlurryPerlinNoise2 => ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/BlurryPerlinNoise2");
        public static Asset<Texture2D> LavaDepths => ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/LavaDepths");
        public static Asset<Texture2D> CloudTexture => ModContent.Request<Texture2D>("Stellamod/Assets/Effects/CloudTexture");
        public static Asset<Texture2D> IrraTexture => ModContent.Request<Texture2D>("Stellamod/Assets/Effects/IrraTexture2");
        public static Asset<Texture2D> SmallNoise => ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/SmallNoise");
        public static Asset<Texture2D> FourPointedStar => ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/Extra_63");
    }
}
