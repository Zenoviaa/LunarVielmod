using Stellamod.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stellamod.Content.Areas.RoyalCapital.BossesRC.RoyalFox
{
    public partial class RoyalFox : IUseMultihitbox
    {
        public int NeededHitboxes => 1;

        public void AddExtraHitboxes(ref Rectangle[] hitboxes, in int start, in int end)
        {
            hitboxes[start] = CorrectedHitbox;
        }
    }
}
