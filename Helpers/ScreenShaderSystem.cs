using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Helpers
{
    internal class ScreenShaderSystem : ModSystem
    {
        //Vignette Shader
        private bool _useVignette;
        private float _vignetteStrength;
        private float _vignetteOpacity;
        private float _vignetteTimer;
        private float _targetVignetteStrength;
        private float _targetVignetteOpacity;

        //Distortion Shader
        private Vector2 _distortionScroll;
        private Vector2 _distortionScrollSpeed;
        private float _distortionBlend;
        private float _targetDistortionBlend;
        private float _distortionTimer;
        private bool _useDistortion;

        //Tint Shader
        private bool _useTint;
        private float _tintTimer;
        private float _tintOpacity;
        private Color _tintColor;
        private float _targetTintOpacity;


        private FilterManager FilterManager => Terraria.Graphics.Effects.Filters.Scene;

        /// <summary>
        /// Negative strength makes it white, positive strength makes it black
        /// </summary>
        /// <param name="strength"></param>
        public void VignetteScreen(float strength, float opacity = 1f, float timer = -1f)
        {
            if (Main.netMode == NetmodeID.Server)
                return;
            _useVignette = true;
            _targetVignetteOpacity = opacity;
            _targetVignetteStrength = strength;
            if(timer != -1)
            {
                _vignetteTimer = timer;
            }
        }

        public void UnVignetteScreen()
        {
            if (Main.netMode == NetmodeID.Server)
                return;
            _useVignette = false;
        }

        public void DistortScreen(string normalTexture, Vector2 scrollSpeed, float blend = 0.05f, float timer = -1f)
        {
            if (Main.netMode == NetmodeID.Server)
                return;
            Texture2D texture = ModContent.Request<Texture2D>(normalTexture).Value;
            DistortScreen(texture, scrollSpeed, blend, timer);
        }

        public void DistortScreen(Texture2D normalTexture, Vector2 scrollSpeed, float blend = 0.05f, float timer = -1f)
        {
            if (Main.netMode == NetmodeID.Server)
                return;
            _useDistortion = true;
            _distortionScrollSpeed = scrollSpeed;
            _targetDistortionBlend = blend;
            if(timer != -1)
            {
                _distortionTimer = timer;
            }

            var shaderData = FilterManager[ShaderRegistry.Screen_NormalDistortion].GetShader();
            shaderData.UseImage(normalTexture, index: 1);
            shaderData.UseProgress(blend);
        }

        public void UnDistortScreen()
        {
            if (Main.netMode == NetmodeID.Server)
                return;
            _useDistortion = false;
        }

        public void TintScreen(Color color, float targetOpacity, float timer = -1f)
        {
            if (Main.netMode == NetmodeID.Server)
                return;
            _useTint = true;
            _tintColor = color;
            _targetTintOpacity = targetOpacity;
            if(timer != -1)
            {
                _tintTimer = timer;
            }
        }

        public void UnTintScreen()
        {
            if (Main.netMode == NetmodeID.Server)
                return;
            _useTint = false;
        }

        public override void PostUpdateEverything()
        {
            if (Main.netMode == NetmodeID.Server)
                return;
            UpdateDistortion();
            UpdateTint();
            UpdateVignette();
        }

        private void UpdateVignette()
        {
            if (_vignetteTimer > 0)
            {
                _vignetteTimer--;
                if (_vignetteTimer <= 0)
                {
                    _useVignette = false;
                }
            }

            if (_useVignette)
            {
                if (!FilterManager[ShaderRegistry.Screen_Vignette].IsActive())
                {
                    FilterManager.Activate(ShaderRegistry.Screen_Vignette);
                }

                _vignetteStrength = MathHelper.Lerp(_vignetteStrength, _targetVignetteStrength, 0.1f);
                _vignetteOpacity = MathHelper.Lerp(_vignetteOpacity, _targetVignetteOpacity, 0.1f);
                var shaderData = FilterManager[ShaderRegistry.Screen_Vignette].GetShader();
                shaderData.UseProgress(_vignetteStrength);
                shaderData.UseOpacity(_vignetteOpacity);
            }
            else
            {
                if(_vignetteStrength != 0)
                {
                    _vignetteOpacity = MathHelper.Lerp(_targetVignetteOpacity, 0, 0.1f);
                    _vignetteStrength = MathHelper.Lerp(_vignetteStrength, 0, 0.1f);
                    var shaderData = FilterManager[ShaderRegistry.Screen_Vignette].GetShader();
                    shaderData.UseProgress(_vignetteStrength);
                    shaderData.UseOpacity(_vignetteOpacity);
                } 
                else
                {
                    if (FilterManager[ShaderRegistry.Screen_Vignette].IsActive())
                    {
                        FilterManager.Deactivate(ShaderRegistry.Screen_Vignette);
                    }
                }
            }
        }

        private void UpdateDistortion()
        {
            if(_distortionTimer > 0)
            {
                _distortionTimer--;
                if(_distortionTimer <= 0)
                {
                    _useDistortion = false;
                }
            }

            if (_useDistortion)
            {
                if (!FilterManager[ShaderRegistry.Screen_NormalDistortion].IsActive())
                {
                    FilterManager.Activate(ShaderRegistry.Screen_NormalDistortion);
                }

                _distortionBlend += 0.005f;
                if(_distortionBlend >= _targetDistortionBlend)
                {
                    _distortionBlend = _targetDistortionBlend;
                }

                _distortionScroll += _distortionScrollSpeed;
                var shaderData = FilterManager[ShaderRegistry.Screen_NormalDistortion].GetShader();
                shaderData.Shader.Parameters["scroll"].SetValue(_distortionScroll);
                shaderData.UseProgress(_distortionBlend);
            }
            else
            {
                _distortionBlend -= 0.005f;
                if (_distortionBlend <= 0)
                {
                    _distortionBlend = 0;
                    if (FilterManager[ShaderRegistry.Screen_NormalDistortion].IsActive())
                    {
                        FilterManager.Deactivate(ShaderRegistry.Screen_NormalDistortion);
                    }
                }
                else
                {
                    _distortionScroll += _distortionScrollSpeed;
                    var shaderData = FilterManager[ShaderRegistry.Screen_NormalDistortion].GetShader();
                    shaderData.Shader.Parameters["scroll"].SetValue(_distortionScroll);
                    shaderData.UseProgress(_distortionBlend);
                }


     
      
            }
        }

        private void UpdateTint()
        {
            if (_tintTimer > 0)
            {
                _tintTimer--;
                if (_tintTimer <= 0)
                {
                    _useTint = false;
                }
            }

            if (_useTint)
            {
                if (!FilterManager[ShaderRegistry.Screen_Tint].IsActive())
                {
                    FilterManager.Activate(ShaderRegistry.Screen_Tint);
                }

                _tintOpacity += 0.01f;
                if(_tintOpacity >= _targetTintOpacity)
                {
                    _tintOpacity = _targetTintOpacity;
                }

                var shaderData = FilterManager[ShaderRegistry.Screen_Tint].GetShader();
                shaderData.Shader.Parameters["uSaturation"].SetValue(_tintOpacity);
                shaderData.UseColor(_tintColor);
            }
            else
            {
                _tintOpacity -= 0.01f;
                if(_tintOpacity <= 0)
                {
                    _tintOpacity = 0;
                    if (FilterManager[ShaderRegistry.Screen_Tint].IsActive())
                    {
                        FilterManager.Deactivate(ShaderRegistry.Screen_Tint);
                    }
                }
                else
                {
                    var shaderData = FilterManager[ShaderRegistry.Screen_Tint].GetShader();
                    shaderData.Shader.Parameters["uSaturation"].SetValue(_tintOpacity);
                    shaderData.UseColor(_tintColor);
                }
            }
        }
    }
}
