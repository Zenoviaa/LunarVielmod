using Stellamod.Core.Rendering.RTs;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Core.Backgrounds;

[Autoload(Side = ModSide.Client)]
public class GaussianBlurHelper : ModSystem
{
    static int _oldBlurSamples;
    static int _newBlurSamples;
    public class BlurredTarget(int Width, int Height, bool HasBlurred)
    {
        public RenderTarget2D RenderTarget { get; set; } = new RenderTarget2D(Main.graphics.GraphicsDevice, Width, Height);
        public RenderTarget2D RenderTargetSwap { get; set; } = new RenderTarget2D(Main.graphics.GraphicsDevice, Width, Height);
        public bool HasBlurred { get; set;  } = HasBlurred;
    }
    static readonly Queue<Texture2D> _texturesToBlur = new();
    static readonly Dictionary<Texture2D, BlurredTarget> _blurredTextureLookup = new();
    public override void Load()
    {
        base.Load();
        On_Main.DoDraw += BlurAllTextures;
    }

    public override void Unload()
    {
        base.Unload();
        Main.QueueMainThreadAction(DisposeBlurredTextures);
    }

    public override void PostUpdateEverything()
    {
        base.PostUpdateEverything();
        _newBlurSamples = ModContent.GetInstance<LunarVeilClientConfig>().blurSamples;
        if(_oldBlurSamples != _newBlurSamples)
        {
            Main.QueueMainThreadAction(DisposeBlurredTextures);
            _oldBlurSamples = _newBlurSamples;
        }
    }

    private void DisposeBlurredTextures()
    {
        foreach (var kvp in _blurredTextureLookup)
        {
            kvp.Value.RenderTarget.Dispose();
            kvp.Value.RenderTargetSwap.Dispose();
        }
        _blurredTextureLookup.Clear();
    }

    private void BlurAllTextures(On_Main.orig_DoDraw orig, Main self, GameTime gameTime)
    {
        orig(self, gameTime);
        if (_texturesToBlur.Count <= 0)
            return;
        Main.QueueMainThreadAction(CreateBlurredTextures);

    }

    private void CreateBlurredTextures()
    {
        while (_texturesToBlur.Count > 0)
        {
            var tex = _texturesToBlur.Dequeue();
            _blurredTextureLookup.Add(tex, new(tex.Width, tex.Height, false));
        }
    }

    /// <summary>
    /// Blurs a texure according to the blur samples in the client config
    /// </summary>
    /// <param name="originalTexture"></param>
    /// <returns></returns>
    public static Texture2D ApplyBlurIfNecessary(Texture2D originalTexture)
    {
        //Skip all of this if not blurred, 0 is no blur
        if (_newBlurSamples == 0)
            return originalTexture;

        if (_blurredTextureLookup.ContainsKey(originalTexture))
        {
            var gb = _blurredTextureLookup[originalTexture];
            if (!gb.HasBlurred)
            {
                //Now we blur it
                var gaussianPass = AssetReferences.Effects.Generic.GaussianBlur.CreatePixelPass();
                gaussianPass.Parameters.texelSize = originalTexture.GetTexelSize();
                gaussianPass.Apply();

                var src = gb.RenderTarget;
                var dst = gb.RenderTargetSwap;
          
                using (RT.Clear(src, Color.Transparent))
                {
                    var sb = Main.spriteBatch;
                    sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearWrap, DepthStencilState.None, RasterizerState.CullNone, gaussianPass.Shader);
                    sb.Draw(originalTexture, Vector2.Zero, Color.White);
                    sb.End();
                }

                for (var i = 1; i < _newBlurSamples; i++)
                {
                    using (RT.Clear(dst, Color.Transparent))
                    {
                        var sb = Main.spriteBatch;
                        sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearWrap, DepthStencilState.None, RasterizerState.CullNone, gaussianPass.Shader);
                        sb.Draw(src, Vector2.Zero, Color.White);
                        sb.End();
                    }

                    var temp = src;
                    src = dst;
                    dst = temp;
                }

                if(_newBlurSamples % 2 == 0)
                {

                    using (RT.Clear(gb.RenderTarget, Color.Transparent))
                    {
                        var sb = Main.spriteBatch;
                        sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearWrap, DepthStencilState.None, RasterizerState.CullNone);
                        sb.Draw(src, Vector2.Zero, Color.White);
                        sb.End();
                    }
                }


                gb.HasBlurred = true;
            }
            return gb.RenderTarget;
        }

        //Perform gaussian blur
        _texturesToBlur.Enqueue(originalTexture);
        return originalTexture;
    }
}
