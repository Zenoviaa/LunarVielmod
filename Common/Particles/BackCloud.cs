using Stellamod.Core.Pixelation;
using Stellamod.Core.Rendering.RTs;
using Terraria;

namespace Stellamod.Common.Particles;

public class BackCloud : ParticleUpdater<BackCloud.Data>
{
    public struct Data : IParticleData
    {
        public Color color;
        public Vector2 position;
        public Vector2 velocity;
        public float scale;
        public float timeLeft;
        public float rotation;
        public bool IsActive => timeLeft > 0;
    }
    public override void LoadSafe()
    {
        base.LoadSafe();
        On_Main.DrawDust += DrawParticles;
    }
    private void DrawParticles(On_Main.orig_DrawDust orig, Main self)
    {
        orig(self);
        if (Main.gameMenu)
            return;
        if (_length <= 0)
            return;
        PixelationManager.QueueSpritebatchDrawAction(Draw, DrawLayer.BehindTiles);
    }

    public override int GetPoolSize()
    {
        return 250;
    }

    protected override void UpdateParticles()
    {
        base.UpdateParticles();
        for (int i = 0; i < _length; i++)
        {
            ref var particle = ref _particles[i];
            particle.position += particle.velocity;
            particle.velocity.Y = MathHelper.Lerp(particle.velocity.Y, -0.5f, 0.1f);
            particle.velocity.X = MathHelper.Lerp(particle.velocity.X, ExtraMath.Osc(-0.5f, 0.5f, speed: 0.2f, i * 3), 0.1f);
            particle.timeLeft--;
        }
    }

    public override void Draw(SpriteBatch spriteBatch, Vector2 screenPos)
    {
        //base.Draw(spriteBatch, screenPos);
        spriteBatch.EndOut(out var oldParameters);
        using var maskTarget = RT.Context(RenderTargets.ScreenTarget);
        using (RT.Clear(maskTarget, Color.Transparent))
        {

            using (spriteBatch.Ctx(SpritebatchParams.InWorldAndZoomed() with { matrix = Matrix.identity, blendState =  CustomBlendStates.Max }))
            {
                for (var i = 0; i < _length; i++)
                {
                    ref var particle = ref _particles[i];
                    float lerpValue = Utils.GetLerpValue(0, 120, particle.timeLeft, clamped: true);
                    float interpolant = EasingFunction.QuadraticBump(lerpValue);

                    var framing = GetParticleFrame(0);
                    var drawer = SpritebatchDrawer.FromTextureAsset(framing.texture, particle.position);
                    
                    drawer.sourceRect = framing.frame;
                    drawer.CenterOrigin();
                    drawer.color = Color.Lerp(Color.Transparent, particle.color, interpolant) * 0.63f;
                    drawer.scale *= MathHelper.SmoothStep(1.4f, 0.2f, lerpValue) * particle.scale;
                    drawer.rotation = particle.rotation + Main.GlobalTimeWrappedHourly * 0.6f;
                    spriteBatch.Draw(drawer);
                }
            }
        }

        spriteBatch.Begin(oldParameters);
        spriteBatch.Draw(maskTarget, Vector2.Zero, Color.White * 0.6f);
    }

    public override void Draw(SpriteBatch spriteBatch, ref Data particle)
    {

    }
}

