using Stellamod.Core.Utilities;

namespace Stellamod.Content.Areas.PunkerTown.BossesPT.Gothivia;

public partial class Gothivia
{
    public void CreateInCircle()
    {
        PixelPrimitiveCircleFactory.CreateGenericInBoom(NPC.Center, Color.White, Color.White, 45, 444);
    }
}
