using ReLogic.Content;
using Terraria.ModLoader;

namespace Stellamod.Helpers
{
    public static class TextureRegistry
    {
        public static string PathHere(this ModType t)
        {
            string path = (t.GetType().Namespace).Replace('.', '/');
            return path;
        }
        public static string ThinCircle => "Stellamod/Assets/NoiseTextures/Extra_67";
        public static string CandleFlame => "Stellamod/Assets/NoiseTextures/CandleFlame";
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
        public static Asset<Texture2D> GlowSword_LightKnives => ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/GlowSword_LightKnives");
        public static Asset<Texture2D> GlowSword_Chillrend => ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/GlowSword_Chillrend");
        public static Asset<Texture2D> GlowSword_Sword => ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/GlowSword_Sword");
        public static Asset<Texture2D> GlowSword_Spear => ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/GlowSword_Spear");
        public static Asset<Texture2D> GlowSword_Scythe => ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/GlowSword_Scythe");
        public static Asset<Texture2D> GlowSword_Scythe2 => ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/GlowSword_Scythe2");
        public static Asset<Texture2D> DimLight => ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/DimLight");
        public static Asset<Texture2D> Clouds6 => ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/Clouds6");
        public static Asset<Texture2D> BasicGlow => ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/BasicGlow");
        public static Asset<Texture2D> StarNoise => ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/StarNoise");
        public static Asset<Texture2D> StarNoise2 => ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/StarNoise2");
        public static Asset<Texture2D> CloudNoise => ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/CloudNoise");
        public static Asset<Texture2D> CloudNoise2 => ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/CloudNoise2");
        public static Asset<Texture2D> CloudNoise3 => ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/CloudNoise3");
        public static Asset<Texture2D> BlurryPerlinNoise => ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/BlurryPerlinNoise");
        public static Asset<Texture2D> BlurryPerlinNoise2 => ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/BlurryPerlinNoise2");
        public static Asset<Texture2D> LavaDepths => ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/LavaDepths");
        public static Asset<Texture2D> CloudTexture => ModContent.Request<Texture2D>("Stellamod/Assets/Effects/CloudTexture");
        public static Asset<Texture2D> IrraTexture => ModContent.Request<Texture2D>("Stellamod/Assets/Effects/IrraTexture2");
        public static Asset<Texture2D> SmallNoise => ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/SmallNoise");
        public static Asset<Texture2D> FourPointedStar => ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/Extra_63");
    }
}
