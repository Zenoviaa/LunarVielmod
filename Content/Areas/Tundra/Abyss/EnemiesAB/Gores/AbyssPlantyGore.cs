using Terraria;
using Terraria.DataStructures;

namespace Stellamod.Content.Areas.Tundra.Abyss.EnemiesAB.Gores;

public class AbyssPlantyGore : AbyssFeatherGore
{
    public override void OnSpawn(Gore gore, IEntitySource source)
    {
        gore.numFrames = 4;
        gore.frame = (byte)Main.rand.Next(4);
        gore.timeLeft = 240;
        //UpdateType = 910;
    }
}
