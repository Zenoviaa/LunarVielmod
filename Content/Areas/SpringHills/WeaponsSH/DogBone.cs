using Stellamod.Content.Areas.Tundra.Snow.WeaponsSN;
using Stellamod.Core.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.SpringHills.WeaponsSH;

public class DogBone : ModItem
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.DefaultToBellMinion(ModContent.ProjectileType<IceboundMinionProj>());
        Item.damage = 16;
        Item.knockBack = 3f;
    }


}
