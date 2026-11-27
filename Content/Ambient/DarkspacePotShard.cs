using Stellamod.Common.Particles;

namespace Stellamod.Content.Ambient;

public class DarkspacePotShard : GoreParticleUpdater
{
    public override ParticleFrameData FrameData => base.FrameData with
    {
        FrameCount = 4,
        Texture = AssetReferences.Assets.Textures.Particle.DarkspacePotFragment.KEY
    };

    public override void OnSpawn(ref Data particle, in int index)
    {
        base.OnSpawn(ref particle, index);

    }
}
