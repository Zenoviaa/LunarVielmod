using Stellamod.Core.Pixelation;
using Stellamod.Core.Rendering;
using System;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Common.Particles;

[Autoload(Side = ModSide.Client)]
public class DarkspaceOrbParticles : ModSystem
{
    public static readonly DarkspaceOrbs BackgroundOrbs = new();
    public static readonly DarkspaceOrbs ForegroundOrbs = new();
    public override void Load()
    {
        base.Load();
        On_Main.CheckMonoliths += ScheduleDraws;
        ForegroundOrbs.Load(Mod);
        BackgroundOrbs.Load(Mod);
    }
    public override void Unload()
    {
        base.Unload();
        ForegroundOrbs.Unload();
        BackgroundOrbs.Unload();
    }
    public override void PostUpdateDusts()
    {
        base.PostUpdateDusts();
        BackgroundOrbs.Update();
        ForegroundOrbs.Update();
    }

    private void ScheduleDraws(On_Main.orig_CheckMonoliths orig)
    {
        orig();
        if (Main.gameMenu)
            return;
        BackgroundOrbs.scale = 1.1f;
        ForegroundOrbs.scale = 0.5F;
        if (BackgroundOrbs.Length > 0)
            PixelationManager.QueueSpritebatchDrawAction(BackgroundOrbs.Draw, DrawLayer.BehindTiles);
        if (ForegroundOrbs.Length > 0)
        {
            //PixelationManager.QueueSpritebatchDrawAction(ForegroundOrbs.Draw, DrawLayer.OverWater);
            DrawLayerHooks.OverWaterDrawActions2.Enqueue((SpriteBatch sb) => ForegroundOrbs.Draw(sb, Main.screenPosition));
        }
       
         
    }
}

public class DarkspaceOrbs : ParticleUpdater<DarkspaceOrbs.Data>
{
    public struct Data : IParticleData
    {
        public Color color;
        public Vector2 position;
        public float parallax;
        public float timer;
        public float timeLeft;
        public bool IsActive => timeLeft > 0;
    }
    public float scale;
    public override ParticleFrameData FrameData => base.FrameData with { FrameCount = 1, Texture = AssetReferences.Assets.Textures.Particle.DarkspaceOrb.KEY };
    protected override void UpdateParticles()
    {
        base.UpdateParticles();
        var diff = Main.screenPosition - Main.screenLastPosition;
        for (var i = 0; i < _length; i++)
        {
            ref var particle = ref _particles[i];
            var xSin = MathF.Sin((float)particle.timeLeft * 0.02f + particle.position.X * 0.001f);
            var velocity = new Vector2(xSin + 0.3f, -0.5f);
            particle.position += velocity;
            particle.position += diff * particle.parallax;
         //   particle.position = new Vector2(particle.position.X % Main.screenWidth, particle.position.Y % Main.screenHeight);
            particle.timer++;
            particle.timeLeft--;
        }
    }
    public override int GetPoolSize()
    {
        return 256;
    }
    public override void Draw(SpriteBatch spriteBatch, Vector2 screenPos)
    {

        (Texture2D texture, Rectangle frame) = GetParticleFrame(0);
        var drawer = SpritebatchDrawer.FromTextureWhite(texture);
        drawer.CenterOrigin();
        for (var i = 0; i < _length; i++)
        {
            ref var particle = ref _particles[i];
            drawer.worldPosition = particle.position;

            var outScale = EasingFunction.InOutSine(particle.timeLeft / 60F);
            var inScale = EasingFunction.InOutSine(particle.timer / 60F);
            drawer.color = particle.color * inScale * outScale;
            drawer.scale = new Vector2(particle.parallax * scale);
            spriteBatch.Draw(drawer);
        }            //base.Draw(spriteBatch, screenPos);


        //base.Draw(spriteBatch, screenPos);
    }
}