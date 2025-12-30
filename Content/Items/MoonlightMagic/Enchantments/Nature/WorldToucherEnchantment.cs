using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Content.Items.MoonlightMagic.Elements;
using Stellamod.Helpers;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Items.MoonlightMagic.Enchantments.Nature
{
    public class WorldToucherEnchantment : BaseEnchantment
    {
        bool HitOnce = false;
        int Attagain = 14;
        int Hits;
        public override float GetStaffManaModifier()
        {
            return 0.5f;
        }

        public override int GetElementType()
        {
            return ModContent.ItemType<NaturalElement>();
        }


        public override void SetMagicDefaults()
        {
            Projectile.penetrate += 2;
            MagicProj.tileHitCount += 2;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.velocity.X != oldVelocity.X)
                Projectile.velocity.X = -oldVelocity.X;

            if (Projectile.velocity.Y != oldVelocity.Y)
                Projectile.velocity.Y = -oldVelocity.Y;
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
