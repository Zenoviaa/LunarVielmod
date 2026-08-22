using Stellamod.Common.Shaders;
using Stellamod.Core.Foggy;
using System;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Core.LunarLightingSystem;

public partial class LunarLightingRenderer
{
    //TODO: better batched
    public Fog SetupFog(Point position, Action<Fog> createFogFunc)
    {
        if (_fogIndex.ContainsKey(position))
            return _fogIndex[position];
        else
        {
            Fog fog = new Fog();
            fog.tilePosition = position;
            fog.position = new Vector2(position.X * 16, position.Y * 16);
            createFogFunc?.Invoke(fog);
            _fogIndex.Add(position, fog);
            return fog;
        }
    }

    private void UpdateFog()
    {
        foreach (var kvp in _fogIndex)
        {
            Fog fog = kvp.Value;
            fog.Update();
            float dist = Vector2.Distance(fog.position, Main.LocalPlayer.position);
            if (dist > 2000)
            {
                _fogsToRemove.Add(fog);
            }
        }

        for (int i = 0; i < _fogsToRemove.Count; i++)
        {
            Fog fog = _fogsToRemove[i];
            _fogIndex.Remove(fog.tilePosition);
        }
        _fogsToRemove.Clear();
    }

    private void RenderFog()
    {
        DomainExpansionManager domainExpansionManager = ModContent.GetInstance<DomainExpansionManager>();
        if (domainExpansionManager.inSpace)
            return;

        SpriteBatch spriteBatch = Main.spriteBatch;
        var config = ModContent.GetInstance<LunarVeilClientConfig>();
        if (_fogIndex.Count <= 0)
            return;

        var texture = TextureRegistry.Clouds6;
        //Apply Fog Shader
        var fogShader = FogShader.Instance;
        fogShader.FogTexture = texture;
        fogShader.ProgressPower = 0.75f;
        fogShader.EdgePower = 1f;
        fogShader.Speed = 1f;
        fogShader.Apply();
        var currentTexture = texture;
        var blendState = BlendState.AlphaBlend;
        BaseShader currentShader = fogShader;

        spriteBatch.Begin(SpriteSortMode.Immediate, blendState, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer,
            currentShader.Effect, Main.GameViewMatrix.TransformationMatrix);

        foreach (var kvp in _fogIndex)
        {
            var fog = kvp.Value;
            if (config.FocusMode && fog.disableWithFocus)
                continue;

            BaseShader newShader = null;
            if (fog.shaderFunc != null)
            {
                newShader = fog.shaderFunc();
            }

            if (blendState != fog.blendState || newShader != currentShader)
            {
                currentTexture = fog.texture;
                currentShader = newShader;
                blendState = fog.blendState;

                Effect effect = null;
                if (currentShader != null)
                    effect = currentShader.Effect;
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Immediate, blendState, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer,
                    effect, Main.GameViewMatrix.TransformationMatrix);
            }

            Vector2 center = fog.position - Main.screenPosition;
            Vector2 scale = Vector2.One * fog.scale;
            Vector2 origin = fog.texture.Size() / 2;
            spriteBatch.Draw(currentTexture.Value, center, null, fog.color, fog.rotation, origin, scale, SpriteEffects.None, 0f);
        }

        spriteBatch.End();
    }
}
