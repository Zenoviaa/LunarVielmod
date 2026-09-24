using Stellamod.Core;
using Stellamod.Core.Pixelation;
using Stellamod.Core.Rendering.RTs;
using Terraria;

namespace Stellamod.Common.Particles;

public class Sparklemist : ParticleUpdater<Sparklemist.Data>
{

    public struct Data : IParticleData
    {
        public static readonly Data Default = new()
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
    public override ParticleFrameData FrameData => base.FrameData with { FrameCount = 4 };

    public override void LoadSafe()
    {
        base.LoadSafe();
        On_Main.DrawDust += DrawParticles;
    }
    public override int GetPoolSize()
    {
        return 200;
    }

    public override void OnSpawn(ref Data particle, in int index)
    {
        base.OnSpawn(ref particle, index);
        particle.frame = Main.rand.Next(4);
    }
    private void DrawParticles(On_Main.orig_DrawDust orig, Main self)
    {
        orig(self);
        if (_length <= 0)
            return;
        PixelationManager.QueueSpritebatchDrawAction(Draw, DrawLayer.OverPlayers);
    }

    protected override void UpdateParticles()
    {
        base.UpdateParticles();
        for(int i  = 0; i < _length; i++)
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
        using var cloudTarget = RT.Context(RenderTargets.ScreenTarget);
        using(RT.Clear(cloudTarget, Color.Transparent))
        {
            using (new SpritebatchContext(spriteBatch, spriteBatch.Parameters with
            {
                blendState = BlendState.Additive,
                matrix = Matrix.Identity
            }))
            {
                for (int i = 0; i < _length; i++)
                {
                    ref var particle = ref _particles[i];
                    var frameData = GetParticleFrame(particle.frame);
                    SpritebatchDrawer drawer = SpritebatchDrawer.FromTextureAsset(frameData.texture, particle.position);
                    float t = EasingFunction.QuadraticBump(particle.timeLeft / 60f);
                    drawer.color = Color.Lerp(Color.Transparent, particle.color, t) * 0.5f;
                    drawer.rotation = Main.GlobalTimeWrappedHourly * 0.3f + particle.timeLeft * 0.02f;
                    drawer.sourceRect = frameData.frame;
                    drawer.scale *= MathHelper.Lerp(1.2f, 0.75f, EasingFunction.InOutSine(particle.timeLeft / 90f));
                    drawer.CenterOrigin();
                    spriteBatch.Draw(drawer);
                }
            }
        }
        var outlinerPass = AssetReferences.Effects.Generic.Outliner.CreatePixelPass();
        outlinerPass.Parameters.texelSize = cloudTarget.Target.GetTexelSize();
        outlinerPass.Apply();
        spriteBatch.Begin(oldParameters with { effect = outlinerPass.Shader, blendState = CustomBlendStates.Max });
        spriteBatch.Draw(cloudTarget, Vector2.Zero, Color.DarkBlue);
        spriteBatch.End();
        spriteBatch.Begin(oldParameters);
    }

    public override void Draw(SpriteBatch spriteBatch, ref Data particle)
    {

    }
}
