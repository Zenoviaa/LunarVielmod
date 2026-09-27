using Stellamod.Common.Particles;
using Stellamod.Core.Camera;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace Stellamod.Content.Areas.Jungle.ZigguratraBoss;

public partial class Zigguratra
{
    Color LightGoldenColor => Color.Lerp(Color.Gold, Color.Black, 0.5f);
    Color DarkGoldenColor => Color.Lerp(Color.DarkGoldenrod, Color.Black, 0.5f);
    
    void GruntSound()
    {

    }

    void FocusOnMe()
    {
        CameraTargetSystem.AddTarget(Vector2.Lerp(Main.LocalPlayer.Center, NPC.Center, 0.5f));
    }

    void SwirlParticlesAround(Vector2 centerPosition)
    {
        if(Timer % 3 == 0)
        {
            var offsetDistance = 80;
            var offset = Main.rand.NextVector2CircularEdge(offsetDistance, offsetDistance);
            var spawnPos = centerPosition + offset;
            var startVelocity = (spawnPos - centerPosition).SafeNormalize(Vector2.Zero) * 6;
            startVelocity = startVelocity.RotatedBy(MathHelper.PiOver2);
            Particles.SwirlingFlameDust.Spawn(BitDustFactory.SlowingOverTime with
            {
                position = spawnPos,
                innerColor = LightGoldenColor.ToVector4(),
                outerColor = DarkGoldenColor.ToVector4(),
                velocity  = startVelocity,
                scale = new Vector2(Main.rand.NextFloat(0.4f, 1f)),
                timeLeft = 120
            });
        }
    }
}
