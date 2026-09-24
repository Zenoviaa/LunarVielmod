using Stellamod.Common.Shaders;
using Stellamod.Core;
using Stellamod.Core.Palettes;
using Stellamod.Core.Pixelation;
using Stellamod.Core.Rendering.RTs;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace Stellamod.Common.Particles;

public struct CometMagicDustData : IParticleData
{
    public static readonly CometMagicDustData Default = new()
    {
        color = Color.White,
        timeLeft = 120
    };
    public Color color;
    public Vector2 position;
    public Vector2 velocity;
    public float timeLeft;
    public int frame;
    public bool IsActive => timeLeft > 0;
}


public class CometMagicDust : ParticleUpdater<CometMagicDustData>
{
    public override ParticleFrameData FrameData => base.FrameData with { FrameCount = 4 };

    public override void LoadSafe()
    {
        base.LoadSafe();
        On_Main.DrawDust += DrawParticles;
    }

    public override int GetPoolSize()
    {
        return 252;
    }

    private void DrawParticles(On_Main.orig_DrawDust orig, Main self)
    {
        orig(self);
        if (_length <= 0)
            return;
        PixelationManager.QueueSpritebatchDrawAction(Draw);
    }

    public override void OnSpawn(ref CometMagicDustData particle, in int index)
    {
        base.OnSpawn(ref particle, index);
        particle.frame = Main.rand.Next(4);
    }

    protected override void UpdateParticles()
    {
        base.UpdateParticles();
        for (int i = 0; i < _length; i++)
        {
            ref var particle = ref _particles[i];
            particle.position += particle.velocity;
            particle.velocity *= 0.96f;
            particle.timeLeft--;
        }
    }
    public override void Draw(SpriteBatch spriteBatch, Vector2 screenPos)
    {

        spriteBatch.EndOut(out var oldParameters);
        using var temp = RT.Context(RenderTargets.ScreenTarget);
        using(RT.Clear(temp, Color.Transparent))
        {
            var pass = AssetReferences.Effects.Generic.BasicBloom.CreatePixelPass();
            pass.Apply();
            spriteBatch.Begin(oldParameters with { blendState = BlendState.AlphaBlend, effect = pass.Shader, samplerState = SamplerState.AnisotropicClamp });
            for (int i = 0; i < _length; i++)
            {
                ref var particle = ref _particles[i];
                float alpha = EasingFunction.InOutSine(particle.timeLeft / 30f);
                Vector2 stretchScale = new Vector2(particle.velocity.Length() * 0.15f, 0f);

                (Texture2D texture, Rectangle frame) = GetParticleFrame(particle.frame);
                SpritebatchDrawer drawer = SpritebatchDrawer.FromTextureAsset(texture, particle.position);
                drawer.sourceRect = frame;
                drawer.CenterOrigin();
                drawer.scale = Vector2.One * 0.6f * alpha * EasingFunction.QuadraticBump(particle.timeLeft / 40f);
                drawer.rotation = -particle.velocity.ToRotation();
                drawer.color = Color.Lerp(Color.Black, particle.color, alpha) * 0.85f;

                spriteBatch.Draw(drawer);

            }
            spriteBatch.End();
        }

        PalettizerShader palettizerShader = PalettizerShader.Instance;
        palettizerShader.PaletteTexture = PaletteAssets.FromPaletteFile(PaletteAssets.ABYSSWATERFALL).Value.ColorAtlas;//PaletteHelper.GetColorSpectrum("MoonspiralTower.pal");
        palettizerShader.Progress = 1f;
        palettizerShader.Dither = ModContent.GetInstance<LunarVeilClientConfig>().Dither;
        palettizerShader.ImageSize = new Vector2(131, 312) * 4f;
        palettizerShader.DitherAlpha = 0.125f;

        spriteBatch.Begin(oldParameters with {  effect = palettizerShader, samplerState = SamplerState.AnisotropicClamp });
        Color g = Color.White;

        spriteBatch.Draw(temp, Vector2.Zero, g);
        spriteBatch.End();
        spriteBatch.Begin(oldParameters);
    }
    public override void Draw(SpriteBatch spriteBatch, ref CometMagicDustData particle)
    {

    }
}
