using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;

namespace Stellamod.Helpers
{
    internal class ScreenShaderSystem : ModSystem
    {
        //Vignette Shader
        private bool _useVignette;
        private float _vignetteStrength;
        private float _targetVignetteStrength;

        //Distortion Shader
        private Vector2 _distortionScroll;
        private Vector2 _distortionScrollSpeed;
        private float _distortionBlend;
        private float _targetDistortionBlend;
        private bool _useDistortion;

        //Tint Shader
        private bool _useTint;
        private float _tintOpacity;
        private Color _tintColor;
        private float _targetTintOpacity;


        private FilterManager FilterManager => Terraria.Graphics.Effects.Filters.Scene;

        /// <summary>
        /// Negative strength makes it white, positive strength makes it black
        /// </summary>
        /// <param name="strength"></param>
        public void VignetteScreen(float strength)
        {
            _useVignette = true;
            _targetVignetteStrength = strength;
        }

        public void UnVignetteScreen()
        {
            _useVignette = false;
        }

        public void DistortScreen(string normalTexture, Vector2 scrollSpeed, float blend = 0.05f)
        {
            Texture2D texture = ModContent.Request<Texture2D>(normalTexture).Value;
            DistortScreen(texture, scrollSpeed, blend);
        }

        public void DistortScreen(Texture2D normalTexture, Vector2 scrollSpeed, float blend = 0.05f)
        {
            _useDistortion = true;
            _distortionScrollSpeed = scrollSpeed;
            _targetDistortionBlend = blend;

            var shaderData = FilterManager[ShaderRegistry.Screen_NormalDistortion].GetShader();
            shaderData.UseImage(normalTexture, index: 1);
            shaderData.UseProgress(blend);
        }

        public void UnDistortScreen()
        {
            _useDistortion = false;
        }

        public void TintScreen(Color color, float targetOpacity)
        {
            _useTint = true;
            _tintColor = color;
            _targetTintOpacity = targetOpacity;
        }

        public void UnTintScreen()
        {
            _useTint = false;
        }

        public override void PostUpdateEverything()
        {
            UpdateDistortion();
            UpdateTint();
            UpdateVignette();
        }

        private void UpdateVignette()
        {
            if (_useVignette)
            {
                if (!FilterManager[ShaderRegistry.Screen_Vignette].IsActive())
                {
                    FilterManager.Activate(ShaderRegistry.Screen_Vignette);
                }

                _vignetteStrength = MathHelper.Lerp(_vignetteStrength, _targetVignetteStrength, 0.1f);
                var shaderData = FilterManager[ShaderRegistry.Screen_Vignette].GetShader();
                shaderData.UseProgress(_vignetteStrength);
            }
            else
            {
                if(_vignetteStrength != 0)
                {
                    _vignetteStrength = MathHelper.Lerp(_vignetteStrength, 0, 0.1f);
                    var shaderData = FilterManager[ShaderRegistry.Screen_Vignette].GetShader();
                    shaderData.UseProgress(_vignetteStrength);
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
