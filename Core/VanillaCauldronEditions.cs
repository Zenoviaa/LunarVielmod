using Stellamod.Content.Items.Materials;
using Stellamod.Items;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Core;

public class VanillaCauldronEditions : ModSystem
{
    public override void PostAddRecipes()
    {
        base.PostAddRecipes();
        Cauldron.SetMaterial(ModContent.ItemType<Mushroom>());
        Cauldron.VanillaBrew(result: ItemID.Aglet);
        Cauldron.VanillaBrew(result: ItemID.JellyfishNecklace);
    
    
        
    }
}
