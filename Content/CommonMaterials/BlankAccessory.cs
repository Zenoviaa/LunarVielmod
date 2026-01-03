using Stellamod.Common;
using Stellamod.Core.Bases;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.CommonMaterials
{
    public abstract class SirestiasMold : ModItem
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ItemSets.IsSoldBySirestias[Type] = true;
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.DefaultToMold();
            Item.maxStack = Item.CommonMaxStack;
            Item.rare = ItemRarityID.Green;
        }
    }
    public class BlankAccessory : SirestiasMold
    {

    }
    public class BlankBag : SirestiasMold
    {

    }
    public class BlankBow : SirestiasMold
    {

    }
    public class BlankBrooch : SirestiasMold
    {

    }
    public class BlankCard : SirestiasMold
    {

    }
    public class BlankGun : SirestiasMold
    {

    }
    public class BlankJuggler : SirestiasMold
    {

    }
    public class BlankOrb : SirestiasMold
    {

    }
    public class BlankRune : SirestiasMold
    {

    }
    public class BlankSafunai : SirestiasMold
    {

    }
    public class BlankStaff : SirestiasMold
    {

    }
    public class BlankStein : SirestiasMold
    {

    }
    public class BlankSword : SirestiasMold
    {

    }
}
