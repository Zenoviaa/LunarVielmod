using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Trails;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace Stellamod.Common.Shaders
{
    public class ColorMultiplyShader : BaseShader
    {
        private EffectParameter _intensityParam;
        private static ColorMultiplyShader _instance;
        public static ColorMultiplyShader Instance
        {
            get
            {
                _instance ??= new();
                _instance.SetDefaults();
                return _instance;
            }
        }

        public float Intensity
        {
            set
            {
                _intensityParam ??= Effect.Parameters["intensity"];
                _intensityParam.SetValue(value);
            }
        }


        public override void SetDefaults()
        {
            base.SetDefaults();
            Intensity = 25;
        }
    }
}
