using Stellamod.Content.Items.MoonlightMagic.Elements;
using Stellamod.Helpers;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Items.MoonlightMagic.Enchantments.Uvilis;

public class TyphoonWaverEnchantment : BaseEnchantment
{
    private Vector2 _velocity;
    public override void AI()
    {
        base.AI();
        //Count up
        Countertimer++;
        if (Countertimer == 1)
        {
            _velocity = Projectile.velocity;
        }

        Vector2 newVelocity = Projectile.velocity.RotatedBy(MathHelper.ToRadians(25));
        Projectile.velocity = newVelocity;
        Projectile.Center += _velocity * 0.3f;
    }


    public override float GetStaffManaModifier()
    {
        return 0.3f;
    }

    public override int GetElementType()
    {
        return ModContent.ItemType<UvilisElement>();
    }

    public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
    {
        return true;
    }

    public override void SpecialInventoryDraw(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
    {
        base.SpecialInventoryDraw(item, spriteBatch, position, frame, drawColor, itemColor, origin, scale);
        DrawHelper.DrawGlowInInventory(item, spriteBatch, position, ColorFunctions.PhantasmalGreen);
    }
}
