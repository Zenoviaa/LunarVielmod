using Terraria;
using Terraria.DataStructures;

namespace Stellamod.Content.Areas.Tundra.Abyss.EnemiesAB.Gores;

public class TheWhispererGore : AbyssFeatherGore
{
    public override void OnSpawn(Gore gore, IEntitySource source)
    {
        gore.numFrames = 3;
        gore.frame = (byte)Main.rand.Next(3);
        gore.timeLeft = 240;
        //UpdateType = 910;
    }
}
