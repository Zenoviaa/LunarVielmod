using Stellamod.Core.Pixelation;
using System;
using Terraria;

namespace Stellamod.Common.Particles;


public struct CinderEmberDustData : IParticleData
{
    public static readonly CinderEmberDustData Default = new CinderEmberDustData
    {
        position = Vector2.Zero,
        velocity = Vector2.Zero,
        timeleft = 150,
    };

    //Colors on this one is calculated
    public Color color;
    public Vector2 position;
    public Vector2 velocity;
    public Vector2 startScale;
    public float timeleft;
    public float turn;
    public float parallaxStrength;
    public bool IsActive => timeleft > 0;
}
public class CinderEmberDustBackground : CinderEmberDust
{
    public override void LoadSafe()
    {
        On_Main.CheckMonoliths += DrawBehindtiles;
    }

    private void DrawBehindtiles(On_Main.orig_CheckMonoliths orig)
    {
        orig();
        if (Main.gameMenu)
            return;

        if (_length <= 0)
            return;

        PixelationManager.QueueSpritebatchDrawAction(Draw, DrawLayer.BehindTiles);
    }
}

public class CinderEmberDust : ParticleUpdater<CinderEmberDustData>
{
    public override ParticleFrameData FrameData => base.FrameData with { FrameCount = 1 };
    public override DrawLayer PixelationDrawLayer => DrawLayer.OverNPCsWithOutline;
    public override int GetPoolSize()
    {
        return 200;
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
        PixelationManager.QueueSpritebatchDrawAction(Draw, PixelationDrawLayer);
    }

    public override void OnSpawn(ref CinderEmberDustData particle, in int index)
    {
        base.OnSpawn(ref particle, index);
        particle.turn = Main.rand.NextFloat(-0.005f, 0.005f);
        particle.velocity.Y = Main.rand.NextFloat(-5, -2.5f);
        particle.velocity.X = Main.rand.NextFloat(1.5f, 3f);

        particle.startScale = Vector2.One * Main.rand.NextFloat(0.025f, 0.3f);
        particle.startScale.Y += Main.rand.NextFloat(0f, 0.05f);
        particle.startScale *= 0.8f;
    }

    protected override void UpdateParticles()
    {
        base.UpdateParticles();
        for (int i = 0; i < _length; i++)
        {
            ref var particle = ref _particles[i];
            particle.position += particle.velocity;
            particle.position += (Main.screenPosition - Main.screenLastPosition) * -particle.parallaxStrength;
            particle.timeleft--;


            float interpolant = Utils.GetLerpValue(0, 120, particle.timeleft, clamped: true);
    
            Color glowColor = Color.Lerp(Color.Yellow, Color.Red,
              ExtraMath.Osc(0f, 1f, speed: 16, offset: i));

            Color color = glowColor;
            color *= EasingFunction.QuadraticBump(interpolant);
            particle.color = color;
            particle.velocity.X += particle.turn;

            particle.turn *= 0.99f;
        }
    }

    public override void Draw(SpriteBatch spriteBatch, ref CinderEmberDustData particle)
    {
        float lerpValue = Utils.GetLerpValue(0, 120, particle.timeleft, clamped: true);
        float interpolant = EasingFunction.QuadraticBump(lerpValue);

        float outScaler = MathHelper.Lerp(1f, 0f, interpolant);
        Vector2 stretchScale = new Vector2(particle.velocity.Length() * 0.15f, 0f);

        (Texture2D texture, Rectangle frame) = GetParticleFrame(0);
        SpritebatchDrawer drawer = SpritebatchDrawer.FromTextureAsset(texture, particle.position);
        drawer.sourceRect = frame;
        drawer.CenterOrigin();
        drawer.scale = (particle.startScale + stretchScale) * outScaler;
        drawer.rotation = particle.velocity.ToRotation();
        drawer.color = particle.color * 2.0f;
        drawer.color.A = 0;
        spriteBatch.Draw(drawer);
    }
}
