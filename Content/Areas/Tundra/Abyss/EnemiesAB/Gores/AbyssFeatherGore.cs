using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Tundra.Abyss.EnemiesAB.Gores;

public class AbyssFeatherGore : ModGore
{
    public override void OnSpawn(Gore gore, IEntitySource source)
    {
        gore.numFrames = 8;
        gore.frame = (byte)Main.rand.Next(8);
        gore.timeLeft = 240;
        //UpdateType = 910;
    }
    public override bool Update(Gore gore)
    {
        gore.velocity.Y = MathHelper.Lerp(gore.velocity.Y, 0.5f, 0.03f);
        gore.velocity.X += MathF.Sin(Main.GameUpdateCount * 0.03f) * 0.03f;
        gore.timeLeft--;
        if (gore.timeLeft <= 0)
            gore.active = false;
        return false;
    }
}
