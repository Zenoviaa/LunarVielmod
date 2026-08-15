using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Core.Pixelation;
using Stellamod.Core.Rendering;
using Stellamod.Core.Utilities;
using Stellamod.Helpers;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Effects.ScreenRipples;


public abstract class ParticleManager
{
    public struct ParticleArrays
    {
        public ParticleArrays(int maxParticleCount)
        {

            position = new Vector2[maxParticleCount];
            velocity = new Vector2[maxParticleCount];
            scale = new Vector2[maxParticleCount];
            timeLeft = new float[maxParticleCount];
            Length = maxParticleCount;
        }

        public Vector2[] position;
        public Vector2[] velocity;
        public Vector2[] scale;
        public float[] timeLeft;
        public readonly float Length;
    }

    public readonly ParticleArrays Particles;
    public ParticleManager(int maxParticleCount)
    {
        Particles = new ParticleArrays(maxParticleCount);
    }

    public void Update()
    {
        for (int i = 0; i < Particles.Length; i++)
        {
            ref float timeleft = ref Particles.timeLeft[i];
            if (timeleft <= 0)
                continue;

            timeleft--;
            UpdateParticle(in i);
        }
    }

    public abstract void UpdateParticle(in int i);

    public void SpawnParticle(Vector2 position, Vector2 velocity, Vector2 scale, float timeLeft)
    {
        int freeIndex = -1;
        for (int i = 0; i < Particles.Length; i++)
        {
            ref float t = ref Particles.timeLeft[i];
            if (t <= 0)
            {
                freeIndex = i;
                break;
            }
            //velocity *= 0.98f;
        }

        if (freeIndex == -1)
            return;

        Particles.scale[freeIndex] = scale;
        Particles.timeLeft[freeIndex] = timeLeft;
        Particles.position[freeIndex] = position;
        Particles.velocity[freeIndex] = velocity;
    }
}

public sealed class RippleParticleManager : ParticleManager
{
    public RippleParticleManager() : base(maxParticleCount: 100) { }
    public override void UpdateParticle(in int i)
    {
        ref float timeLeft = ref Particles.timeLeft[i];
        ref Vector2 position = ref Particles.position[i];
        ref Vector2 velocity = ref Particles.velocity[i];
        position += velocity;

        ref Vector2 scale = ref Particles.scale[i];
        scale = Vector2.Lerp(Vector2.One * 2, Vector2.Zero, EasingFunction.InExpo(timeLeft / 50f));
    }
}

[Autoload(Side = ModSide.Client)]
public sealed class RippleRenderer : ModSystem
{
    private RippleParticleManager _particleManager;
    private RenderTargetProvider _rippleRT = new RenderTargetProvider(RenderTargetParameters.DefaultScreenTargetCreationFunc);
    public override void Load()
    {
        base.Load();
        _particleManager = new RippleParticleManager();
        PrepareRenderTargetDrawsSystem.OnRenderTargetDrawsReady += RenderRipples;
    }
    public override void Unload()
    {
        base.Unload();
        PrepareRenderTargetDrawsSystem.OnRenderTargetDrawsReady -= RenderRipples;
    }

    public void CreateRipple(Vector2 position)
    {
        _particleManager.SpawnParticle(position, Vector2.Zero, Vector2.One, 50);
    }

    public override void PostUpdateDusts()
    {
        base.PostUpdateDusts();
       // DebugSpawnRippler();
        _particleManager.Update();
    }

    private void DrawToRippleTexture()
    {
        GraphicsDevice gDevice = Main.graphics.GraphicsDevice;
        SpriteBatch sb = Main.spriteBatch;
        gDevice.SetRenderTarget(_rippleRT);
        gDevice.Clear(Color.Transparent);

        SpriteBatch spriteBatch = Main.spriteBatch;
        RippleWriteShader writeShader = ShaderContent.GetInstance<RippleWriteShader>();
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, writeShader.Effect);
        SpritebatchDrawer rippleDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SimpleGlowCircle, Vector2.Zero);
        for (int i = 0; i < _particleManager.Particles.Length; i++)
        {
            ref float timeLeft = ref _particleManager.Particles.timeLeft[i];
            if (timeLeft <= 0)
                continue;

            ref Vector2 position = ref _particleManager.Particles.position[i];
            ref Vector2 scale = ref _particleManager.Particles.scale[i];
            rippleDrawer.worldPosition = position;
            rippleDrawer.scale = scale;
            rippleDrawer.color = Color.Lerp(Color.Transparent, Color.White, timeLeft / 100f);
            spriteBatch.Draw(rippleDrawer);
        }

        spriteBatch.End();
        gDevice.SetRenderTarget(null);

        Rippler s = ScreenShader.GetInstance<Rippler>();
        s.rippleTexture = _rippleRT;
        s.alpha = 1;
    }

    private void DrawToRippleArray()
    {
        bool anyRipples = false;
        List<Vector4> ripples = new List<Vector4>();
        for (int i = 0; i < _particleManager.Particles.Length; i++)
        {
            ref float timeLeft = ref _particleManager.Particles.timeLeft[i];
            if (timeLeft <= 0)
                continue;

            anyRipples = true;
            Vector2 worldPos = _particleManager.Particles.position[i];
            Vector2 screenPos = (worldPos - Main.screenPosition) / new Vector2(Main.screenWidth, Main.screenHeight);
            float strength = MathHelper.Lerp(0f, 1f, timeLeft / 50f);
            Vector4 ripple = new Vector4(screenPos, _particleManager.Particles.scale[i].X * 0.2f, strength * 252);
            ripples.Add(ripple);
        }

        if (!anyRipples)
            return;

        Rippler s = ScreenShader.GetInstance<Rippler>();
        s.PrepareShader(ripples);
        s.alpha = 1;
    }
    private void RenderRipples()
    {
        DrawToRippleArray();
    }

    private void DebugSpawnRippler()
    {
        if(Main.mouseLeft && Main.mouseLeftRelease)
        {
            _particleManager.SpawnParticle(Main.MouseWorld, Vector2.Zero, Vector2.One * 0, 90);
        }
    }
    private void DebugDrawToScreen(SpriteBatch sb, Vector2 screenPos)
    {
       // sb.Draw(_rippleRT, Vector2.Zero, Color.White);
    }

}

public class RippleWriteShader : CrystalShader<RippleWriteShader>
{

}