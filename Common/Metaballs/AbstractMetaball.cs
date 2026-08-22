using Stellamod.Assets;
using Stellamod.Common.Particles;
using Stellamod.Common.Shaders;
using Stellamod.Core.Pixelation;
using Stellamod.Core.Rendering;
using Stellamod.Effects.Generic;
using Stellamod.Effects.RekFlames;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace Stellamod.Common.Metaballs;


public interface IMetaballUpdater<ParticleType>
    where ParticleType : struct
{
    void NewMetaball(ParticleType metaballParticleData);
    void UpdateMetaballs();
}


public struct RekFireMetaballData
{
    public Vector2 position;
    public Vector2 velocity;
    public float radius;
    public float timeLeft;
}


public class RekFireBreathRenderer : ModSystem
{
    private RenderTargetProvider _flamethrowerMask = new RenderTargetProvider(RenderTargetParameters.DefaultScreenTargetCreationFunc);
    public override void Load()
    {
        base.Load();
        On_Main.CheckMonoliths += RenderFlames;
    }

    private void RenderFlames(On_Main.orig_CheckMonoliths orig)
    {
        orig();
        if (Main.gameMenu)
            return;

        if (MetaballContent.RekFireMetaball.RequiresRendering)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            GraphicsDevice graphicsDevice = spriteBatch.GraphicsDevice;
            graphicsDevice.SetRenderTarget(_flamethrowerMask);
            graphicsDevice.Clear(Color.Transparent);

            SpritebatchParams defaultParams = SpritebatchParams.InWorldAndZoomed();
            defaultParams.effect = MetaballContent.RekFireMetaball.PrepareMetaballShader();
            defaultParams.matrix = Matrix.Identity;
            using (new SpritebatchContext(spriteBatch, defaultParams))
            {
                spriteBatch.Draw(_flamethrowerMask, Vector2.Zero, Color.Transparent);
            }

            graphicsDevice.SetRenderTarget(null);
            PixelationManager.QueueSpritebatchDrawAction(RenderPixelatedFlames);
        }
    }

    private void RenderPixelatedFlames(SpriteBatch spriteBatch, Vector2 screenPos)
    {
        var pixelatedFlamesShader = ShaderContent.GetInstance<RekFirebreathShader>();
        pixelatedFlamesShader.MetaballTexture = _flamethrowerMask;
        pixelatedFlamesShader.FlameTexture = AssetManager.Noise.Whirly.Value;
        pixelatedFlamesShader.Time = Main.GlobalTimeWrappedHourly * 18;
        pixelatedFlamesShader.InnerColor = Color.Yellow;
        pixelatedFlamesShader.BloomColor = Color.Red;
        SpritebatchParams flamesParams = SpritebatchParams.InWorldAndZoomed();
        flamesParams.effect = pixelatedFlamesShader;

        using (new SpritebatchContext(spriteBatch, flamesParams))
        {
            spriteBatch.Draw(_flamethrowerMask,  Vector2.Zero, Color.White);
        }
    }
}

public class MetaballContent : ModSystem
{
    private List<IMetaball> _metaballs = new List<IMetaball>();

    public static RekFireMetaball RekFireMetaball;
    public override void OnModLoad()
    {
        base.OnModLoad();
        RekFireMetaball = new();
        _metaballs = new List<IMetaball>()
        {
            RekFireMetaball
        };
    }

    public override void PostUpdateDusts()
    {
        base.PostUpdateDusts();
        foreach (var metaball in _metaballs)
        {
            metaball.UpdateMetaballs();
        }
    }
}

public class RekFireMetaball : AbstractMetaball<RekFireMetaballData>
{

    public override void UpdateMetaballs()
    {
        if (_length <= 0)
            return;

        for (int i = 0; i < metaballs.Length; i++)
        {
            metaballs[i].Z = 0;
        }

        for (int i = 0; i < _length; i++)
        {
            ref RekFireMetaballData particle = ref _particles[i];
            particle.position += particle.velocity;
            particle.timeLeft--;
            if (particle.timeLeft <= 60)
                particle.velocity *= 0.96f;
            metaballs[i] = new Vector3(DrawUtilities.WorldToScreenCoordinates(particle.position), particle.radius);

            for (int j = 0; j < 3; j++)
            {
                if (Main.rand.NextBool(3))
                {
                    Vector2 pos = particle.position;
                    pos += Main.rand.NextVector2Circular(16, 16);
                    Color color = Color.Lerp(Color.Yellow, Color.Red, Main.rand.NextFloat(0f, 1f));
                    Particles.Particles.SwirlingFlameDust.Spawn(BitDustFactory.Default with
                    {
                        position = pos,
                        velocity = -particle.velocity * 0.47f,
                        timeLeft = 45,
                        innerColor = color.ToVector4(),
                        outerColor = Color.Red.ToVector4()
                    });
                }
            }

        }

        for (int i = 0; i < _length; i++)
        {
            ref RekFireMetaballData particle = ref _particles[i];
            if (particle.timeLeft <= 0)
            {
                KillParticle(i);
                i--;
            }
        }
    }
}

public interface IMetaball
{
    void UpdateMetaballs();
}
public abstract class AbstractMetaball<ParticleStructType> : IMetaball
    where ParticleStructType : struct
{
    protected int _length;
    protected ParticleStructType _dummyParticle;
    protected readonly ParticleStructType[] _particles;
    public AbstractMetaball()
    {
        _particles = new ParticleStructType[100];
        metaballs = new Vector3[100];
    }
    public bool RequiresRendering => _length > 0;
    public Vector3[] metaballs;
    public abstract void UpdateMetaballs();
    public ref ParticleStructType Spawn(in ParticleStructType particleData)
    {
        //If too many particles just return a reference to one that's not being used or drawn to the screen
        //That way we don't interrupt anything that's happening
        if (_length >= _particles.Length)
            return ref _dummyParticle;

        int index = _length;
        _length++;
        _particles[index] = particleData;
        return ref _particles[index];
    }

    public void KillParticle(in int index)
    {
        //Swap with the last active particle and set the data to default
        //Order does not matter for when they get updated, so we can do it like this :)
        _particles[index] = _particles[_length - 1];
        _particles[_length - 1] = default;
        _length--;
    }

    public virtual Effect PrepareMetaballShader()
    {
        MetaballShader shader = ShaderContent.GetInstance<MetaballShader>();
        shader.Particles = metaballs;
        return shader.Effect;
    }
}
