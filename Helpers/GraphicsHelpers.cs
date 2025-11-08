using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace Stellamod.Helpers
{
    public static class GraphicsHelpers
    {
        private static BlendState _oldBlendState;
        private static CullMode _oldCullMode;
        private static SamplerState _oldSamplerState;
        public static void SaveGraphicsDeviceState()
        {

            GraphicsDevice graphicsDevice = Main.instance.GraphicsDevice;
            _oldBlendState = graphicsDevice.BlendState;
            _oldCullMode = graphicsDevice.RasterizerState.CullMode;
            _oldSamplerState = graphicsDevice.SamplerStates[0];
        }

        public static void RestoreGraphicsDeviceState()
        {
            GraphicsDevice graphicsDevice = Main.instance.GraphicsDevice;
            graphicsDevice.RasterizerState.CullMode = _oldCullMode;
            graphicsDevice.BlendState = _oldBlendState;
            graphicsDevice.SamplerStates[0] = _oldSamplerState;
        }
    }
}
