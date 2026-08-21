using Microsoft.Xna.Framework.Input;
using ReLogic.Threading;
using Stellamod.Common.Shaders;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Core.LunarLightingSystem;

public partial class LunarLightingRenderer
{
    private void RenderToLightsRT()
    {
        if (Keyboard.GetState().IsKeyDown(Keys.K))
        {
            Main.time += 64;
        }
        if (Main.gameMenu)
            return;
        if (!IsLightingEnabled)
            return;
        if (!Lighting.UsingNewLighting)
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


        //We could gpu instance this instead
        //Would be a lot faster
        //Would just need position and color data, would remove a lot of the work from the cpu
        VertexPositionColorTexture[] vertices = new VertexPositionColorTexture[_pointLights.UsedLightCount * 4];

        //Prepare the index buffer, we need to draw all the lights in the same batch
        int[] indices = new int[_pointLights.UsedLightCount * 6];
        int connectIndex = 0;
        for (int i = 0; i < indices.Length; i += 6)
        {
            indices[i] = connectIndex + 0;
            indices[i + 1] = connectIndex + 2;
            indices[i + 2] = connectIndex + 3;
            indices[i + 3] = connectIndex + 0;
            indices[i + 4] = connectIndex + 1;
            indices[i + 5] = connectIndex + 3;
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
            vertices[startIndex + 0] = new VertexPositionColorTexture(new Vector3(topLeft, 0), lightColor, new Vector2(0, 0));
            vertices[startIndex + 1] = new VertexPositionColorTexture(new Vector3(topRight, 0), lightColor, new Vector2(1, 0));
            vertices[startIndex + 2] = new VertexPositionColorTexture(new Vector3(bottomLeft, 0), lightColor, new Vector2(0, 1));
            vertices[startIndex + 3] = new VertexPositionColorTexture(new Vector3(bottomRight, 0), lightColor, new Vector2(1, 1));
        }

        if (vertices.Length <= 0 || indices.Length <= 0)
            return;

        //Get the shadow map texture
        _shadowMap.Output();

        //We have to use a blend state that takes the brightest color otherwies shadows would be able to blend over other
        //Lights
        //Actually not sure if we need that with this specific implementation


        var shadow2 = LightingShader.Instance;
        shadow2.ShadowMap = _shadowMap.Texture;
        shadow2.TransformMatrix = TrailDrawer.WorldViewPoint2;
     //  shadow2.ShadowAlpha = LightingHelper.DayLightEase;


        //Using the max color state gives a really nice look on colors
        //Additive seems to just lerp towards white which looks kinda bland
        graphicsDevice.BlendState = CustomBlendStates.Brightest;
        graphicsDevice.RasterizerState = RasterizerState.CullNone;


        int primitiveCount = vertices.Length / 2;
        shadow2.ApplyPasses();
        graphicsDevice.RasterizerState = RasterizerState.CullNone;
        graphicsDevice.DrawUserIndexedPrimitives(
            PrimitiveType.TriangleList, vertices, 0, vertices.Length, indices, 0, primitiveCount);
    }
}

