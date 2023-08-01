using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent;
using Terraria.Audio;
using static Humanizer.In;

namespace Stellamod.Projectiles.Weapons.Magic
{
    internal class StarFlowerproj3 : ModProjectile
    {
        bool Moved;

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Shadow Hand");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 15;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
        }
        public override void SetDefaults()
        {
            Projectile.penetrate = 150;
            Projectile.hide = true;
            Projectile.width = 150;
            Projectile.height = 150;
            Projectile.timeLeft = 3;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }
        public override void AI()
        {
            Projectile.velocity.X = 0;
            Projectile.velocity.Y = 0;



        }
        public override void Kill(int timeLeft)
        {

        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Main.rand.Next(1) == 0 && !target.boss)
                target.AddBuff(BuffID.OnFire3, 180);
        }
        public override void PostDraw(Color lightColor)
        {
            Lighting.AddLight(Projectile.Center, Color.Orange.ToVector3() * 1.75f * Main.essScale);

        }
    }

}


