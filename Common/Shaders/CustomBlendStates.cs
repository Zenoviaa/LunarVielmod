using Microsoft.Xna.Framework.Graphics;

namespace Stellamod.Common.Shaders
{
    internal static class CustomBlendStates
    {
        private static BlendState _multiply;
        public static BlendState Multiply
        {
            get
            {
                if (_multiply == null)
                {
                    _multiply = new BlendState()
                    {
                        AlphaSourceBlend = Blend.DestinationAlpha,
                        AlphaDestinationBlend = Blend.Zero,
                        AlphaBlendFunction = BlendFunction.Add,
                        ColorSourceBlend = Blend.DestinationColor,
                        ColorDestinationBlend = Blend.Zero,
                        ColorBlendFunction = BlendFunction.Add
                    };
                }

                return _multiply;
            }
        }
    }
}
