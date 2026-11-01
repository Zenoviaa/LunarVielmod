using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace Stellamod.Content.Areas.Illuria.BossesIL.Niivi;

public partial class Niivi
{
    /// <summary>
    /// Makes the ik target just follow the head until it's within range
    /// </summary>
    void LimpIKTarget()
    {
        var dist = Vector2.Distance(_ikTarget, NPC.Center);
        var rigDist = FabrikSolver.RigLength(Segments);
        var iterations = 0;
        while (dist > rigDist)
        {

            //JUST IN CASE
            //Not sure how this would happen tho
            iterations++;
            if (iterations >= 128)
                break;
            var dir = (NPC.Center - _ikTarget).SafeNormalize(Vector2.Zero);
            _ikTarget += dir;
            dist = Vector2.Distance(_ikTarget, NPC.Center);
        }
    }

    //Ok
    //so the way we're gonna do this boss is by making TONS OF HECKING CURVES
    //Every motion will be preset just with calcualted points
    //all the ik points will be calculated too except for a few
    //guh
    Vector2 ArcRightFallDownLeft(float t, Vector2 origin)
    {
        var width = 474;
        var height = 1222;
        var leftToRight = Vector2.Lerp(origin, origin + new Vector2(width, 0), t / 0.4f);
        var bottomToTop = Vector2.Lerp(origin + new Vector2(width, 0), origin + new Vector2(0, -height), t / 0.4f);
        var topToBottomLeft = Vector2.Lerp(origin + new Vector2(0, -height), origin + new Vector2(-512, 2000), t / 1f);

        var lerp1 = Vector2.Lerp(leftToRight, bottomToTop, t / 0.4f);
        var lerp2 = Vector2.Lerp(bottomToTop, topToBottomLeft, t);
        var lerp3 = Vector2.Lerp(lerp1, lerp2, t);
        return lerp3;
    }
}
