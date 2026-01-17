using Stellamod.Common;

namespace Stellamod.Content.Items.MoonlightMagic.Enchantments
{
    public class SuperEnchantment : BaseEnchantment
    {
        bool hasGoneCrazy;
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            isSpecial = true;
        }

        public override void AI()
        {
            base.AI();
            if (hasGoneCrazy)
                return;
            var enchantmentsToSpawn = ItemHelper.Enchantments;
            foreach (var enchantment in enchantmentsToSpawn)
            {
                if (enchantment.Type == Type)
                    continue;

                MagicProj.AddEnchantment(enchantment);
            }
            hasGoneCrazy = true;
        }
    }
}
