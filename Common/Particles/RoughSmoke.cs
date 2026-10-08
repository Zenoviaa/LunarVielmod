using Stellamod.Core.Particles;
using Stellamod.Core.Pixelation;
using Stellamod.Core.Rendering.RTs;
using Terraria;

namespace Stellamod.Common.Particles;


public class RoughSmoke : ParticleUpdater<RoughSmoke.Data>
{
    public struct Data : IParticleData
    {
        public Color color;
        public Vector2 position;
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
        PixelationManager.QueueSpritebatchDrawAction(Draw, DrawLayer.OverPlayers);
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

            using (spriteBatch.Ctx(SpritebatchParams.InWorldAndZoomed() with { matrix = Matrix.identity, blendState = BlendState.Additive }))
            {
                for (var i = 0; i < _length; i++)
                {
                    ref var particle = ref _particles[i];
                    float lerpValue = Utils.GetLerpValue(0, 120, particle.timeLeft, clamped: true);
                    float interpolant = EasingFunction.QuadraticBump(lerpValue);

                    (Texture2D texture, Rectangle frame) = GetParticleFrame(0);
                    SpritebatchDrawer drawer = SpritebatchDrawer.FromTextureAsset(texture, particle.position);
                    //  drawer.worldPosition += Main.screenPosition;
                    drawer.sourceRect = frame;
                    drawer.CenterOrigin();
                    drawer.color = Color.Lerp(Color.Transparent, particle.color, interpolant) * 0.63f;
                    drawer.scale *= MathHelper.SmoothStep(1.4f, 0.2f, lerpValue) * particle.scale;
                    drawer.rotation = particle.rotation + Main.GlobalTimeWrappedHourly * 0.6f;
                    spriteBatch.Draw(drawer);
                }
            }
        }

        spriteBatch.Begin(oldParameters);
        spriteBatch.Draw(maskTarget, Vector2.Zero, Color.White);
    }

    public override void Draw(SpriteBatch spriteBatch, ref Data particle)
    {

    }
}

