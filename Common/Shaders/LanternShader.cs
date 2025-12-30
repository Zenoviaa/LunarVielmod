using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stellamod.Common.Shaders
{
    public class LanternShader : BaseShader
    {
        private EffectParameter _transformMatrixParam;
        private static LanternShader _instance;
        public static LanternShader Instance
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
