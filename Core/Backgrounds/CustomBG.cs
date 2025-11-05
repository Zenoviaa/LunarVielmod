using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Core.Backgrounds
{
    public abstract class CustomBG : ModType
    {
        public int Type;
        public List<CustomBGLayer> Layers = new List<CustomBGLayer>();
        public int Priority;
        public float Alpha;
        public float DrawScale;
        public Vector2 DrawOffset;
        public bool NoSurfaceOffset;
        public bool NoSurfaceLight;
        public float ParallaxYOffset;
        public bool NoParallaxY;
        public Color DrawColor;
        public virtual bool IsActive()
        {
            return false;
        }
        public virtual void SetDrawDefaults()
        {
            DrawColor = Color.White;
        }

        public virtual int GetParallaxYStartHeight()
        {
            return (int)(Main.worldSurface * 16);
        }
        public void AddLayer(CustomBGLayer layer)
        {
            Layers.Add(layer);
        }

        public void AddFogLayer(Color startColor, Color endColor)
        {

        }

        public sealed override void SetupContent()
        {
            base.SetupContent();
            DrawScale = 1;
            SetStaticDefaults();
        }

        protected sealed override void Register()
        {
            ModTypeLookup<CustomBG>.Register(this);
        }

    }
}
