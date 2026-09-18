using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Buffs;

public class Stunlocked : ModBuff
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Main.debuff[Type] = true;
    }
    
    public override void Update(Player player, ref int buffIndex)
    {
        base.Update(player, ref buffIndex);
    }
}
