using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Core.Utilities;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Stellamod.Assets
{
    public class AssetManager : ModSystem
    {
        public class Dithering
        {
            public static LazyAsset<Texture2D> Dither4x4;
            public static LazyAsset<Texture2D> Dither4x4Double;
            public static LazyAsset<Texture2D> Dither8x8;
            public static LazyAsset<Texture2D> Dither8x8Double;
        }
        public class Noise
        {
            public static LazyAsset<Texture2D> SharpPerlinNoise;
            public static LazyAsset<Texture2D> SnowStormNoise;
            public static LazyAsset<Texture2D> InvertedVoronoi;
            public static LazyAsset<Texture2D> FlameVortexNoise;
            public static LazyAsset<Texture2D> FrontClouds;
            public static Asset<Texture2D> CloudsMask;
            public static Asset<Texture2D> Clouds;
            public static Asset<Texture2D> PainterlyNoise;
            public static Asset<Texture2D> CometStars;
            public static Asset<Texture2D> AuroraRays;
            public static Asset<Texture2D> Whirly;
            public static Asset<Texture2D> FlamethrowerNoise;
            public static Asset<Texture2D> Swirl;
            public static Asset<Texture2D> PerlinBlurred;
            public static void Load(ref Asset<Texture2D> asset, string path)
            {
                asset = ModContent.Request<Texture2D>($"Stellamod/Assets/Noise/{path}");
            }
        }

        public class GlowMask
        {
            public static LazyAsset<Texture2D> JumbledGlowCircle;
            public static LazyAsset<Texture2D> SwordSlashForward;
            public static Asset<Texture2D> SwordSlash;
            public static Asset<Texture2D> Impact;
            public static Asset<Texture2D> WhiteCircle;
            public static Asset<Texture2D> Wave;
            public static Asset<Texture2D> BlastPillar;
            public static Asset<Texture2D> SolarEye;
            public static Asset<Texture2D> SolarRing;
            public static Asset<Texture2D> Spotlight;
            public static Asset<Texture2D> RomanceGlowSwordMedium;
            public static Asset<Texture2D> RomanceGlowSwordSmall;
            public static Asset<Texture2D> RomanceGlowSword;
            public static Asset<Texture2D> StarFlare1;
            public static Asset<Texture2D> StarFlare2;
            public static Asset<Texture2D> StarFlare3;
            public static Asset<Texture2D> WhiteSquare;
            public static Asset<Texture2D> SpiralVortex;
            public static Asset<Texture2D> SpiralVortex2;
            public static Asset<Texture2D> SimpleGlowCircle;
            public static Asset<Texture2D> GradientPillar;
            public static Asset<Texture2D> MuzzleFlash;
            public static Asset<Texture2D> Shine;
            public static Asset<Texture2D> AlsisMagicCircle;
            public static Asset<Texture2D> ButterflyCircle;
            public static Asset<Texture2D> MagicCircle;
            public static Asset<Texture2D> MagicSwordCircle;
            public static Asset<Texture2D> MagicCircle2;
            public static Asset<Texture2D> MagicBloodCircle;
            public static Asset<Texture2D> MagicCircleVampiricVine;
            public static Asset<Texture2D> GothinMagicCircle;
            public static Asset<Texture2D> EmptyGradient;
            public static Asset<Texture2D> AuroraGradient;
            public static Asset<Texture2D> AuroraBackGradient;
            public static Asset<Texture2D> ShootingStarTrail;
            public static Asset<Texture2D> ShootingStarGlint;
        }
 
        public class LaserTextures
        {
            public static Asset<Texture2D> FlameTrail;
            public static Asset<Texture2D> SilkStrand;
            public static Asset<Texture2D> Aura;
            public static Asset<Texture2D> CometTrail;
            public static Asset<Texture2D> SplittingTrail;
            public static Asset<Texture2D> Bloom;
            public static Asset<Texture2D> HeavenlySlashTrail;
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
            Noise.SnowStormNoise = new LazyAsset<Texture2D>("Stellamod/Assets/Noise/SnowStormNoise");
            Dithering.Dither4x4 = new LazyAsset<Texture2D>("Stellamod/Assets/Dithering/Dither4x4");
            Dithering.Dither4x4Double = new LazyAsset<Texture2D>("Stellamod/Assets/Dithering/Dither4x4DoubleScaled");
            Dithering.Dither8x8 = new LazyAsset<Texture2D>("Stellamod/Assets/Dithering/Dither8x8");
            Dithering.Dither8x8Double = new LazyAsset<Texture2D>("Stellamod/Assets/Dithering/Dither8x8DoubleScaled");
            Noise.SharpPerlinNoise = new LazyAsset<Texture2D>("Stellamod/Assets/Noise/SharpPerlinNoise");

            GlowMask.JumbledGlowCircle = new LazyAsset<Texture2D>("Stellamod/Assets/GlowMasks/JumbledGlowCircle");
            Noise.InvertedVoronoi = new LazyAsset<Texture2D>("Stellamod/Assets/Noise/InvertedVoronoi");
            Noise.FlameVortexNoise = new LazyAsset<Texture2D>("Stellamod/Assets/Noise/FlameVortexNoise");
            GlowMask.SwordSlashForward = new LazyAsset<Texture2D>("Stellamod/Assets/GlowMasks/SwordSlashForward");
            Noise.FrontClouds = new LazyAsset<Texture2D>("Stellamod/Assets/Noise/FrontClouds");

            Noise.Clouds = ModContent.Request<Texture2D>("Stellamod/Assets/Noise/Clouds");
            Noise.CloudsMask = ModContent.Request<Texture2D>("Stellamod/Assets/Noise/CloudsMask");
            Noise.PainterlyNoise = ModContent.Request<Texture2D>("Stellamod/Assets/Noise/PainterlyNoise");

            GlowMask.SwordSlash = ModContent.Request<Texture2D>("Stellamod/Assets/GlowMasks/SwordSlash");
            GlowMask.Impact = ModContent.Request<Texture2D>("Stellamod/Assets/GlowMasks/Impact");
            GlowMask.Wave = ModContent.Request<Texture2D>("Stellamod/Assets/GlowMasks/Wave");
            GlowMask.BlastPillar = ModContent.Request<Texture2D>("Stellamod/Assets/GlowMasks/BlastPillar");
            GlowMask.SolarEye = ModContent.Request<Texture2D>("Stellamod/Assets/GlowMasks/SolarEye");
            GlowMask.SolarRing = ModContent.Request<Texture2D>("Stellamod/Assets/GlowMasks/SolarRing");
            GlowMask.ButterflyCircle = ModContent.Request<Texture2D>("Stellamod/Assets/GlowMasks/ButterflyCircle");
            GlowMask.Spotlight = ModContent.Request<Texture2D>("Stellamod/Assets/GlowMasks/Spotlight");
            GlowMask.RomanceGlowSwordMedium = ModContent.Request<Texture2D>("Stellamod/Assets/GlowMasks/RomanceGlowSword_Medium");
            GlowMask.RomanceGlowSwordSmall = ModContent.Request<Texture2D>("Stellamod/Assets/GlowMasks/RomanceGlowSword_Small");
            GlowMask.RomanceGlowSword = ModContent.Request<Texture2D>("Stellamod/Assets/GlowMasks/RomanceGlowSword");
            GlowMask.StarFlare1 = ModContent.Request<Texture2D>("Stellamod/Assets/GlowMasks/Star1");
            GlowMask.StarFlare2 = ModContent.Request<Texture2D>("Stellamod/Assets/GlowMasks/Star2");
            GlowMask.StarFlare3 = ModContent.Request<Texture2D>("Stellamod/Assets/GlowMasks/Star3");
            GlowMask.WhiteSquare = ModContent.Request<Texture2D>("Stellamod/Assets/GlowMasks/WhiteSquare");
            GlowMask.SpiralVortex = ModContent.Request<Texture2D>("Stellamod/Assets/GlowMasks/SpiralVortex");
            GlowMask.SimpleGlowCircle = ModContent.Request<Texture2D>("Stellamod/Assets/GlowMasks/SimpleGlowCircle");
            GlowMask.GradientPillar = ModContent.Request<Texture2D>("Stellamod/Assets/GlowMasks/GradientPillar");
            GlowMask.MuzzleFlash = ModContent.Request<Texture2D>("Stellamod/Assets/GlowMasks/MuzzleFlash");
            GlowMask.Shine = ModContent.Request<Texture2D>("Stellamod/Assets/GlowMasks/Shine");
            GlowMask.AlsisMagicCircle = ModContent.Request<Texture2D>("Stellamod/Assets/GlowMasks/AlsisMagicCircle");
            GlowMask.MagicSwordCircle = ModContent.Request<Texture2D>("Stellamod/Assets/GlowMasks/MagicSwordCircle");
            GlowMask.MagicCircle = ModContent.Request<Texture2D>("Stellamod/Assets/GlowMasks/MagicCircle1");
            GlowMask.MagicCircle2 = ModContent.Request<Texture2D>("Stellamod/Assets/GlowMasks/MagicCircle2");
            GlowMask.MagicBloodCircle = ModContent.Request<Texture2D>("Stellamod/Assets/GlowMasks/MagicBloodCircle");
            GlowMask.MagicCircleVampiricVine = ModContent.Request<Texture2D>("Stellamod/Assets/GlowMasks/MagicCircleVampiricVine");
            GlowMask.GothinMagicCircle = ModContent.Request<Texture2D>("Stellamod/Assets/GlowMasks/GothinMagicCircle");
            GlowMask.SpiralVortex2 = ModContent.Request<Texture2D>("Stellamod/Assets/GlowMasks/SpiralVortex2");
            GlowMask.EmptyGradient = ModContent.Request<Texture2D>("Stellamod/Assets/GlowMasks/EmptyGradient");
            GlowMask.AuroraGradient = ModContent.Request<Texture2D>("Stellamod/Assets/GlowMasks/AuroraGradient");
            GlowMask.AuroraBackGradient = ModContent.Request<Texture2D>("Stellamod/Assets/GlowMasks/AuroraBackGradient");
            GlowMask.ShootingStarGlint = ModContent.Request<Texture2D>("Stellamod/Assets/GlowMasks/ShootingStarGlint");
            GlowMask.ShootingStarTrail = ModContent.Request<Texture2D>("Stellamod/Assets/GlowMasks/ShootingStarParticle");
            GlowMask.WhiteCircle = ModContent.Request<Texture2D>("Stellamod/Assets/GlowMasks/WhiteCircle");
            LaserTextures.FlameTrail = ModContent.Request<Texture2D>("Stellamod/Assets/LaserTextures/FlameTrail");
            LaserTextures.TexturedLaser = ModContent.Request<Texture2D>("Stellamod/Assets/LaserTextures/TexturedLaser");
            LaserTextures.TexturedLaser2 = ModContent.Request<Texture2D>("Stellamod/Assets/LaserTextures/TexturedLaser2");
            LaserTextures.SnowflakeLaser = ModContent.Request<Texture2D>("Stellamod/Assets/LaserTextures/SnowflakeLaser");
            LaserTextures.Lightning = ModContent.Request<Texture2D>("Stellamod/Assets/LaserTextures/Lightning");
            LaserTextures.Lightning2 = ModContent.Request<Texture2D>("Stellamod/Assets/LaserTextures/Lightning2");
            LaserTextures.PetalNoise = ModContent.Request<Texture2D>("Stellamod/Assets/LaserTextures/PetalNoise");
            LaserTextures.HeavenlySlashTrail = ModContent.Request<Texture2D>("Stellamod/Assets/LaserTextures/HeavenlySlashTrail");
            LaserTextures.Bloom = ModContent.Request<Texture2D>("Stellamod/Assets/LaserTextures/Bloom");
            LaserTextures.SplittingTrail = ModContent.Request<Texture2D>("Stellamod/Assets/LaserTextures/SplittingTrail");
            LaserTextures.CometTrail = ModContent.Request<Texture2D>("Stellamod/Assets/LaserTextures/CometTrail");
            LaserTextures.Aura = ModContent.Request<Texture2D>("Stellamod/Assets/LaserTextures/Aura");
            LaserTextures.SilkStrand = ModContent.Request<Texture2D>("Stellamod/Assets/LaserTextures/SilkStrand");
            Noise.Whirly = ModContent.Request<Texture2D>("Stellamod/Assets/Noise/Whirly");

            Noise.CometStars = ModContent.Request<Texture2D>("Stellamod/Assets/Noise/CometStars");
            Noise.AuroraRays = ModContent.Request<Texture2D>("Stellamod/Assets/Noise/AuroraRays");
            Noise.FlamethrowerNoise = ModContent.Request<Texture2D>("Stellamod/Assets/Noise/FlameNoise");
            Noise.Swirl = ModContent.Request<Texture2D>("Stellamod/Assets/Noise/Swirl");
            Noise.PerlinBlurred = ModContent.Request<Texture2D>("Stellamod/Assets/Noise/PerlinBlurred");
            //  Noise.Load(ref Noise.PerlinBlurred, "PerlinBlurred");
        }
        public override void OnModUnload()
        {
            base.OnModUnload();
            Noise.SnowStormNoise?.Unload();
            Dithering.Dither4x4?.Unload();
            Dithering.Dither4x4Double?.Unload();
            Dithering.Dither8x8?.Unload();
            Dithering.Dither8x8Double?.Unload();
            Noise.SharpPerlinNoise?.Unload();
            
            GlowMask.JumbledGlowCircle?.Unload();
            Noise.InvertedVoronoi?.Unload();
            Noise.FlameVortexNoise?.Unload();
            GlowMask.SwordSlashForward?.Unload();
            Noise.FrontClouds?.Unload();
            LaserTextures.FlameTrail = null;
            Noise.Clouds = null;
            Noise.CloudsMask = null;
            Noise.PainterlyNoise = null;
            GlowMask.SwordSlash = null;
            LaserTextures.Aura = null;
            Noise.CometStars = null;
            LaserTextures.CometTrail = null;
            GlowMask.Impact = null;
            GlowMask.WhiteCircle = null;
           Noise.PerlinBlurred = null;
            Noise.Swirl = null;
            GlowMask.Wave = null;
            GlowMask.BlastPillar = null;
            GlowMask.SolarEye = null;
            GlowMask.SolarRing = null;
            //Set to null otherwise we'll have a memory leak
            GlowMask.ButterflyCircle = null;
            GlowMask.MagicSwordCircle = null;
            GlowMask.GothinMagicCircle = null;
            GlowMask.AlsisMagicCircle = null;
            GlowMask.Spotlight = null;
            GlowMask.RomanceGlowSwordMedium = null;
            GlowMask.RomanceGlowSwordSmall = null;
            GlowMask.RomanceGlowSword = null;
            GlowMask.StarFlare1 = null;
            GlowMask.StarFlare2 = null;
            GlowMask.StarFlare3 = null;
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
            GlowMask.MagicCircleVampiricVine = null;
            GlowMask.EmptyGradient = null;
            GlowMask.ShootingStarGlint = null;
            GlowMask.ShootingStarTrail = null;
            GlowMask.AuroraGradient = null;
            GlowMask.AuroraBackGradient = null;

            LaserTextures.TexturedLaser = null;
            LaserTextures.TexturedLaser2 = null;
            LaserTextures.SnowflakeLaser = null;
            LaserTextures.Lightning = null;
            LaserTextures.Lightning2 = null;
            LaserTextures.PetalNoise = null;
            LaserTextures.HeavenlySlashTrail = null;
            LaserTextures.Bloom = null;
            LaserTextures.SplittingTrail = null;
            LaserTextures.SilkStrand = null;

    Noise.Whirly = null;
            Noise.AuroraRays = null;
            Noise.FlamethrowerNoise = null;
        }

        public static Asset<Texture2D> LoadBackground(string name)
        {
            return ModContent.Request<Texture2D>($"Stellamod/Assets/Textures/Backgrounds/{name}");
        }
    }
}
