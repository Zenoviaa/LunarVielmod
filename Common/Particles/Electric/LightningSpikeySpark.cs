using Stellamod.Core;
using Stellamod.Core.Pixelation;
using Terraria;

namespace Stellamod.Common.Particles.Electric;

public class LightningSpikeySpark : ParticleUpdater<LightningSpikeySpark.Data>
{
    public override ParticleFrameData FrameData => base.FrameData with { FrameCount = 3 };
    public struct Data : IParticleData
    {
        public static readonly Data Default = new()
        {
            timeLeft = 120,
            color = Color.Gold,
        };

        public Color color;
        public Vector2 position;
        public Vector2 velocity;
        public float scale;
        public float timeLeft;
        public byte frame;
        public bool IsActive => timeLeft > 0;
    }

    public override int GetPoolSize()
    {
        return 64;
    }

    public override void OnSpawn(ref Data particle, in int index)
    {
        base.OnSpawn(ref particle, index);
        particle.frame = (byte)Main.rand.Next(FrameData.FrameCount);
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
            particle.velocity *= 0.8f;
            if (particle.timeLeft < 30)
            {
                particle.velocity *= 0.94f;
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
        PixelationManager.QueueSpritebatchDrawAction(Draw, DrawLayer.OverPlayers);
    }

    public override void Draw(SpriteBatch spriteBatch, Vector2 screenPos)
    {
        var pass = AssetReferences.Effects.Electric.Sparking.CreatePixelPass();
        pass.Parameters.time = Main.GlobalTimeWrappedHourly;
        pass.Apply();
        using (spriteBatch.Ctx(spriteBatch.Parameters with { effect = pass.Shader, blendState = BlendState.Additive }))
        {
            for (var i = 0; i < _length; i++)
            {
                ref var particle = ref _particles[i];
                (var texture, var frame) = GetParticleFrame(particle.frame);
                var fade = EasingFunction.InOutSine(particle.timeLeft / 60f);
                var drawer = SpritebatchDrawer.FromTextureAsset(texture, particle.position);
                drawer.color = particle.color;
                drawer.color = Color.Lerp(drawer.color, Color.DarkOrange, 1f - fade);
                drawer.color *= fade;
                drawer.color *= 0.6f;
                drawer.sourceRect = frame;
                drawer.rotation = particle.velocity.ToRotation();
                drawer.scale *= particle.scale;
                drawer.CenterOrigin();
                spriteBatch.Draw(drawer);

                drawer.scale *= 2f;
                drawer.color *= 0.5f;
                spriteBatch.Draw(drawer);
            }
        }
    }
    public override void Draw(SpriteBatch spriteBatch, ref Data particle)
    {

    }
}
