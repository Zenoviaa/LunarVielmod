using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Stellamod.Common.Shaders
{
    public class TileShadowShader : BaseShader
    {
        private EffectParameter _transformMatrixParam;
        private static TileShadowShader _instance;
        public static TileShadowShader Instance
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
