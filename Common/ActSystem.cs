using System.IO;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Stellamod.Common;

public class ActSystem : ModSystem
{
    public bool act2;
    public bool act3;
    public override void SaveWorldData(TagCompound tag)
    {
        tag["act2"] = act2;
        tag["act3"] = act3;
    }

    public override void LoadWorldData(TagCompound tag)
    {
        act2 = tag.Get<bool>("act2");
        act3 = tag.Get<bool>("act3");
    }


    public override void NetSend(BinaryWriter writer)
    {
        base.NetSend(writer);
        writer.Write(act2);
        writer.Write(act3);
    }

    public override void NetReceive(BinaryReader reader)
    {
        base.NetReceive(reader);
        act2 = reader.ReadBoolean();
        act3 = reader.ReadBoolean();
    }
}
