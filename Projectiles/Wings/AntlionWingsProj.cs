using Microsoft.Xna.Framework;
using Stellamod.Items.Accessories.Wings;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Wings
{
    internal class AntlionWingsProj : WingDefaultProjectile
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.projFrames[Type] = 5;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 44;
            Projectile.height = 32;


            FrameSpeed = 3;
            AlwaysAnimate = false;
            WingOffset = new Vector2(8, 0);
            AccessoryItemType = ModContent.ItemType<AntlionWings>();
        }
    }
}
