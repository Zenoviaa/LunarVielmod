using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using Stellamod.Items.MoonlightMagic.Elements;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Items.MoonlightMagic.Enchantments.Nature
{
    internal class WorldToucherEnchantment : BaseEnchantment
    {
        bool HitOnce = false;
        int Attagain = 14;
        public override float GetStaffManaModifier()
        {
            return 0.5f;
        }

        public override int GetElementType()
        {
            return ModContent.ItemType<NaturalElement>();
        }


        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {

            return true;
        }

        public override void SpecialInventoryDraw(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            base.SpecialInventoryDraw(item, spriteBatch, position, frame, drawColor, itemColor, origin, scale);
            DrawHelper.DrawGlowInInventory(item, spriteBatch, position, ColorFunctions.NaturalGreen);
        }

        public override void SetMagicDefaults()
        {

            Projectile.penetrate += 1;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {

            Projectile.penetrate--;
            if (Projectile.penetrate <= 0)
            {
                return true;
            }
            else
            {
                if (Projectile.velocity.X != oldVelocity.X)
                    Projectile.velocity.X = -oldVelocity.X;

                if (Projectile.velocity.Y != oldVelocity.Y)
                    Projectile.velocity.Y = -oldVelocity.Y;
            }
            HitOnce = true;
            Attagain = 0;
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);

            //Spawn the explosion

        }


        public override void AI()
        {

            if (Attagain <= 14)
            {
                Attagain++;
            }

            if (Attagain > 14)
            {
                Projectile.friendly = false;
            }

            if (Attagain >= 14)
            {
                Projectile.friendly = true;
            }

            if (HitOnce)
            {
                float damage = Projectile.damage;
                damage *= 1.05f;
                Projectile.damage = (int)damage;
                HitOnce = false;
            }

        }
    }


}
