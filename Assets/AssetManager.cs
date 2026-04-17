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
            LaserTextures.TexturedLaser = ModContent.Request<Texture2D>("Stellamod/Assets/LaserTextures/TexturedLaser");
            LaserTextures.TexturedLaser2 = ModContent.Request<Texture2D>("Stellamod/Assets/LaserTextures/TexturedLaser2");
            LaserTextures.SnowflakeLaser = ModContent.Request<Texture2D>("Stellamod/Assets/LaserTextures/SnowflakeLaser");
            LaserTextures.Lightning = ModContent.Request<Texture2D>("Stellamod/Assets/LaserTextures/Lightning");
            LaserTextures.Lightning2 = ModContent.Request<Texture2D>("Stellamod/Assets/LaserTextures/Lightning2");
            LaserTextures.PetalNoise = ModContent.Request<Texture2D>("Stellamod/Assets/LaserTextures/PetalNoise");
            LaserTextures.HeavenlySlashTrail = ModContent.Request<Texture2D>("Stellamod/Assets/LaserTextures/HeavenlySlashTrail");
            LaserTextures.Bloom = ModContent.Request<Texture2D>("Stellamod/Assets/LaserTextures/Bloom");
            LaserTextures.SplittingTrail = ModContent.Request<Texture2D>("Stellamod/Assets/LaserTextures/SplittingTrail");
            Noise.Whirly = ModContent.Request<Texture2D>("Stellamod/Assets/Noise/Whirly");

            Noise.AuroraRays = ModContent.Request<Texture2D>("Stellamod/Assets/Noise/AuroraRays");
        }
        public override void OnModUnload()
        {
            base.OnModUnload();
            //Set to null otherwise we'll have a memory leak
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

    Noise.Whirly = null;
            Noise.AuroraRays = null;
        }
    }
}
