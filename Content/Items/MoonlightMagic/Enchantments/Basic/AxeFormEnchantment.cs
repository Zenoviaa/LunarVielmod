using Stellamod.Content.Items.MoonlightMagic.Elements;
using Stellamod.Content.Items.MoonlightMagic.Forms;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Items.MoonlightMagic.Enchantments.Basic;

public class AxeFormEnchantment : BaseEnchantment
{
    public override float GetStaffManaModifier()
    {
        return 0.3f;
    }

    public override int GetElementType()
    {
        return ModContent.ItemType<BasicElement>();
    }


    public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
    {

        return true;
    }

    public override void SpecialInventoryDraw(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
    {
        base.SpecialInventoryDraw(item, spriteBatch, position, frame, drawColor, itemColor, origin, scale);
        DrawHelper.DrawGlowInInventory(item, spriteBatch, position, Color.Gray);
    }

    public override void SetMagicDefaults()
    {
        Projectile.knockBack *= 5f;
        MagicProj.Form = FormRegistry.Axe.Value;


    }




}
