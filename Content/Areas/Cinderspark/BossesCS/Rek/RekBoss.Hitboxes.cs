using Stellamod.Common;

namespace Stellamod.Content.Areas.Cinderspark.BossesCS.Rek;

public partial class RekBoss : IUseMultihitbox
{
    public int NeededHitboxes => Segments.Length;
    public void AddExtraHitboxes(ref Rectangle[] hitboxes, in int start, in int end)
    {
        for (var i = start; i < end; i++)
        {
            ref var hitbox = ref hitboxes[i];
            var localI = i - start;
            hitbox = DrawUtilities.CenterRectangle(Segments[localI].position, 100, 100);
        }
        //throw new NotImplementedException();
    }
}
