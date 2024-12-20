using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Common.Shaders;
using Stellamod.Trails;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Common.Foggy
{
    internal class FogSystem : ModSystem
    {
        private readonly Dictionary<Point, Fog> _fogIndex = new();
        private readonly List<Fog> _fogsToRemove = new();
        public bool doDraws = true;
        public override void Load()
        {
            base.Load();
            On_Main.DrawDust += DrawFog;
        }

        public override void Unload()
        {
            base.Unload();
            On_Main.DrawDust -= DrawFog;
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
                if (dist > 1000)
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

        private void DrawFog(On_Main.orig_DrawDust orig, Main self)
        {
            orig(self);
            if (doDraws)
            {
                var texture = TrailRegistry.Clouds3;
                SpriteBatch spriteBatch = Main.spriteBatch;


                //Apply Fog Shader
                var fogShader = FogShader.Instance;
                fogShader.FogTexture = texture;
                fogShader.ProgressPower = 0.75f;
                fogShader.EdgePower = 1f;
                fogShader.Speed = 1f;
                fogShader.Apply();
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer,
                    fogShader.Effect, Main.GameViewMatrix.TransformationMatrix);

                foreach (var kvp in _fogIndex)
                {
                    var fog = kvp.Value;
                    Vector2 center = fog.position - Main.screenPosition;
                    Vector2 scale = Vector2.One * fog.scale;
                    Vector2 origin = fog.texture.Size() / 2;

                    fogShader.FogTexture = fog.texture;
                    fogShader.Speed = fog.scrollSpeed;
                    fogShader.Offset = fog.offset;
                    fogShader.Apply();
                    spriteBatch.Draw(fog.texture.Value, center, null, fog.color, fog.rotation, origin, scale, SpriteEffects.None, 0f);
                }

                spriteBatch.End();
            }
        }
    }
}
