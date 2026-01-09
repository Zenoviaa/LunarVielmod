using Microsoft.Xna.Framework;
using Stellamod.Core.Bases;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Shop.ItemsShop
{
    public class CocoSpark : AbstractMagicTome
    {
        public override void SetDefaults2()
        {
            base.SetDefaults2();
            Item.shoot = ModContent.ProjectileType<CocoShot>();
            Item.shootSpeed = 4f;
            Item.damage = 12;
            Item.mana = 8;
        }
        public override Color GetTomeHintColor()
        {
            Item.damage = 12;
            return Color.Lerp(Color.Brown, Color.Black, 0.6f);
        }
    }

    public class CocoShot : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.BoulderStaffOfEarth);
            AIType = ProjectileID.BoulderStaffOfEarth;
            Projectile.width = Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.scale = 0.9f;
        }
    }
}









