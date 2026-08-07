using Microsoft.Xna.Framework;
using Stellamod.Helpers;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles
{
    public class GreekFireFriendly : ModProjectile
	{
        public override string Texture => TextureRegistry.EmptyTexture;
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
			Projectile.CloneDefaults(ProjectileID.GreekFire3);
            Projectile.aiStyle = ProjAIStyleID.GroundProjectile;
            AIType = ProjectileID.GreekFire3;
            Projectile.hostile = false;
            Projectile.friendly = true;
        }

        public override Color? GetAlpha(Color lightColor)
        {
			return Color.Transparent;
        }
	}
}