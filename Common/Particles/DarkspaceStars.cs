using Stellamod.Core.Pixelation;
using Stellamod.Core.Rendering.RTs;
using System;
using Terraria;
using Terraria.Graphics.Shaders;

namespace Stellamod.Common.Particles;

public class DarkspaceStars : ParticleUpdater<DarkspaceStars.Data>
{
    public struct Data : IParticleData
    {
        public Color color;
        public Vector2 position;
        public Vector2 velocity;
        public float timer;
        public float timeLeft;
        public bool IsActive => timeLeft > 0;
    }
    public override ParticleFrameData FrameData => base.FrameData with { FrameCount = 1, Texture = AssetReferences.Assets.Textures.Particle.DarkspaceStar.KEY };
    public override void LoadSafe()
    {
        base.LoadSafe();
        On_Main.DrawDust += DrawParticles;
    }

    protected override void UpdateParticles()
    {
        base.UpdateParticles();
        for (var i = 0; i < _length; i++)
        {
            ref var particle = ref _particles[i];
            particle.position += particle.velocity;
            particle.timer++;
            particle.timeLeft--;
        }
    }
    public override int GetPoolSize()
    {
        return 256;
    }
    private void DrawParticles(On_Main.orig_DrawDust orig, Main self)
    {
        orig(self);
        if (Main.gameMenu)
            return;
        if (_length <= 0)
            return;
        PixelationManager.QueueSpritebatchDrawAction(Draw, DrawLayer.OverWater);
    }
    private Vector2 GetScreenOffset(float scale)
    {
        //Apply an offset so the texture doesn't move when you're moving
        //This will wrap inside the shader
        Vector2 texelSize = Vector2.One / new Vector2(Main.screenWidth, Main.screenHeight);
        Vector2 screenoffset = Main.screenPosition * texelSize;
        screenoffset *= (1f / scale);
        return screenoffset;
    }

    public override void Draw(SpriteBatch spriteBatch, Vector2 screenPos)
    {
        spriteBatch.EndOut(out var oldParameters);
        using var maskTarget = RT.Context(RenderTargets.ScreenTarget);
        using(RT.Clear(maskTarget, Color.Transparent))
        {
            spriteBatch.Begin(oldParameters with { matrix = Matrix.Identity });
            spriteBatch.Draw(Main.waterTarget, Main.sceneWaterPos - Main.screenPosition, Color.White);
            spriteBatch.End();
        }
        using var starryTarget = RT.Context(RenderTargets.ScreenTarget);
        using(RT.Clear(starryTarget, Color.Transparent))
        {
            spriteBatch.Begin(oldParameters with { matrix = Matrix.Identity });
            (Texture2D texture, Rectangle frame) = GetParticleFrame(0);
            var drawer = SpritebatchDrawer.FromTextureWhite(texture);
            drawer.CenterOrigin();

            //base.Draw(spriteBatch, screenPos);
            for (var i = 0; i < _length; i++)
            {
                ref var particle = ref _particles[i];
                drawer.worldPosition = particle.position;

                var outScale = EasingFunction.InOutSine(particle.timeLeft / 60F);
                var inScale = EasingFunction.InOutSine(particle.timer / 60F);
                drawer.color = particle.color;
                drawer.color.A = 0;

                var extraScale = (float)particle.color.A / 1255F;
                drawer.scale = new Vector2(outScale * inScale * 0.08F + extraScale * inScale * outScale) ;
                spriteBatch.Draw(drawer);
     
            }
            spriteBatch.End();
        }

        using var starryTarget2 = RT.Context(RenderTargets.ScreenTarget);
        using(RT.Clear(starryTarget2, Color.Transparent))
        {

            var starsTexture = AssetReferences.Assets.NoiseTextures.Stars.Asset;
            var noiseTexture = TextureRegistry.BlurryPerlinNoise2;
            MiscShaderData eff = GameShaders.Misc["LunarVeil:RoyalCapitalStars"];

            eff.Shader.Parameters["primaryTexture"].SetValue(starsTexture.Value);
            eff.Shader.Parameters["primaryTextureSize"].SetValue(starsTexture.Value.Size());
            eff.Shader.Parameters["resolution"].SetValue(new Vector2(Main.screenWidth, Main.screenHeight));
            eff.Shader.Parameters["screenOffset"].SetValue(GetScreenOffset(scale: 1));
            eff.UseImage2(noiseTexture);
            eff.Shader.Parameters["parallax"].SetValue(Main.screenPosition * starryTarget2.Target.GetTexelSize());
            eff.Shader.Parameters["gradientFade"].SetValue(0F);
            eff.UseOpacity(1f);
            eff.Apply();

            spriteBatch.Begin(
                SpriteSortMode.Deferred,
                BlendState.AlphaBlend,
                SamplerState.PointWrap,
                DepthStencilState.None,
                Main.Rasterizer,
                eff.Shader,
                Main.BackgroundViewMatrix.TransformationMatrix);

            //     spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, Main.Rasterizer, eff.Shader, Main.BackgroundViewMatrix.TransformationMatrix);
            spriteBatch.Draw(starsTexture.Value,
               new Rectangle(0, 0, Main.screenWidth, Main.screenHeight),
                null, Color.White * 1f);

            /*
            spriteBatch.Draw(starsTexture.Value, 
                new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), 
                new Rectangle((int)-_parallax.X, (int)-_parallax.Y, Main.screenWidth, Main.screenHeight), Color.White * 0.3f);
            */
            spriteBatch.End();
        }
        var maskPass = AssetReferences.Effects.MaskA.CreatePixelPass();
        maskPass.Parameters.maskTexture = maskTarget;
        maskPass.Apply();
        spriteBatch.Begin(oldParameters with { effect = maskPass.Shader});
        spriteBatch.Draw(starryTarget, Vector2.Zero, Color.White);
        spriteBatch.Draw(starryTarget2, Vector2.Zero, Color.White);
        spriteBatch.End();
        spriteBatch.Begin(oldParameters);

    }
}
