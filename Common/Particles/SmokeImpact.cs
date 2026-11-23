using Terraria;

namespace Stellamod.Common.Particles;

public class SmokeBoom : ParticleUpdater<SmokeBoom.Data>
{
    public struct Data : IParticleData
    {
        public Color color;
        public Vector2 position;
        public Vector2 velocity;
        public float rot;
        public byte frame;
        public float timer;
        public bool IsActive => timer < 32;
    }
    public override ParticleFrameData FrameData => base.FrameData with
    {
        FrameCount = 6,
        Texture = AssetReferences.Assets.Textures.Particle.SmokeBoom.KEY
    };

    public override void LoadSafe()
    {
        base.LoadSafe();
        On_Main.DrawDust += DrawSmokeImpacts;
    }

    protected override void UpdateParticles()
    {
        base.UpdateParticles();
        for (var k = 0; k < _length; k++)
        {
            ref var particle = ref _particles[k];
            particle.timer++;
            particle.color *= 0.95F;
            particle.position += particle.velocity;
            particle.velocity *= 0.94F;
            particle.rot += 0.005F;
            particle.frame = (byte)(particle.timer / 6F);
            Lighting.AddLight(particle.position, new Vector3(1f));
        }
    }

    private void DrawSmokeImpacts(On_Main.orig_DrawDust orig, Main self)
    {
        orig(self);
        if (Main.gameMenu || _length <= 0)
            return;
        Main.spriteBatch.Begin(SpritebatchParams.InWorldAndZoomed());
        Draw(Main.spriteBatch, Main.screenPosition);
        Main.spriteBatch.End();
    }

    public override void Draw(SpriteBatch spriteBatch, Vector2 screenPos)
    {
        for (var k = 0; k < _length; k++)
        {
            ref var particle = ref _particles[k];
            var framing = GetParticleFrame(particle.frame);
            var drawer = SpritebatchDrawer.FromTextureGetLightingColor(framing.texture, particle.position);
            drawer.sourceRect = framing.frame;
            drawer.CenterOrigin();
            drawer.rotation = particle.rot;
            drawer.color = particle.color;//.MultiplyRGB(drawer.color);
            drawer.scale *= MathHelper.Lerp(1f, 1.6F, particle.timer / 24F);
            spriteBatch.Draw(drawer);
        }
        //base.Draw(spriteBatch, screenPos);
    }
}

public class SmokeImpact : ParticleUpdater<SmokeImpact.Data>
{
    public struct Data : IParticleData
    {
        public Color color;
        public Vector2 position;
        public Vector2 velocity;
        public float rot;
        public byte frame;
        public float timer;
        public bool IsActive => timer < 20;
    }
    public override ParticleFrameData FrameData => base.FrameData with 
    { 
        FrameCount = 5, 
        Texture = AssetReferences.Assets.Textures.Particle.SmokeImpact.KEY 
    };

    public override void LoadSafe()
    {
        base.LoadSafe();
        On_Main.DrawDust += DrawSmokeImpacts;
    }

    protected override void UpdateParticles()
    {
        base.UpdateParticles();
        for(var k = 0; k < _length; k++)
        {
            ref var particle = ref _particles[k];
            particle.timer++;
            particle.position += particle.velocity;
            particle.velocity *= 0.87F;
            particle.frame = (byte)(particle.timer / 4F);
        }
    }

    private void DrawSmokeImpacts(On_Main.orig_DrawDust orig, Main self)
    {
        orig(self);
        if (Main.gameMenu || _length <= 0)
            return;
        Main.spriteBatch.Begin(SpritebatchParams.InWorldAndZoomed());
        Draw(Main.spriteBatch, Main.screenPosition);
        Main.spriteBatch.End();
    }

    public override void Draw(SpriteBatch spriteBatch, Vector2 screenPos)
    {
        for(var k = 0; k <  _length; k++)
        {
            ref var particle = ref _particles[k];
            var framing = GetParticleFrame(particle.frame);
            var drawer = SpritebatchDrawer.FromTextureGetLightingColor(framing.texture, particle.position);
            drawer.sourceRect = framing.frame;
            drawer.CenterOrigin();
            drawer.rotation = particle.rot;
            drawer.color = particle.color.MultiplyRGB(drawer.color);
            spriteBatch.Draw(drawer);
        }
        //base.Draw(spriteBatch, screenPos);
    }
}
