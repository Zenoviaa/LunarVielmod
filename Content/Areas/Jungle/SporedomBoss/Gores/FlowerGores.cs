using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Jungle.SporedomBoss.Gores;

public class GreenFallenLeaf : ModGore
{
    public override void OnSpawn(Gore gore, IEntitySource source)
    {
        gore.numFrames = 4;
        gore.frame = (byte)Main.rand.Next(4);
        gore.timeLeft = 240;
    }

    public override bool Update(Gore gore)
    {
        GoreAI.FeatherAI(gore);
        return false;
    }
}

public class WhiteFallenPetal : ModGore
{
    public override void OnSpawn(Gore gore, IEntitySource source)
    {
        gore.numFrames = 4;
        gore.frame = (byte)Main.rand.Next(4);
        gore.timeLeft = 240;
    }

    public override bool Update(Gore gore)
    {
        GoreAI.FeatherAI(gore);
        return false;
    }
}