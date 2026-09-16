using Stellamod.Core.Particles;
using Stellamod.Core.Pixelation;
using Terraria;

namespace Stellamod.Common.Particles;

public class GoldenLeaf : ParticleUpdater<GoldenLeaf.Data>
{
    public struct Data : IParticleData
    {
        public static readonly Data Default = new Data { timeLeft = 240, xRange = 128, yRange = 32 };
        public Vector2 rootPosition;
        public Vector2 position;
        public int frame;
        public float scale;
        public float xRange;
        public float yRange;
        public float timeLeft;
        public float rotation;
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
        particle.rotation = Main.rand.NextFloat(12);
        particle.scale = Main.rand.NextFloat(0.7f, 1f);
    }

    protected override void UpdateParticles()
    {
        base.UpdateParticles();
        for (int i = 0; i < _length; i++)
        {
            ref var particle = ref _particles[i];
            Vector2 offset = MovementUtilities.OvalProgressPoint(particle.timeLeft / 60f, MathHelper.TwoPi, particle.xRange, particle.yRange);
            particle.position = particle.rootPosition + offset;
            particle.rotation += 0.065f;
            particle.timeLeft--;
        }
    }


    public override void Draw(SpriteBatch spriteBatch, Vector2 screenPos)
    {
        (Texture2D texture, Rectangle frame) = GetParticleFrame(0);
        for (int i = 0; i < _length; i++)
        {
            ref var particle = ref _particles[i];
            float lerpValue = Utils.GetLerpValue(0, 240, particle.timeLeft, clamped: true);
            float interpolant = EasingFunction.OutSine(lerpValue);
            SpritebatchDrawer drawer = SpritebatchDrawer.FromTextureAsset(texture, particle.position);
            drawer.sourceRect = frame;
            drawer.CenterOrigin();
            drawer.rotation = particle.rotation;
            drawer.color *= interpolant;
            drawer.scale = new Vector2(particle.scale);
            drawer.VerticalFrame(particle.frame, 4);
            spriteBatch.Draw(drawer);
        }
    }

    public override void Draw(SpriteBatch spriteBatch, ref Data particle)
    {

  
    }

    public override int GetPoolSize()
    {
        return 200;
    }
}
