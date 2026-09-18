using Stellamod.Core;
using Terraria;

namespace Stellamod.Common.Particles;


public struct InDonutData : IParticleData
{
    public Vector2 position;
    public float timeLeft;
    public bool IsActive => timeLeft > 0;
}

public class InDonutDust : ParticleUpdater<InDonutData>
{
    public override void LoadSafe()
    {
        base.LoadSafe();
        On_Main.DrawInfernoRings += DrawParticles;
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
    private void DrawParticles(On_Main.orig_DrawInfernoRings orig, Main self)
    {
        orig(self);
        if (Main.gameMenu)
            return;
        if (_length <= 0)
            return;
        var pass = AssetReferences.Effects.Generic.InDonut.CreatePixelPass();
        using (new SpritebatchContext(Main.spriteBatch, SpritebatchParams.InWorldAndZoomed() with { effect = pass.Shader }))
        {
            base.Draw(Main.spriteBatch, Main.screenPosition);
        }
    }

    public override void Draw(SpriteBatch spriteBatch, ref InDonutData particle)
    {
        float alpha = particle.timeLeft / 24f;
        alpha = MathHelper.Clamp(alpha, 0f, 1f);
        alpha = 1f - alpha;
        SpritebatchDrawer drawer = SpritebatchDrawer.FromTextureAsset(AssetReferences.Common.Particles.InDonutDust.Asset, particle.position);
        drawer.color = Color.White * MathHelper.Lerp(1f, 0f, alpha);
        drawer.scale = Vector2.Lerp(Vector2.One * 0.5f, Vector2.One * 2, alpha);
        spriteBatch.Draw(drawer);
    }
}
