using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using ReLogic.Threading;
using Stellamod.Common.DungeonGeneration;
using Stellamod.Core.Pixelation;
using Stellamod.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Core.Foreground;


public class ForegroundParticles
{
    public ForegroundParticles(int maxLength)
    {
        frame = new Rectangle?[maxLength];
        position = new Vector2[maxLength];
        velocity = new Vector2[maxLength];
        active = new bool[maxLength];
        type = new int[maxLength];  
        scale = new float[maxLength];
        timer = new float[maxLength];
        rotation = new float[maxLength];
        parallax = new float[maxLength];
    }

    public Rectangle?[] frame;
    public Vector2[] position;
    public Vector2[] velocity;
    public bool[] active;
    public int[] type;
    public float[] scale;
    public float[] timer;
    public float[] rotation;
    public float[] parallax;

}

/// <summary>
/// Renders particles in the foreground layer, that have very nice parallaxing to them
/// </summary>
public class ForegroundParticleRenderer : ModSystem
{
    private static int _lastIndex;
    private Texture2D[] _particleTextureAssets;
    private ForegroundGore[] _gores;
    private readonly ForegroundParticles _particles = new ForegroundParticles(Max_Particle_Count);
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

        //Now i don't need to initialize multiple instances :)
        //SOA is so cool
        /*
        for (int i = 0; i < _particles.Length; i++)
        {
            _particles[i] = new ForegroundParticle();
        }*/
        On_Main.DrawDust += DrawForegroundGores;
    }

    private void DrawForegroundGores(On_Main.orig_DrawDust orig, Main self)
    {
        orig(self);
        PixelationManager.QueueSpritebatchDrawAction(DrawPixelatedForeground, DrawLayer.OverPlayers);
    }


    public override void PostUpdateDusts()
    {
        base.PostUpdateDusts();
        for (int i = 0; i < Max_Particle_Count; i++)
        {
            UpdateParticle(i);
        }
    }

    /// <summary>
    /// Initializes a new particle
    /// </summary>
    /// <param name="index"></param>
    /// <param name="type"></param>
    private void SpawnParticle(int index, int type)
    {
      //  Main.NewText("Test");
        _particles.active[index] = true;
        _particles.velocity[index] = Vector2.Zero;
        _particles.rotation[index] = 0;
        _particles.timer[index] = 0;
        _particles.parallax[index] = Main.rand.Next(25, 150) * 0.01f;
        _particles.scale[index] = _particles.parallax[index] + 1f;
        _particles.type[index] = type;
    }

    private void UpdateParticle(int index)
    {
        bool active = _particles.active[index];
        if (!active)
            return;

        _particles.position[index] += _particles.velocity[index];
        _particles.timer[index] += 1;
        float xVel = (float)Math.Sin(_particles.timer[index] * 0.036) * 0.48f * _particles.scale[index];
        _particles.velocity[index].X = xVel + (_particles.position[index].Y < Main.worldSurface * 16 ? Main.windSpeedCurrent * 8 : 0);
        _particles.velocity[index].Y = (-Math.Abs(xVel) + _particles.scale[index]) * 0.4f;
        _particles.rotation[index] = _particles.velocity[index].X * -0.5f;

        if (_particles.timer[index] >= 600)
        {
            _particles.active[index] = false;
        }

        //Apply parallax
        Vector2 diff = Main.screenLastPosition - Main.screenPosition;
        _particles.position[index] += diff * _particles.parallax[index];
    }

    private Vector2 GetDrawOrigin(int particle)
    {
        Rectangle? frame = _particles.frame[particle];
        Vector2 drawOrigin = Vector2.Zero;
        if (frame.HasValue)
        {
            drawOrigin = frame.Value.Size() / 2f;
        }
        else
        {
            drawOrigin = _particleTextureAssets[_particles.type[particle]].Size() / 2f;
        }

        return drawOrigin;
    }

    private void DrawPixelatedForeground(SpriteBatch spriteBatch, Vector2 screenPos)
    {
  
        for (int i = 0; i < Max_Particle_Count; i++)
        {
            if (!_particles.active[i])
                continue;
        
            Vector2 drawPosition = _particles.position[i] - Main.screenPosition;
            Vector2 drawOrigin = GetDrawOrigin(i);
            Color lightColour = Lighting.GetColor((int)(drawPosition.X / 16f), (int)(drawPosition.Y / 16f));
            Color frontColour = (_particles.position[i].Y / 16f < Main.worldSurface) ? Main.ColorOfTheSkies : new Color(85, 85, 85);
            Color drawColor = Color.Lerp(lightColour, frontColour, (_particles.parallax[i] - (0.25f)) / 1.25f);

            float inAlpha = EasingFunction.InOutSine(_particles.timer[i] / 30f);
            float outAlpha = 1f - ((_particles.timer[i] - 570f) / 30f);
            float alpha = inAlpha * outAlpha;

            //Main.NewText(alpha);
            drawColor *= alpha;
            Texture2D textureAsset = _particleTextureAssets[_particles.type[i]];
            spriteBatch.Draw(textureAsset, drawPosition, _particles.frame[i], drawColor, _particles.rotation[i], drawOrigin, _particles.scale[i], 
                SpriteEffects.None, 0);
        }
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
 
            if (!renderer._particles.active[index])
            {
                T t = ModContent.GetInstance<T>();
                renderer.SpawnParticle(index, t.type);

                Texture2D texture = renderer._particleTextureAssets[t.type];
                int frameHeight = texture.Height / t.frameCount;
                int frameWidth = texture.Width;
                int frameIndex = Main.rand.Next(0, t.frameCount);
                Rectangle frame = new Rectangle(0, frameIndex * frameHeight, frameWidth, frameHeight);
                renderer._particles.frame[index] = frame;
                renderer._particles.position[index] = position;
                renderer._particles.active[index] = true;
                _lastIndex = index;
                break;
            }
            steps++;
        }
    }
    public static void NewParticle<T>() where T : ForegroundGore
    {

      //  Main.NewText("G");
        //DebugHelper.NewTextOnlyInTesting("E");
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
