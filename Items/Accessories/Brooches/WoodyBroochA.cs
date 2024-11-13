using Stellamod.Brooches;
using Stellamod.Buffs.Charms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using Stellamod.Common.Bases;

namespace Stellamod.Items.Accessories.Brooches
{
    internal class WoodyBroochA : BaseBrooch
    {

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = 24;
            Item.height = 28;
            Item.value = Item.buyPrice(0, 0, 90);
            Item.rare = ItemRarityID.LightPurple;
            Item.buffType = ModContent.BuffType<WoodyB>();
            Item.accessory = true;
            BroochType = BroochType.Advanced;
        }
    }
}
