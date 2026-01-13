using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.ModLoader;

namespace Stellamod.Assets
{
    public class AssetManager : ModSystem
    {
        public class GlowMask
        {
            public static Asset<Texture2D> SpiralVortex;
            public static Asset<Texture2D> SimpleGlowCircle;
            public static Asset<Texture2D> GradientPillar;
            public static Asset<Texture2D> MuzzleFlash;
            public static Asset<Texture2D> Shine;
            public static Asset<Texture2D> MagicCircle;
        }
 
        public class LaserTextures
        {
            public static Asset<Texture2D> TexturedLaser;
            public static Asset<Texture2D> TexturedLaser2;
            public static Asset<Texture2D> SnowflakeLaser;
            public static Asset<Texture2D> Lightning;
            public static Asset<Texture2D> Lightning2;

        }
        public override void OnModLoad()
        {
            base.OnModLoad();
            GlowMask.SpiralVortex = ModContent.Request<Texture2D>("Stellamod/Assets/GlowMasks/SpiralVortex");
            GlowMask.SimpleGlowCircle = ModContent.Request<Texture2D>("Stellamod/Assets/GlowMasks/SimpleGlowCircle");
            GlowMask.GradientPillar = ModContent.Request<Texture2D>("Stellamod/Assets/GlowMasks/GradientPillar");
            GlowMask.MuzzleFlash = ModContent.Request<Texture2D>("Stellamod/Assets/GlowMasks/MuzzleFlash");
            GlowMask.Shine = ModContent.Request<Texture2D>("Stellamod/Assets/GlowMasks/Shine");
            GlowMask.MagicCircle = ModContent.Request<Texture2D>("Stellamod/Assets/GlowMasks/MagicCircle1");


            LaserTextures.TexturedLaser = ModContent.Request<Texture2D>("Stellamod/Assets/LaserTextures/TexturedLaser");
            LaserTextures.TexturedLaser2 = ModContent.Request<Texture2D>("Stellamod/Assets/LaserTextures/TexturedLaser2");
            LaserTextures.SnowflakeLaser = ModContent.Request<Texture2D>("Stellamod/Assets/LaserTextures/SnowflakeLaser");
            LaserTextures.Lightning = ModContent.Request<Texture2D>("Stellamod/Assets/LaserTextures/Lightning");
            LaserTextures.Lightning2 = ModContent.Request<Texture2D>("Stellamod/Assets/LaserTextures/Lightning2");
        }
        public override void OnModUnload()
        {
            base.OnModUnload();
            //Set to null otherwise we'll have a memory leak
            GlowMask.SpiralVortex = null;
            GlowMask.GradientPillar = null;
            GlowMask.MuzzleFlash = null;
            GlowMask.SimpleGlowCircle = null;
            GlowMask.Shine = null;
            GlowMask.MagicCircle = null;

            LaserTextures.TexturedLaser = null;
            LaserTextures.TexturedLaser2 = null;
            LaserTextures.SnowflakeLaser = null;
            LaserTextures.Lightning = null;
            LaserTextures.Lightning2 = null;
        }
    }
}
