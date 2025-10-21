using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Core.SummonerSystem
{
    public abstract class BaseBellMinionItem : ModItem
    {
        public sealed override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 15;
            Item.knockBack = 3f;
            Item.mana = 10;
            Item.width = 32;
            Item.height = 32;
            Item.useTime = 36;
            Item.useAnimation = 36;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.rare = ItemRarityID.Blue;

            // These below are needed for a minion weapon
            Item.noMelee = true;
            Item.DamageType = DamageClass.Summon;
            SetDefaults2();
        }
        public virtual void SetDefaults2()
        {

        }
    }
}
