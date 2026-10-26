using Stellamod.Common.Particles;
using Terraria;

namespace Stellamod.Content.Ambient;

public class WoodenCrateShard : GoreParticleUpdater
{
    public override ParticleFrameData FrameData => base.FrameData with
    { 
        FrameCount = 4, 
        Texture = AssetReferences.Assets.Textures.Particle.WoodenCrateFragment.KEY
    };

    public override void OnSpawn(ref Data particle, in int index)
    {
        base.OnSpawn(ref particle, index);
        particle.frame = (byte)Main.rand.Next(4);
    }
}
