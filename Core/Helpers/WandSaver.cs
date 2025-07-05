using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Core.Helpers
{
    public class WandSaver : ModItem
    {
        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.MagicConch);
            Item.useTime = Item.useAnimation = 15;
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }
        public override bool? UseItem(Player player)
        {
            StructureSelection selection = ModContent.GetInstance<StructureSelection>();
            selection.SpawnSelection = true;
            SoundEngine.PlaySound(SoundID.Item47);
            return true;
        }
    }
}

