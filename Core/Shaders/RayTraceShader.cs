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

namespace Stellamod.Core.Shaders
{
    public class RayTraceShader : BaseShader
    {
        private EffectParameter _heightMapParam;
        private EffectParameter _screenResolutionParam;
        private EffectParameter _lightColorParam;
        private EffectParameter _lightRadiusParam;
        private EffectParameter _lightPositionParam;
        private EffectParameter _lightIntensityParam;
        private static RayTraceShader _instance;
        public static RayTraceShader Instance
        {
            get
            {
                _instance ??= new();
                _instance.SetDefaults();
                return _instance;
            }
        }
        public Texture2D HeightMap
        {
            set
            {
                _heightMapParam ??= Effect.Parameters["heightMap"];
                _heightMapParam.SetValue(value);
            }
        }
        public Vector2 ScreenResolution
        {
            set
            {
                _screenResolutionParam ??= Effect.Parameters["uScreenResolution"];
                _screenResolutionParam.SetValue(value);
            }
        }
        public Color LightColor
        {
            set
            {
                _lightColorParam ??= Effect.Parameters["lightColor"];
                _lightColorParam.SetValue(value.ToVector3());
            }
        }
        public float LightRadius
        {
            set
            {
                _lightRadiusParam ??= Effect.Parameters["lightRadius"];
                _lightRadiusParam.SetValue(value);
            }
        }
        public Vector2 LightPosition
        {
            set
            {
                _lightPositionParam ??= Effect.Parameters["lightingPos"];
                _lightPositionParam.SetValue(value);
            }
        }
        public float LightIntensity
        {
            set
            {
                _lightIntensityParam ??= Effect.Parameters["lightIntensity"];
                _lightIntensityParam.SetValue(value);
            }
        }

        public override void SetDefaults()
        {
            base.SetDefaults();

        }
    }
}
