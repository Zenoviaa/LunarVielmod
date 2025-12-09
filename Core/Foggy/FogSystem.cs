using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Assets;
using Stellamod.Core.Effects;
using Stellamod.Core.Shaders;
using Stellamod.Helpers;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;

namespace Stellamod.Core.Foggy
{
    public class FogSystem : ModSystem
    {
        private readonly Dictionary<Point, Fog> _fogIndex = new();
        private readonly List<Fog> _fogsToRemove = new();
        public bool doDraws = true;
        public override void Load()
        {
            base.Load();
            On_OverlayManager.Draw += DrawFog;
        }
        public override void Unload()
        {
            base.Unload();
            On_OverlayManager.Draw -= DrawFog;
        }

        private void DrawFog(On_OverlayManager.orig_Draw orig, OverlayManager self, SpriteBatch spriteBatch, RenderLayers layer, bool beginSpriteBatch)
        {
            orig(self, spriteBatch, layer, beginSpriteBatch);
            if(layer == RenderLayers.ForegroundWater)
            {
                if (doDraws)
                {

                    if (_fogIndex.Count <= 0)
                        return;

                    var texture = TextureRegistry.Clouds6;
                    //Apply Fog Shader
                    var fogShader = Shaders.FogShader.Instance;
                    fogShader.FogTexture = texture;
                    fogShader.ProgressPower = 0.75f;
                    fogShader.EdgePower = 1f;
                    fogShader.Speed = 1f;
                    fogShader.Apply();
                    var currentTexture = texture;
                    var blendState = BlendState.AlphaBlend;
                    BaseShader currentShader = fogShader;

                    spriteBatch.End();
                    spriteBatch.Begin(SpriteSortMode.Immediate, blendState, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer,
                        currentShader.Effect, Main.GameViewMatrix.TransformationMatrix);

                    foreach (var kvp in _fogIndex)
                    {
                        var fog = kvp.Value;
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
                    spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);
                }
            }
        }


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

        public override void PostUpdateWorld()
        {
            base.PostUpdateWorld();
            UpdateFog();

        }


    }
}
