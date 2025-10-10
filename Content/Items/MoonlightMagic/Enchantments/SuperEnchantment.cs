namespace Stellamod.Content.Items.MoonlightMagic.Enchantments
{
    public class SuperEnchantment : BaseEnchantment
    {
        bool hasGoneCrazy;
        public override void AI()
        {
            base.AI();
            if (hasGoneCrazy)
                return;
            var enchantmentsToSpawn = AllEnchantments;
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
