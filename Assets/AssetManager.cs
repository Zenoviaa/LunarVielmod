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
        }
 
        public override void OnModLoad()
        {
            base.OnModLoad();
            GlowMask.SpiralVortex = ModContent.Request<Texture2D>("Stellamod/Assets/GlowMasks/SpiralVortex");
            GlowMask.SimpleGlowCircle = ModContent.Request<Texture2D>("Stellamod/Assets/GlowMasks/SimpleGlowCircle");
            GlowMask.GradientPillar = ModContent.Request<Texture2D>("Stellamod/Assets/GlowMasks/GradientPillar");
            GlowMask.MuzzleFlash = ModContent.Request<Texture2D>("Stellamod/Assets/GlowMasks/MuzzleFlash");
        }
        public override void OnModUnload()
        {
            base.OnModUnload();
            GlowMask.SpiralVortex = null;
            GlowMask.GradientPillar = null;
            GlowMask.MuzzleFlash = null;
            GlowMask.SimpleGlowCircle = null;
        }
    }
}
