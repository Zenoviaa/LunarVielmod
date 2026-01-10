using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.SpringHills.VanitiesSH
{
    [AutoloadEquip(EquipType.Head)]
    public class GnomeHat : ModItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.vanity = true;
        }
    }
    [AutoloadEquip(EquipType.Body)]
    public class GnomeCoat : ModItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.vanity = true;
        }
    }
    [AutoloadEquip(EquipType.Legs)]
    public class GnomeLegs : ModItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.vanity = true;
        }
    }
}
