using Stellamod.Core.Pixelation;
using Stellamod.Core.Rendering.RTs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace Stellamod.Common.Particles;

public class DiamondSparkle : ParticleUpdater<DiamondSparkle.Data>
{
    public struct Data : IParticleData
    {
        public Vector2 position;
        public Vector2 velocity;
        public Color color;
        public float timeLeft;
        public bool IsActive => timeLeft > 0;
    }
    public override ParticleFrameData FrameData => base.FrameData with 
    { 
        FrameCount = 1,
        Texture = AssetReferences.Assets.Textures.Particle.DiamondSparkle.KEY 
    };

    public override int GetPoolSize()
    {
        return 256;
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
    }

    protected override void UpdateParticles()
    {
        base.UpdateParticles();
        for (int i = 0; i < _length; i++)
        {
            ref var particle = ref _particles[i];
            particle.position += particle.velocity;
            particle.velocity *= 0.98f;
            var colo = particle.color;
            colo.ScrollHue(12);
            particle.color = colo;
            particle.timeLeft--;
        }
    }


    public override void Draw(SpriteBatch spriteBatch, Vector2 screenPos)
    {
        var framing = GetParticleFrame(0);
        var drawer = SpritebatchDrawer.FromTextureAsset(framing.texture, Vector2.Zero);
        for(var i = 0; i < _length; i++)
        {
            ref var particle = ref _particles[i];
            if (particle.timeLeft > 45)
                continue;

            drawer.worldPosition = particle.position;
            drawer.scale = new Vector2(MathHelper.Lerp(0f, 0.6f, EasingFunction.InOutSine(particle.timeLeft / 30f)));
            drawer.color = particle.color * MathHelper.Lerp(0f, 1f, EasingFunction.InOutSine(particle.timeLeft / 30F)) ;
            drawer.color.A = 0;
            spriteBatch.Draw(drawer);
        }
    }

}
