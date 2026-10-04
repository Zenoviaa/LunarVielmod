using Stellamod.Core;
using Stellamod.Core.Pixelation;
using Stellamod.Core.Rendering.RTs;
using Terraria;


namespace Stellamod.Common.Particles.Electric;

public class LightningCrackedGround : ParticleUpdater<LightningCrackedGround.Data>
{
    public override ParticleFrameData FrameData => base.FrameData with { FrameCount = 1 };
    public struct Data : IParticleData
    {
        public static readonly Data Default = new()
        {
            timeLeft = 120,
            color = Color.Gold
        };
        public Color color;
        public Vector2 position;
        public Vector2 velocity;
        public float timeLeft;
        public byte frame;
        public bool IsActive => timeLeft > 0;
    }

    public override int GetPoolSize()
    {
        return 16;
    }

    public override void OnSpawn(ref Data particle, in int index)
    {
        base.OnSpawn(ref particle, index);
        particle.frame = 0;
    }

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
            if (particle.velocity.LengthSquared() > 1)
                particle.velocity *= 0.75f;
            else
                particle.velocity = particle.velocity.Resize(0.2f);
            if (particle.timeLeft % 24 == 0)
            {
                particle.frame = (byte)Main.rand.Next(FrameData.FrameCount);
            }
            particle.timeLeft--;
        }
    }

    private void DrawParticles(On_Main.orig_DrawDust orig, Main self)
    {
        orig(self);
        if (Main.gameMenu)
            return;
        if (_length <= 0)
            return;
        PixelationManager.QueueSpritebatchDrawAction(Draw, DrawLayer.OverNPCs);
    }

    public override void Draw(SpriteBatch spriteBatch, Vector2 screenPos)
    {
        using var tileTargetMask = RT.Context(RenderTargets.ScreenTarget);
        using var crackMask = RT.Context(RenderTargets.ScreenTarget);
        spriteBatch.EndOut(out var oldParameters);
        using (RT.Clear(crackMask, Color.Transparent))
        {
            using (spriteBatch.Ctx(spriteBatch.Parameters with { blendState = BlendState.Additive }))
            {
                for (var i = 0; i < _length; i++)
                {

                    ref var particle = ref _particles[i];
                    (var texture, var frame) = GetParticleFrame(particle.frame);
                    var fade = EasingFunction.InOutSine(particle.timeLeft / 60f);
                    var drawer = SpritebatchDrawer.FromTextureAsset(texture, particle.position);
                    drawer.color = particle.color;
                    drawer.color = Color.Lerp(drawer.color, Color.DarkOrange, 0.4f);
                    drawer.color = Color.Lerp(drawer.color, Color.DarkOrange, 1f - fade);
                    drawer.color *= fade;

                    drawer.color *= 0.8f;
                    drawer.scale *= 0.6f;

                    drawer.sourceRect = frame;
                    drawer.TopCenterOrigin();
                    spriteBatch.Draw(drawer);



                }
            }
        }

        using (RT.Clear(tileTargetMask, Color.Transparent))
        {
            spriteBatch.Begin();
            spriteBatch.Draw(Main.instance.tileTarget, Main.sceneTilePos - Main.screenPosition, Color.White);
            spriteBatch.End();
        }


        var maskPass = AssetReferences.Effects.CrystalShaders.MaskCombine.CreatePixelPass();
        maskPass.Parameters.mixTexture = crackMask;
        maskPass.Apply();
        using (new SpritebatchContext(spriteBatch, oldParameters with { effect = maskPass.Shader }))
        {
            spriteBatch.Draw(tileTargetMask, Vector2.Zero, Color.White);
        }

        spriteBatch.Begin(oldParameters);

    }
    public override void Draw(SpriteBatch spriteBatch, ref Data particle)
    {

    }
}
