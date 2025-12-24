using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using ReLogic.Threading;
using Stellamod.Helpers;
using System;
using System.Linq;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Core.Foreground;

/// <summary>
/// Represents a single particle that exists in the foreground layer and has parallaxing, to be used with the particle renderer
/// </summary>
public class ForegroundParticle
{
    public Rectangle? frame;
    public Vector2 position;
    public Vector2 velocity;
    public bool active;
    public int type;
    public float scale;
    public float timer;
    public float rotation;
    public float parallax;
}

/// <summary>
/// Renders particles in the foreground layer, that have very nice parallaxing to them
/// </summary>
public class ForegroundParticleRenderer : ModSystem
{
    private static int _lastIndex;
    private Texture2D[] _particleTextureAssets;
    private ForegroundGore[] _gores;
    private readonly ForegroundParticle[] _particles = new ForegroundParticle[Max_Particle_Count];
    public const int Max_Particle_Count = 400;
    public override void OnModLoad()
    {
        base.OnModLoad();
        _gores = ModContent.GetContent<ForegroundGore>().ToArray();
        _particleTextureAssets = new Texture2D[_gores.Length];
        for (int i = 0; i < _gores.Length; i++)
        {
            ForegroundGore gore = _gores[i];
            gore.type = i;
            _particleTextureAssets[gore.type] = ModContent.Request<Texture2D>(gore.Texture, AssetRequestMode.ImmediateLoad).Value;
        }
        for (int i = 0; i < _particles.Length; i++)
        {
            _particles[i] = new ForegroundParticle();
        }
        On_Main.DrawDust += DrawForegroundGores;
    }

    private void DrawForegroundGores(On_Main.orig_DrawDust orig, Main self)
    {
        orig(self);
        DrawForeground();
    }

    public override void OnModUnload()
    {
        base.OnModUnload();
        On_Main.DrawDust -= DrawForegroundGores;
    }

    public override void PostUpdateDusts()
    {
        base.PostUpdateDusts();
        FastParallel.For(0, Max_Particle_Count, delegate (int start, int end, object context)
        {
            for (int i = start; i < end; i++)
            {
                UpdateParticle(i);
            }
        });
    }

    /// <summary>
    /// Initializes a new particle
    /// </summary>
    /// <param name="index"></param>
    /// <param name="type"></param>
    private void SpawnParticle(int index, int type)
    {
        ref ForegroundParticle particle = ref _particles[index];
        particle.active = true;
        particle.velocity = Vector2.Zero;
        particle.rotation = 0;
        particle.timer = 0;
        particle.parallax = Main.rand.Next(25, 150) * 0.01f;
        particle.scale = particle.parallax + 1f;
        particle.type = type;
    }

    private void UpdateParticle(int index)
    {
        ref ForegroundParticle particle = ref _particles[index];
        if (!particle.active)
            return;

        particle.position += particle.velocity;

        float xVel = (float)Math.Sin(particle.timer++ * 0.036) * 0.48f * particle.scale;
        particle.velocity.X = xVel + (particle.position.Y < Main.worldSurface * 16 ? Main.windSpeedCurrent * 8 : 0);
        particle.velocity.Y = (-Math.Abs(xVel) + particle.scale) * 0.4f;
        particle.rotation = particle.velocity.X * -0.5f;

        if (particle.timer >= 600)
        {
            particle.active = false;
        }

        //Apply parallax
        Vector2 diff = Main.screenLastPosition - Main.screenPosition;
        particle.position += diff * particle.parallax;
    }

    private Vector2 GetDrawOrigin(ForegroundParticle particle)
    {
        Rectangle? frame = particle.frame;
        Vector2 drawOrigin = Vector2.Zero;
        if (frame.HasValue)
        {
            drawOrigin = frame.Value.Size() / 2f;
        }
        else
        {
            drawOrigin = _particleTextureAssets[particle.type].Size() / 2f;
        }

        return drawOrigin;
    }

    private void DrawForeground()
    {
        SpriteBatch spriteBatch = Main.spriteBatch;
        spriteBatch.Begin(SpriteSortMode.Texture, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);
        for (int i = 0; i < Max_Particle_Count; i++)
        {
            ForegroundParticle particle = _particles[i];
            if (!particle.active)
                continue;

            Vector2 drawPosition = particle.position - Main.screenPosition;
            Vector2 drawOrigin = GetDrawOrigin(particle);
            Color lightColour = Lighting.GetColor((int)(drawPosition.X / 16f), (int)(drawPosition.Y / 16f));
            Color frontColour = (particle.position.Y / 16f < Main.worldSurface) ? Main.ColorOfTheSkies : new Color(85, 85, 85);
            Color drawColor = Color.Lerp(lightColour, frontColour, (particle.parallax - (0.25f)) / 1.25f);

            float inAlpha = EasingFunction.InOutSine(particle.timer / 30f);
            float outAlpha = 1f - ((particle.timer - 570f) / 30f);
            float alpha = inAlpha * outAlpha;
            drawColor *= alpha;
            Texture2D textureAsset = _particleTextureAssets[particle.type];
            spriteBatch.Draw(textureAsset, drawPosition, particle.frame, drawColor, particle.rotation, drawOrigin, particle.scale, SpriteEffects.None, 0);
        }
        spriteBatch.End();
    }

    public static void NewParticle<T>(Vector2 position) where T : ForegroundGore
    {
        //Clutters the screen tbh
        DomainExpansionManager domainExpansionManager = ModContent.GetInstance<DomainExpansionManager>();
        if (domainExpansionManager.inSpace)
            return;

        ForegroundParticleRenderer renderer = ModContent.GetInstance<ForegroundParticleRenderer>();
        int steps = 0;

        //Starting from the last search index and looping around for a small performance boost
        int index = _lastIndex;
        int maxSteps = Max_Particle_Count;
        while (steps < maxSteps)
        {
            index++;
            index = index % Max_Particle_Count;
            var gore = renderer._particles[index];
            if (!gore.active)
            {
                T t = ModContent.GetInstance<T>();
                renderer.SpawnParticle(index, t.type);

                Texture2D texture = renderer._particleTextureAssets[t.type];
                int frameHeight = texture.Height / t.frameCount;
                int frameWidth = texture.Width;
                int frameIndex = Main.rand.Next(0, t.frameCount);
                Rectangle frame = new Rectangle(0, frameIndex * frameHeight, frameWidth, frameHeight);
                gore.frame = frame;
                gore.position = position;
                gore.active = true;
                _lastIndex = index;
                break;
            }
            steps++;
        }
    }
    public static void NewParticle<T>() where T : ForegroundGore
    {
        float xPosition = Main.rand.Next(-(int)(Main.screenWidth * 0.52f), (int)(Main.screenWidth * 0.52f));
        if (xPosition < 0)
            xPosition -= Main.screenWidth / 2f;
        else
            xPosition += Main.screenWidth / 2f;
            float yPosition = Main.rand.NextFloat(-Main.screenHeight * 0.52f, 0);
        Vector2 pos = Main.LocalPlayer.Center + new Vector2(xPosition, yPosition);
        NewParticle<T>(pos);
    }
}
