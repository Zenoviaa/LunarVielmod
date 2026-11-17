using Stellamod.Common.Particles;
using Stellamod.Common.Shaders;
using Stellamod.Core.Pixelation;
using Stellamod.Core.Rendering.RTs;
using Stellamod.Effects.Generic;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Common.Metaballs;

public class MetaballParticles : ParticleUpdater<MetaballParticles.Data>
{
    public struct Data : IParticleData
    {
        public Vector2 position;
        public Vector2 velocity;
        public float radius;
        public float timeLeft;
        public bool IsActive => timeLeft > 0;
    }

    public override ParticleFrameData FrameData => base.FrameData with { Texture = ModType.NoTexture };

    public void AddMetaballData(ref List<Vector3> metaballs)
    {
        for (var i = 0; i < _length; i++)
        {
            ref var particle = ref _particles[i];
            var ratio = EasingFunction.InOutSine((float)particle.timeLeft / 60F);
            metaballs.Add(new Vector3(DrawUtilities.WorldToScreenCoordinates(particle.position), particle.radius * ratio));
        }
    }

    public override int GetPoolSize()
    {
        return 384;
    }


    protected override void UpdateParticles()
    {
        base.UpdateParticles();

        for (var i = 0; i < _length; i++)
        {
            ref var particle = ref _particles[i];
            particle.position += particle.velocity;
            particle.velocity.Y += 0.25f;
            particle.timeLeft--;
        }
    }
    public override void Draw(SpriteBatch spriteBatch, Vector2 screenPos)
    {

    }

    public override void Draw(SpriteBatch spriteBatch, ref Data particle)
    {

    }
}


[Autoload(Side = ModSide.Client)]
public class LavaMetaballRenderer : ModSystem
{
    public static List<Vector3> metaballs = new();
    public override void Load()
    {
        base.Load();
        On_Main.CheckMonoliths += PrepareDraws;
    }

    private void PrepareDraws(On_Main.orig_CheckMonoliths orig)
    {
        orig();
        if (Main.gameMenu)
            return;

        PixelationManager.QueueSpritebatchDrawAction(TestDraw);
     //   throw new NotImplementedException();
    }

    public override void PreUpdateProjectiles()
    {
        base.PreUpdateProjectiles();
        /*
        PS.LavaMetaballs.Spawn(new()
        {
            position = Main.LocalPlayer.Center + Main.rand.NextVector2Circular(32, 32),
            velocity = Main.rand.NextVector2Circular(3, 3),
            radius = 0.015f,
            timeLeft = 120
        });*/
        metaballs.Clear();
        PS.LavaMetaballs.AddMetaballData(ref metaballs);
    }
   


    void TestDraw(SpriteBatch sb, Vector2 sp)
    {
        /*
        sb.EndOut(out var oldParameters);

        using var metaballTarget = RT.Context(RenderTargets.ScreenTarget);
        MetaballRenderer.PrepareMetaballContent(metaballTarget, metaballs);

        sb.Begin(oldParameters);
        sb.Draw(metaballTarget, Vector2.Zero, Color.White);
        */
    }
}

public static class MetaballRenderer
{
    static readonly Vector3[] _buffer = new Vector3[100];
    public static void PrepareMetaballContent(RenderTargetHandle target, IList<Vector3> metaballs)
    {
        //Separate out into batches
        int count = 0;
        void FillBuffer(IList<Vector3> metaballs, ref int metaballIndex)
        {
            while (metaballIndex < metaballs.Count && count < _buffer.Length)
            {
                _buffer[count++] = metaballs[metaballIndex++];
            } 
            while (count < _buffer.Length)
            {
                _buffer[count++] = Vector3.Zero;
            }
        }

        void FlushBuffer()
        {
            if (count <= 0)
                return;

            var metaballShader = ShaderContent.GetInstance<MetaballSimpleShader>();
            metaballShader.Particles = _buffer;
            var sb = Main.spriteBatch;
            SpritebatchParams defaultParams = SpritebatchParams.InWorldAndZoomed();
            defaultParams.effect = metaballShader;
            defaultParams.matrix = Matrix.Identity;
            using (new SpritebatchContext(sb, defaultParams))
            {
                sb.DrawScreenRectangle();
                //    sb.Draw(flameThrowerMask, Vector2.Zero, Color.Transparent);
            }

            count = 0;
        }

        using (RT.Clear(target, Color.Transparent))
        {
            var mIndex = 0;
            while (mIndex < metaballs.Count)
            {
                FillBuffer(metaballs, ref mIndex);
                FlushBuffer();
            }
            FlushBuffer();
        }
    }
}
