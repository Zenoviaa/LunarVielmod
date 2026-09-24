using Stellamod.Core;
using Stellamod.Core.Pixelation;
using Stellamod.Core.Rendering.RTs;
using Terraria;

namespace Stellamod.Common.Particles;

public class CrackImpactDust : ParticleUpdater<CrackImpactDust.Data>
{
    public override ParticleFrameData FrameData => base.FrameData with { FrameCount = 3 };
    public struct Data : IParticleData
    {
        public static readonly Data Default = new()
        {
            color = Color.White,
            position = Vector2.Zero,
            scale = 1f,
            timeLeft = 120
        };
        public Color color;
        public Vector2 position;
        public float scale;
        public float timeLeft;
        public int frame;
        public bool IsActive => timeLeft > 0;
    }

    public override void LoadSafe()
    {
        base.LoadSafe();
        On_Main.DrawDust += DrawCrackImpactDusts;
    }
    public override void UnloadSafe()
    {
        base.UnloadSafe();
        On_Main.DrawDust -= DrawCrackImpactDusts;
    }
    private void DrawCrackImpactDusts(On_Main.orig_DrawDust orig, Main self)
    {
        orig(self);
        if (_length <= 0)
            return;
        PixelationManager.QueueSpritebatchDrawAction(Draw);
    }
    public override void OnSpawn(ref Data particle, in int index)
    {
        base.OnSpawn(ref particle, index);
        particle.frame = Main.rand.Next(3);
    }
    protected override void UpdateParticles()
    {
        base.UpdateParticles();
        for(int i = 0; i < _length; i++)
        {
            ref var particle = ref _particles[i];
            particle.timeLeft--;
        }
    }


    public override void Draw(SpriteBatch spriteBatch, Vector2 screenPos)
    {
        using var tileTargetMask = RT.Context(RenderTargets.ScreenTarget);
        using var crackMask = RT.Context(RenderTargets.ScreenTarget);
        spriteBatch.EndOut(out var oldParameters);
        using(RT.Clear(crackMask, Color.Transparent))
        {
            spriteBatch.Begin();
            (Texture2D texture, Rectangle frame) = GetParticleFrame(0);
            SpritebatchDrawer drawer = SpritebatchDrawer.FromTextureAsset(texture, Vector2.Zero);
            drawer.sourceRect = frame;
            drawer.CenterOrigin();

            for (int i = 0; i < _length; i++)
            {
                ref var particle = ref _particles[i];
                float lerp = MathHelper.Lerp(0f, 1f, EasingFunction.InOutSine(particle.timeLeft / 90f));
                drawer.color = particle.color * lerp;
                drawer.worldPosition = particle.position;
                drawer.scale = Vector2.One * particle.scale;
                drawer.VerticalFrame(particle.frame, 3);
                spriteBatch.Draw(drawer);
            }
            spriteBatch.End();
        }

        using(RT.Clear(tileTargetMask, Color.Transparent))
        {
            spriteBatch.Begin();
            spriteBatch.Draw(Main.instance.tileTarget, Main.sceneTilePos - Main.screenPosition, Color.White);
            spriteBatch.End();
        }


        var maskPass = AssetReferences.Effects.CrystalShaders.MaskCombine.CreatePixelPass();
        maskPass.Parameters.mixTexture = crackMask;
        maskPass.Apply();
        using(new SpritebatchContext(spriteBatch, oldParameters with { effect = maskPass.Shader }))
        {
            spriteBatch.Draw(tileTargetMask, Vector2.Zero, Color.White);
        }

        spriteBatch.Begin(oldParameters);
    }

    public override void Draw(SpriteBatch spriteBatch, ref Data particle) { }

}
