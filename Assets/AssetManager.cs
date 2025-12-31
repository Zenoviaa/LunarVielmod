using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.ModLoader;

namespace Stellamod.Assets
{
    public class AssetManager : ModSystem
    {
        public class GlowMask
        {
            public static Asset<Texture2D> GradientPillar;
            public static Asset<Texture2D> MuzzleFlash;
        }
 
        public override void OnModLoad()
        {
            base.OnModLoad();
            GlowMask.GradientPillar = ModContent.Request<Texture2D>("Stellamod/Assets/GlowMasks/GradientPillar");
            GlowMask.MuzzleFlash = ModContent.Request<Texture2D>("Stellamod/Assets/GlowMasks/MuzzleFlash");
        }
        public override void OnModUnload()
        {
            base.OnModUnload();
            GlowMask.GradientPillar = null;
            GlowMask.MuzzleFlash = null;
        }
    }
}
