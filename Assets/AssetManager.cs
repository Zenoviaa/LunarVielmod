using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Stellamod.Assets
{
    public class AssetManager : ModSystem
    {
        public class Noise
        {
            public static Asset<Texture2D> AuroraRays;
            public static Asset<Texture2D> Whirly;
        }

        public class GlowMask
        {
            public static Asset<Texture2D> WhiteSquare;
            public static Asset<Texture2D> SpiralVortex;
            public static Asset<Texture2D> SpiralVortex2;
            public static Asset<Texture2D> SimpleGlowCircle;
            public static Asset<Texture2D> GradientPillar;
            public static Asset<Texture2D> MuzzleFlash;
            public static Asset<Texture2D> Shine;
            public static Asset<Texture2D> MagicCircle;
            public static Asset<Texture2D> MagicCircle2;
            public static Asset<Texture2D> MagicBloodCircle;
            public static Asset<Texture2D> EmptyGradient;
        }
 
        public class LaserTextures
        {
            public static Asset<Texture2D> TexturedLaser;
            public static Asset<Texture2D> TexturedLaser2;
            public static Asset<Texture2D> SnowflakeLaser;
            public static Asset<Texture2D> Lightning;
            public static Asset<Texture2D> Lightning2;
            public static Asset<Texture2D> PetalNoise;
        }
        public static SoundStyle GetSound(string name)
        {
            return new SoundStyle($"Stellamod/Assets/Sounds/{name}");
        }

        public override void OnModLoad()
        {
            base.OnModLoad();
            GlowMask.WhiteSquare = ModContent.Request<Texture2D>("Stellamod/Assets/GlowMasks/WhiteSquare");
            GlowMask.SpiralVortex = ModContent.Request<Texture2D>("Stellamod/Assets/GlowMasks/SpiralVortex");
            GlowMask.SimpleGlowCircle = ModContent.Request<Texture2D>("Stellamod/Assets/GlowMasks/SimpleGlowCircle");
            GlowMask.GradientPillar = ModContent.Request<Texture2D>("Stellamod/Assets/GlowMasks/GradientPillar");
            GlowMask.MuzzleFlash = ModContent.Request<Texture2D>("Stellamod/Assets/GlowMasks/MuzzleFlash");
            GlowMask.Shine = ModContent.Request<Texture2D>("Stellamod/Assets/GlowMasks/Shine");
            GlowMask.MagicCircle = ModContent.Request<Texture2D>("Stellamod/Assets/GlowMasks/MagicCircle1");
            GlowMask.MagicCircle2 = ModContent.Request<Texture2D>("Stellamod/Assets/GlowMasks/MagicCircle2");
            GlowMask.MagicBloodCircle = ModContent.Request<Texture2D>("Stellamod/Assets/GlowMasks/MagicBloodCircle");
            GlowMask.SpiralVortex2 = ModContent.Request<Texture2D>("Stellamod/Assets/GlowMasks/SpiralVortex2");
            GlowMask.EmptyGradient = ModContent.Request<Texture2D>("Stellamod/Assets/GlowMasks/EmptyGradient");

            LaserTextures.TexturedLaser = ModContent.Request<Texture2D>("Stellamod/Assets/LaserTextures/TexturedLaser");
            LaserTextures.TexturedLaser2 = ModContent.Request<Texture2D>("Stellamod/Assets/LaserTextures/TexturedLaser2");
            LaserTextures.SnowflakeLaser = ModContent.Request<Texture2D>("Stellamod/Assets/LaserTextures/SnowflakeLaser");
            LaserTextures.Lightning = ModContent.Request<Texture2D>("Stellamod/Assets/LaserTextures/Lightning");
            LaserTextures.Lightning2 = ModContent.Request<Texture2D>("Stellamod/Assets/LaserTextures/Lightning2");
            LaserTextures.PetalNoise = ModContent.Request<Texture2D>("Stellamod/Assets/LaserTextures/PetalNoise");

            Noise.Whirly = ModContent.Request<Texture2D>("Stellamod/Assets/Noise/Whirly");

            Noise.AuroraRays = ModContent.Request<Texture2D>("Stellamod/Assets/Noise/AuroraRays");
        }
        public override void OnModUnload()
        {
            base.OnModUnload();
            //Set to null otherwise we'll have a memory leak
            GlowMask.SpiralVortex2 = null;
            GlowMask.WhiteSquare = null;
            GlowMask.SpiralVortex = null;
            GlowMask.GradientPillar = null;
            GlowMask.MuzzleFlash = null;
            GlowMask.SimpleGlowCircle = null;
            GlowMask.Shine = null;
            GlowMask.MagicCircle = null;
            GlowMask.MagicCircle2 = null;
            GlowMask.MagicBloodCircle = null;
            GlowMask.EmptyGradient = null;

            LaserTextures.TexturedLaser = null;
            LaserTextures.TexturedLaser2 = null;
            LaserTextures.SnowflakeLaser = null;
            LaserTextures.Lightning = null;
            LaserTextures.Lightning2 = null;
            LaserTextures.PetalNoise = null;

            Noise.Whirly = null;
            Noise.AuroraRays = null;
        }
    }
}
