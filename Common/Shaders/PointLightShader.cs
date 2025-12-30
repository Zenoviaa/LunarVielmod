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
    public class PointLightShader : BaseShader
    {
        private EffectParameter _transformMatrixParam;
        private static PointLightShader _instance;
        public static PointLightShader Instance
        {
            get
            {
                _instance ??= new();
                _instance.SetDefaults();
                return _instance;
            }
        }

        public Matrix TransformMatrix
        {
            set
            {
                _transformMatrixParam ??= Effect.Parameters["transformMatrix"];
                _transformMatrixParam.SetValue(value);
            }
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            TransformMatrix = TrailDrawer.WorldViewPoint2;
        }
    }
}
