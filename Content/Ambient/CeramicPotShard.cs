using Stellamod.Common.Particles;

namespace Stellamod.Content.Ambient;

public class CeramicPotShard : GoreParticleUpdater
{
    public override ParticleFrameData FrameData => base.FrameData with
    {
        FrameCount = 12,
        Texture = AssetReferences.Assets.Textures.Particle.CeramicPotFragment.KEY
    };

    public override void OnSpawn(ref Data particle, in int index)
    {
        base.OnSpawn(ref particle, index);

    }
}
