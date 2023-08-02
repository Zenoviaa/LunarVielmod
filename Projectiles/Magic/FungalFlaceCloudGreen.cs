using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent;
using Terraria.Audio;
using static Humanizer.In;
using Microsoft.Xna.Framework;


namespace Stellamod.Projectiles.Magic
{
    internal class FungalFlaceCloudGreen : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Fungal Flace Cloud");
        }

        public override void SetDefaults()
        {

            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.timeLeft = 210;
            Projectile.tileCollide = false;
            Projectile.height = 35;
            Projectile.width = 35;
            Projectile.penetrate = 10;
            AIType = ProjectileID.Bullet;
            Projectile.extraUpdates = 1;
        }

        public override bool PreAI()
        {
            Projectile.alpha++;

            float num = 1f - (float)Projectile.alpha / 255f;
            Projectile.velocity *= .98f;
            Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f;
            num *= Projectile.scale;
            Lighting.AddLight(Projectile.Center, 0.3f * num, 0.2f * num, 0.1f * num);
            Projectile.rotation = Projectile.velocity.X / 2f;
            return true;
        }
        float alphaCounter;
        public override void AI()
        {
            alphaCounter += 0.04f;
            Projectile.rotation += 0.3f;
        }
    }
}
