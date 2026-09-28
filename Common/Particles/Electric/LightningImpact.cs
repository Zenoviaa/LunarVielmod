using Stellamod.Core;
using Stellamod.Core.Pixelation;
using Terraria;

namespace Stellamod.Common.Particles;

public class LightningImpact : ParticleUpdater<LightningImpact.Data>
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
        public bool IsActive => timeLeft > 0;
    }

    public override int GetPoolSize()
    {
        return 32;
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
            particle.velocity *= 0.96f;
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
        PixelationManager.QueueSpritebatchDrawAction(Draw, DrawLayer.OverPlayers);
    }

    public override void Draw(SpriteBatch spriteBatch, Vector2 screenPos)
    {
        var pass = AssetReferences.Effects.Electric.Sparking.CreatePixelPass();
        pass.Parameters.time = Main.GlobalTimeWrappedHourly;
        pass.Apply();
        using (spriteBatch.Ctx(spriteBatch.Parameters with { effect = pass.Shader }))
        {
            for (var i = 0; i < _length; i++)
            {
                (var texture, var frame) = GetParticleFrame(0);
                ref var particle = ref _particles[i];
                var fade = EasingFunction.InOutSine(particle.timeLeft / 60f);
                var drawer = SpritebatchDrawer.FromTextureAsset(texture, particle.position);
                drawer.color = particle.color;
                drawer.color *= fade;
                drawer.sourceRect = frame;
                drawer.rotation = particle.velocity.ToRotation();
                drawer.LeftCenterOrigin();
                spriteBatch.Draw(drawer);
            }
        }
    }

    public override void Draw(SpriteBatch spriteBatch, ref Data particle)
    {

    }
}
