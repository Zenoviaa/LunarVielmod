using Microsoft.Xna.Framework.Input;
using ReLogic.Threading;
using Stellamod.Common.Shaders;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Core.LunarLightingSystem;

public partial class LunarLightingRenderer
{
    private VertexPositionColorTexture[] _pointLightBuffer = new VertexPositionColorTexture[MAX_POINT_LIGHTS * 4];
    private int[] _pointLightIndices = new int[MAX_POINT_LIGHTS * 6];
    private void RenderToLightsRT()
    {
        if (Main.gameMenu)
            return;
        if (!IsLightingEnabled)
            return;
        if (!LightingHelper.CanRenderPostProcessingEffects)
            return;

        var config = ModContent.GetInstance<LunarVeilClientConfig>();
        int resolution = 64;
        switch (config.ShadowQuality)
        {
            case ShadowQuality.Ultra_Low:
                resolution = 16;
                break;
            default:
            case ShadowQuality.Low:
                resolution = 32;
                break;
            case ShadowQuality.Medium:
                resolution = 64;
                break;
            case ShadowQuality.High:
                resolution = 128;
                break;
            case ShadowQuality.Very_High:
                resolution = 256;
                break;
        }
        if (_shadowMap.Resolution != resolution)
        {
            _shadowMap.Dispose();
            _shadowMap = new ShadowMap(MAX_POINT_LIGHTS, resolution);
        }
        _shadowMap.Clear();


        //Point lights do not need to be calculated every frame, we'll change this later
        if(Main.GameUpdateCount % 1 == 0)
        {
            _pointLights.Clear();
            _pointLights.GatherLights();

            FastParallel.For(0, _pointLights.UsedLightCount, delegate (int start, int end, object context)
            {
                for (int j = start; j < end; j++)
                {
                    Light light = _pointLights[j];

                    //For now all lights will have the same radius
                    //I think we need a custom vertex structure to have difference radiuses
                    _shadowMap.RayMarch(j, light.position, light.diameter);
                }
            });

            //Prepare the index buffer, we need to draw all the lights in the same batch
            int indexLength = _pointLights.UsedLightCount * 6;
            int connectIndex = 0;
            for (int i = 0; i < indexLength; i += 6)
            {
                _pointLightIndices[i] = connectIndex + 0;
                _pointLightIndices[i + 1] = connectIndex + 2;
                _pointLightIndices[i + 2] = connectIndex + 3;
                _pointLightIndices[i + 3] = connectIndex + 0;
                _pointLightIndices[i + 4] = connectIndex + 1;
                _pointLightIndices[i + 5] = connectIndex + 3;
                connectIndex += 4;
            }


            for (int i = 0; i < _pointLights.UsedLightCount; i++)
            {
                Light light = _pointLights[i];
                float r = light.diameter;
                r /= 2;
                Vector2 topLeftOffset = new Vector2(-r, -r);
                Vector2 bottomLeftOffset = new Vector2(-r, r);
                Vector2 topRightOffset = new Vector2(r, -r);
                Vector2 bottomRightOffset = new Vector2(r, r);

                Vector2 center = light.position;
                Vector2 topLeft = center + topLeftOffset;
                Vector2 bottomLeft = center + bottomLeftOffset;
                Vector2 topRight = center + topRightOffset;
                Vector2 bottomRight = center + bottomRightOffset;

                //Rotate around the center pivot
                int startIndex = i * 4;
                Color lightColor = light.color;
                _pointLightBuffer[startIndex + 0] = new VertexPositionColorTexture(new Vector3(topLeft, 0), lightColor, new Vector2(0, 0));
                _pointLightBuffer[startIndex + 1] = new VertexPositionColorTexture(new Vector3(topRight, 0), lightColor, new Vector2(1, 0));
                _pointLightBuffer[startIndex + 2] = new VertexPositionColorTexture(new Vector3(bottomLeft, 0), lightColor, new Vector2(0, 1));
                _pointLightBuffer[startIndex + 3] = new VertexPositionColorTexture(new Vector3(bottomRight, 0), lightColor, new Vector2(1, 1));
            }

            //Get the shadow map texture
            _shadowMap.Output();
        }

        GraphicsDevice graphicsDevice = Main.graphics.GraphicsDevice;
        graphicsDevice.SetRenderTarget(_lightsRT);
        graphicsDevice.Clear(_backLightColor);

        //Render Sun
        RenderSunLight();

        //SunLightManager.RenderSunLight();
        _emitters.Clear();
        foreach (var proj in Main.ActiveProjectiles)
        {
            if (proj.ModProjectile is ILightEmitter emitter)
            {
                _emitters.Add(emitter);
            }
        }

        if (_emitters.Count > 0)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            //Draw additional lights
            foreach (ILightEmitter emitter in _emitters)
            {
                emitter.RenderLight(spriteBatch);
            }
        }


        int primitiveCount = _pointLights.UsedLightCount * 2;
        if (_pointLights.UsedLightCount <= 0)
            return;

        //We have to use a blend state that takes the brightest color otherwies shadows would be able to blend over other
        //Lights
        //Actually not sure if we need that with this specific implementation
        var shadow2 = LightingShader.Instance;
        shadow2.ShadowMap = _shadowMap.Texture;
        shadow2.TransformMatrix = TrailDrawer.WorldViewPoint2;

        //Using the max color state gives a really nice look on colors
        //Additive seems to just lerp towards white which looks kinda bland
        graphicsDevice.BlendState = CustomBlendStates.Brightest;
        graphicsDevice.RasterizerState = RasterizerState.CullNone;


        shadow2.ApplyPasses();
        graphicsDevice.RasterizerState = RasterizerState.CullNone;
        graphicsDevice.DrawUserIndexedPrimitives(
            PrimitiveType.TriangleList, _pointLightBuffer, 0, _pointLightBuffer.Length, _pointLightIndices, 0, primitiveCount);
    }
}

