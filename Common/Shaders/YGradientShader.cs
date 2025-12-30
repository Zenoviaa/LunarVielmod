using Microsoft.Xna.Framework;
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
    public class YGradientShader : BaseShader
    {
        private EffectParameter _startColorParam;
        private EffectParameter _endColorParam;
        private static YGradientShader _instance;
        public static YGradientShader Instance
        {
            get
            {
                _instance ??= new();
                _instance.SetDefaults();
                return _instance;
            }
        }



        public Color StartColor
        {
            set
            {
                _startColorParam ??= Effect.Parameters["startColor"];
                _startColorParam.SetValue(value.ToVector3());
            }
        }

        public Color EndColor
        {
            set
            {
                _endColorParam ??= Effect.Parameters["endColor"];
                _endColorParam.SetValue(value.ToVector3());
            }
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            StartColor = Color.White;
            EndColor = Color.Black;
        }
    }
}
