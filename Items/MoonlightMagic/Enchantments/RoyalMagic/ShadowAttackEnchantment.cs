using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using Stellamod.Items.MoonlightMagic.Elements;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Items.MoonlightMagic.Enchantments.RoyalMagic
{
    internal class ShadowAttackEnchantment : BaseEnchantment
    {
        bool HitOnce = false;
        int Attagain = 14;
        public override float GetStaffManaModifier()
        {
            return 0.8f;
        }

        public override int GetElementType()
        {
            return ModContent.ItemType<RoyalMagicElement>();
        }


        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {

            return true;
        }

        public override void SpecialInventoryDraw(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            base.SpecialInventoryDraw(item, spriteBatch, position, frame, drawColor, itemColor, origin, scale);
            DrawHelper.DrawGlowInInventory(item, spriteBatch, position, Color.MediumPurple);
        }

        public override void SetMagicDefaults()
        {

            Projectile.penetrate += 3;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);

            //Spawn the explosion
            Vector2 direction = Projectile.velocity.SafeNormalize(Vector2.Zero);
            direction = direction.RotatedByRandom(MathHelper.ToRadians(30));
            Projectile.velocity = -direction * 14;
            HitOnce = true;
            Attagain = 0;
        }

        public float maxHomingDetectDistance = 2012;
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
                Projectile.tileCollide = false;
                NPC npcToChase = ProjectileHelper.FindNearestEnemy(Projectile.Center, maxHomingDetectDistance);
                if (npcToChase != null)
                    Projectile.velocity = ProjectileHelper.SimpleHomingVelocity(Projectile, npcToChase.Center, degreesToRotate: 8);
            }

        }
    }


}
