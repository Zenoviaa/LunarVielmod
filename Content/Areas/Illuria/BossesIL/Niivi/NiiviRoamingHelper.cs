using Stellamod.WorldG;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Illuria.BossesIL.Niivi;

public class NiiviRoamingHelper : ModSystem
{
    public static Vector2 LastKnownNiiviLocation
    {
        get
        {
            //TODO: Default to the illuria tree location
            //oh wait
            if (field == Vector2.Zero)
                field = new Vector2(3000);

            return field;
        }
        set
        {
            field = value;
        }
    }
    public override void NetSend(BinaryWriter writer)
    {
        base.NetSend(writer);
        writer.WriteVector2(LastKnownNiiviLocation);
    }
    public override void NetReceive(BinaryReader reader)
    {
        base.NetReceive(reader);
        LastKnownNiiviLocation = reader.ReadVector2();
    }

}
