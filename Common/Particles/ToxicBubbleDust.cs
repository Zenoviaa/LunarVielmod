using Stellamod.Core.Pixelation;
using Terraria;

namespace Stellamod.Common.Particles;

public class ToxicBubbleDust : ParticleUpdater<ToxicBubbleDust.Data>
{
    public struct Data : IParticleData
    {
        public Color color;
        public Vector2 position;
        public Vector2 velocity;
        public float scale;
        public float offset;
        public float timeLeft;
        public bool IsActive => timeLeft > 0;
    }
    public override ParticleFrameData FrameData => base.FrameData with 
    { FrameCount = 1 };
    public override void LoadSafe()
    {
        base.LoadSafe();
        On_Main.DrawDust += DrawParticles;
    }
    protected override void UpdateParticles()
    {
        base.UpdateParticles();
        for (int i = 0; i < _length; i++)
        {
            ref var particle = ref _particles[i];
            particle.position += particle.velocity;
            particle.velocity.Y += 0.3f;
            particle.timeLeft--;
            var t = particle.position.ToTileCoordinates();
            var tile = Main.tile[t];
            if (WorldGen.SolidOrSlopedTile(tile))
            {
                particle.timeLeft = -5;
                for (var j = 0; j < 3; j++)
                {
                    var pos = particle.position;
                    pos += Main.rand.NextVector2Circular(32, 32);
                    var vel = particle.velocity.RotatedByRandom(0.5f) * Main.rand.NextFloat(0.5f, 1f);
                    Particles.GlowDust.Spawn(new()
                    {
                        position = pos,
                        velocity = vel,
                        timeLeft = Main.rand.Next(45, 180),
                        color = particle.color,
                        scale = Main.rand.NextFloat(0.5f, 1f) * 0.25f
                    });
                }
            }
        }
    }

    public override void OnSpawn(ref Data particle, in int index)
    {
        base.OnSpawn(ref particle, index);
        particle.offset = Main.rand.NextFloat(0f, 6.28f);
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

    public override void Draw(SpriteBatch spriteBatch, ref Data particle)
    {
        var outAlpha = EasingFunction.InOutSine(particle.timeLeft / 90f);
        var framing = GetParticleFrame(0);
        var drawer = SpritebatchDrawer.FromTextureAsset(framing.texture, particle.position);
        drawer.sourceRect = framing.frame;
        drawer.CenterOrigin();
        drawer.color = particle.color;
        drawer.color *= outAlpha;
        drawer.rotation = particle.velocity.ToRotation();
        drawer.scale *= particle.scale;
        drawer.scale.Y *= particle.velocity.Length() / 25f;
        drawer.scale.X *= MathHelper.Lerp(0.5f, 1f, particle.velocity.Length() / 25f);
        spriteBatch.Draw(drawer);
    }

    public override int GetPoolSize()
    {
        return 256;
    }
}
