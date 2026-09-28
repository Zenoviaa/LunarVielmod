using Stellamod.Core;
using Stellamod.Core.Pixelation;
using Terraria;


namespace Stellamod.Common.Particles.Electric;

public class LightningBolt : ParticleUpdater<LightningBolt.Data>
{
    public override ParticleFrameData FrameData => base.FrameData with { FrameCount = 3 };
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
        return 256;
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
            if (particle.velocity.LengthSquared() > 1)
                particle.velocity *= 0.75f;
            else
                particle.velocity = particle.velocity.Resize(0.2f);
            if(particle.timeLeft % 24 == 0)
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
        PixelationManager.QueueSpritebatchDrawAction(Draw, DrawLayer.OverPlayers);
    }

    public override void Draw(SpriteBatch spriteBatch, Vector2 screenPos)
    {
        var pass = AssetReferences.Effects.Electric.SparkyBolt.CreatePixelPass();
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
                drawer.color = Color.Lerp(drawer.color, Color.Aquamarine, 1f - fade);
                drawer.color *= fade;
                if (particle.timeLeft > 55)
                    drawer.color *= 2;
                drawer.sourceRect = frame;
                drawer.rotation = particle.velocity.ToRotation();
                drawer.scale.Y *= 0.5f;
                drawer.scale.Y *= fade;
                drawer.LeftCenterOrigin();
                spriteBatch.Draw(drawer);

                drawer.scale.Y *= 1.5f;
                drawer.color *= 0.5f;
                spriteBatch.Draw(drawer);

                drawer.scale.Y *= 1.5f;
                drawer.color *= 0.25f;
                spriteBatch.Draw(drawer);
            }
        }
    }
    public override void Draw(SpriteBatch spriteBatch, ref Data particle)
    {

    }
}
