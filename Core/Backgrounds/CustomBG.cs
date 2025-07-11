﻿using Microsoft.Xna.Framework;
using System.Collections.Generic;
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
        public virtual bool IsActive()
        {
            return false;
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
