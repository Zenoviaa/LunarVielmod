using Stellamod.Core.Pixelation;
using Terraria;

namespace Stellamod.Common.Particles;

public class NiiviIceCrack : ParticleUpdater<NiiviIceCrack.Data>
{
    public struct Data : IParticleData
    {
        public Vector2 position;
        public float scale;
        public float rotation;
        public float timediv;
        public float timeLeft;
        public float timer;
        public bool IsActive => timeLeft > 0;
    }

    public override ParticleFrameData FrameData => base.FrameData with
    {
        Texture = AssetReferences.Assets.Textures.Illuria.NPCs.IceCrack.KEY,
        FrameCount = 1
    };
    public override void LoadSafe()
    {
        base.LoadSafe();
        On_Main.DrawDust += DrawParticles;
    }
    public override int GetPoolSize()
    {
        return 252;
    }

    public override void OnSpawn(ref Data particle, in int index)
    {
        base.OnSpawn(ref particle, index);
        particle.scale = Main.rand.NextFloat(0.95f, 1f);
    }
    private void DrawParticles(On_Main.orig_DrawDust orig, Main self)
    {
        orig(self);
        if (_length <= 0)
            return;
        PixelationManager.QueueSpritebatchDrawAction(Draw, DrawLayer.OverNPCs);
    }

    protected override void UpdateParticles()
    {
        base.UpdateParticles();
        for (int i = 0; i < _length; i++)
        {
            ref var particle = ref _particles[i];
            particle.timer++;
            particle.timeLeft--;
        }
    }

    public override void Draw(SpriteBatch spriteBatch, Vector2 screenPos)
    {
        for (int i = 0; i < _length; i++)
        {
            ref var particle = ref _particles[i];
            var drawer = SpritebatchDrawer.FromTextureGetLightingColor(AssetReferences.Assets.Textures.Illuria.NPCs.IceCrack.Asset, particle.position);
            drawer.rotation = particle.rotation;
            drawer.color = Color.White * 0.4f;
            drawer.color *= MathHelper.Lerp(1f, 0f, EasingFunction.InSine(particle.timer / particle.timediv));
            drawer.color.A = 0;
            var scale = Vector2.Lerp(Vector2.Zero, Vector2.One, EasingFunction.OutExpo(particle.timer / particle.timediv));
            var scale2 = Vector2.Lerp(Vector2.One, new Vector2(0.8f), particle.timer / particle.timediv);
            var scale3 = Vector2.Lerp(scale, scale2, particle.timer / particle.timediv) * 0.95f;
            drawer.scale = scale3;
            drawer.scale *= particle.scale;
            drawer.LeftCenterOrigin();
            spriteBatch.Draw(drawer);
        }
    }

}
