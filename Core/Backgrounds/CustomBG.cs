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
        public Vector2 startParallaxPosition;
        public Vector2 CameraMovement => Main.Camera.Center - startParallaxPosition;
        public int Priority;
        public float Alpha;
        public float DrawScale;
        public Vector2 DrawOffset;
        public bool NoSurfaceOffset;
        public bool NoSurfaceLight;
        public float ParallaxYOffset;
        public bool NoParallaxY;
        public float ParallaxYFactor;
        public bool parallaxInBothWays;
        public Color DrawColor = Color.White;
        public bool IgnorePaletteShader;
        public float LocalParallaxSpeed=1f;
        public bool ignoreSkyColor;
        public override void Unload()
        {
            base.Unload();
            Layers = null;
        }

        public virtual bool IsActive()
        {
            return false;
        }
        public virtual void SetDrawDefaults()
        {
            if (Alpha <= 0)
                DrawColor = Color.White;
        }

        public virtual bool UseCustomDrawing() => false;
        /// <summary>
        /// The spritebatch must be begun and ended within this method, only executes if UseCustomDrawing returns true
        /// </summary>
        /// <param name="spriteBatch"></param>
        public virtual void Draw(SpriteBatch spriteBatch)
        {

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
