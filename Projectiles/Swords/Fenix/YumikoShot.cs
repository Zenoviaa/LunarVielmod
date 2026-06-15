using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Assets;
using Stellamod.Core.Utilities;
using Stellamod.Trails;
using Terraria;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Swords.Fenix
{
    public class YumikoShot : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("SalfaCirle");
            Main.projFrames[Projectile.type] = 20;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 35;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.width = 130;
            Projectile.height = 373;
            Projectile.penetrate = -1;
            Projectile.scale = 1f;
            Projectile.knockBack = 12.9f;
            Projectile.aiStyle = 1;
            AIType = ProjectileID.Bullet;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = false;

        }
        public override bool PreAI()
        {

            Projectile.tileCollide = false;

            return true;
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, 0) * (1f - Projectile.alpha / 50f);
        }

        public float nigga = 0;
        public override void AI()
        {

            nigga++;

            Projectile.velocity *= 1.02f;
            if (nigga < 2)
            {
                ShakeScreenPosition.Shake = 13;
            }


            if (++Projectile.frameCounter >= 2)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 20)
                {
                    Projectile.frame = 0;
                }
            }


        }

        public override bool PreDraw(ref Color lightColor)
        {


            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            return true;
        }
    }
}



