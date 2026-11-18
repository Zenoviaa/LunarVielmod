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
        public bool needsSplit;
        public float splitTimer;
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
            particle.splitTimer++;
            if (particle.needsSplit && particle.splitTimer > 24)
            {
                Spawn(new()
                {
                    position = particle.position + Main.rand.NextVector2Circular(32, 32),
                    velocity = particle.velocity.RotatedByRandom(0.4f),
                    radius = particle.radius * 0.5f,
                    timeLeft = 60 + Main.rand.Next(0, 75)
                });
                particle.timeLeft = 0;
                particle.needsSplit = false;
            }
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
    public static List<Vector3> metaballsFront = new();

    public override void PreUpdateProjectiles()
    {
        base.PreUpdateProjectiles();
     
        metaballs.Clear();
        metaballsFront.Clear();
        PS.LavaMetaballs.AddMetaballData(ref metaballs);
        PS.LavaMetaballsFront.AddMetaballData(ref metaballsFront);
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
