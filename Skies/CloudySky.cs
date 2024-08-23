using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace Stellamod.Skies
{
    public class CloudySky : CustomSky
    {
        public override void Activate(Vector2 position, params object[] args)
        {
            worldSurface = (float)Main.worldSurface * 16f;
            _active = true;
            pointOfInterest = position;
            strengthTarget = 1f;
        }

        public override void Deactivate(params object[] args)
        {
            _active = false;
            strengthTarget = 0;
        }

        public override bool IsActive() => _strength > 0.001f && !Main.gameMenu;

        public override void Reset()
        {
            _active = false;
            strengthTarget = 0;
        }

        private bool _active;
        private float _strength;
        private float _windSpeed;
        private float _brightness;
        public static float strengthTarget;
        public static Color lightColor;
        public static Vector2 pointOfInterest;
        public static float worldSurface;

        public float Strength { get => _strength; }

        public static float? forceStrength;

        public override float GetCloudAlpha() => 1f - _strength;

        public override void Update(GameTime gameTime)
        {
            if (!_active)
            {
                _strength = Math.Max(0f, _strength - 0.01f);
            }
            else if (strengthTarget != 0f)
            {
                if (_active && _strength < strengthTarget)
                {
                    _strength = Math.Min(strengthTarget, _strength + 0.01f);
                }
                else
                {
                    _strength = Math.Max(0, _strength - 0.01f);
                }

                if (forceStrength.HasValue)
                {
                    _strength = forceStrength.Value;
                    forceStrength = null;
                }


                else
                {
                    pointOfInterest = Vector2.Lerp(pointOfInterest, Main.screenPosition + Main.ScreenSize.ToVector2() / 2f, 0.05f);
                    worldSurface = MathHelper.Lerp(worldSurface, (float)Main.worldSurface * 16, 0.1f);
                }
            }

            _brightness = MathHelper.Lerp(_brightness, 0.15f, 0.08f);
            _windSpeed += 0.0025f;// Main.WindForVisuals * 0.005f;
            _windSpeed = _windSpeed % 10f;
        }

        public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
        {
            if(maxDepth >= 10 && minDepth < 10)
            {
                var texture = ModContent.Request<Texture2D>("Stellamod/Textures/Clouds");
                var cloudTexture = ModContent.Request<Texture2D>("Stellamod/Textures/ColorMap");
                MiscShaderData eff = GameShaders.Misc["Stellamod:Clouds"];
                eff.UseImage1(texture);
                eff.UseImage2(cloudTexture);
                eff.Shader.Parameters["uIntensity"].SetValue(0.5f);
                eff.Shader.Parameters["uProgress"].SetValue(2);
                eff.Shader.Parameters["uOpacity"].SetValue(0.66f);
                eff.Shader.Parameters["uTime"].SetValue(_windSpeed / 32f);
                eff.Shader.Parameters["uColorMapSection"].SetValue(0.2f);
                eff.Apply();
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, eff.Shader, Main.BackgroundViewMatrix.TransformationMatrix);

                spriteBatch.Draw(texture.Value, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.White * 0.3f);

                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.BackgroundViewMatrix.TransformationMatrix);
            }
        }
    }
}